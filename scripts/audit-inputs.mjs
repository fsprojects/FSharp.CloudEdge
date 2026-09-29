import { createHash } from "node:crypto";
import { existsSync, mkdirSync, readFileSync, statSync, writeFileSync } from "node:fs";
import path from "node:path";
import { createRequire } from "node:module";
import { fileURLToPath } from "node:url";

const defaultRoot = path.resolve(path.dirname(fileURLToPath(import.meta.url)), "..");
const json = (file) => JSON.parse(readFileSync(file, "utf8"));
const isFile = (file) => existsSync(file) && statSync(file).isFile();
const typesName = (name) => name.startsWith("@") ? `@types/${name.slice(1).replace("/", "__")}` : `@types/${name}`;
const packageName = (specifier) => specifier.startsWith("@") ? specifier.split("/").slice(0, 2).join("/") : specifier.split("/")[0];

// The parser is used only to read syntax. This audit does not use its checker or resolver,
// and its version is reported separately from the generator's compiler version.
export function declarationSyntax(ts, file, source) {
  const tree = ts.createSourceFile(file, source, ts.ScriptTarget.Latest, true);
  const imports = [];
  const ambientModules = [];
  const lines = source.split(/\r?\n/);
  const add = (specifier, kind, position, sideEffectOnly = false) => {
    const at = tree.getLineAndCharacterOfPosition(position);
    const suppression = /^\s*\/\/\s*@(ts-ignore|ts-expect-error)\b/.exec(lines[at.line - 1] ?? "")?.[1] ?? null;
    imports.push({ specifier, kind, line: at.line + 1, column: at.character + 1, suppression, sideEffectOnly });
  };
  const walk = (node) => {
    if ((ts.isImportDeclaration(node) || ts.isExportDeclaration(node)) && node.moduleSpecifier && ts.isStringLiteralLike(node.moduleSpecifier)) {
      add(node.moduleSpecifier.text, ts.isImportDeclaration(node) ? "import" : "export", node.moduleSpecifier.getStart(tree), ts.isImportDeclaration(node) && !node.importClause);
    } else if (ts.isImportTypeNode(node) && ts.isLiteralTypeNode(node.argument) && ts.isStringLiteralLike(node.argument.literal)) {
      add(node.argument.literal.text, "import-type", node.argument.literal.getStart(tree));
    } else if (ts.isImportEqualsDeclaration(node) && ts.isExternalModuleReference(node.moduleReference) && node.moduleReference.expression && ts.isStringLiteralLike(node.moduleReference.expression)) {
      add(node.moduleReference.expression.text, "import-equals", node.moduleReference.expression.getStart(tree));
    } else if (ts.isModuleDeclaration(node) && ts.isStringLiteralLike(node.name) && !ts.isExternalModule(tree)) {
      ambientModules.push(node.name.text);
    }
    ts.forEachChild(node, walk);
  };
  walk(tree);
  tree.referencedFiles.forEach((ref) => add(ref.fileName, "reference-path", ref.pos));
  tree.typeReferenceDirectives.forEach((ref) => add(ref.fileName, "reference-types", ref.pos));
  return { imports, ambientModules, diagnostics: tree.parseDiagnostics.map((diagnostic) => ({ code: diagnostic.code, message: ts.flattenDiagnosticMessageText(diagnostic.messageText, " ") })) };
}

export function packageAt(name, fromFile) {
  for (let directory = path.dirname(fromFile); ; directory = path.dirname(directory)) {
    const candidate = path.join(directory, "node_modules", name);
    if (isFile(path.join(candidate, "package.json"))) return candidate;
    if (path.dirname(directory) === directory) return null;
  }
}

function declarationFile(candidate, seen = new Set()) {
  if (seen.has(candidate)) return null;
  seen.add(candidate);
  const extension = /\.(?:mjs|cjs|js|jsx)$/.exec(candidate);
  const stem = extension ? candidate.slice(0, -extension[0].length) : candidate;
  const guesses = extension
    ? (extension[0] === ".mjs" ? [stem + ".d.mts", stem + ".mts"] : extension[0] === ".cjs" ? [stem + ".d.cts", stem + ".cts"] : [stem + ".d.ts", stem + ".ts", stem + ".tsx"])
    : /\.(?:ts|mts|cts)$/.test(candidate) && !/\.d\.(?:ts|mts|cts)$/.test(candidate)
      ? [candidate, candidate.replace(/\.(ts|mts|cts)$/, ".d.$1")]
      : [candidate, candidate + ".d.ts", candidate + ".ts", candidate + ".tsx", candidate + ".d.mts", candidate + ".d.cts"];
  for (const guess of guesses) if (/\.(?:ts|tsx|mts|cts)$/.test(guess) && isFile(guess)) return guess;
  if (isFile(path.join(candidate, "package.json"))) {
    const meta = json(path.join(candidate, "package.json"));
    for (const entry of [meta.types, meta.typings, meta.main].filter((value) => typeof value === "string")) {
      const result = declarationFile(path.resolve(candidate, entry), seen);
      if (result) return result;
    }
  }
  for (const name of ["index.d.ts", "index.d.mts", "index.d.cts", "index.ts", "index.tsx"]) {
    const file = path.join(candidate, name);
    if (isFile(file)) return file;
  }
  return null;
}

