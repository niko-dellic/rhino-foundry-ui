# Component contracts

Question cancellation never raises Submitted. Skip completes the question with an explicit unanswered marker. Submission/cancellation callbacks run after native input unwinds; hosts own replacement/disposal. Descriptions are optional and preserve string-option construction. Each sequence owns its answer and custom-draft arrays.

## Activity and review surfaces

ActivityTimeline owns its child rows; image data remains caller-owned until the
rows are removed/disposed. Expandable previews borrow the same image for their
modal lifetime. ResourceChip delegates activation through the standard button,
preserving its pointer, Enter/Space, focus and disabled behavior. ApprovalCard owns
its supplied child control but never creates an authorization or applies changes.
Hosts own retention limits, progress identities and workflow cancellation.

## Threading and lifetime

Construct, update and dispose controls on Rhino's Eto UI thread. The parent owns child controls. Dispose a removed top-level surface explicitly. `FoundryCanvas` owns frame/settle timers and its native subscription; unload stops input and removes monitors, reload attaches once, and dispose releases timers. `FoundryScrollable` checks disposal before running deferred width work. Native clipboard subscriptions are disposable and must be detached when their scope unloads.

Gallery item images are **borrowed**: the caller disposes them after replacing/removing all references. A gallery never deletes or generates Rhino previews. Icon drawing functions return caller-owned images. Native colors, pickers, file dialogs and application menus remain native.

## Coordinates and input

Camera world units are caller-defined; screen dimensions are Eto logical pixels, not Retina device pixels. Camera zoom preserves the world point beneath the screen anchor. Positive pan deltas translate content on screen. Mac precise scroll uses AppKit's system-mapped deltas with the established Foundry sign convention; pinch uses exponential magnification. Ordinary wheel input remains zoom. A canvas overlay predicate must cover occupied interactive rows, not an entire empty sidebar column.

Canvas input is coalesced at 60Hz and settled after 80ms. Pending input is discarded on unload, explicit cancellation or disposal. The base's camera is authoritative unless a subclass overrides `ApplyCameraInput`; that subclass owns synchronization of its camera state and zoom limits. `OnCameraFrame` is the rendering invalidation hook, `OnCameraSettled` is the downstream notification hook. Keep Rhino document work out of these handlers.

Generic selection keys are stable value types. Selection does not own documents, infer parent/child semantics, navigate, or mutate anything. A consumer supplies visible order and handles domain filtering.

## Keyboard and failure behavior

Buttons activate on Enter/Space; checkbox on Space; slider arrows/Home/End; gallery arrows/Home/End. Disabled controls ignore interaction. Native text editors keep their own shortcuts. The Mac clipboard adapter requires focused descendants, rejects text editors, and consults the consumer's edit-state predicate before invoking callbacks.

The library does not silently recover failed host mutations or swallow application exceptions. Consumer operations own cancellation, Undo, errors and recovery. Platform initialization is explicit; Windows does not require the Mac assembly. Both themes and native high-DPI behavior require host validation, beyond pure unit tests.

## Compatibility

The existing five-argument `FoundryFormField` constructor remains available; fixed-height fields use an additional overload. Existing 0.2 icon presets retain their drawings. Layout keeps public/persisted geometry and selection adapters while canonical algorithms live in Primitives. Prerelease API evolution is permitted, but persistence migrations are a consumer responsibility. Never introduce a schema reset as part of a UI extraction.
# Question and input ownership (preview.22)

`FoundryQuestionSequence` owns its generated controls and retains answer strings
while navigating. Only final Submit raises `Submitted`; it never grants document
authorization. Consumers own session persistence and the replacement composer.
Option buttons use the shared keyboard/focus contract and full-text tooltips.
`FoundryGrowingTextField` owns its supplied editor and coalesces measurement on
the UI queue. Empty input stays compact; input scrolls after reaching its cap.
# Content-height table contract

FoundryReadOnlyTable copies header/row values and requires at least one header. A supplied wrapping list must match the column count. Wrapping defaults off. Height is recalculated after width changes through a coalesced UI callback; there is no internal scrolling. Optional row actions execute asynchronously and are suppressed after disposal. System fonts remain framework-owned. FoundryChatComposer's optional leading action is a child of the composer and follows the normal Eto control lifetime.
# Editable title (preview.34)

`FoundryEditableTitle(value, width = 280)` requires a nonblank value and width ≥80 logical pixels. It is 32px high and uses shared button/field states. `Value` is the full trimmed title; assignment cancels editing without raising `Committed`. Pointer activation and Enter/Space start the same editor. Enter or focus loss commits a nonblank trimmed value and raises `Committed` once, including unchanged values so consumers can record manual-name intent. Escape, blank edits, disabling, unloading and disposal cancel without events. Keyboard completion restores button focus; blur completion does not steal focus. The caption truncates with an ellipsis; tooltip/editor preserve the full value. Child controls and font are component-owned. No native adapter, document or persistence dependency is required.
# Native action menus (preview.36)

`IFoundryNativeServices.ShowActionMenu(menu, anchor)` optionally shows a caller-owned native menu as an action popup, not an editor context menu. Call on the UI thread; true means handled, including cancellation; false means call Eto's `menu.Show(anchor)`. The Mac adapter uses `NSMenu.PopUpMenu` and temporarily disables context plugins, restoring the prior setting after tracking. It owns no menu, view, event subscription or document. Existing native service implementations default to false. Normal editor menus and OS file pickers are unchanged.
