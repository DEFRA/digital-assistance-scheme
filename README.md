# digital-assistance-scheme
Digital Assistance Scheme

The digital Assistance Scheme demo project

**Reference links**

Defra API developer portal: [https://developer-portal.trade.defra.gov.uk](https://developer-portal.trade.defra.gov.uk)

User guide page: [https://developer-portal.trade.defra.gov.uk/User-Guide/Sample-Project](https://developer-portal.trade.defra.gov.uk/User-Guide/Sample-Project)

Terms of use: [https://developer-portal.trade.defra.gov.uk/User-Guide/Terms-Of-Use](https://developer-portal.trade.defra.gov.uk/User-Guide/Terms-Of-Use)

**Requirements**

- A version of Visual Studio supporting ASP .NET Core 3.1.
- Login credentials for an active account on the Defra API developer portal.
- Client ID, Client Secret and Redirect URI for an active application created via the Defra API developer portal.

**Setup**

- Make a local copy of this repository.
- Open the appsettings.Development.json file and add the client ID, client secret and redirect URI from your application created in the Defra API developer portal.
- Ensure the localhost address and port number match the redirect URL you have setup in the application created in the Defra API developer portal.

**Running the project**

- Run the project from Visual Studio and then you may follow the on screen steps which will walk through the process of:
  - Building requests. 
  - Logging in using your Defra API developer portal account. 
  - Accepting scopes as a user for contacting APIs.
  - Requesting access and refresh tokens.
  - Using the access token to make a request to a protected API.
  - Viewing the response from the request on screen.
