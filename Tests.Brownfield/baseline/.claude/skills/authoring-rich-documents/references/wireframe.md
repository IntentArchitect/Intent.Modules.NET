# Authoring a `<Wireframe>`

Read this before authoring ANY `<Wireframe>` block or `<Canvas>` artboard body. Do not author one from memory — the constraints below are what make a wireframe readable, comparable and safe, and every one of them is enforced or assumed by the renderer.

## What a wireframe is for

A wireframe answers "what will the user see and do", at the fidelity of a whiteboard sketch. It is the right block when a change alters a screen — new controls, a moved affordance, a state the user has not seen before — and prose would leave the reader guessing at the layout.

It is the WRONG block for:

- **Model shape.** Entities, services, DTOs and their relationships get a ```mermaid `classDiagram` / `erDiagram` fence plus `<ModelChanges>`. A wireframe of an aggregate says nothing about it.
- **Architecture and flow.** Use a ```mermaid fence or `<Diagram>`.
- **A change with no visible surface.** If you cannot name what the user sees differently, there is nothing to draw.

## The authored shape

```mdx
<Wireframe id="settings-after" surface="desktop" caption="Settings — after"
  data={{ html: `<div style="display:flex; flex-direction:column; gap:14px;">
  <h2>Notification settings</h2>
  <div class="wf-card">
    <div class="wf-row"><span data-icon="mail"></span><span>Email digest</span><span class="wf-pill">Daily</span></div>
    <div class="wf-divider"></div>
    <div class="wf-row"><span data-icon="bell"></span><span>Push alerts</span><span class="wf-pill">Off</span></div>
  </div>
  <p class="wf-muted">Changes save automatically.</p>
</div>` }}
/>
```

- `id` — stable and unique, as for every block.
- `surface` — `browser` | `desktop` | `mobile` | `popover` | `panel`. This fixes the frame's footprint and its chrome. An unknown value falls back to `desktop`.
- `caption` — optional, one line under the frame.
- `title` — optional; shown in the browser omnibox / window title bar.
- `data.html` — the fragment. A template literal, so a literal backtick or `${` inside it must be escaped.

## The rules

**Never set width, height or position.** No `width:`, no `max-width:`, no `position: absolute`, no page coordinates. The surface preset owns the frame; the fragment owns only what is inside it. Breaking this is what makes two states of the same screen stop being comparable, and it is the single most common way a wireframe goes wrong.

**No `<script>`, no `<style>`, no event handlers.** The renderer strips them, so anything you write there simply vanishes — you will be debugging a fragment that silently lost half its content. All styling is inline `style` plus the helper classes below.

**Colour only through the helpers and tokens.** Do not write hex or named colours. The renderer themes the fragment for the document's light AND dark themes off `--wf-ink`, `--wf-muted`, `--wf-line`, `--wf-card`, `--wf-paper`; a hard-coded `#fff` background is invisible in one of the two themes. If you genuinely need a colour, use `var(--wf-muted)` or a helper class.

**Real product labels, never lorem ipsum.** Use the actual labels, field names, entity names and copy the change will ship. Real labels do not mean full-length real data: truncate paths and values (see Density budget). A wireframe of "Item one / Item two" cannot be reviewed; one showing `Email digest` / `Daily` can.

**Persistent chrome spans the frame.** A top bar, tab strip or side nav that the real screen has should run the full width (or height) of the frame, with the content area inside it — otherwise the mockup reads as a floating card and the reader cannot tell what is chrome and what is the change.

**Before/after must be comparable.** When showing a change to an existing screen, give both frames the same `surface` and keep everything the change does not touch identical between them. A reader spots the difference by diffing the two pictures; gratuitous variation destroys that.

**No decoration.** No drop shadows, gradients, rounded-corner flourishes, brand colours or imagery. This is a sketch of structure and behaviour, not a visual design. Pixel-accurate branded mockups are deliberately out of scope for this renderer.

**Keep it to one screen state.** One `<Wireframe>` shows one state. Two or more states go on a `<Canvas>` — read `references/canvas.md` before authoring one. The exception: two states that each fit in about eight rows and share one screen are one `<Wireframe>` with the second state beneath the first, not a Canvas.

