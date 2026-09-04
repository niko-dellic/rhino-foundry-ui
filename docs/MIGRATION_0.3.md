# 0.3 consumer migration record

Validated on 2026-09-04 with Rhino 8.34.26223.11002 on macOS (arm64, dark theme). Package candidate: `0.3.0-preview.1`. No package was published.

## Results

- Shared Release builds: UI, Primitives, MacOS and HostChecks, zero warnings/errors.
- Explicit Mac managed builds: Layout, AI, Block and Maps, zero warnings/errors.
- Explicit Windows managed cross-builds: Layout, AI, Block and Maps, zero warnings/errors. All Windows output excluded the Mac DLL and matched the UI/Primitives package hashes.
- Pure tests: Layout 350, Block 8, Maps 22, AI 7, Primitives 15 and UI 2; **404 passed**.
- Rhino native component checks: **7 passed** (keyboard/disabled actions, Maps geometry, checkbox, slider bounds/tooltip, independent accordion state, gallery keyboard/image ownership, canvas anchor/pan/cancel).
- Layout canvas routing checks: **5 passed** (short/hidden/empty tree bounds, overflowing/non-overflowing tree wheel ownership, ordinary canvas wheel zoom).
- Creation dialog scroll observation: five initialization size changes, still five after repeated scrolling and 48 idle samples. No recurring width/layout feedback was observed. Create was disabled and the dialog was cancelled.
- Mac smoke checks: Layout creation/list, Block table and canvas, Maps form and AI dialog opened from freshly restarted, coordinated bundles. AI sent no request; Maps did not connect, search, locate or stream.
- Package validator accepted one DLL per package, matching package/dependency versions and exact SHA-256 values. Development installers verified all three shared DLLs before copy; installed RHP/DLL bytes matched assembly outputs.
- `git diff --check` passed in all five repositories. Maps' existing untracked `external/` tree was not touched.

## Material limits and observations

Windows native input, Rhino light theme, and native Windows DPI were not tested on this Mac. The Windows managed builds and package exclusions establish build compatibility, not native parity; the stable release checklist still requires a licensed Windows Rhino host and both themes.

The preview-only creation smoke check changed Rhino's Modified flag from false to true even after cancel, while creating zero layouts. This matches the conservative preview-safety policy of leaving the document dirty when safe restoration cannot be proven, but should be included in the separate preview investigation and manual release sign-off.

Computer-generated wheel events and reflection checks do not fully reproduce a physical trackpad. The installed Mac adapter handled its native surfaces without duplicate-load errors, but the physical two-finger/pinch checklist remains a human sign-off item.
