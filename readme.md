# CsvTimer

Converts the csv from Toggl Track to a format that matches what Dynamics wants.

## First setup

1. Copy appsettings.json and call it `appsettings.Personal.json`
2. Change the role to your role. (You can check previously submitted/rejected hours in Dynamics to see what your role is)

## How to run

```
dotnet run -- /path/to/exported/hours/from/toggl.csv
```

## Toggl setup

(This setup requires a paid Toggl account)

| Toggl       | Dynamics                                |
| ----------- | --------------------------------------- |
| (automatic) | Work Type                               |
| Client      | Project                                 |
| Project     | Project task                            |
| Task        | Hour Type                               |
| Description | [Jira Ticket :] External comments (\*) |

(\*) if the comment contains a `:`, then the first part will be interpreted as Jira Ticket.  
Format: `CODE-1234 : My comment`
