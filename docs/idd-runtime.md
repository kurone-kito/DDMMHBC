# IDD helper runtime

DDMMHBC uses a repository policy file and imports the IDD instruction payload
from the operator's installed runtime. The helper package is resolved through
`ephemeral-npx`. Keep the declared IDD version and helper package pin aligned
with the payload that the activation helper reports. A version-only update can
leave configured workflow commands unavailable.

## Selected package

| Setting | Value |
| --- | --- |
| IDD version | `0.17.0` |
| Helper package | `https://codeload.github.com/kurone-kito/idd-skill/tar.gz/77befdd4030757464bed7f31c7dfe6d0a404a18b` |
| Helper profile and launcher | `ephemeral-npx` / `npx` |
| Policy file | `.github/idd/config.json` |

The pin aligns this repository with the v0.17.0 instruction payload and its
helper entry points. This change updates only `iddVersion` and
`helperRuntime.packageSpec`; the existing marker prefix, validation commands,
merge and review policy, and issue-authoring journal remain in place.

## Reproduce the read-only checks

The checks below stage the package in a temporary directory. The activation
payload must be at `$XDG_DATA_HOME/idd-skill/current`; setting
`XDG_DATA_HOME` to the payload directory itself does not select it.

Run this complete block from the repository root in one POSIX-compatible
shell session. The checks share the temporary staging path created at the
start of the block.

```sh
set -eu
STAGE="$(mktemp -d)"
ARCHIVE_URL="https://codeload.github.com/kurone-kito/idd-skill/tar.gz/77befdd4030757464bed7f31c7dfe6d0a404a18b"
curl -fsSL "$ARCHIVE_URL" -o "$STAGE/idd-skill.tar.gz"
mkdir "$STAGE/package"
tar -xzf "$STAGE/idd-skill.tar.gz" --strip-components=1 -C "$STAGE/package"
mkdir -p "$STAGE/isolated-data/idd-skill"
ln -s "$STAGE/package" "$STAGE/isolated-data/idd-skill/current"

# Validate the repository policy with the schema and validator shipped in this package.
PACKAGE_ROOT="$STAGE/package" REPO_ROOT="$(pwd)" node --input-type=module <<'NODE'
import { readFileSync } from 'node:fs';
import path from 'node:path';
const packageRoot = process.env.PACKAGE_ROOT;
const repoRoot = process.env.REPO_ROOT;
const { loadJson, checkSchemaKeywords, validate } = await import(
  path.join(packageRoot, 'scripts/validate-schemas.mjs'),
);
const schema = loadJson('schemas/policy.schema.json', packageRoot);
const context = { root: packageRoot, schemaPath: 'schemas/policy.schema.json' };
const config = JSON.parse(readFileSync(path.join(repoRoot, '.github/idd/config.json'), 'utf8'));
const errors = [
  ...checkSchemaKeywords(schema, '$', context),
  ...validate(config, schema, '$', context),
];
if (errors.length) throw new Error(errors.join('\n'));
console.log('PASS: repository policy validates against the pinned policy schema.');
NODE

# Compare the policy with origin/main after normalizing only the two intended values.
node --input-type=module <<'NODE'
import assert from 'node:assert/strict';
import { execFileSync } from 'node:child_process';
import { readFileSync } from 'node:fs';
const baseline = JSON.parse(execFileSync('git', ['show', 'origin/main:.github/idd/config.json'], { encoding: 'utf8' }));
const current = JSON.parse(readFileSync('.github/idd/config.json', 'utf8'));
assert.equal(current.iddVersion, '0.17.0');
assert.equal(
  current.helperRuntime.packageSpec,
  'https://codeload.github.com/kurone-kito/idd-skill/tar.gz/77befdd4030757464bed7f31c7dfe6d0a404a18b',
);
const normalized = structuredClone(current);
normalized.iddVersion = baseline.iddVersion;
normalized.helperRuntime.packageSpec = baseline.helperRuntime.packageSpec;
assert.deepStrictEqual(normalized, baseline);
console.log('PASS: all other policy values match origin/main.');
NODE

# Run activation, inspect the effective local policy, and smoke-check the staged entry points.
XDG_DATA_HOME="$STAGE/isolated-data" node "$STAGE/package/bin/idd-activation.mjs" --cwd "$(pwd)"
node "$STAGE/package/bin/idd-effective-config.mjs" --no-user-global
for helper in idd-claim-issue idd-claimed-push idd-f4-worktree-branch-cleanup idd-load-gate idd-local-admission idd-publish-authoring-hold; do
  node "$STAGE/package/bin/$helper.mjs" --help
done
```

## Recorded verification

Using the archive pinned above, the policy schema check and the normalized
comparison with `origin/main` passed after asserting the expected version and
archive URL. Activation through the pinned package returned `active: true`,
`tier: repository-local`, and `reason: repository-policy-minimal-import`, with
`instructionsRoot` resolving to the isolated `current` payload. Effective
policy inspection confirmed marker prefix `ddmmhbc`, the pinned
`ephemeral-npx` / `npx` helper, the configured validation commands,
`fully_autonomous_merge`, `copilot-advisory`, `fast-agent-resolve`, and journal
`kurone-kito/DDMMHBC#1`. The six staged helper `--help` commands above all
exited successfully.

These checks do not launch Unity, a standalone player, an X display, or a
Windows desktop window. They do not install a payload or change a pointer in
the operator's home directory.

## Updating the pin

For a future IDD runtime update, inspect the selected instruction payload,
skills, helper commands, and policy schema together. Update the version and
package pin only after confirming the repository's configured commands exist
in that package and the effective policy preserves the intended project rules.
