<!-- cspell:ignore DDMMHBC Shmup -->

# DDMMHBC documentation

This directory is the static knowledge corpus for DDMMHBC. Markdown pages are
canonical; YAML records make those pages easier to discover by title, category,
and tags. The corpus is intended for normal repository browsing and text search
and does not require a hosted retrieval service, embeddings, or game runtime
code.

## Corpus layout

- `index.md` is the entry point and the canonical project overview while the
  project is being bootstrapped.
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

DDMMHBC is the codename and provisional title for a one-shot 2D vertical
bullet-hell shooter made with Unity 6. A playthrough is expected to last about
three minutes. The game targets Windows and Linux PC and uses URP. Its
backgrounds are 3D, while characters use prerendered sprites to evoke the look
of games from the mid-to-late 2000s. Until project art is supplied, a Unity Cube
will serve as a Minecraft-like placeholder model for prerendering. The project
uses a reusable structure even though the game itself is intended as a single
work.

## Knowledge records

- [DDMMHBC project overview](knowledge/format-example.yml) — confirmed project
  scope and the record format in use.
- [Shmup framework survey](knowledge/shmup-framework-survey.yml) — provisional,
  source-linked inventory of candidate frameworks and the coroutine alternative.
- [Knowledge-entry schema](schemas/knowledge-entry.schema.json) — validation
  contract for YAML records.
