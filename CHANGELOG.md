# Changelog

## 0.3.0-preview.1 — coordinated consumer migration

- Extract shared composites, viewport-width scroll guard, native table treatment and borrowed-image gallery from Layout.
- Introduce dependency-free Primitives for camera/grid/selection logic and a separate MacOS adapter for native gestures and scoped clipboard behavior.
- Preserve Layout's public record shapes through adapters and Block's existing glyphs and zoom limits.
- Add explicit Maps 34px variants without restyling existing screens.
- Add a native host contract runner/gallery, package hashes and Windows portable CI.
- Stable release remains gated on both licensed Rhino hosts; no claim of completed Windows native verification.
