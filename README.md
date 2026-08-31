# Rhino Foundry UI

`RhinoFoundry.UI` is a small, accessible set of Eto.Forms controls for Rhino 8 plug-ins. It follows Rhino dark and light themes while providing consistent fields, buttons, toolbar groups, view switching, search fields, vector icons, checkboxes, sliders, and color triggers.

The package is intentionally presentation-only. It does not own document mutation, persistence, commands, panel registration, or product-specific domain behavior.

## Status

The `0.2.0-preview.1` API expands the initial control extraction with the product-neutral workspace pieces used by table, thumbnail, and canvas plug-in views. Preview releases may refine public names and constructor signatures before `1.0.0`.

## Install

```xml
    <PackageReference Include="RhinoFoundry.UI" Version="0.2.0-preview.1" />
```

Rhino plug-in projects should continue to reference the RhinoCommon version they target. Do not ship RhinoCommon or Eto runtime assemblies inside the plug-in bundle; Rhino provides them.

## Example

```csharp
using Eto.Drawing;
using Eto.Forms;
using RhinoFoundry.UI;

var nameInput = new TextBox { PlaceholderText = "Definition name" };
var nameField = new FoundryFormField(nameInput);

var apply = new FoundryDialogButton(
    "Apply",
    FoundryDialogButtonStyle.Primary);

var color = new FoundryColorField(
    Colors.CornflowerBlue,
    toolTip: "Choose a preview color");

var search = new FoundrySearchField("Search definitions");
var views = new FoundryViewModeSelector(FoundryViewMode.Table);
views.SelectedModeChanged += (_, args) => ShowView(args.Mode);
```

## Design and accessibility contract

- Single-line controls and toolbar actions are 32 px high.
- Controls expose normal Eto focus behavior with a neutral visible focus ring.
- Enter and Space activate buttons and color triggers.
- Space toggles checkboxes.
- Arrow keys operate sliders; Home and End reach their bounds.
- Icon-only controls require useful tooltips supplied by the consumer.
- `FoundryViewModeSelector` provides arrow-key traversal between table, thumbnail, and canvas modes.
- Native file pickers, color dialogs, context menus, and message boxes remain native.
- Consumers should verify dark/light themes and Retina/high-DPI scale in the Rhino versions they support.

## Contributing

Keep additions product-neutral and reusable by more than one Rhino plug-in. New controls must include rest, hover, pressed, focus, disabled, and keyboard states. Product-specific panels, icons, and document behavior belong in the consuming plug-in.

## License

MIT
