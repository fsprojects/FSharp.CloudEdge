# NuGet release and publisher setup

FSharp.CloudEdge's first release targets **0.1.0** for the 47 projects selected in `FSharp.CloudEdge.Bindings.slnx`. **GitHub Actions builds, validates, and publishes the packages.** Like Furnace, the workflow supports `v*` release tags and manual dispatch. A `v0.1.0` tag must match the configured package version. Local commands below reproduce the checks when investigating a failure; maintainers do not need to upload packages manually.

NuGet account ownership and publishing authorization are setup tasks independent of GitHub repository administration. This workflow uses **NuGet Trusted Publishing** with GitHub OIDC. GitHub obtains temporary publishing credentials for each run.

## Current release blocker

The public `Xantham.Fable.Core` and `Xantham.Fable.Core.TS` 0.1.0 packages omit their `fable/` source assets. A consumer of `Xantham.Fable.Core` builds successfully with .NET but fails during Fable compilation of `TypeKeyOf.create` with `Cannot find inline member: XanthamFableCore.TypeKeyOf_create`. Erased binding examples can still compile, so those alone do not establish that the support package works.

Reproduce using only public support dependencies:

```bash
dotnet run --project tools/NuGetRelease -- support
```

The probe uses `tests/SupportPackage/Smoke.fs` and `UpstreamHelpers.fs`. Its staged project, isolated package cache, and logs are under `artifacts/nuget-release/0.1.0/public-support/`. The release workflow requires this check to pass before publishing.

