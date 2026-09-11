# Consumer guide

### Activity disclosures

```csharp
var answers = new FoundryAccordionItem("Answers submitted",
    new FoundryChatMessage(questionAndAnswerText, outgoing: false), isActivity: true);
```

Starts collapsed; click or Enter/Space toggles. Activity styling uses muted text and no separator; normal accordions retain their existing appearance. No submission or permission behavior is owned by the disclosure.

### Toolbar action menu

```csharp
if (FoundryNative.Services?.ShowActionMenu(menu, toolbarButton) != true)
    menu.Show(toolbarButton);
```

The caller owns the menu and its actions. Use this for toolbar action popups, not text-editor context menus.

### Editable titles and action icons

Titles use a borderless 13pt bold heading action at rest, a quiet hover surface, and a neutral keyboard focus ring. The active rename editor retains its field focus treatment. `FoundryDialogButtonStyle.Heading` exposes the same presentation for other heading actions.

```csharp
var title = new FoundryEditableTitle("Untitled chat", width: 280);
title.Committed += (_, _) => SaveName(title.Value); // includes unchanged explicit commits
title.Value = loadedName; // silent; cancels any open edit
var sendIcon = FoundryViewIcons.Send(inverse: true);
var stopIcon = FoundryViewIcons.Stop(inverse: true);
var cameraIcon = FoundryViewIcons.Camera();
```

Click or Tab then Enter/Space starts editing. Enter commits, Escape cancels, focus loss commits; blank input cancels. The title owns its native editors. The consumer owns returned icon images and chooses normal or inverse send/stop foreground for the host surface. Icons have 1×/2×/3× frames; recreate after a theme change.

### Rich questions

```csharp
var questions = new FoundryQuestionSequence(new[] {
    new FoundryQuestion("Which scope?", new[] { "Selected (Recommended)", "All" }) {
        Descriptions = new[] { "Review selected items.", "Review every available item." }
    }
});
questions.Submitted += (_, _) => ConsumeAnswers(questions.Answers);
questions.Cancelled += (_, _) => RestoreComposer();
```

Skip records `[Skipped — no answer supplied]`, not approval. X/Escape cancels without submitting. Enter/Space activates an option; Enter submits custom text. Full options are available as tooltips.

### Chat surfaces

`FoundryChatMessage` wraps plain text to 86% of the available width (maximum 760 logical pixels).
Outgoing messages align right on the same neutral surface as `FoundryChatComposer`;
incoming text aligns left without a background. The host owns scrolling.

```csharp
timeline.Items.Add(new FoundryChatMessage("Please review this drawing.", outgoing: true));
timeline.Items.Add(new FoundryChatMessage("Here is the review.", outgoing: false));
var composer = new FoundryChatComposer(editor, sendButton, stopButton);
var conversationIcon = FoundryViewIcons.Conversation();
```

The composer hosts caller-supplied controls; the host owns send/stop behavior and visibility.
Native multiline editing, Tab traversal, and button keyboard/focus states are preserved.

`FoundryToolbarIconButton` supports `IsComposerAction = true` for circular send/stop
actions. Supply a vector icon drawn in `FoundryTheme.PanelBackground`; its surface
uses `PrimaryText`. Existing mouse, Enter/Space, focus and disabled handling apply.
Activity text in preview.12 is width-constrained and reflows with the pane.

Activity cards in 0.3.0-preview.11 use borderless text rows and a 280px capture
thumbnail with an integrated inspection caption. Images remain caller-owned.
Click or keyboard selection opens the existing image inspector; closing it resets
selection so the same capture can be opened again.

In 0.3.0-preview.10, `FoundryRemovableBadge(text, removable: false)` provides
the same badge styling without a remove glyph, focus stop or activation.
Preflight summaries wrap these badges with available width. Queued resize work
checks disposal and unchanged widths; all updates belong on the UI thread.

## Preflight summary (0.3.0-preview.8)

```csharp
var summary = new FoundryPreflightSummary();
summary.SetSummary(new[] { "Floor plans", "Sections" },
    new Dictionary<string, string> { ["Destination"] = "Root", ["Page"] = "A3 landscape" });
```

