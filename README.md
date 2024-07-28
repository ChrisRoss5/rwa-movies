# RWA Movies

_RWA ("Razvoj web aplikacija") - translates to Development of web applications (college course name)._

### https://rwa-movies-k2uko7wmrq-ew.a.run.app

| USERNAME | PASSWORD | ROLE  |
| -------- | -------- | ----- |
| admin    | 2023     | ADMIN |
| john     | 1        | USER  |

**IMPORTANT: The app is live but Cloud Run still doesn't support SQL Server running in the same container. Only the login screen is accessible until there is a cost-effective solution.  
More info: https://www.googlecloudcommunity.com/gc/Serverless/MSSQL-server-in-Cloud-Run-without-external-db/td-p/778818**

The lecturer only provided [setup.sql](setup.sql), which had to stay unmodified.

Previously, the app was deployed on Azure (App Service, SQL Server, SQL Database) at https://rwa.k1k1.dev. Student credits covered big expenses! So, I decided to dockerize it and have it deployed permanently with cost near to 0.

## Docker

**GOAL: Have SQL Server 2019 as base image with ASP.NET Core 6 installed on top of it, running in a single container, as a single service in Google Cloud Run.**

**PROBLEM IN CLOUD RUN: _"Server is not found or not accessible."_ I asked for help here:
https://www.googlecloudcommunity.com/gc/Serverless/MSSQL-server-in-Cloud-Run-without-external-db/td-p/778818**

---

Hub repo:  
https://hub.docker.com/r/kristijanross/rwa-movies/tags

Local testing with compose:
`docker compose up --build`

Local testing with Dockerfile.cloudrun:  
`docker build -t kristijanross/rwa-movies . -f Dockerfile.cloudrun`  
`docker run -p 1433:1433 -p 7116:7116 kristijanross/rwa-movies`

Cloud run deployment:  
`docker push kristijanross/rwa-movies`  
`gcloud run services update rwa-movies --image docker.io/kristijanross/rwa-movies@[digest]`  
See [gcloud reference](https://cloud.google.com/sdk/gcloud/reference/run/services/update) for all update params.

MSSQL example with database initialization:  
https://github.com/microsoft/mssql-docker/tree/master/linux/preview/examples/mssql-customize

MSSQL useful commands in overview section:  
https://hub.docker.com/r/microsoft/mssql-server

Installing ASP.NET Core manually:  
https://learn.microsoft.com/en-us/dotnet/core/install/linux-ubuntu

## Email

I'm using SendGrid, the world's largest cloud-based email delivery platform:  
https://app.sendgrid.com/settings/sender_auth

## Notes to lecturer

I went beyond all the requirements and added unique insights and expertise!

#### HR:

- Odvojio sam API kontrolere od View kontrolera koristeći [Area] atribute kako bih osigurao routing istoimenih kontrolera i akcija.
- Limitirao sam slike na .PNG i Admin role na jednog korisnika kako bih zadržao originalnu strukturu zadane baze podataka.
- Omogućio sam praćenje navigacije u pregledniku za Ajax straničenja i potpuno upravljanje žanrovima prema SPA principima.
- Onemogućio sam prijavu deaktiviranih (soft-deleted) korisnika i korisnika s nepotvrđenim emailom.
- Koristio sam SignalR paket koji utilizira Web Sockete za real-time komunikaciju sa statičkom stranicom notifikacija.
- Koristio sam MailKit paket za veću fleksibilnost i kontrolu SMTP postavki.
- Koristio sam Cookie autentifikaciju kao default, a za API sam forsirao JWT autentifikaciju. Samo dvije akcije dopuštaju ili Cookie ili JWT autentifikaciju, a to su akcije za dohvaćanje broja neposlanih notifikacija i slanje istih, koje koristi statička stranica za administratore.
- Aplikacija je potpuno responzivna i uređena.

#### EN:

- I separated API controllers from View controllers using [Area] attributes to ensure routing of controllers and actions with the same names.
- I restricted images to .PNG format and limited the Admin role to a single user to maintain the original structure of the provided database.
- I enabled navigation tracking in the browser for Ajax pagination and complete management of genres according to SPA principles.
- I disabled login for deactivated (soft-deleted) users and users with unconfirmed email addresses.
- I used the SignalR package, which utilizes WebSockets for real-time communication with the static notification page.
- I used the MailKit package for greater flexibility and control over SMTP settings.
- I used Cookie authentication as default, and for the API, I enforced JWT authentication. Only two actions allow either Cookie or JWT authentication, which are the actions for fetching the number of unsent notifications and sending them, used by the static admin page.
- The application is fully responsive and well-styled.

## ASP.NET BUG!

After presenting the app to my lecturer, I explained a bug I couldn't resolve: blocking access to the static site (sites/Notifications.html) for non-admin users. Although this isn't a critical security issue since all the API endpoints used by the static site are protected, I was determined to fix it. I even went beyond the official documentation (https://learn.microsoft.com/en-us/aspnet/core/fundamentals/static-files?view=aspnetcore-6.0#static-file-authorization) but without success.

The core issue is that the CSS completely breaks and doesn't load once the static files are protected, even if only one file is restricted. When I presented the bug, my lecturer was confident he could solve it and began coding with enthusiasm. We both worked on the issue, but after an hour, he had to leave. He assured me he would find a solution and get back to me. Unfortunately, I never received the solution, and the bug remains unresolved.

---

For more info, see project specifications:

- [specs-en.pdf](RwaMovies/wwwroot/documents/specs-en.pdf)
- [specs-hr.pdf](RwaMovies/wwwroot/documents/specs-hr.pdf)
