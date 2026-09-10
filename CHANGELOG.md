# Changelog

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