Read-only presentation: no focus stops or approval events. Facts wrap beneath
their labels on narrow panes. Each update disposes the previous owned content;
call on the UI thread. Consumers own validation, readiness messages and actions.

## Activity surfaces (0.3.0-preview.6)

Version 0.3.0-preview.7 also provides `new FoundryApprovalCard("Review task", reviewContent)`.
Pass a stretch-aligned StackLayout containing the consumer's fields and actions.
The card adds a neutral heading/border, not authorization logic or event handlers.

```csharp
var timeline = new FoundryActivityTimeline();
timeline.AddActivity("Inspecting", "Completed", "Model overview captured", previewImage);
var card = new FoundryActivityCard("Review task", "Waiting", "Check the proposed scope");
var reference = new FoundryResourceChip("Sheet", "A01", () => OpenSheet());
timeline.Items.Add(reference);
```

Call on the Eto UI thread. Keep images alive until the surface is disposed; the
caller owns their disposal. `Inspect image` opens a resizable preview and Escape
closes it. Resource activation reuses the standard button keyboard/focus states.
Hosts own scrolling, retention limits and any binding of activity IDs to rows.
No control starts a model request or accesses a Rhino document.

This guide shows how a Rhino 8 plugin consumes Rhino Foundry UI without moving product behavior into the component library. Examples target .NET 8 and Eto.Forms on Windows and macOS.

## 1. Choose the packages

Use only the packages needed by each project:

| Consumer project | Package | Purpose |
| --- | --- | --- |
| Domain or test project | `RhinoFoundry.UI.Primitives` | Host-independent camera, geometry, selection, grid, and thumbnail layout logic |
| Eto presentation project | `RhinoFoundry.UI` | Shared controls, themes, tables, scroll behavior, and canvas input |
| Mac composition project | `RhinoFoundry.UI.MacOS` | AppKit trackpad, clipboard, and native table adapters |

Pin all Foundry packages to the same exact version. Do not use floating versions for coordinated package updates.

```xml
<!-- Directory.Packages.props -->
<Project>
  <PropertyGroup>
    <ManagePackageVersionsCentrally>true</ManagePackageVersionsCentrally>
  </PropertyGroup>
  <ItemGroup>
    <PackageVersion Include="RhinoFoundry.UI" Version="0.3.0-preview.40" />
    <PackageVersion Include="RhinoFoundry.UI.Primitives" Version="0.3.0-preview.40" />
    <PackageVersion Include="RhinoFoundry.UI.MacOS" Version="0.3.0-preview.40" />
  </ItemGroup>
</Project>
```

The version above is a development example; use the exact version of your downloaded bundle. Obtain all three `.nupkg` files and their `foundry-ui-manifest.json` from release assets or a package feed. For local consumption, put them in `packages/`, add `/packages/` to the consumer’s `.gitignore`, and configure that directory as a NuGet source. Provision these files before locked restore in CI; do not commit package binaries or the generated manifest:

```xml
<?xml version="1.0" encoding="utf-8"?>
<!-- NuGet.Config -->
<configuration>
  <packageSources>
    <clear />
    <add key="foundry-local" value="packages" />
    <add key="nuget.org" value="https://api.nuget.org/v3/index.json" protocolVersion="3" />
  </packageSources>
</configuration>
```

Enable lock files and restore with `--locked-mode` in CI and release builds. Commit the resulting lock files. Verify packaged consumer DLLs with `python3 scripts/verify-shared-ui.py <bundle-directory> MacOS --manifest packages/foundry-ui-manifest.json` (use `Windows` for Windows bundles). The PowerShell equivalent requires `-Manifest packages/foundry-ui-manifest.json`.

## 2. Keep platform references explicit

A presentation project always references the portable UI package. Add the Mac adapter only to Mac builds.