function exportPaths(value, mode) {
  if (typeof value === "string") return [value];
  if (Array.isArray(value)) return value.flatMap((item) => exportPaths(item, mode));
  if (!value || typeof value !== "object") return [];
  // Configured targets have no customConditions. A bun/workerd/browser runtime branch
  // must not create false dependency requirements for their default declaration surface.
  return Object.keys(value).filter((key) => ["types", mode, "default", "node"].includes(key))
    .flatMap((key) => exportPaths(value[key], mode));
}

export function resolveImport(specifier, file, kind = "import") {
  if (specifier.startsWith(".") || path.isAbsolute(specifier) || kind === "reference-path") {
    const resolved = declarationFile(path.resolve(path.dirname(file), specifier));
    return resolved ? { status: "resolved-relative", files: [resolved] } : { status: "missing-relative", files: [] };
  }
  if (specifier.startsWith("#")) {
    for (let directory = path.dirname(file); ; directory = path.dirname(directory)) {
      const manifest = path.join(directory, "package.json");
      if (isFile(manifest)) {
        const meta = json(manifest);
        if (meta.imports) {
          for (const [pattern, value] of Object.entries(meta.imports)) {
            const [before, after] = pattern.split("*");
            if (pattern === specifier || (pattern.includes("*") && specifier.startsWith(before) && specifier.endsWith(after))) {
              const replacement = pattern.includes("*") ? specifier.slice(before.length, after.length ? -after.length : undefined) : "";
              const found = exportPaths(value, /\.d\.cts$/.test(file) ? "require" : "import")
                .map((entry) => declarationFile(path.resolve(directory, entry.replaceAll("*", replacement)))).find(Boolean);
              return found ? { status: "resolved-package-import", files: [found], package: meta.name, version: meta.version, directory } : { status: "unresolved-package-import", files: [], package: meta.name, directory };
            }
          }
        }
        if (meta.name) break;
      }
      if (path.dirname(directory) === directory) break;
    }
    return { status: "unresolved-package-import", files: [], package: specifier };
  }
  const name = packageName(specifier);
  const direct = packageAt(name, file);
  const types = packageAt(typesName(name), file);
  const candidates = kind === "reference-types" ? [types, direct] : [direct, types];
  for (const directory of candidates.filter(Boolean)) {
    const meta = json(path.join(directory, "package.json"));
    const subpath = specifier.slice(name.length);
    const key = subpath ? "." + subpath : ".";
    let exports = meta.exports;
    let replacement;
    if (exports && typeof exports === "object" && !Array.isArray(exports) && Object.keys(exports).some((key) => key.startsWith("."))) {
      const pattern = Object.keys(exports).find((pattern) => {
        if (!pattern.includes("*")) return pattern === key;
        const [before, after] = pattern.split("*");
        return key.startsWith(before) && key.endsWith(after);
      });
      if (pattern?.includes("*")) {
        const [before, after] = pattern.split("*");
        replacement = key.slice(before.length, after.length ? -after.length : undefined);
      }
      exports = pattern ? exports[pattern] : undefined;
    } else if (subpath) exports = undefined;
    const mode = /\.d\.cts$/.test(file) || kind === "import-equals" ? "require" : "import";
    const exportCandidates = exportPaths(exports, mode).map((entry) => replacement === undefined ? entry : entry.replaceAll("*", replacement));
    const entries = [...exportCandidates, ...(subpath ? ["." + subpath] : [meta.types, meta.typings, meta.main, "index.d.ts"].filter(Boolean))];
    const resolved = entries.map((entry) => declarationFile(path.resolve(directory, entry))).find(Boolean);
    const files = resolved ? [resolved] : [];
    if (files.length) return { status: "resolved-package", files, package: meta.name, version: meta.version, directory };
  }
  return { status: direct || types ? "unresolved-declaration-path" : "missing-package", files: [], package: name, directory: direct ?? types };
}

