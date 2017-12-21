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

try {
    # Login to Azure
    $response = Login-AzureRmAccount

    if($response -ne $null) {
        # Fetch subscription id
        $response =  (Get-AzureRmSubscription -SubscriptionName $SubscriptionName | Set-AzureRmContext)
        if ($response.Subscription.Id -ne $null) {
            $subscriptionId = $response.Subscription.Id
        }elseif ($response.Subscription.SubscriptionId -ne $null) {
            $subscriptionId = $response.Subscription.SubscriptionId
        }
        
        # Fetch tenand id
        if ($response.Subscription.TenantId -ne $null) {
            $tenantId = $response.Subscription.TenantId
        }
        elseif ($response.Tenant.Id -ne $null) {
            $tenantId = $response.Tenant.Id
        }elseif ($response.Tenant.TenantId -ne $null) {
            $tenantId = $response.Tenant.TenantId
        }

        # Fetch AADApp password type
        $passwordType = (((Get-Help New-AzureRmADApplication).syntax).syntaxItem.parameter | where name -eq 'Password').parameterValue
        if ($passwordType -eq 'SecureString') {
            $AADPassword = ConvertTo-SecureString -String $ActiveDirectoryKey -AsPlainText -Force
        } else {
            $AADPassword = $ActiveDirectoryKey
        }
        
        # Register an application with the Active Directory
        $azureAdApplication = New-AzureRmADApplication -DisplayName $AppName -HomePage "https://www.contoso.org" -IdentifierUris "https://www.$AppName.org/example" -Password $AADPassword
        $response = New-AzureRmADServicePrincipal -ApplicationId $azureAdApplication.ApplicationId

        # Wait for some time for the service principal to get created
        $SLEEPORWAITTIME = 10
        $SLEEPINTERVALCOUNT = 0
        do {
            Start-Sleep -s $SLEEPORWAITTIME
            $SLEEPINTERVALCOUNT++
        } while ($azureAdApplication.ApplicationId.Guid -eq $null -and $SLEEPINTERVALCOUNT -le 18)

        # Assign the contributor role to the service principal
        $response = New-AzureRmRoleAssignment -RoleDefinitionName "Contributor" -ServicePrincipalName $azureAdApplication.ApplicationId.Guid

        $applicationId = $azureAdApplication.ApplicationId

        PrettyWriter "Subscription Id: $subscriptionId"
        PrettyWriter "Tenant Id: $tenantId"
        PrettyWriter "Client Id: $applicationId"
        PrettyWriter "ActiveDirectoryKey: $ActiveDirectoryKey"
    }
}
catch {
    Write-Error $_.Exception.Message
}