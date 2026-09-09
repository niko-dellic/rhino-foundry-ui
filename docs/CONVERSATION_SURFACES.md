# Conversation surfaces (preview.19)

`FoundryAccordionTrigger.Quiet` suppresses pointer hover/pressed surface fills while retaining keyboard focus feedback and activation. Use it for disclosure headers inside document-like cards.

`FoundryFormField.IsReadOnlySurface` suppresses the field's outer hover/focus treatment for borrowed read-only card content. Interactive children retain their own focus feedback. This is not an authorization or editability control.

`FoundryReadOnlyTable` uses the existing native `FoundryTable` presentation. It accepts immutable header/row values, limits the native grid height, and offers full row inspection by double-click. Markdown tables now use this component rather than text editors per cell.

Expandable consumers must update their outer allocated height when content changes; changing only visibility inside a fixed-height field allows native controls to overlap subsequent content. Image consumers retain ownership of cached bytes and decoded bitmaps.

Automated builds and tests cover the shared assemblies. Windows and a full dark/light, high-DPI accessibility matrix remain release checks; they are not implied by a successful macOS build.
