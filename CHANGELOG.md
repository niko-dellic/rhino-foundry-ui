# Changelog

## Unreleased

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
