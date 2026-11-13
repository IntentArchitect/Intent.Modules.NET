# Authoring a `<Canvas>`

Read this before authoring ANY `<Canvas>` block. Also read `references/wireframe.md` — every artboard body is a wireframe fragment and every rule there applies to it unchanged.

## When a board beats one inline frame

A `<Canvas>` is a pan/zoom board of **artboards**, one per user-visible state. It is the right block when the change is only reviewable as a set:

- A screen that gains states the reader must see together — default, empty, error, loading.
- A short sequence the user moves through — form, confirm dialog, result.
- One screen at more than one surface, where the responsive behaviour is the change.

It is the wrong block for a single state. A board of one artboard is a `<Wireframe>` with extra chrome and less context, because it leaves the prose column. **Default to `<Wireframe>`; reach for `<Canvas>` at two or more states.** Keep a board to **at most 4 artboards** (3 per row-width) — see Legibility budget below. Beyond that, split by journey or area, or ask whether every state is really load-bearing.

## The authored shape

```mdx
<Canvas id="settings-states" caption="Settings redesign — user-visible states"
  artboards={[
    { id: "default", title: "Default", surface: "desktop", html: `…wireframe fragment…` },
    { id: "invalid", title: "Validation error", surface: "desktop", html: `…` },
    { id: "confirm", title: "Discard changes", surface: "popover", html: `…` },
  ]}
  annotations={[
    { target: "default", placement: "right", text: "Autosave replaces the Save button" },
  ]}
  connectors={[
    { from: "default", to: "invalid", label: "invalid input" },
  ]}
/>
```

- Each artboard needs `id` (unique on the board), `title` (what the reader sees above the frame — name the STATE, not the screen), `surface`, and `html`.
- Artboard height follows content within each lane, so frames in a row line up without you setting a height — see Sizing the board.
- `height` is optional and only overrides the strip the board is seen through; leave it off and the strip sizes itself to the board.

## Legibility budget

The board fits its **whole width** into the page when it opens (its height never lowers the zoom), so every extra frame (and every left/right note) lowers the opening zoom. Three 380px frames open near 100%; five open near 45%, where text is unreadable without zooming in. Validation warns (`canvas-opens-small`) when a board would open under 70% in a 1000px pane.

- At most **3 artboards per row-width, 4 per board**. More states than that: split into several boards, one per journey or area, each introduced by a sentence of prose.
- Choose the surface that matches the element that changes, not a full-width `desktop` frame just to host a strip or dialog; a `desktop` frame is 820px wide on its own.
- Show unchanged surroundings as one labelled row (`Toolbar — unchanged`), so the reader can see where the change sits.

**Before/after of a split.** Instead of one board of six dialogs:

```mdx
<Canvas id="classify-pick" caption="Classify — choosing"
  artboards={[
    { id: "pick", title: "Pick a script", surface: "panel", html: `…` },
    { id: "preview", title: "Preview", surface: "panel", html: `…` },
    { id: "confirm", title: "Confirm", surface: "popover", html: `…` },
  ]}
  connectors={[{ from: "pick", to: "preview", label: "selects" }]}
/>

<Canvas id="classify-outcome" caption="Classify — outcomes"
  artboards={[
    { id: "done", title: "Done", surface: "panel", html: `…` },
    { id: "error", title: "Script error", surface: "panel", html: `…` },
    { id: "empty", title: "No scripts", surface: "panel", html: `…` },
  ]}
/>
```

Two 3-frame boards, each readable on open.

## Sizing the board

Artboard height follows content: the frames in a lane (the wide `browser`/`desktop` row, or the small `mobile`/`popover`/`panel` row) share the height of the tallest one's content, up to that surface's default in the footprint table in `references/wireframe.md`. The two lanes can differ, so there is nothing to set and short states do not leave empty frames.

The strip grows to show the board at its opening zoom, capped at three quarters of the pane; the rest is reached by panning or Maximize. Set `height` only to force a different strip.

## Layout

Artboards flow into two lanes automatically, in authored order:

- **Broad lane (top):** `browser` and `desktop` frames.
- **Compact lane (below):** `mobile`, `popover` and `panel` frames.

That is deliberately simple, and it is enough for almost every board. Author the artboards in the order a reader should meet them and let the lanes do the work.

Override with `x` / `y` on an artboard only when the arrangement itself carries meaning — a branch where two outcomes should sit one above the other, or a pair meant to be read as a column. An override opts that artboard out of the flow entirely, so overriding one frame of a lane and not the rest usually produces an overlap. Override all of them or none.

## Annotations

`{ target, placement, text }` — a note pinned beside the frame it describes, with a leader line to it.

- `target` must be an artboard `id` on this board. One that isn't renders as a visible warning strip above the board rather than silently disappearing — but it is still an authoring error to fix.
- `placement`: `right` (default) | `left` | `top` | `bottom`.
- One sentence. An annotation says why the frame looks like this, or what the reader would otherwise miss — not a caption restating what is plainly visible.
- At most **one** `left`/`right` note per board — each costs a frame-width of horizontal space and lowers the opening zoom. Prefer `top`/`bottom`; anything else belongs in the prose around the board, not one note per frame.

## Connectors

`{ from, to, label }` — an arrow between two artboards.

Use them **only for genuinely sequential states**: the user does something on `from` and arrives at `to`. Label the arrow with the transition (`"invalid input"`, `"confirms"`), not with a description of the destination.

Do not use connectors to express "these two are related", to draw a legend, or to link every frame to every other. A board with more arrows than frames has stopped communicating a sequence. Comparative states — default vs empty vs error — need no connectors at all; the lane order already reads left to right.

A connector naming an artboard that isn't on the board renders as a warning strip, same as a stray annotation.

## Quality bar

- **One artboard per state, and every state earns its place.** If two frames differ only in a value nobody will look at, they are one frame.
- **Title the state, not the screen.** `Validation error` and `Empty — no saved views` tell the reader what they are looking at; `Settings` three times does not.
- **Keep surfaces consistent across comparable states.** Same screen, different states → same `surface`. A different surface is a claim that the surface changed.
- **The board is not the document.** It shows the states; the prose around it says what changes and why. Do not push explanation into annotations because there is space on the board.
- **Never leave an artboard body empty.** An empty or malformed fragment renders as a visible placeholder frame, which is a bug report, not a design.

## Before you move on

- Two or more artboards (two short states on one screen, each at most eight rows, are one `<Wireframe>` with the second beneath instead), each a distinct user-visible state with a state-naming title.
- Every artboard body obeys `references/wireframe.md` — no widths, no positions, no hard-coded colour, real product copy.
- At most 4 artboards and at most one left/right note.
- Annotation targets and connector endpoints all name real artboard ids on this board.
- Connectors join sequential states only; comparative states have none.
- The board renders with no warning strip above it.
