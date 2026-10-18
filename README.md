<!-- cspell:ignore shmup -->

# DDMMHBC

[![License: MIT](https://img.shields.io/badge/License-MIT-blue.svg)](./LICENSE)
[![Linting](https://github.com/kurone-kito/DDMMHBC/actions/workflows/lint.yml/badge.svg)](https://github.com/kurone-kito/DDMMHBC/actions/workflows/lint.yml)

DDMMHBC is the codename and provisional title for a one-shot, roughly
three-minute 2D vertical bullet-hell shooter for Windows and Linux PC. It is
being developed with Unity 6 and the Universal Render Pipeline (URP), using
Unity Editor `6000.3.26f1`.

## Project overview

The game is intended as one complete, short playthrough. Its visual direction
combines 3D backgrounds with prerendered character sprites to evoke arcade
shooters from the mid-to-late 2000s. The target composition is a 16:9 PC image
with a centered 3:4 vertical playfield, leaving room around the playfield for
video framing.

The current repository contains the reusable Unity project baseline. The boot
scene and placeholder sprite are planned work tracked in [issue #11].
Until original art is available, the planned placeholder is a replaceable
sprite rendered from a Unity Cube at a single view. These visual placeholders
are not final art. Gameplay has not been implemented, and the framework
approach remains undecided; see the [framework survey] for the candidate
inventory.

## Open the Unity project

1. Install Unity Hub and Unity Editor `6000.3.26f1`.
2. Add this repository's root folder as a Unity project and open it with that
   Editor version. Unity resolves the declared packages from
   [`Packages/manifest.json`](Packages/manifest.json).

## Documentation

Start with the [documentation index](docs/index.md). It describes the static
requirements knowledge base, how to search its YAML records, and where to find
the authoritative project overview and schema.

## 日本語概要

DDMMHBC は、Windows/Linux PC 向けに Unity 6 と URP で制作する、プレイ時間
約 3 分の一発もの縦型弾幕シューティングのコードネーム兼暫定タイトルです。
3D 背景とプリレンダリングしたキャラクターで、2000 年代中後半の STG らしい
映像表現を目指します。オリジナル素材が届くまでは、Unity の Cube を単一視点で
描画した差し替え可能な仮素材を使う予定です。現在は Unity の基礎設定までで、
起動シーンと Cube の Sprite は [issue #11] で整備する予定です。

## License

[MIT](./LICENSE)

[issue #11]: https://github.com/kurone-kito/DDMMHBC/issues/11
[framework survey]: docs/architecture/shmup-framework-survey.md
