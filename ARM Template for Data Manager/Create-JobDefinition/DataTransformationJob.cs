//---------------------------------------------------------------
//      Copyright (c) Microsoft Corporation. All rights reserved.
//----------------------------------------------------------------

using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

/// <summary>
/// This class is used to trigger and track a Data Transformation Job.
/// </summary>
public class DataTransformationJob
{
    private static IServiceHelper _serviceHelper;
    private readonly string _resourceGroup;
    private readonly string _resourceName;

    /// <summary>
    /// The constructor initializes the service helper 
    /// which is used to trigger and track the job.
    /// </summary>
    /// <param name="configParams">Configuration parameters.</param>
    public DataTransformationJob(ConfigurationParams configParams)
    {
        _resourceGroup = configParams.ResourceGroupName;
        _resourceName = configParams.ResourceName;
        _serviceHelper = new DefaultServiceHelper(configParams);
    }

    /// <summary>
    /// This method creates Data manager resource.
    /// </summary>
    /// <param name="resourceName">Resource name</param>
    internal void CreateDataManager(string resourceName,
        string resourceGroupName, 
        string location)
    {
        // Create the payload to make the call to create data manager resource.
        //string json = JsonConvert.SerializeObject();
    }

    /// <summary>
    /// This method creates new Data source
    /// </summary>
    /// <param name="resourceGroupName">Resource group name.</param>
    /// <param name="resourceName">Resource name.</param>
    /// <param name="dataSourceInput">Data source input</param>
    /// <param name="dataSourceName">data source name</param>
    internal bool CreateDataSource(string resourceGroupName, 
        string resourceName, 
        DataSourceInput dataSourceInput, 
        string dataSourceName, 
        out string message)
    {
        // Create a payload to make the call to create data source.
        string json = JsonConvert.SerializeObject(dataSourceInput);
        return _serviceHelper.CreateDataStore(resourceGroupName, resourceName, json, dataSourceName, out message);
    }

    /// <summary>
    /// This method creates new Data sink
    /// </summary>
    /// <param name="resourceGroupName">Resource group name.</param>
    /// <param name="resourceName">Resource name.</param>
    /// <param name="dataSinkInput">Data sink input.</param>
    /// <param name="dataSinkName">Data sink name.</param>
    internal bool CreateDataSink(string resourceGroupName, 
        string resourceName, 
        DataSinkInput dataSinkInput, 
        string dataSinkName, 
        out string message)
    {
        // Create a payload to make the call to create data source.
        string json = JsonConvert.SerializeObject(dataSinkInput);
        return _serviceHelper.CreateDataStore(resourceGroupName, resourceName, json, dataSinkName, out message);
    }

    /// <summary>
    /// This method creates new Job definition
    /// </summary>
    /// <param name="resourceGroupName">Resource group name</param>
    /// <param name="resourceName">Resource name.</param>
    /// <param name="jobDefinitionInput">Job definition input.</param>
    /// <param name="jobDefinitionName">Job definition name.</param>
    /// <param name="message">Message</param>
    /// <returns></returns>
    internal bool CreateJobDefinition(string resourceGroupName, 
        string resourceName, 
        JobDefinitionInput jobDefinitionInput, 
        string jobDefinitionName, 
        out string message)
    {
        // Create a payload to make the call to create data source.
        string json = JsonConvert.SerializeObject(jobDefinitionInput);
        return _serviceHelper.CreateJobDefinition(resourceGroupName, resourceName, json, jobDefinitionName, out message);
    }

    /// <summary>
    /// This method returns public keys of resource.
    /// </summary>
    /// <param name="jobDefinitionName">Job definition name.</param>
    /// <returns>Data transformation input.</returns>
    public PublicKeys GetPublicKeys()
    {
        var getResponse = _serviceHelper.GetPublicKeys(_resourceGroup, _resourceName);

        if (getResponse == null)
        {
            //throw new WebJobException("Job definition entity is being returned as null.");
        }

        JObject output = JObject.Parse(getResponse);
        string dataServiceLevel1KeyString = output[Constants.Properties][Constants.DataServiceLevel1Key].ToString();
        var dataServiceLevel1Key = JsonConvert.DeserializeObject<DataServiceLevel1Key>(dataServiceLevel1KeyString);
        string dataServiceLevel2KeyString = output[Constants.Properties][Constants.DataServiceLevel2Key].ToString();
        var dataServiceLevel2Key = JsonConvert.DeserializeObject<DataServiceLevel2Key>(dataServiceLevel2KeyString);
        var publicKeys = new PublicKeys()
        {
            dataServiceLevel1Key = dataServiceLevel1Key,
            dataServiceLevel2Key = dataServiceLevel2Key
        };
        return publicKeys;
    }

    /// <summary>
    /// This method triggers a Data Transformation Job.
    /// </summary>
    /// <param name="jobDefinitionName">Job definition name.</param>
    /// <param name="runJobParams">Parameters required to run the job.</param>
    /// <param name="trackJobUrl">Returns the url to track the job.</param>
    /// <param name="jobStatusIntervalInSeconds">Returns the ideal interval after which 
    /// the job status should be queried using TrackJobAsync.</param>
    public void RunJobAsync(string jobDefinitionName,
        DataServiceProperties runJobParams,
        out string trackJobUrl,
        out string jobStatusIntervalInSeconds)
    {

        // Create the payload to make the call to run the job.
        string json = JsonConvert.SerializeObject(runJobParams);

        _serviceHelper.RunJob(_resourceGroup,
            _resourceName,
            jobDefinitionName,
            json,
            out trackJobUrl,
            out jobStatusIntervalInSeconds);
    }

    /// <summary>
    /// This method returns the parameters with which the job definition is configured.
    /// </summary>
    /// <param name="jobDefinitionName">Job definition name.</param>
    /// <returns>Data transformation input.</returns>
    public JobDefinitionInput GetJobDefinitionParameters(string jobDefinitionName)
    {
        var getResponse = _serviceHelper.GetJobDefinition(jobDefinitionName, _resourceGroup, _resourceName);

        JObject output = JObject.Parse(getResponse);
        string jobDefinitionInputString = output.ToString();
        var jobDefinitionInput = JsonConvert.DeserializeObject<JobDefinitionInput>(jobDefinitionInputString);
        return jobDefinitionInput;
    }
}
