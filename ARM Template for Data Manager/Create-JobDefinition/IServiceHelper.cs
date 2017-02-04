//---------------------------------------------------------------
//      Copyright (c) Microsoft Corporation. All rights reserved.
//----------------------------------------------------------------

/// <summary>
/// Service helper interface.
/// </summary>
internal interface IServiceHelper
{
    /// <summary>
    /// Helper method which makes the REST call to create new data source.
    /// </summary>
    /// <param name="resourceGroup"></param>
    /// <param name="resourceName"></param>
    /// <param name="dataManagerName"></param>
    /// <param name="payload"></param>
    bool CreateDataStore(string resourceGroup, string resourceName, string payload, string dataStoreName, out string message);

    /// <summary>
    /// Helper method which makes the REST call to create new job definition.
    /// </summary>
    /// <param name="resourceGroup">Resource group name.</param>
    /// <param name="resourceName">Resource name.</param>
    /// <param name="payload">Payload.</param>
    /// <param name="jobDefinitionName">Job definition name.</param>
    /// <param name="message">Message.</param>
    /// <returns></returns>
    bool CreateJobDefinition(string resourceGroup, string resourceName, string payload, string jobDefinitionName, out string message);

    /// <summary>
    /// Helper method which makes the REST call for retrieving public keys.
    /// </summary>
    /// <param name="resourceGroup">Resource group.</param>
    /// <param name="resourceName">Resource name.</param>
    string GetPublicKeys(string resourceGroup, string resourceName);

    /// <summary>
    /// Helper method which makes the REST call for retrieving resource data.
    /// </summary>
    /// <param name="getRequestUrl">Get request url.</param>
    /// <param name="methodType">Method type.</param>
    /// <param name="status">Status.</param>
    /// <param name="isReadyState">Is resource ready state</param>
    bool GetDataRequestByUrl(string url, out string message);

    /// <summary>
    /// Helper method which makes the REST call for running the job.
    /// </summary>
    /// <param name="resourceGroup">Resource group.</param>
    /// <param name="resourceName">Resource name.</param>
    /// <param name="jobDefinitionName">Job definition name.</param>
    /// <param name="payload">Payload for the REST call.</param>
    /// <param name="trackJobUrl">Returns the url to track the job.</param>
    /// <param name="interval">Returns the ideal interval after 
    /// which the job should be tracked.</param>
    void RunJob(string resourceGroup, string resourceName, string jobDefinitionName, string payload, out string trackJobUrl, out string interval);

    /// <summary>
    /// Helper method which makes the REST call for tracking the job.
    /// </summary>
    /// <param name="url">Track job url.</param>
    /// <returns>Response</returns>
    string TrackJob(string url);

    /// <summary>
    /// Helper method which makes the REST call for retrieving a job.
    /// </summary>
    /// <param name="jobDefinitionName">Job definition name.</param>
    /// <param name="resourceGroup">Resource group.</param>
    /// <param name="resourceName">Resource name.</param>
    /// <returns>Response.</returns>
    string GetJob(string jobDefinitionName, string resourceGroup, string resourceName);

    /// <summary>
    /// Helper method which makes the REST call for retrieving a job definition.
    /// </summary>
    /// <param name="jobDefinitionName">Job definition name.</param>
    /// <param name="resourceGroup">Resource group.</param>
    /// <param name="resourceName">Resource name.</param>
    /// <returns>Response.</returns>
    string GetJobDefinition(string jobDefinitionName, string resourceGroup, string resourceName);
}
