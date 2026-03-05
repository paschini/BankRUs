# BankRUs
A simple banking application, done as homework for my C# course.
Uses a LocalDB SQL Database on Windows and a SQL Server which must run in Docker/Podman on Mac.

## **Deployment instructions**
Azure deployment instructructions can be found [here](Docs/Deploy).

## **Running the application**
There is a `launchSettings`.json with the most basic running settings, so running from VS interface or Rider is enough.
`dotnet run` will also work.

### Update EF Tools:
For Windows, remember to update the DB before first run, so thaat it creates the LocalDB database.
```bash 
dotnet tool update --global dotnet-ef
```

### On Mac - Run SQL Server in docker:
The app runs fine on Macs, given that the required variables are set. I recommend using Rider and Docker.

Make sure to set the environment variable SQL_SA_PASSWORD to your desired password.
Make sure to set the environment variable ASPNETCORE_ENVIRONMENT to DevMac. 

This can be done in the `launchSettings.json` file, or directly in the terminal.
If the environment variables are added directly in the terminal, use `dotnet run` to run the application, not the
▶️ button in Rider. To use the run button in Rider, you must set the environment variables in the `launchSettings.json`
file. Just please do not commit that file with your SQL Server password. And please do not use the same password for
your local docker DB and the production database, just in case. Paranoia pays off in this job, ya know.

```bash
docker run \
  -e "ACCEPT_EULA=Y" \
  -e "SA_PASSWORD=$SQL_SA_PASSWORD" \
  -p 1433:1433 \
  mcr.microsoft.com/mssql/server:2022-latest
```

### Database Update:
```bash 
dotnet ef database update \
  --project BankRUs.Intrastructure \
  --startup-project BankRUs.Api
```

On Mac:
```
ASPNETCORE_ENVIRONMENT=DevMac dotnet ef database update \
  --project BankRUs.Intrastructure \
  --startup-project BankRUs.Api 
```

### Add Migration:
```bash 
dotnet ef migrations add TestMigration \
  --project BankRUs.Intrastructure \
  --startup-project BankRUs.Api
```

On Mac:
```bash 
ASPNETCORE_ENVIRONMENT=DevMac dotnet ef migrations add TestMigration \
  --project BankRUs.Intrastructure \
  --startup-project BankRUs.Api
```

### Remove Last Migration:
```bash 
dotnet ef migrations remove \
  --project BankRUs.Intrastructure \
  --startup-project BankRUs.Api
```

On Mac:
```bash 
ASPNETCORE_ENVIRONMENT=DevMac dotnet ef migrations remove \
  --project BankRUs.Intrastructure \
  --startup-project BankRUs.Api
```

