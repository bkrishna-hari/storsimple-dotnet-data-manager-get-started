<#
.DESCRIPTION
    This runbook triggers the Data Transformation Job. 
    This runbook depends on C# dlls which handles the Data transformation calls.
     
.ASSETS
    ResourceGroupName: The Resource group name of the DMS
    DataManagerName: The name of the DataManager Resource within the specified resource group.
    JobDefinitionName: The name of the Job definition.

.NOTES:
#>

workflow Trigger-DataTransformation-Job
{
    param
    (
        [Parameter(Mandatory=$true)]
        [string]$ResourceGroupName,

        [Parameter(Mandatory=$true)]
        [string]$DataManagerName,

        [Parameter(Mandatory=$true)]
        [string]$JobDefinitionName
    )

    $ClientCertificate = Get-AutomationCertificate -Name "AzureRunAsCertificate"
    if ($ClientCertificate -eq $null) {
         throw "The AzureRunAsCertificate asset has not been created in the Automation service."
    }

    $ServicePrincipalConnection = Get-AutomationConnection -Name "AzureRunAsConnection"
    if ($ServicePrincipalConnection -eq $null) {
         throw "The AzureRunAsConnection asset has not been created in the Automation service."
    }

    # Get the SubscriptionId, TenantId & ApplicationId
    $SubscriptionId = $ServicePrincipalConnection.SubscriptionId
    $TenantId = $ServicePrincipalConnection.TenantId
    $ClientId = $ServicePrincipalConnection.ApplicationId
    
    Write-Output "Initiating to trigger the job"
    #ls C:\Modules\User\DataTransformationApp

    InlineScript
    {
        try {
            # Load all dependent dlls
            $data = [Reflection.Assembly]::LoadFile("C:\Modules\User\DataTransformationApp\Newtonsoft.Json.dll")
            $data = [Reflection.Assembly]::LoadFile("C:\Modules\User\DataTransformationApp\DataTransformationApp.dll")
            $data = [Reflection.Assembly]::LoadFile("C:\Modules\User\DataTransformationApp\Microsoft.Rest.ClientRuntime.dll")
            $data = [Reflection.Assembly]::LoadFile("C:\Modules\User\DataTransformationApp\Microsoft.Rest.ClientRuntime.Azure.Authentication.dll")
            $data = [Reflection.Assembly]::LoadFile("C:\Modules\User\DataTransformationApp\Microsoft.IdentityModel.Clients.ActiveDirectory.dll")

            # Instantiate clientAssertionCertificate
            $ClientAssertionCert = New-Object Microsoft.IdentityModel.Clients.ActiveDirectory.ClientAssertionCertificate -ArgumentList $Using:ClientId, $Using:ClientCertificate

            # Set ConfigurationParams
            $Configparams = New-Object DataTransformationApp.ConfigurationParams
            $Configparams.ClientId = $Using:ClientId
            $Configparams.ResourceGroupName = $Using:ResourceGroupName
            $Configparams.ResourceName = $Using:DataManagerName
            $Configparams.SubscriptionId = $Using:SubscriptionId
            $Configparams.TenantId = $Using:TenantId
            $Configparams.Certificate = $ClientAssertionCert

            # Trigger Job definition
            [DataTransformationApp.DataTransformationApp]::RunJob($Configparams, $Using:JobDefinitionName)
        } catch {
            throw $_.Exception
        }
    }
}
