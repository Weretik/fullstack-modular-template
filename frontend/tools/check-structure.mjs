// Folder rules of the frontend, checked in CI:
// - a folder holds at most six production files (tests and index.ts do not count; generated code is exempt);
// - a folder holds either files or subfolders, never both, and never a single file alone;
// - folders are named by feature or meaning, never as a dump (utils, helpers, common, ...).
// The Next.js `app/` folder follows the framework's routing layout and is exempt, as is each `src/` root.
import { readdirSync, statSync } from 'node:fs';
import { join, relative, sep } from 'node:path';

const MAX_FILES = 6;
const FORBIDDEN = new Set([
  'utils',
  'utilities',
  'helpers',
  'common',
  'shared',
  'misc',
  'types',
  'constants',
  'lib',
]);
const roots = ['apps', 'libs'];
const root = new URL('..', import.meta.url).pathname;

const isProductionFile = (name) =>
  /\.(ts|tsx)$/.test(name) &&
  !/\.(spec|test)\.tsx?$/.test(name) &&
  name !== 'index.ts' &&
  !name.endsWith('.d.ts');

function sourceFolders(dir) {
  const result = [];
  for (const name of readdirSync(dir)) {
    const path = join(dir, name);
    if (
      !statSync(path).isDirectory() ||
      ['node_modules', '.next', 'dist', 'generated', 'public'].includes(name)
    )
      continue;
    if (path.split(sep).includes('src')) result.push(path);
    result.push(...sourceFolders(path));
  }
  return result;
}

const problems = [];
for (const top of roots) {
  for (const folder of [...new Set(sourceFolders(join(root, top)))]) {
    const rel = relative(root, folder);
    const parts = rel.split(sep);
    const entries = readdirSync(folder);
    const files = entries.filter(
      (name) => statSync(join(folder, name)).isFile() && isProductionFile(name),
    );
    const folders = entries.filter(
      (name) =>
        statSync(join(folder, name)).isDirectory() && name !== 'generated',
    );
    const inRoutes =
      parts.includes('app') && parts[0] === 'apps' && parts.includes('site');
    const isSrcRoot = parts.at(-1) === 'src';

    if (FORBIDDEN.has(parts.at(-1).toLowerCase()))
      problems.push(`${rel}: dump folder name, name it by meaning`);
    if (inRoutes || isSrcRoot) continue;
    if (files.length > MAX_FILES)
      problems.push(
        `${rel}: ${files.length} files, split it into subfolders by meaning`,
      );
    if (files.length > 0 && folders.length > 0)
      problems.push(`${rel}: holds files and subfolders, keep one kind`);
    if (files.length === 1 && folders.length === 0)
      problems.push(`${rel}: a folder with a single file, merge it`);
  }
}

if (problems.length > 0) {
  console.error(problems.join('\n'));
  process.exit(1);
}
console.log('Folder structure is fine.');
