# Rhino Foundry UI

Shared Eto.Forms components for Foundry plugins running on Rhino 8 / .NET 8 on Windows and macOS. Version **0.3.0-preview.1** is a coordinated prerelease migration; native Windows sign-off is required before a stable release.

## Packages and installation

| Package | Responsibility | Host dependency |
| --- | --- | --- |
| RhinoFoundry.UI.Primitives | Camera, geometry, thumbnail layout and generic selection | None |
| RhinoFoundry.UI | Theme, controls, accordion, resizable panes, gallery, tables, scroll container and canvas | Rhino-provided Eto / RhinoCommon |
| RhinoFoundry.UI.MacOS | AppKit trackpad, scoped clipboard and native table adapters | Installed Rhino Mac assemblies |

Pin all packages to the same exact version. Consumers vendor the prerelease `.nupkg` files in `packages/`, use that local NuGet source and commit `packages.lock.json`. Bundle the UI and Primitives DLLs beside every consumer RHP; include MacOS only in Mac bundles. This library is not installed as a separate Rhino plugin. Update all co-installed consumers together: Rhino can share assembly loads between plugins, so different DLLs with the same assembly version are unsafe.

Initialize `RhinoFoundry.UI.MacOS.FoundryMacOS.Initialize()` from each Mac consumer's composition boundary before controls load. Windows uses Eto input and native Windows controls; it must not reference or ship the Mac adapter. Never compile the Mac project using fallback stubs.

## Components

- `FoundryTheme`: semantic colors, typography, spacing and hierarchy surfaces.
- `FoundryDialogButton`, `FoundryToolbarIconButton`, `FoundryToolbarButtonGroup`: quiet 32px actions and mode controls.
- `FoundryFormField`, `FoundryToolbarField`, `FoundryCheckBox`, `FoundryColorField`, `FoundrySlider`: fields with keyboard, disabled and focus behavior.
- `FoundryAccordion`, `FoundryPaneResizeHandle`, `FoundryTextSegmentedControl`, `FoundryMultiSelectField`, `FilteredPicker`, badges: reusable compositions.
- `FoundryTable`: native row presentation and formatting; the consumer supplies data, columns, selection and editing commands.
- `FoundryThumbnailGallery`: responsive image cards, selection and optional drag data. Set `EmptyText` and `DragDataFormat` in the consumer.
- `FoundryScrollable`: guards and coalesces viewport-width synchronization. This avoids AppKit layout feedback while scrolling an accordion.
- `FoundryCanvas`: a drawing surface with camera math, batched gestures and disposable native subscriptions. Override occupied overlay hit testing and camera application when a product has its own zoom policy.
- `FoundrySurfaceButton`, `FoundryInsetFormField`, `FoundrySurfaceTheme`: explicit 34px variants preserving Maps' existing presentation. They are not replacements for the standard 32px family.

Product branding, white paper surfaces, Rhino document mutation, persistence, preview generation, domain drag formats, hierarchy rules, card rendering and application workflows remain in consumers. A product-specific rendering class is expected; it should compose these foundations rather than duplicate widget behavior.

See [contracts](docs/CONTRACTS.md), [validation](docs/VALIDATION.md) and [migration notes](CHANGELOG.md).

## Build and test

Use the SDK policy in `global.json` and a .NET 8 runtime. Portable checks:

```sh
dotnet restore src/RhinoFoundry.UI/RhinoFoundry.UI.csproj --locked-mode
dotnet build src/RhinoFoundry.UI/RhinoFoundry.UI.csproj --no-restore -c Release
dotnet test tests/RhinoFoundry.UI.Tests -c Release
dotnet test tests/RhinoFoundry.UI.Primitives.Tests -c Release
```

On a provisioned Mac, restore/build the solution and pack all three `src` projects. `RhinoMacResources` can point to the installed Rhino resources directory. Build to an isolated `BaseOutputPath` while Rhino is running. Do not use an ordinary hosted macOS CI runner as proof of native Rhino compatibility.

`tools/RhinoFoundry.UI.HostChecks` builds a document-free contract runner and visual gallery. Run its `ComponentChecks.Run()` and `ShowGallery()` inside Rhino's UI thread using the example in `samples/run-host-checks.py`. The ordinary xUnit tests do not initialize Eto or certify native behavior.

Packages remain local until explicit publication. Use `scripts/validate-packages.py` to verify payloads and generate a bundle hash manifest before syncing consumers. Rebuilds during development are staging candidates; once a version is distributed, publish changes under a new version.
