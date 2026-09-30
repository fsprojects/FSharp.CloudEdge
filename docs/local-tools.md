# Local generator tools

Contributors regenerate bindings with pinned Xantham and Hawaii .NET tools. Their source
checkouts are used by the bootstrap step to build the local versions recorded in
`.config/dotnet-tools.json` and `config/tool-packages.json`.

Applications consuming [CloudEdge packages](https://www.nuget.org/packages?q=FSharp.CloudEdge)
use the released Xantham support packages selected in
[`config/nuget-release.json`](../config/nuget-release.json). They do not need generator
checkouts or the contributor bootstrap process.

## First setup

Use the reviewed Xantham and Hawaii branches beside FSharp.CloudEdge. The bootstrap script accepts alternate locations when your checkout layout differs:

```sh
npm ci
npm run tools:bootstrap -- --xantham-source ../Xantham --hawaii-source ../Hawaii
npm run tools:restore
npm run tools:check
npm run test:support-package
```

Bootstrap runs `dotnet pack` on each generator project in Release configuration, writes the packages to the ignored `artifacts/tool-feed/` directory, and installs them into this repository's local tool manifest. The normal calls are:

```sh
dotnet tool run xantham -- schema
dotnet tool run hawaii-unofficial -- --version
```

The manifest at `.config/dotnet-tools.json` pins the versions. `config/tool-packages.json` records package and installed payload hashes, which the generation runners check before and after invoking a tool. Source and build receipts remain under `artifacts/tools/bootstrap/`. The pinned TypeScript compiler still comes from the root npm installation; Xantham's tool package does not embed it.

Local package versions include a digest of the source and build recipe. Updating a generator checkout has no effect on routine generation until you explicitly bootstrap that tool again:

```sh
npm run tools:bootstrap -- --only xantham --xantham-source ../Xantham
npm run tools:bootstrap -- --only hawaii --hawaii-source ../Hawaii
```

Review the resulting manifest, package lock and generated-source changes together. Keep existing package versions immutable. Bootstrap refuses a cached archive that disagrees with its build receipt.

## Support library

Bootstrap also creates local `Xantham.Fable.Core` and `Xantham.Fable.Core.TS` packages
from the peer's unchanged sources. Core includes `Library.fs` and `Helpers.fs`, assemblies
for .NET Standard 2.1 and .NET 8, and the Fable project and source assets. Core.TS includes
the generated standard-library support and references the matching local Core package.
Their temporary projects live under `artifacts/`. Generated SDK projects reference exact
package versions rather than peer source projects.

`NuGet.Config` includes the local feed and nuget.org. `npm run test:support-package` checks
package-only .NET compilation and Fable key-value properties and indexers.
`npm run repro:upstream-support-helpers` exercises the corrected helper functions in the
bootstrapped package; the regression is described in [upstream issues](upstream-issues.md).

## Routine generation

After bootstrap, use the generation and build commands in the root README and the [Hawaii pipeline](../generators/hawaii/README.md). They resolve installed tools through the local manifest. They do not compile the generator repositories or run a sibling DLL.

`npm run tools:restore` restores the versions already in the manifest. It requires access to the local archives until those versions are available from a package feed. A fresh contributor therefore needs either the matching reviewed archives or the source bootstrap step. Locally rebuilding a package can change archive bytes; review the new package lock and rerun validation before treating its output as equivalent.

## Updating generator tools

To adopt a published generator version, install its exact version into the same manifest
and record its package identity in the lock. The generation calls and project structure
remain the same. Update support references when a generator version needs a different
support API, then regenerate and validate the consumers.

CloudEdge publication uses the released support versions in `config/nuget-release.json`.
The [NuGet release guide](nuget-release.md) describes its separate package and consumer
checks. Run the public support check with:

```sh
dotnet run --project tools/NuGetRelease -- support
```

This restores public packages into an isolated cache, builds and compiles through Fable,
and executes the binding properties and indexers used by CloudEdge. The upstream helper
regression remains a separate contributor check.
