# Release and parity checklist

## Preflight correction — 2026-09-08

0.3.0-preview.10 installed and inspected using Computer in Rhino 8.34 on macOS,
dark theme. Verified seven selected-drawing badges flow horizontally at wide
width and wrap into two rows at narrow width; facts and the readiness card align
left in both sizes. Document geometry was not edited and no AI request was sent.
Light theme and Windows checks remain outstanding. Existing 17 shared tests and
478 consumer tests pass; both installed bundle hashes match their staged builds.

## Preflight summary — 2026-09-08

0.3.0-preview.8: managed builds, package validation and existing regression suites
pass. Native preflight replacement check added to HostChecks. Native visual,
dark/light, narrow-pane, Retina and Windows checks have not yet been performed
for this component; this prerelease is not cross-platform sign-off.

## Drawing Set workspace pass — 2026-09-08

Activity cards, expandable image previews and resource references were exercised
in Rhino 8.34 on macOS, dark theme/Retina. Real form submission reached a staged
plan, an edited proposal applied after one approval, and an inline sheet capture
and resource reference appeared. The sheet reference opened the actual layout.
Native review found a missing stretch alignment in the consumer's review layout;
the coordinated preview.7 approval surface fixes that composition.
Light theme, native Windows, reduced-motion behavior, very long histories and
full keyboard traversal are not signed off by this pass.

A successful build is not native sign-off. Record the exact package/binary hashes, Rhino build, OS, theme and scale with each result.

## Automated

- [ ] Locked restore and zero-warning Release build of portable projects on Windows.
- [ ] Pure UI and Primitives tests on .NET 8.
- [ ] Provisioned Mac build of native adapter; missing references fail.
- [ ] Package validator: matching versions, only intended DLLs, no bundled Rhino/Eto framework binaries.
- [ ] All four consumer solutions compile for explicit Windows and MacOS targets.
- [ ] Existing consumer core suites and native component contracts pass.
- [ ] Consumer bundle hashes match the shared package manifest.
- [ ] `git diff --check` in all five repositories.

## Licensed Rhino hosts: repeat on Windows and Mac

- [ ] Fully quit Rhino before loading a changed bundle; open all co-installed consumers in different orders.
- [ ] Component gallery: keyboard/disabled/focus, native text input, selects and numeric fields, accordion expansion and resizing, scroll stability, borrowed image lifetime.
- [ ] Layout: creation dialog, batch/page/layout accordions, table multi-selection and editing, thumbnail selection/drag, canvas tree wheel routing, two-finger pan/pinch and overlay transitions.
- [ ] Block: table, gallery and canvas fit/selection/zoom; established framing and glyphs unchanged.
- [ ] AI: chat/settings/approval surfaces open; no requests or mutations are sent during UI smoke checks.
- [ ] Maps: 34px variants preserve dimensions, field focus and selection, status/disabled behavior. Do not request location or download tiles for a presentation-only test.
- [ ] Repeat dark/light and high-DPI. Preserve and restore the user's theme preference.
- [ ] Close/reopen panels and dialogs; no duplicate events, timers or native monitors.

## Limits

Mouse-wheel automation cannot reproduce every physical two-finger/pinch gesture. A human trackpad check and native Windows sign-off remain release gates. UI migration does not replace the consumers' document safety, PDF, Undo or import recovery validation.
# Conversation rendering preview.15

Superseded by preview.18's native renderer: Rhino 8.35/macOS dark-theme host gallery
visually verified headings, emphasis, inline code, tables and numbered lists on
2026-09-09. Browser-backed candidates rendered blank and are not the current
implementation. Narrow/wide stress, native text chrome refinement, light theme,
Windows, keyboard traversal and repeated disposal still need a complete pass.

Markdown conversion tests cover headings, emphasis, code, tables and raw HTML
escaping. Circular composer disabled painting no longer overlays the glyph with
a square. Native Markdown scrolling, keyboard navigation, sizing, disposal, both
themes and Windows require host verification; managed tests are not sign-off.
# Preview.22 candidate limitations

Compiled against provisioned Rhino Mac assemblies. Native question paging,
tooltips, focus restoration, wheel forwarding and input-height checks are pending;
the active user conversation has unsent answers and was not restarted. Windows,
dark/light theme comparison and Retina interaction sign-off are also pending.
Do not interpret package validation as native visual acceptance.
# Preview.24 native follow-up

On Rhino Mac, mouse-through of the installed question control passed: select
both answers, Submit, replace/dispose component, no crash. Left-aligned text,
placeholder and right-aligned counter baseline visually checked. Preview.23
also passed back/forward retained-answer navigation. No API calls. Keyboard-only,
Windows and light-theme checks remain pending.
# Preview.34 extraction candidate — 2026-09-10

- Shared title and generic action icons compile with zero warnings/errors; coordinated three-package feed validates.
- 19 shared portable tests, 65 AI core tests, and 448 Layout core tests pass.
- Added document-free title commit/cancel, blank input, silent assignment, disabled and disposal HostChecks, plus gallery examples.
- Candidate is not yet installed or native-verified. Rhino was in a user conversation-export dialog; installation/restart was deferred to avoid interrupting it.
- Native macOS dark/light, Retina/input and Windows sign-off remain pending. No claim of native parity from the managed build alone.
