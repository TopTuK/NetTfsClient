# NetTfsClient

[![Run tests](https://github.com/TopTuK/NetTfsClient/actions/workflows/tests.yaml/badge.svg)](https://github.com/TopTuK/NetTfsClient/actions/workflows/tests.yaml)

A .NET client library for the **Azure DevOps** and **Team Foundation Server (TFS)** REST APIs. It focuses on work items, projects, teams, and related areas you typically need in automation and tools.

- **API surface:** [Azure DevOps REST API 6.0](https://learn.microsoft.com/en-us/rest/api/azure/devops/) (with a few non-public endpoints where noted in XML docs).
- **Stack:** [RestSharp](https://restsharp.dev/) and [Newtonsoft.Json](https://www.newtonsoft.com/json) under the hood.

## Features

- **Connection & auth** — Personal Access Token (PAT), NTLM (username/password), default Windows credentials, `ClaimsPrincipal`, or a custom `IHttpClient`.
- **Projects & org** — `IProjectClient`: projects, teams, team members, identities/groups (see interface for full list).
- **Work items** — `IWorkitemClient`: get/update/create/copy, relations, history, WIQL and saved queries, changesets.
- **Mentions** — `IMentionClient` to add @mentions via history (uses a non-public API; see remarks on `SendMentionAsync`).

## Requirements

- A .NET SDK compatible with the target framework in [`NetTfsClient/NetTfsClient.csproj`](NetTfsClient/NetTfsClient.csproj) (currently `net10.0`).

## Installation

Add the [NuGet package](https://www.nuget.org/packages/NetTfsClient) to your project:

```bash
dotnet add package NetTfsClient
```

Or via Visual Studio: *Manage NuGet Packages* → search for `NetTfsClient`.

## Concepts

### Server URL

Pass the base URL of your organization or TFS instance, for example:

- Azure DevOps Services: `https://dev.azure.com/your-organization/`
- On-premises TFS / Azure DevOps Server: `https://your-server/tfs/`

A trailing `/` is optional; the client normalizes it.

### Project name: `Collection/Project`

The `projectName` argument identifies the **team project** and, when applicable, the **project collection**. Use the form `CollectionName/ProjectName` (e.g. `DefaultCollection/MyProduct`). If you only need collection-level APIs, you can use a collection-only value such as `DefaultCollection` (see `IClientConnection` for how this splits into `CollectionName` and `ProjectName`).

## Authentication

All flows go through `ClientFactory.CreateClientConnection` and return `IClientConnection`, which exposes `ServerUrl`, `CollectionName`, `ProjectName`, and the underlying `IHttpClient`.

| Method | Usage |
|--------|--------|
| PAT | `CreateClientConnection(serverUrl, projectName, personalAccessToken)` |
| NTLM | `CreateClientConnection(userName, userPassword, serverUrl, projectName)` — `projectName` defaults to `DefaultCollection` if omitted |
| Default credentials (current user) | `CreateClientConnection(serverUrl, projectName)` |
| Claims | `CreateClientConnection(serverUrl, projectName, claimsPrincipal)` |
| Custom | `CreateClientConnection(httpClient, projectName)` — `httpClient` must have `BaseUrl` set |

Extension methods on `IClientConnection` (in `NetTfsClient.Services`):

- `GetWorkitemClient()` → `IWorkitemClient`
- `GetProjectClient()` → `IProjectClient`
- `GetMentionClient()` → `IMentionClient`

## Quick start

```csharp
using NetTfsClient.Services;

var serverUrl = "https://dev.azure.com/contoso";
var projectName = "DefaultCollection/MyProject";
var pat = /* Personal Access Token with required scopes */;

var connection = ClientFactory.CreateClientConnection(serverUrl, projectName, pat);

// Projects and teams
var projectClient = connection.GetProjectClient();
var projects = await projectClient.GetProjectsAsync();

// Work items
var workitemClient = connection.GetWorkitemClient();
var workItem = await workitemClient.GetSingleWorkitemAsync(42);

if (workItem != null)
{
    Console.WriteLine(workItem["System.Title"]);
    workItem.Title = "Updated title";
    await workItem.SaveFieldsChangesAsync();
}
```

For a full runnable sample (including **User Secrets** for URL, project, and PAT), see [`TfsClient.App/Program.cs`](TfsClient.App/Program.cs).

## Building and testing

From the repository root:

```bash
dotnet restore
dotnet build --configuration Release
dotnet test
```

Integration tests may require Azure DevOps/TFS environment variables (see the test project and [`.github/workflows/tests.yaml`](.github/workflows/tests.yaml) for `ENV_SERVER_URL`, `ENV_PROJECT_NAME`, and `ENV_PAT`).

## Documentation in code

Public contracts are documented with XML comments on `ClientFactory`, `IClientConnection`, `IWorkitemClient`, `IProjectClient`, and related types. For REST endpoint references, many `IProjectClient` members link to the official Microsoft REST docs.

## Coding style

This repository follows the [Google C# Style Guide](https://google.github.io/styleguide/csharp-style.html).

## License

See [LICENSE.txt](LICENSE.txt).
