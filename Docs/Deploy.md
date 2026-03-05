# **Deploying to Azure**

## Using the Azure CLI - First deployment
Remember to replace the variables with your own values.

### Create a SQL Server
```bash
# Variables
RESOURCE_GROUP="BankRUs"
LOCATION="swedencentral"       # or your preferred Azure region
SQL_SERVER_NAME="bankrus-sql-001"  # must be globally unique
ADMIN_USER="bankrusadmin"
ADMIN_PASSWORD="SuperSecret123!"    # strong password

# Create the server
az sql server create \
    --name $SQL_SERVER_NAME \
    --resource-group $RESOURCE_GROUP \
    --location $LOCATION \
    --admin-user $ADMIN_USER \
    --admin-password $ADMIN_PASSWORD
```

### Create a SQL Database
```bash
# Variables
DB_NAME="BankRUsDB"

# Create the database
az sql db create \
    --resource-group $RESOURCE_GROUP \
    --server $SQL_SERVER_NAME \
    --name $DB_NAME \
    --compute-model "Provisioned" \
    --edition "GeneralPurpose" \
    --family "Gen5" \
    --capacity 2 \  #small, for dev/test
    --max-size 5GB
```

### Set firewall rules to conenct to SQL Server from your dev machine
Optional, ok for a staging servers or learning project.
```bash
# Variables
MY_IP=$(curl -s ifconfig.me)

# Create the firewall rule
az sql server firewall-rule create \
    --resource-group $RESOURCE_GROUP \
    --server $SQL_SERVER_NAME \
    --name AllowMyIP \
    --start-ip-address $MY_IP \
    --end-ip-address $MY_IP
```
### Create a key vault
```bash
KEYVAULT_NAME="bankrus-kv-001"  # must be globally unique

az keyvault create \
    --name $KEYVAULT_NAME \
    --resource-group $RESOURCE_GROUP \
    --location $LOCATION
```

### Add the passowrd to the secret
```bash
SQL_PASSWORD="SuperSecret123!"

az keyvault secret set \
    --vault-name $KEYVAULT_NAME \
    --name "BankRUsSqlPassword" \
    --value $SQL_PASSWORD
```

### Create a Docker image and push it to Docker Hub
This will sue the included Dockerfile.
```bash
docker build --platform linux/amd64 -t yourdockerhubusername/bankrus:latest .
```

Test the image locally
```bash
docker run \
  -e SQL_SA_PASSWORD=YourLocalDockerPassword \
  -e ASPNETCORE_ENVIRONMENT=Development \
  -p 8000:80 \
  bankrus:latest
```

Upload to Dockere Hub
```bash
docker login
docker tag bankrus:latest yourdockerhubusername/bankrus:latest
docker push yourdockerhubusername/bankrus:latest
```

### Create an App Service Plan
```bash
APP_SERVICE_PLAN="bankrus-asp-001"
APP_SERVICE_NAME="bankrus-app-001"
DOCKER_IMAGE="yourdockerhubusername/bankrus:latest"

az appservice plan create \
    --name $APP_SERVICE_PLAN \
    --resource-group $RESOURCE_GROUP \
    --location $LOCATION \
    --sku B1 \
    --is-linux
```

### Create the App Service
```bash
az webapp create \
    --resource-group $RESOURCE_GROUP \
    --plan $APP_SERVICE_PLAN \
    --name $APP_SERVICE_NAME \
    --docker-custom-image-name $DOCKER_IMAGE
```

### In Azure App Service / container, set the environment variables:
```bash
ASPNETCORE_ENVIRONMENT="Production"
KEYVAULT_URI=https://bankrus-kv-001.vault.azure.net/
```
We can use the Azure CLI to do that:
```bash
# Set the Key Vault URI
az webapp config appsettings set \
    --name $APP_SERVICE_NAME \
    --resource-group $RESOURCE_GROUP \
    --settings KEYVAULT_URI="https://<your-keyvault-name>.vault.azure.net/"

# Optional: explicitly set environment - should default to Production in the Docker image
az webapp config appsettings set \
    --name $APP_SERVICE_NAME \
    --resource-group $RESOURCE_GROUP \
    --settings ASPNETCORE_ENVIRONMENT=Production
    
# Docker registry credentials
az webapp config container set \
    --name $APP_SERVICE_NAME \
    --resource-group $RESOURCE_GROUP \
    --docker-custom-image-name your_username/bankrus:latest \
    --docker-registry-server-url https://index.docker.io/v1/ \
    --docker-registry-server-user username \
    --docker-registry-server-password <your-docker-hub-password-or-PAT>
```

Check the settings:
```bash
az webapp config appsettings list \
    --name $APP_SERVICE_NAME \
    --resource-group $RESOURCE_GROUP
```

### Diagnose Issue
```bash
az webapp log tail --name $APP_SERVICE_NAME --resource-group $RESOURCE_GROUP
```

