# Postman Sync (BETA)

[![Maintenance](https://img.shields.io/nuget/v/StadionHQ.PostmanSync)](https://www.nuget.org/packages/StadionHQ.PostmanSync/)
![example workflow](https://github.com/stadionHQ/PostmanSync/actions/workflows/dotnet.yml/badge.svg)

This is a .NET local development tool that syncs your API's schema definition (such as Open API) automatically with your postman collection, during the start up of your app.

Changes made in code to your APIs are instantly available in your Postman app as soon as your .NET core API finishes launching.

Make your changes, run your app, and test your endpoints instantly using the Postman GUI.

- No more manually updating you postman collections whenever something changes in your code.
- No more broken / out of date postman collections.
- Keep your documentation and test collections in sync at all times with one simple process.

## See it in action

> TODO - The GIF makes it feel much slower than normal to update. Need better frame rate

![Example](https://github.com/stadionHQ/PostmanSync/blob/main/readme/images/example-flow.gif?raw=true)

## Getting Started

### 0. Setup your solution to generate API schemas

This topic is too broad to cover here. But if you use a .NET core boilerplate, then Swashbuckle will be automatically setup and a swagger.json document will be automatically generated.

In our demo app, we are just using the boilerplate `WeathersController` with default configuration for Swashbuckle.

### 1. Install Postman Sync

Install the [Nuget package](https://www.nuget.org/packages/StadionHQ.PostmanSync/).

```bash
dotnet add package StadionHQ.PostmanSync
```

### 2. Configure Postman Sync

> TODO - Need to add more detailed documentation for each property in the configuration structure. Also need better examples of finding the required ids in Postman.

This is where you tell Postman Sync where to sync your local API schema into Postman.

All the information required to sync an API schema is captured inside a `Profile` configuration object. Each profile is effectively a collection of Postman object ids that tell PostmanSync exactly which Postman objects need to be updated. This is powerful, because it means that individual developers can set their own ids, only updating their own resources till changes are ready to be merged.

Below is an example configuration for Postman Sync's own Postman API which is managed in the [Postman Sync Workspace](https://www.postman.com/stadionapis/workspace/postman-swagger-sync/overview).

This configuration will update the definition of our ["draft" version](https://www.postman.com/stadionapis/workspace/postman-swagger-sync/api/acc66f3c-05a6-437c-a019-34f76b07b607/version/51713c9f-f9f0-4b5a-b447-f6a28c07c6e7) in the "Postman Sync Demo" API.

As mentioned, your documentation and test collections are just relations connected to the API "container". Once the schema has been updated for that API, we can sync the changes automatically to any number of relations.

The `SourceSchema` property defines what type of schema source you are syncing to Postman, and where it is (i.e the URL).

The example below specifies a [documentation collection](https://www.postman.com/stadionapis/workspace/postman-swagger-sync/collection/8423190-1d8b581d-bc6d-44c9-bccf-b604d9c5e033?ctx=documentation) relation. This collection will be automatically updated as part of the sync process. Any other possible relations will not be updated, because they have not been specified.

```json
{
  "PostmanSync": {
    "PostmanApiKey": "<YOUR_POSTMAN_KEY>",
    "Profiles": [
      {
        "Key": "PostmanSyncDemo/draft/openapi3",
        "ApiId": "acc66f3c-05a6-437c-a019-34f76b07b607",
        "VersionId": "51713c9f-f9f0-4b5a-b447-f6a28c07c6e7",
        "SchemaId": "f1176fe5-ddf3-4fcf-bb40-129d580ea216",
        "Relations": [
          {
            "EntityType": "documentation",
            "EntityId": "7a889850-ae7c-440d-8a32-7266077495e6"
          }
        ],
        "SourceSchema": {
          "Type": "openapi3",
          "Language": "json",
          "Url": "https://localhost:7038/swagger/v1/swagger.json"
        }
      }
    ],
    "HostedService": {
      "Enabled": "true",
      "ThrowExceptions": "false"
    }
  }
}
```

Note that most of these ids you can get straight from the Postman GUI. However, for the relations, you'll need to call the `https://api.getpostman.com/apis/:apiId` endpoint with your api id in order to find out the relations ids.

#### Multiple profiles

We've built to allow syncing multiple profiles, but in reality it's unlikely it will be needed. After all, a single API only runs in a process in .NET Core, so logically doesn't quite make sense to have multiple profiles configured. This may be removed in the future.

#### Local configuration

It can be beneficial to create local git ignored configuration per developer to:

- Prevent the Postman api credentials being committed
- Prevent developers overwriting each others local setup / syncing to the wrong apis / etc

This can be achieved quite simply with this

```c#
var builder = WebApplication.CreateBuilder(args);

builder.Host.ConfigureAppConfiguration((hostingContext, config) =>
{
    if (hostingContext.HostingEnvironment.IsDevelopment())
    {
        config.AddJsonFile("appsettings.Local.json", true, true);
    }
});
```

### 3. Add Postman Sync to startup

The following is an example using .NET 6

```c#
#if DEBUG
builder.Services.AddPostmanSync(builder.Configuration);
#endif
```

Under the hood, this will add a hosted service (when the app is running locally). On startup, this hosted service will trigger Postman Sync to start syncing the current api schema to the Postman configuration you specified above.

This runs asynchronously with the app startup. There may be a slight delay, but if you watch the logs during startup, that will give you feedback as to the status of the sync (so you know if you're waiting for nothing because of an error). See logging below.

### 4. Make changes to your api and run it

Make changes to your API, run the app, and all configured correctly, your Postman app should be receiving updates in real time.

## How it works

### Quick overview on Postman's internal structure

- In Postman, you have an **API**.
- The API is a container for all the things you can do in Postman with your actual API.
- The postman API will have one or more **versions**.
- Each version has a **schema** (openapi for example)
- For each schema you can create **relations**:
  - **Documentation** (collections)
  - **Tests** (collections)
  - **Monitors**
  - etc

### Updating schemas

- As mentioned, each version of your Postman API has a schema.
- This schema should be a 1:1 mapping to your API in code for that particular version.
- Postman has some features to help keep things in sync (such as syncing a `swagger.json` file from github for example)
- However, this doesn't help us in development, where we want to make changes quickly and test them.
- Postman Sync makes this possible, by fetching your most recently generated `swagger.json` (or other supported schema) when your app launches. It sends this to Postman to an API that you can configure, so that on each launch the Postman app is updated ~~instantly~~ extremely quickly with your newly changed or added endpoints.

### Logging

You can see in your console logs the status of Postman Sync during startup.

![image-20211102120304488](readme/images/logs.png)

To debug issues with PostmanSync, you can set your logging level to `trace` for extra information.

### Exceptions

Postman Sync specific exceptions will be thrown if things go wrong (such as Postman API request errors). As this is a development workflow, we think this is ok, though later may look to improve this.

You'll see thes in the logs, with hopefully enough detail to diagnose what's happened. We surfacing Postman API error messages in the exception message

![image-20211102125628465](readme/images/image-20211102125628465.png)

## Example workflow

- Fork main postman collection
- Connect Postman Sync to the forked collection
- Change code, test in Postman against the fork
- When finished, merge the fork back into the main postman collection using Postman pull requests.

## Development

- You can develop PostmanSync directly against the included api `StadionHQ.PostmanSync.DevApi`

### Using nuget locally

- If you'd like to develop with PostmanSync locally in your own project, you can include the project's output directory as a nuget source.
- Find your local nuget.config file
  - On macOS, usually at at `/Users/<YOU>/.nuget/NuGet/NuGet.Config`
- Add a package source entry to where you've added PostmanSync's repo

```xml
<add key="PostmanSync Local" value="<YOUR_DEV_FOLDER>/PostmanSync/src/StadionHQ.PostmanSync/bin/Debug" />
```

Here's mine on macOS, for example:

```bash
cat /Users/chrispaynter/.nuget/NuGet/NuGet.Config
```

```xml
<?xml version="1.0" encoding="utf-8"?>
<configuration>
  <packageSources>
    <add key="nuget.org" value="https://api.nuget.org/v3/index.json" protocolVersion="3" />
    <add key="PostmanSync Local" value="/Users/chrispaynter/Development/stadion/PostmanSync/src/StadionHQ.PostmanSync/bin/Debug" />
  </packageSources>
</configuration>
```

Here's some [further information](https://stackoverflow.com/a/44463578/518341) if you need help.

## Publishing

From the root of the repo.

```bash
cd src/Stadion.PostmanSync
dotnet pack
cd bin/Debug
dotnet nuget push StadionHQ.PostmanSync.<VERSION>.nupkg --api-key <API_KEY> --source https://api.nuget.org/v3/index.json
```

## Issue Tracking

### Optional query parameters missing in Postman

If you have an optional query parameter in an endpoint, it doesn't show up as a query parameter in the pastman request. It should by default show up as a disabled query parameter that could then be turned on. Required parameters show up correctly.

https://github.com/postmanlabs/postman-app-support/issues/7301

https://community.postman.com/t/parameters-from-json-openapi-3-0-schema-not-displaying-in-postman-api-documentation/19139/6