```xml
<Project Sdk="Microsoft.NET.Sdk">
  <PropertyGroup>
    <TargetFramework>net8.0</TargetFramework>
    <FoundryPlatform Condition="'$(FoundryPlatform)' == '' and $([MSBuild]::IsOSPlatform('OSX'))">MacOS</FoundryPlatform>
    <FoundryPlatform Condition="'$(FoundryPlatform)' == '' and $([MSBuild]::IsOSPlatform('Windows'))">Windows</FoundryPlatform>
  </PropertyGroup>

  <ItemGroup>
    <PackageReference Include="RhinoFoundry.UI" />
    <PackageReference Include="RhinoCommon" ExcludeAssets="runtime" />
  </ItemGroup>

  <PropertyGroup Condition="'$(FoundryPlatform)' == 'MacOS'">
    <DefineConstants>$(DefineConstants);FOUNDRY_SHARED_MACOS</DefineConstants>
  </PropertyGroup>
  <ItemGroup Condition="'$(FoundryPlatform)' == 'MacOS'">
    <PackageReference Include="RhinoFoundry.UI.MacOS" />
  </ItemGroup>
</Project>
```

Initialize native services once, from the consumer's UI composition boundary, before any shared controls are loaded:

```csharp
namespace ExamplePlugin.UI;

internal static class SharedUiPlatform
{
    public static void Initialize()
    {
#if FOUNDRY_SHARED_MACOS
        RhinoFoundry.UI.MacOS.FoundryMacOS.Initialize();
#endif
    }
}
```

Call `SharedUiPlatform.Initialize()` from plugin or panel startup on Rhino's UI thread. Windows must not reference or ship `RhinoFoundry.UI.MacOS.dll`.

## Component map

| Need | Use | Consumer still owns |
| --- | --- | --- |
| Dialog or inline text action | `FoundryDialogButton`, `FoundryDialogActions` | Validation and command execution |
| Toolbar icon action or modes | `FoundryToolbarIconButton`, `FoundryToolbarButtonGroup`, `FoundryViewModeSelector`, `FoundryToolbarSeparator` | Mode meaning and commands |
| Text, numeric, or select shell | `FoundryFormField`, `FoundryToolbarField`, `FoundrySearchField` | Values and validation |
| Boolean, color, or numeric range | `FoundryCheckBox`, `FoundryColorField`, `FoundrySlider` | Domain mapping |
| Expandable settings | `FoundryAccordion`, `FoundryScrollable` | Section content and state persistence |
| Filtered single or multi choice | `FilteredPicker`, `FoundryMultiSelectField` | Choice source and validation |
| Compact status or removable value | `FoundryBadgeCell<T>`, `FoundryRemovableBadge` | Status meaning and removal command |
| Flat or hierarchical data | `FoundryGridView`, `FoundryTreeGridView`, `FoundryTable` | Rows, columns, editing, and selection meaning |
| Image choices | `FoundryThumbnailGallery` | Images, image disposal, and domain drag format |
| Resizable panes | `FoundryPaneResizeHandle` | Pane bounds, collapsed size, and persistence |
| Text modes | `FoundryTextSegmentedControl` | Labels and selected-mode meaning |
| Pan and zoom surface | `FoundryCanvas` | Rendering, world units, overlays, and selection |
| Pure interaction math | `RhinoFoundry.UI.Primitives` | Domain identity and application rules |
| Preserved Maps geometry | `FoundrySurfaceButton`, `FoundryInsetFormField`, `FoundrySurfaceTheme` | Deciding that 34px compatibility is required |

`FoundryAccordionTrigger` is exposed for custom compositions, but prefer `FoundryAccordionItem` so trigger and content visibility remain synchronized. `FoundryNative` is an advanced adapter boundary; normal consumers initialize the platform package and use higher-level controls.

## 3. Build a form from shared controls

Foundry controls own presentation and input behavior. The consumer owns values, validation, commands, and document changes.

