# Implementing shared components

This guide defines how to add a reusable control to Rhino Foundry UI. The goal is one product-neutral implementation with the same behavior on Rhino 8 for Windows and macOS.

## Decide where the code belongs

Add behavior to this repository when all of these are true:

- At least two Foundry plugins can use the same interaction without knowing each other's domain.
- The API can be expressed with UI values, callbacks, and product-neutral models.
- The component owns presentation or input behavior, rather than a Rhino document workflow.
- Windows and macOS can expose the same public contract.

Keep code in the consumer when it knows about layouts, blocks, maps, document metadata, Rhino undo, preview generation, product drag formats, hierarchy rules, or application commands. A consumer-specific renderer can subclass or compose a shared foundation without moving the renderer itself here.

Examples:

| Requirement | Location |
| --- | --- |
| Button hover, focus, disabled, and keyboard behavior | `RhinoFoundry.UI` |
| Apply layout changes to a Rhino document | Consumer |
| Camera transforms and generic range selection | `RhinoFoundry.UI.Primitives` |
| Render a page, block, or map card | Consumer |
| AppKit trackpad recognition | `RhinoFoundry.UI.MacOS` |
| Windows behavior available through Eto | `RhinoFoundry.UI` |

## Choose the project

Use `RhinoFoundry.UI.Primitives` for deterministic data structures and policies that have no Eto, RhinoCommon, or platform dependency. Prefer immutable records or small state models. These types should be testable on any .NET 8 host.

Use `RhinoFoundry.UI` for Eto controls, semantic theme tokens, product-neutral drawing, and public adapter interfaces. It may compile against Rhino-provided Eto/RhinoCommon but must not mutate Rhino documents.

Use `RhinoFoundry.UI.MacOS` only for an unavoidable AppKit gap. This project must build against real assemblies from a provisioned Rhino Mac. Do not add fallback stubs that make a native build appear valid.

Windows-specific code should be rare. Prefer Eto's Windows behavior. If a native Windows adapter becomes necessary, give it the same contract and lifecycle as the Mac adapter and validate it on a licensed Windows host.

## Start with the public contract

Before drawing the control, write down:

- Constructor inputs and valid ranges.
- Mutable properties and whether setters raise events.
- Events, their ordering, and whether programmatic changes raise them.
- Keyboard and pointer behavior.
- Disabled and read-only behavior.
- Resource ownership and disposal.
- Coordinate and DPI units.
- Failure behavior when a native service is unavailable.

Keep public APIs small. Prefer product-neutral values and event arguments. Do not expose Rhino documents, consumer view models, or product enums from this library. Avoid inheriting from a shared control solely to change colors; add a documented variant only when geometry or behavior differs.

Document every ownership rule in XML comments and in [CONTRACTS.md](CONTRACTS.md) when it affects more than one component.

## Build the visual states

Use semantic values from `FoundryTheme`. Add a semantic token there if an existing token does not express the role. Do not place literal application colors throughout a component.

Standard form controls and toolbar buttons use these dimensions:

- 32 logical pixels for a single-line control or toolbar button.
- 1 logical pixel borders.
- 6 logical pixel radii for fields and buttons.
- `FoundryTheme.Space*` for layout gaps and padding.

Implement every relevant state:

- Rest
- Hover
- Pressed
- Keyboard focus
- Selected or checked
- Disabled
- Error, warning, or destructive when the control exposes that meaning

A focus ring must be visible and neutral. Do not stack a native focus bezel inside a Foundry field shell. Toolbar controls need an opaque resting surface because they may overlap a white sheet on a canvas.

Use vector or custom-drawn icons. Supply a useful tooltip for every icon-only action.

## Implement input as one state machine

Mouse and keyboard paths must call the same state-changing method. This avoids parity drift between click, Enter, and Space.

A button-shaped control normally supports:

- Primary mouse down to focus and enter pressed state.
- Primary mouse up to activate and leave pressed state.
- Enter and Space to activate.
- Lost focus or disabled state to cancel a pending press.

A selection control normally adds arrow-key navigation. Sliders support arrows and Home/End. Checkboxes support Space. Popups support Escape and return focus to their anchor when appropriate.

Mark an Eto event handled only after the control accepts it. Clear transient hover, press, and drag state when disabling or unloading the control.

Keep gesture math in logical Eto coordinates. Do not multiply positions by backing scale on Retina displays. Native adapters must convert native input into the same logical deltas delivered by other platforms.

## Own native resources explicitly

Native hooks, gesture recognizers, timers, forms used as popups, and subscriptions must have one owner and one release path.

- Attach native services on `LoadComplete` when a native handle is available.
- Dispose native subscriptions on `UnLoad` and again defensively from `Dispose(bool)`.
- Make disposal idempotent.
- Close component-owned popups on unload.
- Stop timers before disposing them.
- Do not retain a control, window, or document through a static event.

