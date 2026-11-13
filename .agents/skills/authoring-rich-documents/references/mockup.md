# Embedding a `<Mockup>`

Read this before authoring a `<Mockup>` block. The mockup itself (the HTML file) is built with the **`mockup-kit`** skill; this page is only about showing one in a document.

## When to use it

`<Mockup>` shows a **design-fidelity** HTML mockup: a real screen of the app, in its real styles, light and dark, with numbered pins that cite requirement ids. Use it when a requirement changes what the user sees and the look matters: the reader should be able to say "yes, that is our app".

- **One rough screen state, no snapshot of the app** → `<Wireframe>`. **Several rough states side by side** → `<Canvas>`.
- **No mockup file yet** → build it first with `mockup-kit`. A `<Mockup>` never carries HTML inline.

## The authored shape

```mdx
<Mockup id="inbox-mockup" src="mockups/inbox/after.html" before="mockups/inbox/before.html"
  title="Inbox" caption="Unread count and filters (R3)" height={640} />
```

- `id` — stable and unique, as for every block.
- `src` — **required.** The proposed screen's `.html` file, **relative to the document** (from a spec's `requirements.mdx`, `mockups/<screen>/after.html`).
- `before` — optional. Today's screen. When given, the block shows a Before/After toggle, opening on After.
- `title` — optional; the block's header. Defaults to the file name.
- `caption` — optional, one line under the frame. Name the requirement ids it illustrates.
- `height` — optional, in the mockup's own pixels (default 720). The frame is laid out 1280px wide and scaled down to fit the column, so a height matching the mockup's own layout avoids a scrollbar or a blank band.

No body: `<Mockup … />` is self-closing.

## How it renders

The block reads the file, embeds every stylesheet, font, image and script it references (from the `intent/.mockups/<app>/` snapshot or beside the mockup), and shows it in a sandboxed frame that runs its scripts but cannot reach Intent, the file system or the network. So:

- **A snapshot not built on this clone** (its `snapshot.json` is there, its `styles.css` is not, because the team doesn't commit it) shows the mockup unstyled under a **Build snapshot** button, which builds it and reloads. Don't treat that as a broken mockup.
- **Only files inside the repository are embedded**, and only the kind a reference can use (`.css` for a stylesheet, `.js` for a script, images and fonts for `url()`). Anything else, and any remote URL, is left out; the block lists what it could not embed under the frame.
- **Theme, pins and comments work through the kit's `mockup.js`** (Intent swaps in its own current copy). A mockup that doesn't load it still shows, but can't switch theme, hide pins or take comments.
- **A pin with `data-req="R3.2"` goes to criterion R3.2** when the reader clicks it: in the same document it scrolls there, otherwise it opens the spec's requirements at it.
- **Comments come back to you** wherever the document can post to a conversation (a chat reply, a spec's documents): each names the element clicked (see `mockup-kit`, step 7).
- **In a chat reply** a `<Mockup>` renders too, its paths relative to the folder the agent works in rather than to a document.

## Rules

- **Put the block beside the requirement it illustrates**, and cite the same ids in its pins and its caption.
- **One block per screen.** Two screens of a flow are two blocks, each with its own before/after.
- **Screenshots are optional.** The block renders the HTML itself; keep light and dark PNGs under `mockups/<screen>/screenshots/` only when the team wants them, for a pull request read outside Intent or a printout (a frame does not print).
- A `src` or `before` that names no file is a write-time warning, and renders as a note naming the file.
