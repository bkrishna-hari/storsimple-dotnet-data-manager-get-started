//---------------------------------------------------------------
//      Copyright (c) Microsoft Corporation. All rights reserved.
//----------------------------------------------------------------

using Microsoft.Rest;
using Microsoft.Rest.Azure.Authentication;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Newtonsoft.Json.Serialization;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Reflection;
using System.Runtime.Serialization.Formatters;
using System.Text;
using System.Threading;

/// <summary>
/// This is a helper class which sets up the HTTP client
/// to make run and track job calls.
/// </summary>
internal class DefaultServiceHelper : IServiceHelper
{
    public static string GetJobStatusInterval;
    public static string GetPutInterval;
    public static string GetRequestUrl;

    private static HttpClient httpClient = null;
    private static DateTimeOffset expirationTime;

    private static string tenantId;
    private static string clientId;
    private static string secret;
    private static string subscriptionId;

    /// <summary>
    /// Constuctor.
    /// It initalizes the http client to make the REST calls.
    /// </summary>
    /// <param name="configParams">Configuration parameters.</param>
    public DefaultServiceHelper(ConfigurationParams configParams)
    {
        // Service point manager configuration.                                                                                        
        ServicePointManager.DefaultConnectionLimit = Environment.ProcessorCount * Constants.ConnectionLimitMultiplier;
        ServicePointManager.MaxServicePointIdleTime = Constants.MaxServicePointIdleTime;
        ServicePointManager.UseNagleAlgorithm = false;
        ServicePointManager.Expect100Continue = false;

        tenantId = configParams.TenantId;
        clientId = configParams.ClientId;
        secret = configParams.ActiveDirectoryKey;
        subscriptionId = configParams.SubscriptionId;

        var credentials = GetCredentials();
        InitializeHttpClient(credentials);
    }

    /// <summary>
    /// This method initalizes the Http client.
    /// </summary>
    /// <param name="credentials"></param>
    private static void InitializeHttpClient(ServiceClientCredentials credentials)
    {
        // Fetching private fields and properties using reflection.
        // This is required to construct the authorization header for the HTTP client.
        PropertyInfo tokenProviderProperty = credentials.GetType().GetProperties(BindingFlags.NonPublic | BindingFlags.Instance).Single(pi => pi.Name == Constants.TokenProvider);
        var tokenProvider = (ITokenProvider)tokenProviderProperty.GetValue(credentials, null);

        FieldInfo accessTokenField = tokenProvider.GetType().GetFields(BindingFlags.NonPublic | BindingFlags.Instance).Single(pi => pi.Name == Constants.AccessToken);
        string accessToken = (string)accessTokenField.GetValue(tokenProvider);

        FieldInfo accessTokenTypeField = tokenProvider.GetType().GetFields(BindingFlags.NonPublic | BindingFlags.Instance).Single(pi => pi.Name == Constants.AccessTokenType);
        string accessTokenType = (string)accessTokenTypeField.GetValue(tokenProvider);

        FieldInfo expirationField = tokenProvider.GetType().GetFields(BindingFlags.NonPublic | BindingFlags.Instance).Single(pi => pi.Name == Constants.Expiration);
        expirationTime = (DateTimeOffset)expirationField.GetValue(tokenProvider);

        // Initialize the Http client.
        httpClient = new HttpClient();
        httpClient.DefaultRequestHeaders.Add(Constants.Authorization, accessTokenType + Constants.AuthorizationDelimiter + accessToken);
    }

    /// <summary>
    /// This method retrieves the credentials.
    /// </summary>
    /// <returns></returns>
    private static ServiceClientCredentials GetCredentials()
    {
        ServiceClientCredentials credentials;
        try
        {
            credentials = ApplicationTokenProvider.LoginSilentAsync(tenantId, clientId, secret,
                new ActiveDirectoryServiceSettings()
                {
                    AuthenticationEndpoint = new Uri(Constants.AadUrl),
                    TokenAudience = new Uri(Constants.TokenUrl),
                    ValidateAuthority = true,
                }).Result;
        }
        catch (Exception e)
        {
            //throw new WebJobException("Invalid configuration params.", e);
            throw e;
        }
        return credentials;
    }

    /// <summary>
    /// This method appends the api version to the url.
    /// </summary>
    /// <param name="url">Url.</param>
    /// <param name="apiVersion">Api version</param>
    /// <returns>Url appended with the api version.</returns>
    private static string AppendApiVersion(string url, string apiVersion = Constants.ResourceProviderApiVersion)
    {
        return !string.IsNullOrWhiteSpace(apiVersion) ? string.Concat(url, "?", Constants.ApiVersion, "=", apiVersion) : url;
    }

