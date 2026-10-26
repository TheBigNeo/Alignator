---
apply: by model decision
instructions: When implementing a change that is more than one small method change (features, refactors, bug fixes requiring investigation or touching multiple files)
---

# Plan Rules — Structured Implementation Workflow

This workflow applies to every implementation task that goes beyond a single small
method change. For a one-line fix, skip this file. For everything else, follow the
phases below in order. Do not start writing code before Phase 3.

## Phase 1 — Understand the input

* Read the user request or the issue completely. Rephrase the goal in one sentence.
* Separate: what is requested, what is implied, what is merely assumed.
* If a missing detail would materially change the implementation, ask max 4 concrete
  questions before planning (Rule 1). Otherwise state the assumption explicitly and
  proceed.
* Identify the acceptance criteria: how will we know the task is done and correct?

## Phase 2 — Investigate and write the reference

* Read and search the associated code parts: callers, configuration, tests, data
  models. Read before you write (Rule 8). Never plan against code you have not seen.
* Check for existing patterns that already solve the problem (Rule 2, Rule 11).
* Create a task folder in the repo: `doc/tasks/<task-id>/`
  (e.g. `doc/tasks/SBS-4553-quartz-update/`).
* Write `doc/tasks/<task-id>/reference.md` containing the distilled context:
  * Goal and acceptance criteria (from Phase 1)
  * Relevant files with paths and the key symbols in each
  * Current behavior and why the change is needed
  * Constraints, conventions and gotchas found in the code
  * Open questions or risky assumptions
* Keep the reference small: only what a fresh context needs to work without
  re-searching the codebase (Rule 6).

## Phase 3 — Write the plan

* Write `doc/tasks/<task-id>/plan.md` containing:
  * The approach: how the issue will be fixed, step by step, in plain sentences
  * Affected files per step
  * Verification per step (test, build, manual check) — a step without a
    verification is not a step (Rule 4)
* Derive from the plan a step plan with a status tracker:

```markdown
## Step Plan

| Status | Step            | Verification    |
|--------|-----------------|-----------------|
| [ ]    | 1. <what to do> | <how to verify> |
| [ ]    | 2. <what to do> | <how to verify> |

```

* Status values: `[ ]` open, `[~]` in progress, `[x]` done, `[!]` blocked.
* Steps must be small enough that one step can be implemented and verified
  independently (Rule 10). If a step cannot be verified on its own, split it.

## Phase 4 — Execute the Ralph loop

One step at a time. Never stack step N+1 on an unverified step N.

For each step:

1. Mark the step `[~]` in `plan.md`.
2. Spawn a **subagent with a fresh context** for this single step. Tell it:
   * The path to `plan.md` and `reference.md` (it must read both first)
   * The exact step number to implement and nothing else
   * The verification it must run
3. Verify the subagent's result yourself in the orchestrator context: run the
   tests/build/targeted check defined for the step and read the actual output.
   Do not trust the subagent's claim of success (Rule 4, Rule 12).
4. Only if verification passes: mark the step `[x]`, update `plan.md` with any
   findings that change later steps, then continue with the next step.
5. If verification fails: stop, fix or re-plan before moving on. Fail visibly,
   never silently (Rule 12).

## The 12 Rules

Each rule earned its place from a concrete failure mode. Skipping a rule
re-introduces that failure.

1. **Think before coding.** Understand the request, restate the goal, surface
   assumptions. Closes: silent wrong assumptions discovered three commits later.
2. **Simplicity first.** Smallest change that satisfies the request. No
   speculative abstraction. Closes: a 12-line fix becoming 300 lines.
3. **Surgical changes.** Touch only what the task requires. No reformatting,
   renaming or cleanup of unrelated code. Closes: orthogonal damage.
4. **Goal-driven execution.** Every step ends with verifying the actual goal,
   not just completing the step's instructions. Closes: steps all done, feature
   still broken.
5. **Don't make the model do non-language work.** Logic that belongs in code
   (retry policies, parsing rules) must be written as code, not decided ad hoc
   by the model. Closes: flaky, random behavior.
6. **Hard token budget.** Keep context small: persist findings to the task
   folder, read only what the current step needs. Closes: debugging spirals
   iterating on the same error message.
7. **Surface conflicts, don't average them.** If the codebase has two error
   handling patterns, pick the one the surrounding code uses and say why.
   Closes: pattern pollution (errors swallowed twice).
8. **Read before you write.** Search for existing functions/patterns before
   adding new ones. Closes: duplicate functions where import order decides.
9. **Tests are gated by correctness.** Tests must assert meaningful behavior,
   not shapes. A test that passes while the logic is wrong is worse than no
   test. Closes: shape-only tests, all green, auth broken in production.
10. **Long-running operations need checkpoints.** Verify each step before
    starting the next; keep steps independently verifiable. Closes: cascading
    breakage discovered at step 6 of a broken step 4.
11. **Convention beats novelty.** Follow the patterns, paradigms and style of
    the surrounding code and this repo's rule files. Closes: mixed-paradigm
    drift breaking existing infrastructure.
12. **Fail visibly, not silently.** Errors, skipped records and partial
    completions must be reported, never swallowed or logged away. A lying
    success is the worst possible outcome. Closes: migrations "succeeding"
    while silently skipping 14% of records.
