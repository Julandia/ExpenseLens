# ExpensenLens Deployment
Expenselens services and functions are deployed using Pulumi.

## Pre-requisites

### Resource group (`expenselens-admin`)
Before deploying to first environment, resource group `expenselens-admin` should be manually created.

The resource group must contain below resources that are shared among deployment environments like `development` and `production`.

### Azure Storage Account (`expenselenspulumi`)
The storage account is used by Pulumi to store its state files.

Create two containers:
- `development`
- `production`

Update storage account name and access key to ExpenseLens repository secrets.
- `AZURE_STORAGE_ACCOUNT_NAME`: 'expenselenspulumi'
- `AZURE_STORAGE_ACCOUNT_KEY`: <key>

### Azure Key Vault (`expenselens-pulumi`)
The key vault is used to store secrets that are used by Pulumi to deploy resources.

Create key: `pulumi-secret-provider`

### Container Registry (`expenselens`)
The ACR is used to store ExpenseLens backend service container images.

Create repository: `expense-lens-service`

Update ACR secrets to ExpenseLens repository secrets.
- `ECR_LOGIN_SERVER`: <server>
- `ECR_USERNAME`: <username>
- `ECR_PASSWORD`: <password>

## Service Principal (`ExpenseLens github action`)
The service principal is used by GitHub Actions to authenticate with Azure.

Follow the steps in [this guide](https://learn.microsoft.com/en-us/azure/developer/github/connect-from-azure?tabs=azure-portal%2Clinux) to create a service principal.

For each deployment environment, add credential under Certificates & Secrets -> Federated credentials

See [example of development](docs/github_action_credential_development.jpg)

Update following secrets to ExpenseLens environment secrets.
- `AZURE_CLIENT_ID`: <client_id>
- `AZURE_SUBSCRIPTION_ID`: <subscription_id>
- `AZURE_TENANT_ID`: <tenant_id>

## Pulumi stack
Create a Pulumi stack for each deployment environment.

Take `development` as an example:
```pwsh
cd deploy\ExpenseLens
az login
$env:AZURE_STORAGE_ACCOUNT = 'expenselenspulumi'
$env:AZURE_STORAGE_key = <key>
pulumi stack init --secrets-provider="azurekeyvault://expenselens-pulumi.vault.azure.net/keys/pulumi-secret-provider"
```

## Deploy Locally (optional)
```pwsh
cd deploy\ExpenseLens
az login
$env:AZURE_STORAGE_ACCOUNT = 'expenselenspulumi'
$env:AZURE_STORAGE_key = <key>
dotnet r preview-dev
dotnet r deploy-dev
```
To deploy Functions, find the FunctionApp name, run
```pwsh
cd src\Functions
az login
func azure functionapp publish <functionname>
```

## Deploy from GitHub Workflow
After initial deployment to an environment, add/update following variables and secrets to GitHub repository.
- Environment variable: AZURE_FUNCTIONAPP_NAME
- Environment secret: AZURE_FUNCTIONAPP_PUBLISH_PROFILE



