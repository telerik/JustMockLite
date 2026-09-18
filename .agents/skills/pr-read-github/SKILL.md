---
name: pr-read-github
description: >
  Retrieves and reads pull requests from GitHub using gh CLI.
  Use when working with GitHub PRs, or when the user mentions reading,
  viewing, or retrieving pull requests from GitHub.
version: "1.0.0"
---

# GitHub Pull Request Retrieval

This skill guides you through retrieving and reading GitHub pull requests using the gh CLI tool.

## Prerequisites

- gh CLI installed and authenticated
- Access to the target repository

## Basic Commands

### List Pull Requests

```bash
# List open PRs
gh pr list

# List PRs with specific state
gh pr list --state merged
gh pr list --state closed
gh pr list --state all

# Filter by author
gh pr list --author @me
gh pr list --author username

# Filter by base branch
gh pr list --base main
```

### View Pull Request

```bash
# View PR by number
gh pr view 123

# View with specific details
gh pr view 123 --comments

# Output as JSON
gh pr view 123 --json title,body,files,commits,reviews
```

### View PR Diff

```bash
# View the diff
gh pr diff 123

# View files changed
gh pr view 123 --json files
```

### Cross-Repository Access

```bash
gh pr list --repo owner/repo
gh pr view 123 --repo owner/repo
```

## Best Practices

1. **Review the description**: PR descriptions explain the purpose and approach.
2. **Check review status**: Look for approvals, change requests, and comments.
3. **Review CI status**: Check if tests are passing.
4. **Check linked issues**: PRs often link to issues they address.
5. **Review commits**: Understand the change history.

## Common Patterns

### Get Full PR Context

```bash
gh pr view 123 --json title,body,files,commits,reviews,comments
```

### Check Merge Readiness

```bash
gh pr checks 123
gh pr view 123 --json mergeable,reviewDecision
```

## Complete Review Retrieval

Before reporting that a pull request is ready or that it has no actionable
review feedback, retrieve the current platform state. Do not infer it from
local branches or assume that the target branch is `main`.

```bash
# Retrieve the PR's source and target branches, commit IDs, merge state, and review decision.
gh pr view 123 --repo {{code_platform_repo_slug}} --json baseRefName,baseRefOid,headRefName,headRefOid,mergeable,mergeStateStatus,reviewDecision,body

# Retrieve review summaries and conversation comments.
gh pr view 123 --repo {{code_platform_repo_slug}} --comments --json reviews,comments

# Retrieve every inline review comment, including replies and comments from Copilot.
gh api --paginate repos/{{code_platform_repo_slug}}/pulls/123/comments

# Retrieve every review thread's resolved/outdated state. The first comment ID and URL
# correlate the thread with the complete, separately paginated REST inventory above.
gh api graphql --paginate -f query='query($owner:String!, $repo:String!, $number:Int!, $endCursor:String) { repository(owner:$owner, name:$repo) { pullRequest(number:$number) { reviewThreads(first:100, after:$endCursor) { nodes { id isResolved isOutdated comments(first:1) { nodes { databaseId url } } } pageInfo { hasNextPage endCursor } } } } }' -F owner=OWNER -F repo=REPOSITORY -F number=123
```

Use the actual PR number and repository slug in every command. The paginated REST request
is the authoritative inline-comment inventory; the GraphQL request adds thread state and
uses each root comment to correlate the results. Include Copilot review summaries and
inline comments, human review comments, conversation comments, and unresolved review
threads in the review.

For every actionable item, retain its author, permalink, thread status, requested change,
and disposition: applied, deferred, or intentionally not changed with a reason. Do not
claim the PR is ready until this inventory is complete.

The platform's base branch, base commit, head commit, and merge state are authoritative.
Local Git comparisons may corroborate the platform state only after fetching the declared
base branch. Do not push, force-push, merge the PR, edit the remote PR description, reply
to review threads, or resolve remote threads as part of retrieval.

## Placeholders

- `{{code_platform_repo_slug}}` - The repository (owner/repo format)
