# Demo for deploying a minimal .Net webapi to Azure App Service

The purpose of this repo is to have a demo project for deploying a minimal .Net webapi to Azure App Service using GitHub Actions.

## Develop locally

### Create a new webapi project

```bash
dotnet new webapi -o . -n Demo --no-https -minimal
```

### Run the webapi locally

```bash
dotnet run
```

### Test the webapi

```bash
curl http://localhost:5051/weatherforecast
```

## Deploy to Azure App Service

### Create a new Azure App Service

```bash
export RG_NAME=rg-demo-webapi
export APP_NAME=demo-$RANDOM
export APP_PLAN_NAME=$APP_NAME-plan
export LOCATION=swedencentral

az group create --name $RG_NAME --location $LOCATION

az appservice plan create --name $APP_PLAN_NAME --resource-group $RG_NAME --sku B1 --is-linux

az webapp create --name $APP_NAME --resource-group $RG_NAME --plan $APP_PLAN_NAME --runtime "DOTNETCORE:7.0"

az webapp config appsettings set --name $APP_NAME --resource-group $RG_NAME --settings WEBSITE_WEBDEPLOY_USE_SCM=true

echo "Web app url: https://$APP_NAME.azurewebsites.net/weatherforecast"
```

### Create a GitHub action workflow

```bash
mkdir -p .github/workflows
touch .github/workflows/dotnet.yml
```

I will create the content of the workflow file using GitHub Copilot chat providing the following input:

- Create a github action workflow for building, releasing and deploying a .net 7 webapi to azure app service. Use login action to login into Azure. The following parameters are stored as github secrets: `AZURE_CREDENTIALS`, `AZURE_WEBAPP_NAME`.

### Create a Service Principal

```bash
export SUBSCRIPTION_ID=$(az account show --query id -o tsv)
export AZURE_CREDENTIALS=$(az ad sp create-for-rbac --name "demo-webapi-sp" --role contributor --scopes /subscriptions/$SUBSCRIPTION_ID/resourceGroups/$RG_NAME/providers/Microsoft.Web/sites/$APP_NAME --sdk-auth)
```
### Create a GitHub secret

Create both `AZURE_CREDENTIALS` and `AZURE_WEBAPP_NAME` as GitHub secrets. The `AZURE_CREDENTIALS` secret should contain the output from the previous command (echo $AZURE_CREDENTIALS). The `AZURE_WEBAPP_NAME` secret should contain the name of the webapp (echo $APP_NAME).

### Create repo and push code

```bash
export REPOSITORY=<your_github_repository>
git init
git add .
git commit -m "Added code"
git branch -M main
git remote add origin $REPOSITORY
git push -u origin main
```

## Demo

Verify: 

- The GitHub action workflow is triggered and succeeds
- The webapi is deployed to Azure App Service and is accessible from the webapp url (https://$APP_NAME.azurewebsites.net/weatherforecast)

Enable security:
- Code Scanning
- Dependabot (with version updates)
- Secret scanning

Notice that there will be 2 PR:
- Bump Microsoft.AspNetCore.OpenApi from 7.0.4 to 7.0.9
- Bump Swashbuckle.AspNetCore from 6.4.0 to 6.5.0