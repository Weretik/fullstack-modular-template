#!/usr/bin/env node
// Stop hook: before Claude says it is done, compiles the projects touched by uncommitted code changes.
// backend/: `dotnet build` of each changed project. frontend/: Nx `typecheck` of the affected projects.
// A failure sends the errors back to Claude, which keeps working. Unchanged code is not checked twice.
import { execFileSync, spawnSync } from 'node:child_process';
import { createHash } from 'node:crypto';
import { existsSync, readFileSync, readdirSync, writeFileSync } from 'node:fs';
import path from 'node:path';

let data = '';
for await (const chunk of process.stdin) data += chunk;
const input = JSON.parse(data || '{}');
const root = process.env.CLAUDE_PROJECT_DIR || input.cwd || process.cwd();

const changed = git(['status', '--porcelain=v1', '-uall'])
  .split('\n')
  .filter((line) => line && line[0] !== 'D' && line[1] !== 'D')
  .map((line) => line.slice(3).split(' -> ').pop().replace(/^"|"$/g, ''));

const backend = path.join(root, 'backend');
const frontend = path.join(root, 'frontend');
const backendCode = changed
  .filter((f) => /^backend\/(src|tests)\/.*\.(cs|csproj)$/.test(f))
  .map((f) => f.slice('backend/'.length));
const frontendCode = changed
  .filter((f) => /^frontend\/(apps|libs)\/.*\.(ts|tsx|mts|cts)$/.test(f))
  .map((f) => f.slice('frontend/'.length));
const code = [...backendCode.map((f) => `backend/${f}`), ...frontendCode.map((f) => `frontend/${f}`)];
if (code.length === 0) process.exit(0);

const stampFile = path.resolve(root, git(['rev-parse', '--git-dir']).trim(), 'claude-verify.json');
const hash = createHash('sha1');
for (const file of code.sort()) {
  hash.update(file);
  try {
    hash.update(readFileSync(path.join(root, file)));
  } catch {
    // A file removed between status and read only changes the hash.
  }
}
const fingerprint = hash.digest('hex');
const stamp = readStamp();
// Same code as the last check: it already passed, or Claude already saw the errors.
if (stamp?.fingerprint === fingerprint) process.exit(0);

const results = [
  backendCode.length ? buildDotnet(backendCode) : null,
  frontendCode.length ? typecheckNx(frontendCode) : null,
].filter(Boolean);
const failed = results.filter((r) => !r.ok);
const result = {
  ok: failed.length === 0,
  output: failed.map((r) => r.output).join('\n'),
  label: failed.map((r) => r.label).join(' and '),
};
writeFileSync(stampFile, JSON.stringify({ fingerprint, ok: result.ok }));
if (!result.ok) {
  const tail = result.output.split('\n').filter(Boolean).slice(-60).join('\n');
  process.stdout.write(
    JSON.stringify({
      decision: 'block',
      reason: `${result.label} failed for your uncommitted changes. Fix these errors before finishing:\n${tail}`,
    }),
  );
}

function buildDotnet(files) {
  const projects = [...new Set(files.map(nearestProject).filter(Boolean))];
  let output = '';
  for (const project of projects) {
    const run = spawnSync('dotnet', ['build', project, '--nologo', '-v', 'q', '-clp:ErrorsOnly'], {
      cwd: backend,
      encoding: 'utf8',
      env: { ...process.env, DOTNET_NOLOGO: '1', DOTNET_CLI_TELEMETRY_OPTOUT: '1' },
    });
    if (run.error) return { ok: true, output: '', label: 'dotnet build' }; // dotnet is not installed here.
    if (run.status !== 0) output += `${project}\n${run.stdout}${run.stderr}\n`;
  }
  return { ok: output === '', output, label: 'dotnet build' };
}

function nearestProject(file) {
  let dir = path.dirname(path.join(backend, file));
  while (dir.startsWith(backend) && dir !== backend) {
    const project = readdirSync(dir).find((f) => f.endsWith('.csproj'));
    if (project) return path.relative(backend, path.join(dir, project));
    dir = path.dirname(dir);
  }
  return null;
}

function typecheckNx(files) {
  const nx = packageBin(frontend, 'nx', 'nx');
  if (!nx) return { ok: true, output: '', label: 'nx typecheck' };
  const run = spawnSync(
    process.execPath,
    [nx, 'affected', '-t', 'typecheck', `--files=${files.join(',')}`, '--outputStyle=static'],
    {
      cwd: frontend,
      encoding: 'utf8',
      maxBuffer: 64 * 1024 * 1024,
      env: { ...process.env, FORCE_COLOR: '0', NX_TUI: 'false' },
    },
  );
  if (run.error) return { ok: true, output: '', label: 'nx typecheck' };
  return { ok: run.status === 0, output: `${run.stdout}${run.stderr}`, label: 'nx typecheck' };
}

function packageBin(dir, name, bin) {
  const manifest = path.join(dir, 'node_modules', name, 'package.json');
  if (!existsSync(manifest)) return null;
  const bins = JSON.parse(readFileSync(manifest, 'utf8')).bin;
  const file = typeof bins === 'string' ? bins : bins?.[bin];
  return file ? path.join(path.dirname(manifest), file) : null;
}

function readStamp() {
  try {
    return JSON.parse(readFileSync(stampFile, 'utf8'));
  } catch {
    return null;
  }
}

function git(args) {
  return execFileSync('git', args, { cwd: root, encoding: 'utf8' });
}