export async function auditInputs({ root = defaultRoot, parser } = {}) {
  const configFile = path.join(root, "config/targets.json");
  const config = json(configFile);
  const require = createRequire(import.meta.url);
  const ts = parser ?? require(path.join(root, "profiles/worker-bundler/node_modules/typescript"));
  const profiles = new Map(config.profiles.map((profile) => [profile.id, path.resolve(root, profile.directory)]));
  const cache = new Map();
  const ownerCache = new Map();
  const relative = (file) => file ? path.relative(root, file).split(path.sep).join("/") : null;
  const syntax = (file) => {
    if (!cache.has(file)) cache.set(file, declarationSyntax(ts, file, readFileSync(file, "utf8")));
    return cache.get(file);
  };
  const owner = (file) => {
    if (ownerCache.has(file)) return ownerCache.get(file);
    for (let directory = path.dirname(file); ; directory = path.dirname(directory)) {
      const manifest = path.join(directory, "package.json");
      if (isFile(manifest)) {
        const meta = json(manifest);
        if (!meta.name) continue;
        const result = { name: meta.name, version: meta.version, manifest: relative(manifest), dependencies: meta.dependencies ?? {}, peerDependencies: meta.peerDependencies ?? {}, peerDependenciesMeta: meta.peerDependenciesMeta ?? {}, optionalDependencies: meta.optionalDependencies ?? {}, devDependencies: meta.devDependencies ?? {}, deprecated: meta.deprecated ?? null };
        ownerCache.set(file, result);
        return result;
      }
      if (path.dirname(directory) === directory) return null;
    }
  };
  const targets = [];
  for (const target of config.targets) {
    const profile = profiles.get(target.profile);
    const packageDirectory = path.join(profile, "node_modules", target.package);
    const entry = path.resolve(packageDirectory, target.generator.entry ?? "index.d.ts");
    const queue = [entry];
    const configuredProviders = [];
    for (const provider of target.generator.types ?? []) {
      const result = resolveImport(provider, path.join(profile, "audit-root.ts"), "reference-types");
      configuredProviders.push({ provider, status: result.status, files: result.files.map(relative) });
      queue.push(...result.files);
    }
    const seen = new Set();
    const ambient = new Map();
    const edges = [];
    const parseDiagnostics = [];
    const packages = new Map();
    while (queue.length) {
      const file = queue.shift();
      if (seen.has(file)) continue;
      seen.add(file);
      if (!isFile(file)) {
        edges.push({ file: relative(file), status: "missing-entry", specifier: null });
        continue;
      }
      const parsed = syntax(file);
      const declaring = owner(file);
      if (declaring) packages.set(declaring.manifest, declaring);
      parsed.ambientModules.forEach((name) => ambient.set(name, relative(file)));
      parsed.diagnostics.forEach((diagnostic) => parseDiagnostics.push({ file: relative(file), ...diagnostic }));
      for (const imported of parsed.imports) {
        const result = resolveImport(imported.specifier, file, imported.kind);
        edges.push({ file: relative(file), ...imported, ...result, files: result.files.map(relative), directory: relative(result.directory), owner: declaring?.manifest });
        queue.push(...result.files);
      }
    }
    const matches = (pattern, name) => pattern === name || (pattern.includes("*") && name.startsWith(pattern.split("*")[0]) && name.endsWith(pattern.split("*")[1]));
    for (const edge of edges) {
      if (!["missing-package", "unresolved-declaration-path"].includes(edge.status)) continue;
      const provider = [...ambient].find(([pattern]) => matches(pattern, edge.specifier));
      if (provider) {
        edge.status = "resolved-ambient";
        edge.ambientProvider = { pattern: provider[0], file: provider[1] };
      }
    }
    targets.push({ id: target.id, profile: target.profile, package: target.package, version: target.version, entry: relative(entry), configuredProviders, declarationFileCount: seen.size, importCount: edges.length, parseDiagnostics, packages: [...packages.values()], unresolved: edges.filter((edge) => !edge.status.startsWith("resolved-")), ambientProviders: Object.fromEntries([...ambient].sort()), imports: edges });
  }
  return {
    schemaVersion: 1,
    generatedAt: new Date().toISOString(),
    configSha256: createHash("sha256").update(readFileSync(configFile)).digest("hex"),
    parser: { package: "typescript", version: ts.version, purpose: "Syntax only; no checker or TypeScript module-resolution API invoked" },
    generatorCompilerVersion: config.toolchain.typescript,
    resolutionPolicy: "Anchored filesystem declaration lookup, published standard package export conditions (types/import/require/default/node, no custom conditions), package types/typings, declaration-relative paths and reachable ambient module declarations. This is dependency evidence, not a TypeScript 7 semantic diagnostic verdict. Private paths found on disk are recorded even if exports would prohibit them under a particular resolution mode. Unresolved imports annotated ts-ignore/ts-expect-error remain recorded separately from unsuppressed imports.",
    summary: { targets: targets.length, profiles: profiles.size, uniqueParsedFiles: cache.size, targetsWithUnresolved: targets.filter((target) => target.unresolved.length).length, unresolvedOccurrences: targets.reduce((count, target) => count + target.unresolved.length, 0), parseDiagnostics: targets.reduce((count, target) => count + target.parseDiagnostics.length, 0) },
    targets
  };
}

if (process.argv[1] && path.resolve(process.argv[1]) === fileURLToPath(import.meta.url)) {
  const report = await auditInputs();
  const directory = path.join(defaultRoot, "artifacts/input-audit");
  mkdirSync(directory, { recursive: true });
  writeFileSync(path.join(directory, "declaration-imports.json"), JSON.stringify(report, null, 2) + "\n");
  console.log(JSON.stringify(report.summary, null, 2));
}