    /// <summary>
    /// This method deserializes an object from Json.
    /// </summary>
    /// <typeparam name="T">The object to which the json will be deserialized.</typeparam>
    /// <param name="json">The json string.</param>
    /// <returns>Deserialized object.</returns>
    private static T FromJson<T>(string json)
    {
        var settings = new JsonSerializerSettings
        {
            ContractResolver = new CamelCasePropertyNamesContractResolver(),
            Converters = new List<JsonConverter>
            {
                new StringEnumConverter { CamelCaseText = false },
                new IsoDateTimeConverter { DateTimeStyles = DateTimeStyles.AssumeUniversal },
            },
        };

        return JsonConvert.DeserializeObject<T>(json, settings);
    }

    /// <summary>
    /// This method serializes an object to a json.
    /// </summary>
    /// <param name="objectToConvert">The object to serialize.</param>
    /// <returns>Serialized json stirng.</returns>
    private static string ToJson(object objectToConvert)
    {
        var settings = new JsonSerializerSettings
        {
            TypeNameHandling = TypeNameHandling.Auto,
            TypeNameAssemblyFormat = FormatterAssemblyStyle.Simple,
        };

        settings.Converters.Add(new StringEnumConverter());

        return JsonConvert.SerializeObject(objectToConvert, Formatting.Indented, settings);
    }

    /// <summary>
    /// Helper method which makes the REST call to create new Data source
    /// </summary>
    /// <param name="resourceGroup">Resource group.</param>
    /// <param name="payload">Paylaod for the REST call.</param>
    public bool CreateDataStore(string resourceGroup, string resourceName, string payload, string dataStoreName, out string message)
    {
        string url = DefaultServiceHelperUrls.GetDataStoreUrl(Constants.FrontdoorUrl, subscriptionId, resourceGroup, resourceName, Constants.ResourceProviderName, dataStoreName);
        message = SubmitRequest(url, payload, Constants.PUT);

        if (message.ToUpper().Contains("REQUEST FAILED"))
            return false;

        string getRequestUrl = message;
        return GetDataRequestByUrl(getRequestUrl, out message);
    }

    /// <summary>
    /// Helper method which makes the REST call to create new Job definition
    /// </summary>
    /// <param name="resourceGroup">Resource group name</param>
    /// <param name="resourceName">Resource name</param>
    /// <param name="payload">Payload</param>
    /// <param name="jobDefinitionName">Job Definition name</param>
    /// <param name="message">Message</param>
    /// <returns></returns>
    public bool CreateJobDefinition(string resourceGroup, string resourceName, string payload, string jobDefinitionName, out string message)
    {
        string url = DefaultServiceHelperUrls.GetJobDefinitionUrl(Constants.FrontdoorUrl, subscriptionId, resourceGroup, resourceName, Constants.ResourceProviderName, jobDefinitionName);
        message = SubmitRequest(url, payload, Constants.PUT);

        if (message.ToUpper().Contains("REQUEST FAILED"))
            return false;

        string getRequestUrl = message;
        return GetDataRequestByUrl(getRequestUrl, out message);
    }

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
    public void RunJob(string resourceGroup, string resourceName, string jobDefinitionName, string payload, out string trackJobUrl, out string interval)
    {
        string url = DefaultServiceHelperUrls.GetRunJobUrl(Constants.FrontdoorUrl, subscriptionId, resourceGroup, resourceName, Constants.ResourceProviderName, jobDefinitionName);
        trackJobUrl = SubmitRequest(url, payload, Constants.POST);
        interval = GetJobStatusInterval;
    }

    /// <summary>
    /// Helper method which makes the REST call for retrieving the data transformation object info by Url
    /// </summary>
    /// <param name="url">Url.</param>
    /// <param name="message">Message.</param>
    /// <returns></returns>
    public bool GetDataRequestByUrl(string url, out string message)
    {
        do
        {
            int retryInterval;
            bool isConverted = int.TryParse(GetPutInterval, out retryInterval);

            if (!isConverted)
                retryInterval = 10;

            Thread.Sleep(retryInterval * 1000);
            message = SubmitRequest(url, null, Constants.GET);

            if (message.ToUpper().Contains("REQUEST FAILED."))
                return false;

            url = GetRequestUrl;
        }
        while (string.IsNullOrEmpty(message));

        return true;
    }

    /// <summary>
    /// Helper method which makes the REST call for tracking the job.
    /// </summary>
    /// <param name="url">Track job url.</param>
    /// <returns>Response</returns>
    public string TrackJob(string url)
    {
        // A new claim needs to be generated in case the current claim expires.
        // Current claim is valid only for an hour.
        if (expirationTime.Subtract(DateTime.UtcNow).TotalMinutes < Constants.MinimumClaimValiditiyDurationInMinutes)
        {
            var credentials = GetCredentials();
            InitializeHttpClient(credentials);
        }

        string response = SubmitRequest(url, string.Empty, Constants.GET);
        return response;
    }

