---
description: Resolve the conflicts of a paused git merge or rebase, or update a pull request's branch with its base — see the rebase through every stop until it completes, then build, test and (when asked) push. Use when a prompt names this skill, or when a merge/rebase is paused on conflict markers.
short-description: Resolve merge/rebase conflicts, finish the operation, prove it builds, optionally push.
intent-updates: automatic
group: Review and pull requests
requiredTools:
  - run_task
  - bash
  - ask_user_question
---

# Resolve git conflicts

The `key: value` facts in the prompt that named this skill are all that varies between runs; everything else is below. Read `mode` first.

## Facts you may be given

| Fact                                    | Meaning                                                                                                                                                      |
| --------------------------------------- | ------------------------------------------------------------------------------------------------------------------------------------------------------------ |
| `mode`                                  | `source-control`: resolve the listed files in the solution's own checkout. `pr-update`: bring the base into a pull request's head in your own worktree (§4). |
| `operation`                             | `merge` or `rebase`. A rebase swaps the sides, see §1.                                                                                                       |
| `branch` | merge: what the merge is INTO (names "ours"). rebase: the branch being rebased, informational only. |
| `files`                                 | The conflicted files you own.                                                                                                                                |
| `complete`                              | `true`: finish the operation yourself (§3). `false`: stage and stop (§2). Always `true` in `pr-update`.                                                                                    |
| `remote`, `base`, `head`, `pr`          | `pr-update` only: remote name, base and head branch names, pull request number.                                                                              |
| `dropMerges`                            | `pr-update` rebase only: the host refused a rebase-merge because the branch's merge commits do not replay cleanly.                                           |
| `push`, `askBeforePush`, `buildAndTest` | `pr-update` only: §4.                                                                                                                                        |

## 1. Which side is which

- **merge**: "ours" is the current branch, "theirs" is the incoming branch.
- **rebase**: "ours" is the branch being rebased onto plus the commits already replayed; "theirs" is the commit currently being replayed.

Conflict markers are the lines starting `<<<<<<<`, `=======`, `>>>>>>>`.

## 2. Resolve

Rules:

- Before resolving a region, work out WHY each side changed it — read the commits behind both sides (`git log` / `git blame` on the file, and the wider change each commit belongs to). You are merging two intents, not two blocks of text.
- Resolve every conflict region — no conflict markers may remain.
- Keep both sides' independent additions; never silently drop content from either side.
- Preserve the file's formatting and style. Structured files (XML/JSON/YAML) must stay well-formed.
- If both sides changed the same thing incompatibly, make a judgement call. When it is a real guess (not an obvious reading of the two intents), ask with `ask_user_question` and wait — do NOT end your turn to ask. Otherwise explain it in your summary.
- Resolve the listed files (or, in `pr-update`, the conflicted files) only. Do not run the Software Factory or change Intent metadata. The exception is a fix the merge itself demands (e.g. one side renamed a member whose callers came in from the other side): make it, then call it out.

Then prove it (in `pr-update` with `buildAndTest: false`, skip building and testing — reading the result is the check, §4 step 6):

- Build the affected application(s) with `run_task` (the `build` task, or the equivalent in their `tasks.json`) to a clean build, and run the tests covering the merged code — the relevant project or file, or the whole suite if that is cheap.
- If the build breaks only because OTHER conflicted files still hold markers, that is expected: report it and stop rather than resolving files outside your list.
- Do not fix pre-existing failures your resolution did not cause, and never weaken or skip a test — report instead.
- If none of the resolved files affect compiled code or tests (docs, config), say so rather than silently skipping.

Then stage every file you touched — the listed ones AND any collateral fix — with `git add`. This is not optional: continuing commits the index, so an unstaged edit silently drops out of the commit and is stranded in the working tree. Git already staged what it merged cleanly, so your edit to one of those sits on top of a staged copy that does NOT contain it.

If `complete` is `false`: stop here. Do NOT commit and run no git command that moves HEAD. Summarise per file what you merged, report build and test results, list every file you touched outside `files` under its own heading, and confirm you staged everything. The user reviews the staged diff and continues.

## 3. Finish (`complete: true`)

