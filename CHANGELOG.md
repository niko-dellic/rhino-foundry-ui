# 0.3.0-preview.58

- Preserve visible native table selection over per-cell row formatting and explicitly apply themed text colors inside plugin-owned dialogs.

- Add FormFont/FormEmphasisFont and opt-in FoundryTypography.ApplyForm for consistent Windows form typography, including disclosure headings and nested native editors.

- Detect Windows dark mode from Rhino paint colors and provide a neutral dark content surface.
- Remove nested native field borders; support Rhino 8 WPF editors and Rhino 9 composite numeric editors after lazy page creation.
- Add scoped native ComboBox and DataGrid presentation: dark dropdowns and headers, borderless cells, alternating rows, and column resize handles.
- Normalize compact Windows tab typography and paint opaque button resting surfaces.
- Preserve native editing and keyboard behavior; do not modify Rhino application styles.

# Changelog

## 0.3.0-preview.33

- Content-height read-only tables with per-column wrapping; Markdown tables wrap and no longer trap scrolling.
- Optional leading composer action for attachment menus.

## 0.3.0-preview.32

- Preserve Eto-owned system fonts when drawing rich question rows; repeated native paints no longer dispose shared fonts.

## 0.3.0-preview.31

- Rich question rows with descriptions, recommendation badges, pencil trigger, Skip and asynchronous Cancelled event.
- Compact muted activity rows distinguish actions from communication.

## Unreleased

## 0.3.0-preview.30 — table width fitting

- Fit Markdown table columns to the actual grid bounds, reserving native gutter and column spacing. Allow columns to shrink instead of forcing unnecessary horizontal overflow.

## 0.3.0-preview.18 — native Markdown

- Replace the unreliable Rhino-hosted browser display with native formatted text,
  lists and tables. No per-message browser, remote navigation or image fetching.

## 0.3.0-preview.17 — mounted Markdown lifecycle

- Load generated HTML after the native browser mounts and ignore its initial blank-page event.

## 0.3.0-preview.16 — native Markdown loading

- Permit Eto's initial generated local HTML document, then block further navigation.

## 0.3.0-preview.15 — readable conversation content

- Add read-only Markdown messages with headings, emphasis, code, lists and tables, using Markdig.
- Block raw HTML, remote resources and navigation in the Markdown surface.
- Remove the rectangular disabled overlay from circular composer actions.

## 0.3.0-preview.14 — chat resize correction

- Clear a message label's previous height before measuring wrapped text after pane resizing.

## 0.3.0-preview.13 — conversation surfaces

- Add width-limited incoming/outgoing chat messages and a rounded multiline composer with embedded actions.
- Add a high-DPI conversation icon. Native editors retain keyboard and accessibility behavior.

## 0.3.0-preview.12 — wrapping and composer controls

- Constrain activity labels to available width and wrap multiline text.
- Add a high-contrast circular composer variant to the shared icon button.

## 0.3.0-preview.11 — conversation presentation

- Replace boxed activity rows with a quiet left-aligned timeline.
- Reuse rounded thumbnail-gallery cards for compact, keyboard-accessible capture inspection.

## 0.3.0-preview.10 — preflight layout correction

- Wrap preflight badges using the selector's badge renderer in read-only mode.
- Anchor facts and approval-card headings to the leading edge under Rhino styling.

## 0.3.0-preview.8 — preflight summary

- Add a read-only preflight summary with value badges and labeled, wrapping facts.

## 0.3.0-preview.7 — review surface

- Add a neutral, stretch-aligned approval card for consumer-owned editors and actions.

## 0.3.0-preview.6 — activity surfaces

- Add reusable activity timeline/cards with expandable image previews and keyboard-accessible resource references.
- Package all three shared assemblies together; native Windows verification remains outstanding.

## 0.3.0-preview.4 — picker accessories

- Let filtered pickers host a fixed footer for related controls such as an orientation segment while keeping results scrollable and table cells compact.

