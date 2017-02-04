//---------------------------------------------------------------
//      Copyright (c) Microsoft Corporation. All rights reserved.
//----------------------------------------------------------------

using Newtonsoft.Json;

public class ConfigurationParams
{
    public string SubscriptionId { get; set; }    
    public string TenantId { get; set; }
    public string ClientId { get; set; }
    public string ActiveDirectoryKey { get; set; }
    public string ResourceGroupName { get; set; }
    public string ResourceName { get; set; }
}