## Choosing a surface

Pick the surface whose footprint matches the real thing; the frame size sets how legible a `<Canvas>` is.

| Surface   | Width | Board height | Lane    | Use for                                                       |
| --------- | ----- | ------------ | ------- | ------------------------------------------------------------- |
| `browser` | 900   | 520          | broad   | A web page where the URL/tabs matter                          |
| `desktop` | 820   | 500          | broad   | A whole app window — only when persistent chrome is the change |
| `mobile`  | 320   | 560          | compact | A phone screen                                                |
| `popover` | 380   | 260          | compact | A small transient menu, tooltip or simple confirm             |
| `panel`   | 400   | 460          | compact | A modal dialog, form, side panel or anything with long rows   |

A modal dialog with a form or long paths is a `panel`, not a `popover`. Choose the surface that matches the element that changes; use `desktop` only when persistent chrome is part of the change, never merely to host a strip or dialog.

## Density budget

- **One idea per frame**, at most about 6–8 rows.
- **Draw only what the change touches.** Collapse unchanged chrome to a single labelled row (`Toolbar — unchanged`).
- **Truncate, don't wrap.** Show the filename in bold with its folder in `wf-muted` beneath, never a full path.
- Hint and helper text under 8 words.
- **Summarise lists:** `3 files` plus one sample row, not every row.

## Helper classes

Small on purpose. Anything not covered here should be expressed in inline `style` (flex layout, gaps, ordering) rather than by inventing a class the renderer does not know.

| Class          | What it renders                                                                                           |
| -------------- | --------------------------------------------------------------------------------------------------------- |
| `.wf-card`     | A bordered, filled content block — the workhorse container.                                               |
| `.wf-row`      | Horizontal flex row, centre-aligned, with a gap.                                                          |
| `.wf-col`      | Vertical flex column with a gap.                                                                          |
| `.wf-pill`     | A small rounded chip — a status, a count, a selected value.                                               |
| `.wf-btn`      | A button. Add `.wf-btn--primary` for the one primary action.                                              |
| `.wf-divider`  | A hairline rule inside a card.                                                                            |
| `.wf-muted`    | Secondary text.                                                                                           |
| `.wf-invalid`  | Error styling for an input or message — the validation-state colour.                                      |
| `.wf-skeleton` | A loading placeholder bar. Its text is hidden but still sizes the bar, so write the real label inside it. |

`<h1>`–`<h6>`, `<p>`, `<ul>`/`<ol>`, `<table>`, `<input>`, `<select>`, `<textarea>` and `<button>` are all themed already — reach for the semantic element before a helper class.

**Icons:** put `data-icon="name"` on an empty `<span>` and the renderer draws that glyph. **Any icon name from <https://lucide.dev/icons> resolves** — all 1,541 of them — spelled the way lucide spells it, in kebab-case: `chevron-down`, `triangle-alert`, `circle-check`, `git-branch`. A few everyday words are aliased onto it for convenience: `close`, `tick`, `edit`, `checkbox`, `checkbox-checked`, `radio-button`, `radio-selected`, `warning`, `error`, `success`, `help`, `document`, `gear`, `refresh`. `claude` is the one brand glyph, since lucide ships no brand marks. A name lucide itself ships always means lucide's icon rather than an alias of ours, and an unknown name degrades silently to a dashed placeholder square of the right size — so check the spelling on lucide.dev rather than inventing one.

**Loading states:** `.wf-skeleton` bars are static by design — a mockup is a picture of a screen, so nothing in it animates, and the product itself does not use spinners or progress bars.

## Before you move on

- Every frame has a `surface` and the fragment sets no width or position.
- The surface fits the real thing (footprint table) and the fragment stays inside the density budget.
- The content is real product labels, not placeholder text.
- Before/after pairs share a surface and differ only where the change lands.
- No hex colours, no `<style>`, no `<script>`, no `on*` attributes.
- A reader who knows nothing about the implementation could describe what the user sees.
