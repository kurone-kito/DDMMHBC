# Gameplay Requirements

<!-- cspell:ignore shmup -->

This page captures the gameplay scope confirmed by the project brief. It avoids
filling gaps with genre conventions that the project has not chosen.

## Confirmed scope

- The game is a vertical bullet-hell shooter, also commonly called a vertical
  shmup. Here, “vertical” describes the intended orientation of the gameplay
  field; it does not specify controls or camera behavior.
- The intended experience is one short, complete playthrough of roughly three
  minutes. The brief does not define stage count, player lives, continues,
  scoring, difficulty, or ending rules.
- Gameplay targets Windows and Linux PC and uses Unity Editor `6000.3.26f1`
  with URP.

## Deferred: bullet-pattern authoring

A pattern-authoring approach describes how the project will define enemy bullet
movement and attack sequences. No third-party package, scripting format, or
project-owned tool has been selected, and this issue does not add gameplay code.

The [survey in issue #16][survey] records OSS candidates and a project-owned C#
coroutine option without ranking them. The [maintainer decision in issue #6]
[decision] is still required before choosing an approach.

## Provisional details

The project name DDMMHBC is provisional. Character art is also interim; see
[Visual Direction](visual-direction.md). These details do not settle gameplay
mechanics.

## Source

- [Project foundation roadmap, issue #4][foundation]

[foundation]: https://github.com/kurone-kito/DDMMHBC/issues/4
[survey]: https://github.com/kurone-kito/DDMMHBC/issues/16
[decision]: https://github.com/kurone-kito/DDMMHBC/issues/6
