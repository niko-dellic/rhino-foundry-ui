# Release and parity checklist

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