Finish only when `git diff --name-only --diff-filter=U` lists nothing, the build is clean (when building applies, §2), AND your resolution introduced no test failure. A test failure that also happens without your resolution does not block finishing, but prove that first — run it in a temporary `git worktree add --detach` of the upstream commit (merge: current HEAD), removing it afterwards, or show the test touches none of the files you resolved — and say which in your report. If conflicts remain, the build is broken or you broke a test, do NOT finish: staging is where you stop.

- **merge**: `git commit --no-edit`.
- **rebase**: loop until the rebase is over. Each pass:
  1. `git -c core.editor=true rebase --continue`.
  2. If it stops with "No changes" because the replayed commit is now empty (already upstream) and `git status --porcelain --untracked-files=no` prints nothing, `git rebase --skip` it only after confirming its change really is upstream (`git diff REBASE_HEAD^ REBASE_HEAD` applied to HEAD adds nothing) — an empty commit you emptied by discarding its side is lost work, not a skip — and note its sha and subject; if status lists anything, `git add` it and continue instead.
  3. If it stops on new conflicts, go back to §2 for that stop — the files `git diff --name-only --diff-filter=U` now lists are in scope (not just the original `files`), and the "OTHER conflicted files" build exception applies per stop — resolve, build/test, stage, then continue the loop. Each stop is a new commit; the rebase is not done at the first `--continue`.
  4. Done when neither `git rev-parse --git-path rebase-merge` nor `git rev-parse --git-path rebase-apply` names an existing directory (a linked worktree keeps them outside `.git/`). Stop early only for a guess you must ask about, a failure your resolution caused, or a stop you cannot make progress on — and say exactly where you are.

In `source-control` mode never run another git command that moves HEAD (no reset, no push, no amend); `pr-update` has its own exceptions (§4). If git reports an autostash that failed to apply, say so prominently.

Summarise per file what you merged, report build and test results, flag anything needing attention, list files touched outside `files`, and say whether the operation is complete — for a rebase, naming every commit you skipped (sha, subject, why it was already upstream).

## 4. Update a pull request's branch (`mode: pr-update`)

You are in your own worktree on a session branch cut from the pull request's head as fetched when its tab loaded, which may be stale. `<base>` = `remote/base`, `<head>` = `remote/head`.

1. `git fetch <remote>`, then `git reset --hard <head>` so you start from the pull request's current head — the session branch holds no work yet. If either fails, stop and report.
2. Record `git rev-parse <head>`: the head you are updating.
3. Integrate:
   - merge: `git merge --no-edit <base>`.
   - rebase: `git rev-list --merges <base>..HEAD`. If it prints anything and `dropMerges` is not set, stop without rebasing and say the branch contains merge commits, so a rebase would replay conflicts already resolved and rewrite their history — suggest switching the update settings (the cog) to Merge. Otherwise `git rebase <base>`. With `dropMerges`, the rebase drops the merge commits and replays the rest, so conflicts those merges already resolved stop it again: resolve each as the step-2 head did (`git show <that sha>:<path>` is the resolved file) unless `<base>` changed it since.
4. On conflicts (`git diff --name-only --diff-filter=U`) resolve per §1 and §2, stage, then continue per §3's loop (merge: `git commit --no-edit`). If there were no conflicts it completes by itself. If `head` already contains `<base>`, say so and stop.
5. With `dropMerges`, run `git diff <step-2 sha> HEAD`: it must show only changes `<base>` made after the branch last merged it. Restore anything else the dropped merges carried, commit it, and list it.
6. `buildAndTest: true`: build with `run_task`, run the tests covering the merged code, and stop and report if the build or your resolution broke a test. `false`: do NOT build or test — a fresh worktree has no outputs or restored packages, so it takes minutes and is not asked for. Judge by reading: no markers left, well-formed, names and signatures from both sides still line up.
7. `push: true`: `askBeforePush: true` means ask with `ask_user_question` first and wait — do NOT end your turn to ask; if declined, stop without pushing. Push with exactly `git push <remote> HEAD:refs/heads/<head>` (rebase: `git push --force-with-lease=refs/heads/<head>:<step-2 sha> <remote> HEAD:refs/heads/<head>`, so the push fails rather than overwriting anything pushed since you started). If rejected, report git's output and stop. `push: false`: do NOT push; the result stays on this worktree branch for review.

Apart from the reset in step 1, never reset, amend, or push anywhere else. Finish by summarising per file what you merged, anything needing attention, and whether you pushed or finished — and if not, why not.
