# Upstream issues

These notes describe generator development and retained regression tests. Contributor
builds use locally packed tools through [bootstrap](local-tools.md); published CloudEdge
packages use the public Xantham support dependencies listed in
[`config/nuget-release.json`](../config/nuget-release.json).

## Xantham support module helpers

Status: corrected in Xantham. Executable helpers now live in the global auto-open module
`XanthamFableCore`. The erased types retain their `Fable.Core.JS.JS` identities. Bootstrap
packages both source files unchanged; CloudEdge supplies no replacement globals.

Previously, the support source declared its module as `Fable.Core.JS.JS`. Fable treats entities under
`Fable.Core.JS.` as JavaScript globals, including the nested `TypeKeyOf`, `KeyOf`, and `Brand`
modules. Their inline helper bodies were consequently emitted as global calls. This source:

```fsharp
open Fable.Core.JS
type Settings = { Count: int }
let propertyName () =
    TypeKeyOf.create (fun (settings: Settings) -> settings.Count) |> TypeKeyOf.value
```

emitted a call to `TypeKeyOf.create`, and execution raised `ReferenceError: TypeKeyOf is not
defined`. Both package and source-project references reproduced the failure. The compiler rule is in
`Fable.Transforms/FSharp2Fable.Util.fs`, `tryGlobalOrImportedAttributes`.

Run the retained reproducer after bootstrapping tools and support packages:

```sh
npm run repro:upstream-support-helpers
```

The self-contained source-project reproducer for the Xantham handoff lives in that
repository at `tests/repros/support-helper-globals/`. Its README gives the pinned tool
setup and commands; it can run without a CloudEdge checkout.

This command asserts the intended helper behavior. It also retains
`TypeKeyOf.item`, `KeyOf.item`, and the `Brand` string round-trip cases. Temporary source,
NuGet assets, and generated JavaScript remain under `artifacts/tools/support-smoke/` for
inspection.

The ordinary package migration check is separate:

```sh
npm run test:support-package
```

It restores into an isolated package cache, checks the installed archive digest and Fable
source assets, compiles using package references only, and executes the support library's
`Emit` key-value properties and readonly/mutable indexers. Passing this check establishes
those bindings and package transport; the helper regression checks the four affected calls.

Ordinary inferred helper calls keep their spelling. Explicit `keyof<Settings>` calls become
`keyof<Settings, _>` so Fable retains the lambda's return type during property-name inference.
Qualified helper names use `XanthamFableCore`; rebuild callers when updating the support
library.
