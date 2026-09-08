# Rhino Foundry UI contributor guidance

- Keep this repository presentation-only and product-neutral.
- Do not add Rhino document mutation, persistence, command, or panel-registration behavior.
- Reuse `FoundryTheme` semantic tokens instead of adding literal control colors.
- Public controls require mouse, keyboard, focus, disabled, and high-DPI behavior.
- Keep native file pickers, color dialogs, context menus, and message boxes native.
- Treat public API changes as semantic-versioning changes and document them in the README.
- Follow `docs/IMPLEMENTING_COMPONENTS.md` for ownership, input, native lifetime, testing, and consumer migration requirements.
- Add or update a copyable example in `docs/USAGE.md` for every new public component.
- Require zero build warnings, passing tests, successful package creation, and `git diff --check` before release.
