# BankRUs

### Update EF Tools:
```bash 
dotnet tool update --global dotnet-ef
```

### On Mac - Run SQL Server in docker:
Make sure to set the environment variable SQL_SA_PASSWORD to your desired password.
Make sure to set the environment variable ASPNETCORE_ENVIRONMENT to DevMac. 

This can be done in the `launchSettings.json` file, or directly in the terminal.
If the environment variables are added directly in the terminal, use `dotnet run` to run the application.

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
