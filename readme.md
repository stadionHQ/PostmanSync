# Postman Sync (BETA)
[![Maintenance](https://img.shields.io/nuget/v/StadionHQ.PostmanSync)](https://www.nuget.org/packages/StadionHQ.PostmanSync/)

This is a .NET local development tool that syncs your API's schema definition (such as Open API) automatically with your postman collection, during the start up of your app.

Changes made in code to your APIs are instantly available in your Postman app as soon as your .NET core API finishes launching.

No more manually updating you postman whenever something changes in your code. Make your changes, run your app, and test your endpoints instantly using the Postman GUI.

## Getting Started
### Install Postman Sync
Install the [Nuget package](https://www.nuget.org/packages/StadionHQ.PostmanSync/).
```bash
dotnet add package StadionHQ.PostmanSync
```

### Configure Postman Sync


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
- Postman Sync makes this possible, by fetching your most recently generated `swagger.json` (or other supported schema) when your app launches. It sends this to Postman to an API that you can configure, so that on each launch the Postman app is updated instantly with your newly changed or added endpoints.



## Example workflow

- Fork main postman collection
- Connect Postman Sync to the forked collection
- Change code, test in Postman against the fork
- When finished, merge the fork back into the main postman collection using Postman pull requests.

## Publishing
From the root of the repo.

```sql
cd src/Stadion.PostmanSync
dotnet pack
cd bin/Debug

```