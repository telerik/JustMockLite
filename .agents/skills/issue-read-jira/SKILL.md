---
name: issue-read-jira
description: >
  Retrieves and reads issues from JIRA using the Atlassian CLI or REST API.
  Use when working with JIRA issues, or when the user mentions reading,
  viewing, or retrieving JIRA tickets.
version: "1.0.0"
---

# JIRA Issue Retrieval

This skill guides you through retrieving and reading JIRA issues.

## Prerequisites

- Atlassian CLI (acli) installed, OR
- JIRA API access configured
- JIRA project access

## CLI Commands (acli)

### View Issue

```bash
# View issue by key
acli jira --action getIssue --issue PROJECT-123

# Get issue with all fields
acli jira --action getIssue --issue PROJECT-123 --outputFormat 2
```

### List Issues with JQL

```bash
# Find issues in project
acli jira --action getIssueList --jql "project = PROJECT"

# Find assigned issues
acli jira --action getIssueList --jql "assignee = currentUser()"

# Find by status
acli jira --action getIssueList --jql "status = 'In Progress'"
```

## API Access

### View Issue via API

```
GET /rest/api/2/issue/PROJECT-123
```

### Search with JQL via API

```
GET /rest/api/2/search?jql=project=PROJECT
```

## Best Practices

1. **Check comments and attachments**: JIRA issues often have critical information in comments and attachments. Always review these.
2. **Review linked issues**: Check for parent/child and "blocks/is blocked by" relationships.
3. **Check custom fields**: Projects often have custom fields with important information.
4. **Review subtasks**: Complex issues may have subtasks to track.
5. **Check sprint context**: Review the sprint and epic for broader context.

## Common Patterns

### Get Full Issue Context

```bash
# Get issue with comments
acli jira --action getIssue --issue PROJECT-123 --comment

# Get attachments list
acli jira --action getAttachmentList --issue PROJECT-123
```

## Placeholders

- `{{issue_id}}` - The JIRA issue key (e.g., PROJECT-123)
