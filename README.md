# RwaMovies - [rwa.k1k1.dev](rwa.k1k1.dev)

Deployed on Azure (App Service, SQL Server, SQL Database).

Using SendGrid, the world's largest cloud-based email delivery platform.

## Running locally

Follow these steps before running locally, in development mode:

- Execute [RwaMovies.sql](/RwaMovies.sql).
- Add at least one row to `Countries` in order to register an account later.
- Enter your SendGrid password in [appsettings](/RwaMovies/appsettings.Development.json#L17)
  or prepare a fake SMTP server and verify your `TargetMailSettings`.

Run the app and register a new account with the username "admin".
The "Admin" role is restricted to a single user with the username "admin" due to a hardcoded check [here](/RwaMovies/Services/AuthService.cs#L99).
Confirm the admin's email address and sign in.