Use `FoundryNative.Services` as the portable boundary. A missing native service should leave the control with its portable Eto behavior unless the contract explicitly requires a native capability.

Native adapters must stay presentation-only. They must not open, mutate, or retain Rhino documents.

## Keep layout stable while scrolling

Avoid writing size properties synchronously from a nested `SizeChanged` callback when the write can cause the parent viewport to measure again. This is especially important for AppKit scroll views.

For vertical configuration panels, compose content inside `FoundryScrollable`. It coalesces width changes through the UI queue and ignores repeated widths. A new shared container with similar behavior should:

1. Queue at most one width update.
2. Exit when disposed or unloaded.
3. Skip unchanged or invalid sizes.
4. Change only the dimension it owns.
5. Have a regression test for recursive or repeated resize notifications.

For canvas input, accumulate gesture deltas and apply them once per frame. Expensive document queries, previews, and persistence do not belong in per-frame input handlers.

## Add tests before moving consumers

Place deterministic tests in the matching xUnit project:

- `tests/RhinoFoundry.UI.Primitives.Tests` for geometry, camera, selection, and layout policy.
- `tests/RhinoFoundry.UI.Tests` for portable contract helpers that do not require an initialized Eto application.

Do not construct native Eto widgets in ordinary xUnit tests when the test runner has no Rhino UI host. Add document-free native checks to `tools/RhinoFoundry.UI.HostChecks` instead.

A public component should have checks for the behavior it owns:

- Constructor validation and clamping.
- Programmatic property changes and event counts.
- Mouse and keyboard equivalence.
- Focus and disabled behavior.
- Empty data and large data.
- Resize and high-DPI geometry.
- Unload and disposal with an open popup, timer, or native subscription.
- Missing native service fallback.

Run `ComponentChecks.Run()` inside Rhino before showing the visual gallery. Add the control to `ShowGallery()` with enabled, focused, selected, empty, and disabled examples as applicable.

## Verify both platforms

Automated tests establish portable behavior. They do not certify Eto handlers or native Rhino integration.

On both a licensed Windows host and a provisioned Rhino Mac:

1. Load the exact candidate package into a small consumer or HostChecks.
2. Check dark and light Rhino themes.
3. Check normal and high-DPI/Retina displays.
4. Traverse controls with Tab and Shift+Tab.
5. Activate controls with their documented keys.
6. Exercise mouse, trackpad, scrolling, resizing, and popups.
7. Confirm disabled text remains readable.
8. Open and close the containing panel repeatedly and watch for retained callbacks or native handlers.
9. Compare the result with existing consumers before deleting their local component.

Record platform-specific limitations in [VALIDATION.md](VALIDATION.md). Do not describe a managed cross-build as native Windows sign-off.

## Migrate a consumer without parity loss

Move one component family at a time:

1. Capture the consumer's current geometry, events, shortcuts, disabled behavior, and native workaround.
2. Add equivalent tests or HostChecks in this repository.
3. Implement the shared component and document its contract.
4. Pack a new package version.
5. Update one consumer to the package and remove only the replaced implementation.
6. Run that consumer's tests and native smoke checks.
7. Compare dark/light, keyboard, DPI, and platform behavior.
8. Update the remaining consumers after the first migration passes.

Do not keep two active implementations behind runtime type discovery. Use an explicit package reference and an explicit platform build. Delete the old consumer control only after callers are migrated and parity is verified.

The 34px `FoundrySurface*` family preserves Rhino Maps geometry. Treat it as an explicit compatibility variant. Do not silently substitute it for the standard 32px controls or merge the dimensions without consumer sign-off.

## Document and release the component

For every new public control:

- Add XML documentation to the type and non-obvious members.
- Add a short entry and example to [USAGE.md](USAGE.md).
- Update the component list in the root [README](../README.md).
- Update [CONTRACTS.md](CONTRACTS.md) for shared ownership, input, or failure rules.
- Add a user-visible entry to [CHANGELOG.md](../CHANGELOG.md).
- Update migration notes when a consumer must change code.

Before packaging:

```sh
dotnet restore RhinoFoundry.UI.sln --locked-mode
dotnet build RhinoFoundry.UI.sln --no-restore -c Release
dotnet test tests/RhinoFoundry.UI.Tests -c Release --no-build
dotnet test tests/RhinoFoundry.UI.Primitives.Tests -c Release --no-build
dotnet pack src/RhinoFoundry.UI.Primitives -c Release --no-build -o artifacts/packages
dotnet pack src/RhinoFoundry.UI -c Release --no-build -o artifacts/packages
dotnet pack src/RhinoFoundry.UI.MacOS -c Release --no-build -o artifacts/packages
python3 scripts/validate-packages.py

git diff --check
```

Pack `RhinoFoundry.UI.MacOS` on the provisioned Mac. Require zero warnings and errors. Once any package version has been installed or distributed, publish fixes under a new version and update all co-installed consumers together.