## 0.3.0-preview.3 — compact table editors

- Let text segmented controls opt into the shared 24px table row height while preserving their existing 32px default, interaction states, focus behavior, and keyboard navigation.

## 0.3.0-preview.2 — rebuilt coordinated candidate

- Rebuild the current shared UI source as a new immutable prerelease package set for Layout, AI, Block, and Maps.
- Include the consumer and component implementation documentation added after preview.1.

- Add practical consumer setup, component examples, ownership guidance, and a shared-component implementation checklist.

## 0.3.0-preview.1 — coordinated consumer migration

- Extract shared composites, viewport-width scroll guard, native table treatment and borrowed-image gallery from Layout.
- Introduce dependency-free Primitives for camera/grid/selection logic and a separate MacOS adapter for native gestures and scoped clipboard behavior.
- Preserve Layout's public record shapes through adapters and Block's existing glyphs and zoom limits.
- Add explicit Maps 34px variants without restyling existing screens.
- Add a native host contract runner/gallery, package hashes and Windows portable CI.
- Stable release remains gated on both licensed Rhino hosts; no claim of completed Windows native verification.
# 0.3.0-preview.22

- Add composer-based question sequence and bounded growing text fields.
- Align quiet disclosure titles with their content.
- Forward read-only Markdown wheel input to the containing conversation.
# 0.3.0-preview.24

- Defer question page replacement and final submission beyond native input callbacks.
- Bind editor changes to their page instead of the currently displayed question.
- Left-align questions/options, right-align navigation and add custom-answer placeholder.
- Correct counter baseline following native mouse-through verification of preview.23.
# 0.3.0-preview.26

- Growing text fields suppress the native macOS border inside their shared shell.
- Chat composer height reserves the action row and padding as the editor grows.
# 0.3.0-preview.27

- Markdown paragraph height accounts for native macOS rich-text layout runs;
  portable measurement remains the fallback. Native acceptance pending.
# 0.3.0-preview.29

- Fixed zero-height Eto text measurement by using finite measurement bounds.
  Native long Markdown paragraphs now expand without internal scrollbars;
  chat messages and growing editors use the same corrected measurement bound.
# 0.3.0-preview.35

- Borderless 13pt heading title presentation and coordinated consumer package refresh. Supersedes the uninstalled preview.34 candidate.

# 0.3.0-preview.34

- Editable titles use borderless heading typography, retaining hover and accessible keyboard focus.

- Extracted a product-neutral editable title with silent programmatic assignment and explicit user commits.
- Added generic send, stop and camera icons, preserving composer inverse foreground and multi-resolution rendering.
- Added document-free native title lifecycle/input checks and gallery examples. Windows and light-theme native validation remain pending.
# 0.3.0-preview.36

- Native action popups avoid text-context menu augmentation for toolbar actions. Normal editor context menus are unchanged. Native verification pending installation.
# 0.3.0-preview.37

- Reduced activity row vertical padding and timeline gaps.
- Added muted accordion activity styling for answer histories and similar disclosures.

## 0.3.0-preview.42 (local, unpublished)

- Centralize theme-aware icon factories and unload-safe theme observation.
- Center toolbar separators and refresh their colors; update embedded search and multi-select icons.
- Share generic hierarchy glyphs, contextual help dialogs and destructive confirmation presentation.
- Preserve the borrowed-image constructor and consumer-owned operation behavior.

- Native checks also fixed idempotent disposal and editable-title ownership of an Eto-cached font. Preview.41 was an intermediate local validation package; preview.42 is the final migration candidate.

## 0.3.0-preview.43 (local candidate)

- Extract responsive form rows/pages from Navigation and Maps, including deferred native layout and unload guards.
- Share keycap/mouse drawing and pure notice timing; viewport lifecycle remains consumer-owned.
- Document Rhino 9 dock padding and live appearance integration. Includes existing preview.42 theme work.
