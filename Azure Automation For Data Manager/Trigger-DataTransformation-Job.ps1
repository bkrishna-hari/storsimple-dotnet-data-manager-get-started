<#
.DESCRIPTION
    This runbook triggers the Data Transformation Job. 
    This runbook depends on C# dlls which handles the Data transformation calls.  
     
.ASSETS     
    SubscriptionId: The Subscription Id of the Azure. 
    TenantId: The Tenant Id (guid) of the Azure Active Directory (AAD) tenant where the service principal resides.
    ClientId: The Client Id of AAD app which has permissions to KeyVault
    ActiveDirectoryKey: Client Secret of AAD app which has permissions to KeyVault
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
        [string]$SubscriptionId,

        [Parameter(Mandatory=$true)]
        [string]$TenantId,

        [Parameter(Mandatory=$true)]
        [string]$ClientId,

        [Parameter(Mandatory=$true)]
        [string]$ActiveDirectoryKey,

        [Parameter(Mandatory=$true)]
        [string]$ResourceGroupName,

        [Parameter(Mandatory=$true)]
        [string]$DataManagerName,

        [Parameter(Mandatory=$true)]
        [string]$JobDefinitionName
    ) 
    
    Write-Output "Job initiated"
    #ls C:\Modules\User\DataTransformationApp
    
    $jobParams = @{
        SubscriptionId = $SubscriptionId;
        TenantId = $TenantId;
        ApplicationId = $ClientId;
        ActiveDirectoryKey = $ActiveDirectoryKey;
        ResourceGroupName = $ResourceGroupName;
        ResourceName = $DataManagerName;
        JobDefinitionName = $JobDefinitionName
    }

    InlineScript {
        $jobParams = $Using:jobParams
        Write-Output "Input params: "
        Write-Output $jobParams 

        # Load all dependent dlls
        $data = [Reflection.Assembly]::LoadFile("C:\Modules\User\DataTransformationApp\Newtonsoft.Json.dll")
        $data = [Reflection.Assembly]::LoadFile("C:\Modules\User\DataTransformationApp\DataTransformationApp.dll")
        $data = [Reflection.Assembly]::LoadFile("C:\Modules\User\DataTransformationApp\Microsoft.Rest.ClientRuntime.dll")
        $data = [Reflection.Assembly]::LoadFile("C:\Modules\User\DataTransformationApp\Microsoft.Rest.ClientRuntime.Azure.Authentication.dll")
        $data = [Reflection.Assembly]::LoadFile("C:\Modules\User\DataTransformationApp\Microsoft.IdentityModel.Clients.ActiveDirectory.dll") 

        # Trigger Job definition
        [DataTransformationApp.DataTransformationApp]::RunJob($jobParams)
    }
}
