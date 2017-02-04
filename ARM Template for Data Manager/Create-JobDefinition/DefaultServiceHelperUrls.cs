//---------------------------------------------------------------
//      Copyright (c) Microsoft Corporation. All rights reserved.
//----------------------------------------------------------------

internal class DefaultServiceHelperUrls
{
    /// <summary>
    /// This helper method returns the url for data source creation
    /// </summary>
    /// <param name="hostUrl">Host url.</param>
    /// <param name="subscriptionName">Subscription name.</param>
    /// <param name="resourceGroupName">Resource group name.</param>
    /// <param name="resourceName">Resource name.</param>
    /// <param name="providerName">Provider name.</param>
    /// <returns>Data source url</returns>
    internal static string GetDataStoreUrl(string hostUrl,
        string subscriptionName,
        string resourceGroupName,
        string resourceName,
        string providerName,
        string dataStoreName)
    {
        return string.Join("/", hostUrl,
            "subscriptions", subscriptionName,
            "resourceGroups", resourceGroupName,
            "providers", providerName,
            "dataManagers", resourceName,
            "dataStores", dataStoreName);
    }

    /// <summary>
    /// This helper method returns the url for data source repository
    /// </summary>
    /// <param name="subscriptionName">Subscription name.</param>
    /// <param name="resourceGroupName">Resource group name.</param>
    /// <param name="providerName">Provider name.</param>
    /// <param name="resourceName">Resource name.</param>
    /// <returns>Data source repository url</returns>
    internal static string GetDataSourceRepositoryUrl(string subscriptionName,
        string resourceGroupName,
        string providerName,
        string resourceName)
    {
        return string.Join("/",
            "/subscriptions", subscriptionName,
            "resourceGroups", resourceGroupName,
            "providers", providerName,
            "managers", resourceName);
    }

    /// <summary>
    /// This helper method returns the url for data sink repository
    /// </summary>
    /// <param name="subscriptionName">Subscription name.</param>
    /// <param name="resourceGroupName">Resource group name.</param>
    /// <param name="providerName">Provider name.</param>
    /// <param name="mediaServiceName">Resource name.</param>
    /// <returns>Data source repository url</returns>
    internal static string GetMediaServiceDataSinkRepositoryUrl(string subscriptionName,
        string resourceGroupName,
        string providerName,
        string mediaServiceName)
    {
        return string.Join("/",
            "/subscriptions", subscriptionName,
            "resourceGroups", resourceGroupName,
            "providers", providerName,
            "mediaservices", mediaServiceName);
    }

    /// <summary>
    /// This helper method returns the url for data sink repository
    /// </summary>
    /// <param name="subscriptionName">Subscription name.</param>
    /// <param name="resourceGroupName">Resource group name.</param>
    /// <param name="providerName">Provider name.</param>
    /// <param name="storageAccountName">Resource name.</param>
    /// <returns>Data source repository url</returns>
    internal static string GetStorageAccountDataSinkRepositoryUrl(string subscriptionName,
        string resourceGroupName,
        string providerName,
        string storageAccountName)
    {
        return string.Join("/",
            "/subscriptions", subscriptionName,
            "resourceGroups", resourceGroupName,
            "providers", providerName,
            "storageAccounts", storageAccountName);
    }

    /// <summary>
    /// This helper method returns the url for data store type
    /// </summary>
    internal static string GetDataStoreTypeUrl(string subscriptionName,
        string resourceGroupName,
        string providerName,
        string resourceName,
        string dataStoreTypeName)
    {
        return string.Join("/",
            "/subscriptions", subscriptionName,
            "resourceGroups", resourceGroupName,
            "providers", providerName,
            "dataManagers", resourceName,
            "dataStoreTypes", dataStoreTypeName);
    }

    /// <summary>
    /// This helper method returns the url for data path (DataSource / DataSink)
    /// </summary>
    /// <param name="subscriptionName">Subscription name.</param>
    /// <param name="resourceGroupName">Resource group name.</param>
    /// <param name="providerName">Resource provider name</param>
    /// <param name="resourceName">resoruce name</param>
    /// <param name="dataSourceName">Data source name (Data source / Data sink)</param>
    /// <returns></returns>
    internal static string GetDataPath(string subscriptionName,
        string resourceGroupName,
        string providerName,
        string resourceName,
        string dataSourceName)
    {
        return string.Join("/",
            "/subscriptions", subscriptionName,
            "resourcegroups", resourceGroupName,
            "providers", providerName,
            "dataManagers", resourceName,
            "dataStores", dataSourceName);
    }

    /// <summary>
    /// This helper method returns the url for running the job.
    /// </summary>
    /// <param name="hostUrl">Host url.</param>
    /// <param name="subscriptionName">Subscription name.</param>
    /// <param name="resourceGroupName">Resource group name.</param>
    /// <param name="resourceName">Resource name.</param>
    /// <param name="providerName">Provider name.</param>
    /// <param name="jobDefinitionName">Job definition name.</param>
    /// <returns>Run job url.</returns>
    internal static string GetRunJobUrl(string hostUrl,
            string subscriptionName,
            string resourceGroupName,
            string resourceName,
            string providerName,
            string jobDefinitionName)
    {
        return string.Join("/", hostUrl,
            "subscriptions", subscriptionName,
            "resourceGroups", resourceGroupName,
            "providers", providerName,
            "dataManagers", resourceName,
            "dataServices/DataTransformation/jobDefinitions",
            jobDefinitionName, "run");
    }

    /// <summary>
    /// This helper method returns the url for fetching the job definition.
    /// </summary>
    /// <param name="hostUrl">Host url.</param>
    /// <param name="subscriptionName">Subscription name.</param>
    /// <param name="resourceGroupName">Resource group name.</param>
    /// <param name="resourceName">Resource name.</param>
    /// <param name="providerName">Provider name.</param>
    /// <param name="jobDefinitionName">Job definition name.</param>
    /// <returns>Run job url.</returns>
    internal static string GetJobDefinitionUrl(string hostUrl,
            string subscriptionName,
            string resourceGroupName,
            string resourceName,
            string providerName,
            string jobDefinitionName)
    {
        return string.Join("/", hostUrl,
            "subscriptions", subscriptionName,
            "resourceGroups", resourceGroupName,
            "providers", providerName,
            "dataManagers", resourceName,
            "dataServices/DataTransformation/jobDefinitions",
            jobDefinitionName);
    }

    /// <summary>
    /// This helper method returns the url for fetching the job definition.
    /// </summary>
    /// <param name="hostUrl">Host url.</param>
    /// <param name="subscriptionName">Subscription name.</param>
    /// <param name="resourceGroupName">Resource group name.</param>
    /// <param name="resourceName">Resource name.</param>
    /// <param name="providerName">Provider name.</param>
    /// <param name="jobDefinitionName">Job definition name.</param>
    /// <returns>Run job url.</returns>
    internal static string GetPublicKeysUrl(string hostUrl,
            string subscriptionName,
            string resourceGroupName,
            string resourceName,
            string providerName)
    {
        return string.Join("/", hostUrl,
            "subscriptions", subscriptionName,
            "resourceGroups", resourceGroupName,
            "providers", providerName,
            "dataManagers", resourceName,
            "publicKeys/default");
    }
}