The upstream packaging correction includes the project and source assets required by [Fable library packaging](https://fable.io/docs/your-fable-project/author-a-fable-library.html). `publicDependencies` in `config/nuget-release.json` now selects **0.1.1** for both support packages. Locally packed corrected packages pass the helper compilation probe; the upstream maintainer still needs to publish them before the public check can pass. Existing NuGet versions cannot be replaced. CloudEdge's intended first release remains 0.1.0.

To test an upstream packaging fix before publication, pass `--support-feed /absolute/path/to/packages` to `support`, `pack`, and `examples --fable`. These commands map the two Xantham package IDs to that explicit local feed and use isolated caches. This is local evidence only: the GitHub workflow uses public support packages, and `examples --public` rejects a local support feed.

## Restore account access

Sign into NuGet.org as **houstonhaynes** with the linked Microsoft account. Confirm that the profile name is the intended identity, the email address is confirmed and current, and Microsoft-account two-factor authentication works. Check recovery options and package-publication notifications. The public profile alone cannot establish any of these private settings. [NuGet account requirements](https://learn.microsoft.com/en-us/nuget/nuget-org/individual-accounts).

Review old API keys for expiry and remove unused ones. This workflow uses Trusted Publishing, so no manually generated API key or GitHub publishing secret is needed.

## Give Shayan publishing access

The current policy publishes under **houstonhaynes**. Shayan can trigger the shared GitHub workflow through his repository access. To give him independent NuGet access after the first publication, add his NuGet username under each package's **Manage Owners** and have him accept the invitation.

For ongoing shared ownership across all 47 packages, a NuGet organization is easier to maintain. If an appropriate organization already exists, use it; otherwise create a dedicated organization through **Manage Organizations → Add new organization**. Its name and email must be available. This does not require converting the personal `houstonhaynes` account into an organization.

In that organization's members page, add Shayan using his **NuGet.org username**, which may differ from his GitHub username. A **collaborator** can publish new packages and update or unlist existing packages. An **administrator** can also manage membership, metadata, and co-ownership. Choose administrator if you want him to share those responsibilities. Confirm that his membership is active before relying on it. [NuGet organization roles](https://learn.microsoft.com/en-us/nuget/nuget-org/organizations-on-nuget-org).

For packages already owned personally, add the organization as an owner using **Manage Owners** and complete the ownership acceptance. For new packages, select the organization as the owner when creating the Trusted Publishing policy. GitHub roles do not transfer package ownership.

## Understand the verification badge

NuGet does not require a separate certified-publisher credential for Shayan to publish. The visible verification indicator concerns **package ID prefix reservation**. An owner can request the specific prefix `FSharp.CloudEdge.`; approval is discretionary and separate from account authentication and publishing access. It is not a code-quality certification. [Prefix reservation requirements](https://learn.microsoft.com/en-us/nuget/nuget-org/id-prefix-reservation).

After agreeing on the owning organization, request that prefix through NuGet's documented process. Include the owner's NuGet profile and the public repository as evidence. Do not request the broader `FSharp.` prefix on behalf of this project. Prefix reservation is useful protection but is not a prerequisite for ordinary publication when the IDs are available.

## Connect NuGet to GitHub Actions

Sign into NuGet.org as **houstonhaynes** and create a **Trusted Publishing** policy. Configure:

| Setting | Value |
| --- | --- |
| Policy owner | `houstonhaynes` for the current policy |
| Repository owner | `fsprojects` |
| Repository | `FSharp.CloudEdge` |
| Workflow filename | `publish.yml` |
| Environment | `production` |
| Scope | Push new packages and package versions |
| Glob Patterns and Packages | `FSharp.CloudEdge.*` |

Enter the filename only, without `.github/workflows/`. Enter `FSharp.CloudEdge.*` as one line in **Glob Patterns and Packages**. The first release creates 47 new package IDs, so the policy must permit new packages as well as new versions. [NuGet Trusted Publishing setup](https://learn.microsoft.com/en-us/nuget/nuget-org/trusted-publishing).

In **fsprojects/FSharp.CloudEdge → Settings → Environments**, create the **`production` environment**, matching the policy and publication job. Configure environment branch/tag rules to permit `main` and `v*` release tags as appropriate. The repository owner is `fsprojects`; the NuGet package owner and login user are `houstonhaynes`. These fields identify different things.

The job has `id-token: write` permission and calls `NuGet/login@v1` with `user: houstonhaynes`. This is the individual username associated with the policy, including when its owner is an organization. The action obtains a temporary API key and passes it to the F# release tool as `NUGET_API_KEY`; the tool invokes `dotnet nuget push` in dependency order. No `NUGET_KEY` repository secret is required.

Either authorized GitHub maintainer can run this shared workflow. NuGet checks its configured policy, not whether the triggering GitHub actor has the same NuGet username. Shayan's NuGet organization membership additionally gives him package-management access and the ability to establish his own publishing policy. Organization membership associated with an organization-owned policy must remain active.

If NuGet shows a seven-day activation window, complete a successful publish during that window or restart it with **Activate for 7 days** when ready. The temporary policy becomes permanently active after successful publication supplies the repository identity. The current upstream release blocker should be resolved before publishing; the activation window can be restarted. [Policy activation](https://learn.microsoft.com/en-us/nuget/nuget-org/trusted-publishing#policies-pending-full-activation).

## Candidate package preparation

The F# release tool reads the selected solution, orders packages by dependency, and stages clean projects from checked-in source. Generator-only tools and the other unselected projects under `src/` are excluded. Staging leaves the contributor projects and their local generator pins unchanged.

`config/nuget-release.json` pins the release and public support dependency versions. Release consumers must not restore `0.1.0-local.*` dependencies. Candidate packages contain license and README metadata; Fable packages also contain their project and source files under `fable/`.

Sample projects use `0.1.*` so they can adopt stable patches. Published CloudEdge dependencies permit `[0.1.0,0.2.0)`; Xantham support dependencies permit `[0.1.1,0.2.0)`. Release validation replaces sample floats with exact candidate references and pins both support packages to the configured versions. Each consumer's `resolved-packages/*.json` report records the actual dependency versions and is retained in the workflow artifact.

From the repository root:

```bash
dotnet run --project tools/NuGetRelease -- plan
dotnet run --project tools/NuGetRelease -- check
dotnet run --project tools/NuGetRelease -- pack --jobs 2
dotnet run --project tools/NuGetRelease -- verify
dotnet run --project tools/NuGetRelease -- examples --fable
dotnet run --project tools/NuGetRelease -- snippets --js-root artifacts/nuget-release/0.1.0/consumers-candidate/js
dotnet run --project tools/NuGetRelease -- support
```

The pack step compiles the large shared API assembly and can take several minutes. It builds that assembly alone, then groups independent projects into temporary solutions and invokes native MSBuild with `-maxcpucount:2`. Each group finishes packing before its dependents restore. `--jobs N` adjusts the process limit; the default is at most two processes. Use `--jobs 1` for serial builds or a larger value on a machine with enough CPU and memory.

Outputs are under `artifacts/nuget-release/0.1.0`. `pack-logs/` contains the generated solutions, text logs, and MSBuild binary logs. `manifest.json` records package hashes and dependency order. Consumer validation stages all 19 site projects with package references only, uses a fresh package cache, builds them, and compiles Worker examples through Fable. Logs and partial results remain available if a check fails. This validates packaging and compilation, not hosted Cloudflare service behavior.

Inspect the README, license, dependency versions, source assets, and emitted imports before publishing. The Xantham support packages are owned upstream; use their public packages and report reproducible compatibility failures to their maintainer instead of publishing replacements under those IDs.

## Publish and verify the public feed

Run the **NuGet** workflow on `main` with **publish disabled** first. It builds candidates, checks consumers, and uploads the packages and verification evidence as an artifact. Fix failures before requesting the actual release.

After committing and merging fixes into `main`, use **Actions → NuGet → Run workflow**, choose `main`, and leave **publish** unchecked. The equivalent command is `gh workflow run publish.yml --ref main -f publish=false`. Here `publish.yml` is the workflow filename and `-f publish=false` supplies its boolean input. Rerunning the old tag's job uses the old commit; dispatching on `main` picks up the merged fixes without changing the package version or moving the tag. Enable **publish** only when ready to upload.

GitHub runs the public support-helper probe in a separate job alongside package validation, so upstream failures appear early. The publication job requires both jobs to succeed. Package compilation uses two MSBuild processes on the standard runner; uploads remain ordered by dependency.

When account ownership, the Trusted Publishing policy, and candidate checks are complete, push the release tag **`v0.1.0`**, or run the workflow manually with **publish enabled**. GitHub repeats validation, obtains temporary credentials, then uploads packages in the manifest's dependency order. A partial upload is possible: NuGet publication is not a transaction across 47 packages. The workflow stops on an upload error rather than silently skipping an existing version. Inspect ownership, versions, and package contents before deciding how to resume.

NuGet package versions are immutable. Do not publish a placeholder `0.1.0` as a test. If an uploaded version is wrong, unlisting does not free that version for replacement; publish a corrected version. [NuGet publication behavior](https://learn.microsoft.com/en-us/dotnet/standard/library-guidance/publish-nuget-package).

After packages finish processing, verify every planned version and repeat the consumer checks against the public feed alone:

```bash
dotnet run --project tools/NuGetRelease -- availability
dotnet run --project tools/NuGetRelease -- examples --fable --public
```

The workflow performs these public checks after pushing. A processing delay can make the checks fail even after an upload succeeds. Resolve public availability and repeat the checks before announcing success; do not republish different bytes with the same version.

Finally, update the release-availability paragraph in `site/content/guide/packages.md`, deploy the site, and announce the release. Package IDs, `0.1.0` references, and NuGet URLs can be prepared beforehand with that pending-release notice. The beginner path should not claim public installation works until the public consumer checks pass.