```csharp
using Eto.Forms;
using RhinoFoundry.UI;

var nameInput = new TextBox { PlaceholderText = "Layout name" };
var nameField = new FoundryFormField(nameInput, fixedHeight: 32);
var includeTitleBlock = new FoundryCheckBox("Include title block", isChecked: true);
var scale = new FoundrySlider(25, 200, 100, toolTipFormatter: value => $"{value}%");

var create = new FoundryDialogButton("Create", FoundryDialogButtonStyle.Primary);
var cancel = new FoundryDialogButton("Cancel", FoundryDialogButtonStyle.Secondary);

var dialog = new Dialog
{
    Title = "Create layout",
    Content = new StackLayout
    {
        Padding = FoundryTheme.Space4,
        Spacing = FoundryTheme.Space3,
        Items =
        {
            nameField,
            includeTitleBlock,
            scale,
            new StackLayout
            {
                Orientation = Orientation.Horizontal,
                Spacing = FoundryTheme.Space2,
                Items = { new StackLayoutItem(null, true), cancel, create },
            },
        },
    },
};

create.Click += (_, _) =>
{
    // Validate input and call the consumer's application service here.
};
cancel.Click += (_, _) => dialog.Close();
FoundryDialogActions.Bind(dialog, accept: create, cancel: cancel);
```

Use `FoundryColorField` when a Foundry-owned trigger should open the native color dialog:

```csharp
var color = new FoundryColorField(Eto.Drawing.Color.FromArgb(70, 120, 170));
color.ValueChanged += (_, _) => viewModel.Color = color.Value;
```

File pickers, color dialogs, context menus, and message boxes remain native. The shared control may own the styled trigger, as `FoundryColorField` does.

## 4. Use accordions inside the shared scroll container

`FoundryScrollable` coalesces content-width updates and avoids AppKit layout feedback during scrolling. Use it for tall configuration panels containing accordions.

```csharp
var batchSection = new FoundryAccordionItem(
    "Batch",
    BuildBatchFields(),
    isExpanded: true);
var pageSizeSection = new FoundryAccordionItem(
    "Page size",
    BuildPageSizeFields(),
    isExpanded: true);
var layoutSection = new FoundryAccordionItem(
    "Layout",
    BuildLayoutFields());

var accordion = new FoundryAccordion(batchSection, pageSizeSection, layoutSection);
var scrollable = new FoundryScrollable(accordion);
```

Do not wrap this composition in another width-synchronizing loop. `FoundryScrollable` responds to its own `SizeChanged` and `LoadComplete` events. Call `RequestWidthSync()` only when an external host changes the viewport without raising either event.

## 5. Pickers and multi-select fields

`FilteredPicker` accepts a value typed by the user and exposes `ContainsChoice` when the consumer requires a listed value.

```csharp
var picker = new FilteredPicker(
    new[] { "A0", "A1", "A2", "A3", "A4" },
    placeholder: "Choose a page size");

picker.SelectionCommitted += (_, _) => viewModel.PageSize = picker.Text;
picker.DismissRequested += (_, _) => ValidatePageSize(picker.Text);
```

For a compact table editor, compose the picker with the shared segmented control. Its optional
`controlHeight` keeps the interaction states and keyboard behavior intact at the table's 24px row height.

```csharp
var orientation = new FoundryTextSegmentedControl(
    new[] { "Portrait", "Landscape" },
    segmentWidth: 70,
    controlHeight: FoundryTheme.TableRowHeight);
```

Pass an accessory such as the same segmented control through `popupFooter` when it must remain
visible below a picker's results instead of consuming width in a compact table cell.

```csharp
var pickerWithOrientation = new FilteredPicker(
    new[] { "A0", "A1", "A2", "A3", "A4", "Custom…" },
    placeholder: "Choose a page size",
    popupFooter: orientation);
```

`FoundryMultiSelectField` supports listed choices and custom values. Treat `Values` as a snapshot and update it through `SetValues` or `AddValues`.

```csharp
var drawings = new FoundryMultiSelectField(new[]
{
    new FoundryMultiSelectChoice("Plans", "Floor plan"),
    new FoundryMultiSelectChoice("Plans", "Reflected ceiling plan"),
    new FoundryMultiSelectChoice("Details", "Wall section"),
});

drawings.SetValues(viewModel.DrawingTypes);
drawings.ValueChanged += (_, _) =>
    viewModel.DrawingTypes = drawings.Values.ToArray();
```

