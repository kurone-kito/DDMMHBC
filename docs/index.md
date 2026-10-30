<!-- cspell:ignore DDMMHBC Shmup -->

# DDMMHBC documentation

This directory is the static knowledge corpus for DDMMHBC. Markdown pages are
canonical; YAML records make those pages easier to discover by title, category,
and tags. The corpus is intended for normal repository browsing and text search
and does not require a hosted retrieval service, embeddings, or game runtime
code.

## Corpus layout

- `index.md` is the entry point and a short project overview. Focused
  requirements live in `requirements/`.
- `knowledge/*.yml` contains small metadata records. Each record links to its
  authoritative Markdown page through its `document` field, using a path
  relative to the YAML file.
- `schemas/knowledge-entry.schema.json` defines the required record fields and
  their allowed values.
- Add focused Markdown pages here as requirements or design decisions grow;
  update the corresponding YAML record to link to the focused page.

## Find a knowledge record

Search the `title`, `category`, or `tags` fields with your repository browser or
`rg`. For example, this local search finds records related to the initial game
brief:

```sh
rg -n -i '^(title|category|tags):|vertical-shooter|unity-6' docs/knowledge
```

Open the matching YAML record, then follow its `document` path. The Markdown
page is authoritative; a YAML `summary` is only a short discovery aid. If a
record and its linked page differ, use the Markdown page and update the record.

## Project overview

DDMMHBC is the codename and provisional title for a one-shot, roughly
three-minute 2D vertical bullet-hell shooter made with Unity 6 and URP for
Windows and Linux PC. Its visual direction combines 3D backgrounds with
prerendered character sprites. The planned composition is a 16:9 PC image with
a centered 3:4 vertical playfield. Until original art is available, a
replaceable static sprite rendered from one view of a Unity Cube serves as
interim character artwork.

## Requirement pages

- [Project overview](requirements/project-overview.md) —
  [YAML record](knowledge/project-overview.yml).
- [Gameplay requirements](requirements/gameplay.md) —
  [YAML record](knowledge/gameplay.yml).
- [Visual direction](requirements/visual-direction.md) —
  [YAML record](knowledge/visual-direction.yml).
- [Platforms and delivery](requirements/platforms-and-delivery.md) —
  [YAML record](knowledge/platforms-and-delivery.yml).

## Development tooling

- [IDD helper runtime](idd-runtime.md) — pinned helper and payload
  compatibility checks.

## Other knowledge records

- [DDMMHBC project overview example](knowledge/format-example.yml) — confirmed
  scope and the original record-format example.
- [Shmup framework survey](knowledge/shmup-framework-survey.yml) — provisional,
  source-linked candidate inventory; no option is selected.
- [Knowledge-entry schema](schemas/knowledge-entry.schema.json) — validation
  contract for YAML records.
