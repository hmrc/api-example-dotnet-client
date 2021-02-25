api-example-dotnet-client
=========================

*api-example-dotnet-client* is a sample .Net 5 application that provides a reference implementation of a HMRC client application.

It accesses three endpoints, each with their own authorisation requirements:

* Hello World - an Open endpoint that responds with the message “Hello World!”
* Hello Application - an Application-restricted endpoint that responds with the message “Hello Application!”
* Hello User - a User-restricted endpoint (accessed using an OAuth 2.0 token) that responds with the message “Hello User!”

You will need to add the Redirect URI 'https://localhost:8081' to your application ('https://developer.service.hmrc.gov.uk/developer/applications/').

API documentation is available at https://developer.service.hmrc.gov.uk/api-documentation

Application developers need to register with the platform and will be provided with key, secret and tokens upon registration.

The .net SDK will need to be installed in order to run the application locally.

This can be downloaded from https://dotnet.microsoft.com/download

The server can be started with the following command:
```
dotnet run
```

### License

This code is open source software licensed under the [Apache 2.0 License]("http://www.apache.org/licenses/LICENSE-2.0.html").