## 6. Toolbars and view modes

Use `FoundryToolbarIconButton` for a single 32px icon action. Set a useful tooltip. Use `FoundryViewModeSelector` for the standard table, thumbnail, and canvas choice.

```csharp
var viewMode = new FoundryViewModeSelector(FoundryViewMode.Table);
viewMode.SelectedModeChanged += (_, args) => ShowView(args.Mode);

var addLayout = new FoundryToolbarIconButton(
    FoundryViewIcons.NewLayout(),
    "Create layout");
addLayout.Click += (_, _) => ShowCreateDialog();
```

For a product-specific mutually exclusive group, the consumer owns exclusivity by setting `Checked` on every button when a mode changes. `FoundryToolbarButtonGroup` supplies the capsule, spacing, focus movement, and arrow-key navigation.

Use `FoundrySearchField` for the standard embedded-icon search box. `FoundryToolbarField` wraps a different native editor in a 32px toolbar shell. Add `FoundryToolbarSeparator` between unrelated groups.

```csharp
var search = new FoundrySearchField("Filter layouts", width: 240);
search.TextChanged += (_, _) => ApplyFilter(search.Text);

var density = new FoundryTextSegmentedControl(
    new[] { "Compact", "Comfortable" },
    selectedIndex: 0,
    segmentWidth: 96,
    leadingLabel: "Rows",
    leadingLabelWidth: 60);
density.SelectedIndexChanged += (_, _) => SetCompactRows(density.SelectedIndex == 0);
```

### Resizable panes

`FoundryPaneResizeHandle` reports deltas and collapse requests. It does not decide minimum sizes or mutate panes, so the consumer can apply its own bounds and persistence rules.

```csharp
var handle = new FoundryPaneResizeHandle(
    FoundryPaneResizeAxis.Horizontal,
    leadingPaneName: "Hierarchy");

handle.ResizeRequested += (_, args) =>
{
    hierarchyWidth = Math.Clamp(hierarchyWidth + args.Delta, 180, 520);
    ApplyPaneWidths();
};
handle.CollapseToggleRequested += (_, _) =>
{
    handle.IsCollapsed = !handle.IsCollapsed;
    ApplyPaneWidths();
};
```

A horizontal axis reports X deltas for side-by-side panes. A vertical axis reports Y deltas for stacked panes. Double-click and keyboard behavior are implemented by the handle; the consumer applies the requested result.

## 7. Tables

The consumer supplies columns, data, selection, and commands. The shared library supplies row height, typography, zebra presentation, and native platform adaptation.

```csharp
var table = new FoundryGridView
{
    DataStore = rows,
    AllowMultipleSelection = true,
};

table.Columns.Add(new GridColumn
{
    HeaderText = "Name",
    DataCell = new TextBoxCell { Binding = Binding.Property<RowModel, string>(row => row.Name) },
});

FoundryTable.Configure(table);
table.CellFormatting += (_, args) =>
{
    var selected = table.SelectedRows.Contains(args.Row);
    FoundryTable.FormatCell(args, selected);
};
```

For a status column, use `FoundryBadgeCell<T>` and map domain state to `Neutral`, `Warning`, or `Error`. If a cell adds its own semantic background, call `FoundryTable.SetCellBackground`; the helper preserves native macOS selection painting.

For trees, use `FoundryTreeGridView`. Call `ConfigureAlternatingRows()` after the tree is loaded when needed, and `RestoreSelectedRows(rows)` after replacing its data store.

## 8. Thumbnail galleries

The consumer creates and disposes thumbnail images. `FoundryThumbnailGallery` borrows them; replacing items does not dispose old images.

```csharp
var gallery = new FoundryThumbnailGallery
{
    EmptyText = "No templates",
    DragDataFormat = "com.example-plugin.template",
};

gallery.SetItems(thumbnails.Select(item =>
    new FoundryThumbnailItem(item.Name, item.Image)));
gallery.SetSelectedName(viewModel.SelectedTemplate);
gallery.SetLayout(availableWidth: 720, requestedTileWidth: 180);

gallery.SelectionChanged += (_, args) =>
    viewModel.SelectedTemplate = args.Name;
```