    /// <summary>
    /// Helper method which makes the REST call for retrieving a job.
    /// </summary>
    /// <param name="jobDefinitionName">Job definition name.</param>
    /// <param name="resourceGroup">Resource group.</param>
    /// <param name="resourceName">Resource name.</param>
    /// <returns>Response.</returns>
    public string GetJob(string jobDefinitionName, string resourceGroup, string resourceName)
    {
        string url = DefaultServiceHelperUrls.GetRunJobUrl(Constants.FrontdoorUrl, subscriptionId, resourceGroup, resourceName, Constants.ResourceProviderName, jobDefinitionName);
        return SubmitRequest(url, string.Empty, Constants.GET);
    }

    /// <summary>
    /// Helper method which makes the REST call for retrieving a job definition.
    /// </summary>
    /// <param name="jobDefinitionName">Job definition name.</param>
    /// <param name="resourceGroup">Resource group.</param>
    /// <param name="resourceName">Resource name.</param>
    /// <returns>Response.</returns>
    public string GetJobDefinition(string jobDefinitionName, string resourceGroup, string resourceName)
    {
        string url = DefaultServiceHelperUrls.GetJobDefinitionUrl(Constants.FrontdoorUrl, subscriptionId, resourceGroup, resourceName, Constants.ResourceProviderName, jobDefinitionName);
        string response = SubmitRequest(url, string.Empty, Constants.GET);
        return response;
    }

    /// <summary>
    /// Helper method which makes the REST call for retrieving public keys.
    /// </summary>
    /// <param name="jobDefinitionName">Job definition name.</param>
    /// <param name="resourceGroup">Resource group.</param>
    /// <param name="resourceName">Resource name.</param>
    /// <returns>Response.</returns>
    public string GetPublicKeys(string resourceGroup, string resourceName)
    {
        string url = DefaultServiceHelperUrls.GetPublicKeysUrl(Constants.FrontdoorUrl, subscriptionId, resourceGroup, resourceName, Constants.ResourceProviderName);
        string response = string.Empty;
        int cnt = 1;

        do
        {
            Thread.Sleep(10 * 1000);
            response = SubmitRequest(url, string.Empty, Constants.GET);
            cnt++;
        }
        while (response.ToUpper().Contains("REQUEST FAILED.") && cnt <= 6);

        return response;
    }

    /// <summary>
    /// This helper method to submit the request.
    /// </summary>
    /// <param name="url">Url.</param>
    /// <param name="payload">Payload.</param>
    /// <param name="methodType">Method type (POST/GET).</param>
    /// <returns>Response.</returns>
    public static string SubmitRequest(string url, string payload, string methodType)
    {
        if (!url.Contains(Constants.ApiVersion))
            url = AppendApiVersion(url);

        var request = (HttpWebRequest)WebRequest.Create(url);
        request.Method = methodType;
        request.ContentType = Constants.ContentTypeJson;

        HttpResponseMessage response = new HttpResponseMessage();

        try
        {
            switch (methodType)
            {
                case Constants.GET:
                    response = httpClient.GetAsync(url).Result;
                    break;

                case Constants.POST:
                    response = httpClient.PostAsync(url,
                        new StringContent(payload, Encoding.UTF8, Constants.ContentTypeJson))
                        .Result;
                    break;

                case Constants.PUT:
                    response = httpClient.PutAsync(url,
                        new StringContent(payload, Encoding.UTF8, Constants.ContentTypeJson))
                        .Result;
                    break;
            }
        }
        catch (Exception ex)
        {
            //throw new WebJobException("Failed while submitting request.", e);
            return string.Concat("Request failed. ", Environment.NewLine, "Message: ", ex.Message);
        }

        // Un comment while debugging.
        // PrintResponse(response);

        if (response.StatusCode != HttpStatusCode.OK
            && response.StatusCode != HttpStatusCode.Accepted)
        {
            return string.Concat("Request failed. Received HTTP ", response.StatusCode);
            //throw new WebJobException(message);
        }

        if (string.Equals(methodType, Constants.GET))
        {
            if (response != null && response.Headers != null && response.Headers.Location != null)
                GetRequestUrl = response.Headers.Location.ToString();

            return response.Content.ReadAsStringAsync().Result;
        }
        else if (string.Equals(methodType, Constants.PUT))
        {
            GetPutInterval = response.Headers.RetryAfter.ToString();
            IEnumerable<string> values;
            if (response.Headers.TryGetValues(Constants.Location, out values))
            {
                return values.First();
            }
        }
        else if (string.Equals(methodType, Constants.POST))
        {
            GetJobStatusInterval = response.Headers.RetryAfter.ToString();
            IEnumerable<string> values;
            if (response.Headers.TryGetValues(Constants.AzureAsyncOperation, out values))
            {
                return values.First();
            }
        }

        return string.Empty;
    }
}
