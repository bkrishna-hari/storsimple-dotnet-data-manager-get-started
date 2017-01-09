param
(
    [Parameter(Mandatory=$true)]
    [string]$SubscriptionName,

    [Parameter(Mandatory=$true)]
    [string]$ActiveDirectoryKey,

    [Parameter(Mandatory=$true)]
    [string]$AppName
) 

Function PrettyWriter($Content, $Color = "Yellow") { Write-Host " #### " + $Content -Foregroundcolor $Color }

# Login to Azure
$response = Login-AzureRmAccount

# Fetch subscription id and tenand it
$response =  (Get-AzureRmSubscription -SubscriptionName $SubscriptionName | Set-AzureRmContext)
$subscriptionId = $response.Subscription.SubscriptionId
$tenantId = $response.Tenant.TenantId

# Register an application with the Active Directory
$azureAdApplication = New-AzureRmADApplication -DisplayName $AppName -HomePage "https://www.contoso.org" -IdentifierUris "https://www.$AppName.org/example" -Password $ActiveDirectoryKey
$response = New-AzureRmADServicePrincipal -ApplicationId $azureAdApplication.ApplicationId

# Wait for some time for the service principal to get created
Start-Sleep -s 10

# Assign the contributor role to the service principal
$response = New-AzureRmRoleAssignment -RoleDefinitionName "Contributor" -ServicePrincipalName $azureAdApplication.ApplicationId.Guid

$applicationId = $azureAdApplication.ApplicationId

PrettyWriter "Subscription Id: $subscriptionId"
PrettyWriter "Tenant Id: $tenantId"
PrettyWriter "Client Id: $applicationId"
PrettyWriter "ActiveDirectoryKey: $ActiveDirectoryKey"