Call `SetLayout` when the available width or desired density changes. Use `ContentHeight` to size an enclosing layout. Keep domain-specific card drawing in the consumer when the standard image-and-label card is insufficient.

## 9. Canvas foundations

`FoundryCanvas` owns portable camera math, 60 Hz input batching, settle notifications, and native gesture subscriptions. A consumer subclass owns world units, rendering, selection, document content, and overlay geometry.

```csharp
using Eto.Drawing;
using Eto.Forms;
using RhinoFoundry.UI;
using RhinoFoundry.UI.Primitives;

internal sealed class SheetCanvas : FoundryCanvas
{
    private RectangleF _toolbarBounds;

    public SheetCanvas()
    {
        Paint += Draw;
        CameraChanged += (_, _) => UpdateVisibleEditorPositions();
        CameraSettled += (_, _) => PersistViewportIfNeeded();
    }

    public void Fit(FoundryRect worldBounds)
    {
        CanvasCamera = FoundryCamera.Fit(
            worldBounds,
            new FoundrySize(Math.Max(1, Width), Math.Max(1, Height)));
        Invalidate();
    }

    protected override bool IsCanvasOverlay(PointF point) =>
        _toolbarBounds.Contains(point);

    private void Draw(object? sender, PaintEventArgs args)
    {
        var viewport = new FoundrySize(Math.Max(1, Width), Math.Max(1, Height));
        foreach (var sheet in GetVisibleSheets())
        {
            var bounds = CanvasCamera.WorldToScreen(sheet.WorldBounds, viewport);
            args.Graphics.FillRectangle(
                Colors.White,
                new RectangleF((float)bounds.X, (float)bounds.Y,
                    (float)bounds.Width, (float)bounds.Height));
        }
    }
}
```

Return `true` from `IsCanvasOverlay` for every control or interactive strip drawn over the canvas. This prevents trackpad and wheel input from moving the camera while the pointer is over an overlay. If the product already has a zoom policy, set `UseDefaultWheelZoom = false` in the subclass and override `ApplyCameraInput`.

Do not attach AppKit handlers in a consumer canvas. The Mac adapter owns recognizers and disposes them when the control unloads.

### Advanced native clipboard scope

Most consumers do not call `FoundryNative` directly. Use its clipboard hook only when the consumer already owns copy and paste commands for a non-text surface. Keep the returned subscription and dispose it when the surface unloads. The Mac service rejects focused text editors so standard editing shortcuts continue to work.

```csharp
private IDisposable? _clipboardSubscription;

private void AttachNativeShortcuts(Control scope)
{
    _clipboardSubscription?.Dispose();
    _clipboardSubscription = FoundryNative.Services?.AttachClipboardShortcuts(
        scope,
        canHandle: () => CanEditSelection(),
        copy: CopySelection,
        paste: PasteSelection);
}

private void DetachNativeShortcuts()
{
    _clipboardSubscription?.Dispose();
    _clipboardSubscription = null;
}
```

Attach after the surface loads and detach on unload and disposal. `FoundryNative.Services` may be `null`; keep the consumer's portable command path available.

## 10. Host-independent primitives

Reference `RhinoFoundry.UI.Primitives` from domain or test code when interaction policy does not require Eto or Rhino.

```csharp
var selection = new FoundrySelectionModel<Guid>();
selection.Replace(new[] { firstId }, firstId);
selection.Toggle(secondId);
selection.SelectRange(visibleIds, lastId, additive: false);
selection.Prune(existingIds);

var gridSpacing = FoundryCanvasGridPolicy.EffectiveWorldSpacing(camera.Zoom);
var layout = FoundryThumbnailGridLayout.CreateForDensity(
    itemCount: items.Count,
    availableWidth: viewportWidth,
    density: 0.5);
var visible = layout.VisibleIndices(scrollTop, scrollBottom);
```

These types are deterministic and can be covered by ordinary unit tests without initializing Eto.

## 11. Ownership and cleanup

Use these ownership rules consistently:

- Consumers own domain data, view models, commands, Rhino documents, persistence, preview generation, and product workflows.
- Consumers own images passed to `FoundryThumbnailGallery` and dispose them when no control can still paint them.
- Shared controls own their native adapters, timers, popups, and internal drawing resources.
- Close dialogs and popups from the UI thread. Do not update an Eto control from background work.
- Unsubscribe consumer event handlers when a longer-lived publisher references a shorter-lived view. Control-owned events normally die with the control.
- Treat coordinates, sizes, and wheel deltas as logical Eto units. Do not add manual Retina scaling.

See [contracts](CONTRACTS.md) for the full lifetime, input, and failure guarantees.

## 12. Package and bundle checks

A consumer release bundle must contain `RhinoFoundry.UI.dll` and `RhinoFoundry.UI.Primitives.dll` beside the RHP. A Mac bundle also contains `RhinoFoundry.UI.MacOS.dll`; a Windows bundle does not.

Build both configurations explicitly:

```sh
dotnet restore YourPlugin.sln --locked-mode -p:FoundryPlatform=MacOS
dotnet build YourPlugin.sln --no-restore -c Release -p:FoundryPlatform=MacOS

dotnet restore YourPlugin.sln --locked-mode -p:FoundryPlatform=Windows
dotnet build YourPlugin.sln --no-restore -c Release -p:FoundryPlatform=Windows
```

Run Windows commands on Windows and Mac commands on a provisioned Rhino Mac. Test the exact assembled bundle on clean Rhino profiles. Compare SHA-256 hashes between build output and installed files, then fully quit and reopen Rhino between bundle updates.
# Read-only Markdown conversation content

```csharp
var message = new FoundryMarkdownMessage("### Review\n\n**A01** is ready.\n\n| Sheet | Status |\n|---|---|\n| A01 | Reuse |");
```

The control owns native read-only rich-text and table controls and remeasures
after width changes. Links display their labels without navigation; raw HTML is
literal text and images display alt text without fetching. Captured images belong
in the thumbnail gallery. Deploy the transitive Markdig dependency. Theme colors
are captured at construction. No embedded browser instances are retained.
# Composer questions and growing text (preview.22)

```csharp
var questions = new FoundryQuestionSequence(new[] {
    new FoundryQuestion("Which scale?", new[] { "1:100 (Recommended)", "1:50" }),
    new FoundryQuestion("Presentation?", new[] { "Monochrome", "Colour" })
});
questions.Submitted += (_, _) => SubmitAnswers(questions.Answers);
composerHost.Content = questions;
var customAnswer = new FoundryGrowingTextField(new TextArea(), maximumHeight: 120);
```

Question controls own their children. Arrow buttons navigate without sending;
option selection stores an answer and advances. Final Submit is enabled only
when all answers are nonempty. Consumer owns submission, authorization and
restoring the normal composer. Full option text is available as a tooltip.
# Question sequence fixes (preview.24)

Answer rows use `FoundryDialogButton.LeftAlignText`; the default remains centered
for other actions. `FoundryGrowingTextField` accepts `placeholder: "Say something else..."`.
Question navigation coalesces UI updates after native callbacks return, retains
per-question text, and queues final submission before consumers may dispose it.
# Content-height conversation tables

```csharp
var table = new FoundryReadOnlyTable(
    ["Sheets", "Finding", "Follow-up"], rows,
    wrapColumns: [false, true, true]);
var composer = new FoundryChatComposer(editor, send, stop, attachmentButton);
```

Place the table in the conversation's scroll container. Do not wrap it in another scrolling surface. All values have full-text tooltips; optional row actions expose keyboard-accessible Open buttons.

### Custom tree selection color

```csharp
FoundryTable.ConfigureSelectionColor(tree, () => selectionColor);
tree.CellFormatting += (_, e) => FoundryTable.FormatCell(e, tree.SelectedItems.Contains(e.Item), selectionColor);
// After changing selectionColor, call tree.ReloadData().
```
The caller owns the color; no Rhino or OS preference is modified. The native adapter retains Eto editing/selection behavior and paints the selected row using that color.
