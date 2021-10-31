# Postman Sync (BETA)

This is a .NET local development tool that syncs your API's schema definition (such as Open API) automatically with your postman collection, during the start up of your app.

It helps you keep your postman collections exactly up to date with your generated api definitions, and prevents you having to manually update postman whenever something changes in your code.

Changes made in code to your APIs are instantly available in your Postman app as soon as your .NET core API finishes launching.

## Get Started


## Example workflow

- Fork main postman collection
- Connect Postman Sync to the forked collection
- Change code, test in Postman against the fork
- When finished, merge the fork back into the main postman collection using Postman pull requests.
