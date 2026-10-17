# Open-Source Shmup Framework Survey

<!-- cspell:ignore Shmup shmup Danmokou Bagoum Danmaku BDSL SZYU DDMMHBC -->

Research date: 2026-10-18.

This inventory records upstream-documented features and compatibility for a
Unity 6 2D vertical bullet-hell game targeting Windows and Linux PC builds.
The project baseline is Unity Editor `6000.3.26f1` with URP. Links below point
to project repositories, official documentation, or license notices. Dates
identify when the linked facts were checked; repository state and licensing
details can change.

Compatibility is reported only when an upstream source documents it. A lower
Unity minimum does not establish compatibility with this project's exact
Editor version or render pipeline. “Unknown” means no supporting statement was
found in the linked sources during this survey, not that the feature is
incompatible.

## Bulletfury

- **Pattern authoring and integration:** Bulletfury is distributed as the
  `com.wayfarergames.bulletfury` Unity Package Manager source package and can be
  installed by Git URL. Patterns are composed from a `BulletSpawner`, Inspector
  settings, and modules; the official docs also describe C# extension modules.
  ([repository README](https://github.com/WayfarerGames/bulletfury),
  [setup guide](https://wayfarer-games.com/bulletfury-documentation/getting-started/setup-and-first-spawn/),
  [free modules](https://wayfarer-games.com/bulletfury-documentation/modules/free-modules/),
  [custom modules](https://wayfarer-games.com/bulletfury-documentation/extending/write-your-own-modules/);
  checked 2026-10-18.)
- **Maintenance state:** The GitHub repository was public and not marked
  archived when checked; its page showed 11 commits and no listed releases.
  This snapshot does not establish the maintainer's future activity.
  ([GitHub repository](https://github.com/WayfarerGames/bulletfury);
  checked 2026-10-18.)
- **License scope:** The repository contains an MIT `LICENSE.md` for the
  package source. The checked license does not separately identify licenses
  for the included sample scene or any bundled assets, so their individual
  terms are unknown from that notice.
  ([LICENSE.md](https://github.com/WayfarerGames/bulletfury/blob/main/LICENSE.md),
  [repository README and sample listing](https://github.com/WayfarerGames/bulletfury);
  checked 2026-10-18.)
- **Compatibility (checked 2026-10-18):**
  - Editor `6000.3.26f1`: **Unknown** — the README documents Unity 6+, but
    does not name this exact Editor patch.
  - URP: **Documented** — the README states that Bulletfury supports URP only.
  - Windows: **Unknown** — the cited upstream compatibility sources do not
    name this player platform.
  - Linux: **Unknown** — the cited upstream compatibility sources do not name
    this player platform.
  ([repository README](https://github.com/WayfarerGames/bulletfury),
  [official documentation](https://wayfarer-games.com/bulletfury-documentation/).)

## Danmokou

- **Pattern authoring and integration:** Danmokou is a C# Unity bullet-hell
  engine whose behavior is authored in its Bagoum Danmaku Scripting Language
  (BDSL). Setup uses the full Git repository and submodules; its setup guide
  identifies the SZYU submodule as required for the engine. This is a project
  source integration route rather than a Unity Package Manager package.
  ([setup guide](https://github.com/Bagoum/danmokou/blob/master/docs/articles/setup.md),
  [repository README](https://github.com/Bagoum/danmokou);
  checked 2026-10-18.)
- **Maintenance state:** The public repository was not marked archived when
  checked. Its changelog contains v11.2.0 dated 2025-01-20; this is the dated
  version entry cited here, not a claim about the latest commit or future
  maintenance cadence. ([repository](https://github.com/Bagoum/danmokou),
  [changelog](https://github.com/Bagoum/danmokou/blob/master/docs/articles/changelog.md);
  checked 2026-10-18.)
- **License scope:** The engine source is MIT. The license notice says
  submodules may have different terms and lists additional restrictions for
  some non-code assets, including assets that are non-commercial or have
  unlisted licenses. Any adopted files would need their individual notices
  checked. ([Danmokou license inventory](https://github.com/Bagoum/danmokou/blob/master/Assets/Danmokou.LICENSE.md),
  [repository licensing notes](https://github.com/Bagoum/danmokou);
  checked 2026-10-18.)
- **Compatibility (checked 2026-10-18):**
  - Editor `6000.3.26f1`: **Unknown** — the v11.2.0 changelog names
    `6000.0.34f1`, not this project patch.
  - URP: **Unknown** — the v9.2.0 note describes a discouraged historical
    workaround; it does not establish URP support for v11.2.0.
  - Windows: **Unknown** — the cited setup and changelog pages do not name
    this player platform.
  - Linux: **Unknown** — the cited setup and changelog pages do not name this
    player platform.
  ([v11.2.0 and v9.2.0 changelog](https://github.com/Bagoum/danmokou/blob/master/docs/articles/changelog.md),
  [setup guide](https://github.com/Bagoum/danmokou/blob/master/docs/articles/setup.md).)

## BulletMLPlayer

- **Pattern authoring and integration:** BulletMLPlayer reads XML authored
  against BulletML and documents XY coordinates for vertical shooters and YZ
  coordinates for horizontal shooters. Its README describes a Unity project
  with `Assets` and `Packages` directories and does not document a UPM install
  URL; the documented route is to use the project sources and configure its
  prefab in a scene. ([README](https://github.com/jam0824/BulletMlPlayer),
  [repository](https://github.com/jam0824/BulletMlPlayer);
  checked 2026-10-18.)
- **Maintenance state:** The public GitHub repository was not marked archived
  when checked. The README says it was last updated in August 2025; this does
  not establish current maintenance cadence.
  ([repository README](https://github.com/jam0824/BulletMlPlayer);
  checked 2026-10-18.)
- **License scope:** The README declares the project MIT-licensed. The checked
  top-level file listing does not show a separate license file, and the README
  does not state separate terms for bundled assets; asset-specific terms are
  therefore unknown from these sources.
  ([README and top-level listing](https://github.com/jam0824/BulletMlPlayer);
  checked 2026-10-18.)
- **Compatibility (checked 2026-10-18):**
  - Editor `6000.3.26f1`: **Unknown** — the README recommends Unity 2021.3+
    and lists .NET Standard 2.1, but does not name this exact Editor patch.
  - URP: **Unknown** — the README does not document URP compatibility.
  - Windows: **Unknown** — the README does not name this player platform.
  - Linux: **Unknown** — the README does not name this player platform.
  ([README](https://github.com/jam0824/BulletMlPlayer).)

## DanmakU

- **Pattern authoring and integration:** DanmakU presents a composable
  `Fireables` API for bullet patterns and describes a C# Jobs and GPU-instanced
  bullet toolkit. The repository contains a Unity project (`Assets`, `Packages`,
  and `ProjectSettings`); the cited README does not document a UPM package
  install route. ([README](https://github.com/james7132/DanmakU),
  [repository](https://github.com/james7132/DanmakU);
  checked 2026-10-18.)
- **Maintenance state:** GitHub marks the repository archived on 2026-01-09.
  ([GitHub repository](https://github.com/james7132/DanmakU);
  checked 2026-10-18.)
- **License scope:** The repository has an MIT `LICENSE`. The checked README
  and license do not provide an asset-by-asset license inventory, so separate
  terms for bundled assets are unknown from those notices.
  ([LICENSE](https://github.com/james7132/DanmakU/blob/develop/LICENSE),
  [repository README](https://github.com/james7132/DanmakU);
  checked 2026-10-18.)
- **Compatibility (checked 2026-10-18):**
  - Editor `6000.3.26f1`: **Unknown** — the README lists Unity 2018.1+ and
    procedural GPU-instancing requirements, but does not name this exact
    Editor patch.
  - URP: **Unknown** — the README does not document URP compatibility.
  - Windows: **Unknown** — the README does not name this player platform.
  - Linux: **Unknown** — the README does not name this player platform.
  ([README](https://github.com/james7132/DanmakU).)

## Project-Owned C# Coroutine DSL

- **Pattern authoring and integration:** A project-owned DSL could represent
  each attack as C# `IEnumerator` routines that yield between frames using
  Unity coroutines. Unity documents `MonoBehaviour.StartCoroutine` and
  `yield return` as runtime APIs. This would require project-owned pattern
  conventions and any desired authoring tools; it adds no third-party shmup
  package. ([Unity coroutine manual](https://docs.unity3d.com/6000.3/Documentation/Manual/Coroutines.html),
  [StartCoroutine API](https://docs.unity3d.com/6000.3/Documentation/ScriptReference/MonoBehaviour.StartCoroutine.html);
  checked 2026-10-18.)
- **Maintenance and license scope:** There is no third-party framework
  repository or bundled framework asset to assess for this alternative. Its
  implementation and maintenance would belong to DDMMHBC. Unity coroutines
  are Unity engine APIs, not an independently adopted OSS shmup framework.
  ([Unity coroutine manual](https://docs.unity3d.com/6000.3/Documentation/Manual/Coroutines.html);
  checked 2026-10-18.)
- **Compatibility (checked 2026-10-18):**
  - Editor `6000.3.26f1`: **Unknown** — the Unity 6000.3 manual documents
    coroutine APIs for the Unity Player, but does not identify this exact
    Editor patch.
  - URP: **Unknown** — the coroutine documentation does not state compatibility
    with this project's render pipeline.
  - Windows: **Unknown** — the coroutine documentation does not name this
    player platform.
  - Linux: **Unknown** — the coroutine documentation does not name this player
    platform.
  ([Unity coroutine manual](https://docs.unity3d.com/6000.3/Documentation/Manual/Coroutines.html),
  [StartCoroutine API](https://docs.unity3d.com/6000.3/Documentation/ScriptReference/MonoBehaviour.StartCoroutine.html).)
  Local compatibility testing was not performed.

## Verification Boundary

No candidate was installed or run in this project. No Unity import, build, or
runtime compatibility test was performed for this survey. Unknown entries
remain open evidence gaps. This inventory records candidates and their
published facts; it does not rank or recommend them and does not select or
install a framework.
