# Platforms and Delivery Requirements

<!-- cspell:ignore DDMMHBC URP -->

This page records the supported PC targets and the expected build-delivery
behavior. It is a requirements reference; check the linked issues and workflows
for implementation status.

## Confirmed project baseline

- Target platforms are Windows and Linux PC.
- The Unity project uses Editor `6000.3.26f1` and the Universal Render Pipeline
  (URP), as recorded in
  [`ProjectVersion.txt`](../../ProjectSettings/ProjectVersion.txt).

## Required delivery behavior

- **Pull requests:** validation runs for every pull request. Documentation and
  spell checks continue when Unity player builds are unnecessary. Product or
  relevant workflow changes request Unity validation. Fork pull requests may
  run structural checks, but licensed Unity jobs must not receive secrets or
  run untrusted code.
- **Reusable player build:** a callable workflow uses the GameCI Unity Builder
  and the Editor version in `ProjectSettings/ProjectVersion.txt` to build
  Windows and Linux players from the same revision. Each output includes a
  launchable player and its required companion files. License inputs come from
  the repository's encrypted
  secrets only in the trusted licensed build step; they must not appear in
  logs, source files, or artifacts.
- **Main-branch Nightly:** product changes merged to `main` produce Windows and
  Linux build artifacts. Each artifact identifies its commit and platform. A
  generation means one successful main-branch product build; retain the latest
  three successful generations and remove only older Nightly artifacts.
- **GitHub Releases:** publishing a release attaches one Windows build archive
  and one Linux build archive to that release.

These are target behaviors for the Unity delivery pipeline. Their
implementation is tracked separately by [reusable build issue #12][build],
[pull-request validation issue #13][pull-request], [Nightly issue #14][nightly],
and [release build issue #15][release]. The existing documentation lint
workflow is not itself evidence that the Unity build pipeline is complete.

## Provisional and deferred choices

DDMMHBC is the codename and provisional title. The Cube sprite is replaceable
interim artwork, not a platform or delivery requirement.

The framework for authoring bullet patterns remains deferred to the maintainer
decision in [issue #6][decision] after the evidence in [issue #16][survey].
That choice does not change the Windows/Linux targets or delivery requirements
above.

## Sources

- [Project foundation roadmap, issue #4][foundation]
- [Build workflow issues #12–15][build]

[foundation]: https://github.com/kurone-kito/DDMMHBC/issues/4
[decision]: https://github.com/kurone-kito/DDMMHBC/issues/6
[survey]: https://github.com/kurone-kito/DDMMHBC/issues/16
[build]: https://github.com/kurone-kito/DDMMHBC/issues/12
[pull-request]: https://github.com/kurone-kito/DDMMHBC/issues/13
[nightly]: https://github.com/kurone-kito/DDMMHBC/issues/14
[release]: https://github.com/kurone-kito/DDMMHBC/issues/15
