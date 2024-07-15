# App

### Docker

Hub repo:
https://hub.docker.com/r/kristijanross/rwa-movies/tags

Local testing with compose:
docker compose up --build

Local testing with Dockerfile.cloudrun:
docker build -t kristijanross/rwa-movies . -f Dockerfile.cloudrun
docker run -p 1433:1433 -p 7116:7116 kristijanross/rwa-movies

Cloud run deployment:
docker push kristijanross/rwa-movies
gcloud run services update rwa-movies --image docker.io/kristijanross/rwa-movies@[digest]
https://cloud.google.com/sdk/gcloud/reference/run/services/update

Initialization example:  
https://github.com/microsoft/mssql-docker/tree/master/linux/preview/examples/mssql-customize

Useful commands in overview section:  
https://hub.docker.com/r/microsoft/mssql-server

Installing ASPNET Core manually:
https://learn.microsoft.com/en-us/dotnet/core/install/linux-ubuntu

### Email

Using SendGrid, the world's largest cloud-based email delivery platform:  
https://app.sendgrid.com/  
https://app.sendgrid.com/settings/sender_auth

### Notes to lecturer (HR)

U aplikaciji sam pazio na sljedeće:
Odvojio sam API kontrolere od View kontrolera koristeći [Area] atribute kako bih osigurao routing istoimenih kontrolera i akcija.
Limitirao sam slike na .PNG i Admin role na jednog korisnika kako bih zadržao originalnu strukturu zadane baze podataka.
Omogućio sam praćenje navigacije u pregledniku za Ajax straničenja i potpuno upravljanje žanrovima prema SPA principima.
Onemogućio sam prijavu deaktiviranih (soft-deleted) korisnika i korisnika s nepotvrđenim emailom.
Koristio sam SignalR paket koji utilizira Web Sockete za real-time komunikaciju sa statičkom stranicom notifikacija.
Koristio sam MailKit paket za veću fleksibilnost i kontrolu SMTP postavki.
Koristio sam Cookie autentifikaciju kao default, a za API sam forsirao JWT autentifikaciju. Samo dvije akcije dopuštaju ili Cookie ili JWT autentifikaciju, a to su akcije za dohvaćanje broja neposlanih notifikacija i slanje istih, koje koristi statička stranica za administratore.
Aplikacija je potpuno responzivna i uređena.

Za prijavu na objavljenu aplikaciju kao administrator koristite username "admin" s lozinkom "2023".
Svi drugi korisnici imaju lozinku "1".

### Notes to lecturer (EN)

_Previously deployed on Azure (App Service, SQL Server, SQL Database)_

# Setup

Development mode:

- Execute [RwaMovies.sql](/RwaMovies.sql).
- Add at least one row to `Countries` in order to register an account later.
- Enter your SendGrid password in [appsettings](/RwaMovies/appsettings.Development.json#L17)
  or prepare a fake SMTP server and verify your `TargetMailSettings`.

Run the app and register a new account with the username "admin".
The "Admin" role is restricted to a single user with the username "admin" due to a hardcoded check [here](/RwaMovies/Services/AuthService.cs#L99).
Confirm the admin's email address and sign in.

---

For more info, see project specifications:

- [specs-en.pdf](RwaMovies/wwwroot/documents/specs-en.pdf)
- [specs-hr.pdf](RwaMovies/wwwroot/documents/specs-hr.pdf)
