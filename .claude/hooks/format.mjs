#!/usr/bin/env node
// PostToolUse formatter: formats the file Claude just wrote, so diffs keep the repository style.
// backend/**/*.cs gets `dotnet format whitespace`; frontend web files get Prettier. Failures never block the edit.
import { execFileSync } from 'node:child_process';
import { existsSync, readFileSync } from 'node:fs';
import path from 'node:path';

let data = '';
for await (const chunk of process.stdin) data += chunk;
const input = JSON.parse(data || '{}');
const root = process.env.CLAUDE_PROJECT_DIR || input.cwd || process.cwd();
const filePath = input.tool_input?.file_path;

if (filePath && existsSync(filePath)) {
  const relative = path.relative(root, path.resolve(root, filePath));
  const [half, ...rest] = relative.split(path.sep);
  const inside = rest.length > 0 && ['backend', 'frontend'].includes(half);
  const projectRoot = path.join(root, half);
  const inProject = rest.join('/');
  try {
    if (inside && half === 'backend' && /\.cs$/i.test(inProject)) {
      run('dotnet', ['format', 'whitespace', projectRoot, '--folder', '--include', inProject], projectRoot);
    } else if (
      inside &&
      half === 'frontend' &&
      /\.(ts|tsx|mts|cts|js|jsx|mjs|cjs|json|css|scss|html|md)$/i.test(inProject)
    ) {
      const prettier = packageBin(projectRoot, 'prettier', 'prettier');
      if (prettier)
        run(
          process.execPath,
          [prettier, '--write', '--log-level', 'warn', '--ignore-unknown', inProject],
          projectRoot,
        );
    }
  } catch {
    // Formatting is a convenience; the edit stands even if the formatter is missing or fails.
  }
}

function packageBin(dir, name, bin) {
  const manifest = path.join(dir, 'node_modules', name, 'package.json');
  if (!existsSync(manifest)) return null;
  const bins = JSON.parse(readFileSync(manifest, 'utf8')).bin;
  const file = typeof bins === 'string' ? bins : bins?.[bin];
  return file ? path.join(path.dirname(manifest), file) : null;
}

function run(command, args, cwd) {
  execFileSync(command, args, { cwd, stdio: 'ignore', timeout: 50_000 });
}
