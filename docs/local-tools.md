# Local generator tools

Xantham and Hawaii remain independent repositories. FSharp.CloudEdge consumes their .NET tool packages and the Xantham support library. The source checkouts are needed for the explicit bootstrap step while these generator changes await publication.

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

Bootstrap also creates an `Xantham.Fable.Core` package from the peer's unchanged `Library.fs` and README. Its temporary project lives under `artifacts/`, includes assemblies for .NET Standard 2.1 and .NET 8, and supplies the Fable project and source assets. Generated SDK projects reference this exact package version; they do not reference the peer source project.

`NuGet.Config` includes the local feed and nuget.org. `npm run test:support-package` checks package-only .NET compilation and supported Fable type/indexer operations. An existing helper-function failure is documented in [upstream issues](upstream-issues.md); `npm run repro:upstream-support-helpers` deliberately exercises that unresolved case.

## Routine generation

After bootstrap, use the generation and build commands in the root README and the [Hawaii pipeline](../generators/hawaii/README.md). They resolve installed tools through the local manifest. They do not compile the generator repositories or run a sibling DLL.

`npm run tools:restore` restores the versions already in the manifest. It requires access to the local archives until those versions are available from a package feed. A fresh contributor therefore needs either the matching reviewed archives or the source bootstrap step. Locally rebuilding a package can change archive bytes; review the new package lock and rerun validation before treating its output as equivalent.

## Published packages

Once suitable packages are available on NuGet, install their exact versions into the same manifest and record their package identities in the lock. The generation calls and project structure remain the same. Update the `Xantham.Fable.Core` PackageReference alongside any generator version that requires a different support API, then regenerate and validate the consumers.

The generator package release and FSharp.CloudEdge's own release are independent. The future fsprojects publication should pin reviewed packages and include repeatable validation instructions; it should not require generator source to be copied into this repository.
