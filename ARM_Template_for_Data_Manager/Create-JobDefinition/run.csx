//---------------------------------------------------------------
//      Copyright (c) Microsoft Corporation. All rights reserved.
//----------------------------------------------------------------

#load "ConfigurationParams.cs"
#load "Constants.cs"
#load "DataSinkInput.cs"
#load "DataSourceInput.cs"
#load "DataTransformationInput.cs"
#load "DataTransformationJob.cs"
#load "DefaultServiceHelper.cs"
#load "DefaultServiceHelperUrls.cs"
#load "IServiceHelper.cs"
#load "JobDefinitionInput.cs"
#load "PublicKeys.cs"
#r "System.Configuration"

using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Configuration;

public static void Run(TimerInfo myTimer, TraceWriter log)
{
    log.Info($"C# Timer trigger function executed at: {DateTime.Now}");

    var appSettings = ConfigurationManager.AppSettings;

    /*// Status
    string status = appSettings["NEW_JOBDEFINITION_STATUS"];
    if (string.IsNullOrEmpty(status))
    {
        log.Info($"AppSetting 'NEW_JOBDEFINITION_STATUS' not available.");
        return;
    }
    if (status.ToUpper().Trim() != "NONE")
    {
        log.Info($"Job definition creation status: {status}");
        return;
    }

    // Update Status
    appSettings["NEW_JOBDEFINITION_STATUS"] = "In progress";*/

    /// Data transformation Config params
    string subscriptionId = appSettings["SUBSCRIPTIONID"];
    string tenantId = appSettings["TENANTID"];
    string clientId = appSettings["CLIENTID"];
    string activeDirectoryKey = appSettings["ACTIVEDIRECTORYKEY"];

    /// Resource group params
    string resourceGroupName = appSettings["RESOURCEGROUPNAME"];
    string location = appSettings["LOCATION"];

    /// Data Manager params
    string resourceName = appSettings["DATAMANAGERNAME"];

    /// Data Source params
    string dataSourceName = appSettings["DATASOURCENAME"];
    string resourceId = appSettings["RESOURCEID"];
    string serviceEncryptionKey = appSettings["SERVICEENCRYPTIONKEY"];

    /// Data Sink params
    string dataSinkName = appSettings["DATASINKNAME"];
    string storageAccountName = appSettings["STORAGEACCOUNTNAME"];
    string storageAccountKey = appSettings["STORAGEACCOUNTKEY"];
    string mediaServiceName = appSettings["MEDIA_ACCOUNT_NAME"];
    string mediaServiceKey = appSettings["MEDIA_ACCOUNT_KEY"];

    /// Job Definition params
    string jobDefinitionName = appSettings["JOBDEFINITIONNAME"];
    string deviceName = appSettings["DEVICENAME"];
    string volumeName = appSettings["VOLUMENAME"];
    string rootDirectories = appSettings["ROOTDIRECTORIES"];
    string fileNameFilter = appSettings["FILENAMEFILTER"];
    string containerName = string.Empty;
    string backupChoice = appSettings["BACKUPCHOICE"];
    string userConfirmation = appSettings["USERCONFIRMATION"];
    string isDirectoryMode = appSettings["ISDIRECTORYMODE"];

    string message = string.Empty;
    bool isResourceCreated = false;

    var configParams = new ConfigurationParams
    {
        SubscriptionId = subscriptionId,
        TenantId = tenantId,
        ClientId = clientId,
        ActiveDirectoryKey = activeDirectoryKey,
        ResourceGroupName = resourceGroupName,
        ResourceName = resourceName,
    };

    // Initialize the Data Transformation Job instance.
    DataTransformationJob dataTransformationJob = new DataTransformationJob(configParams);

    // Read public keys
    PublicKeys publicKeys = dataTransformationJob.GetPublicKeys();

    // Encrypt Customer secrets
    string encryptedServiceEncryptionKey = EncryptCustomerSecret(serviceEncryptionKey, publicKeys);

    // Create StorSimple Data Source
    DataSourceInput dataSourceInput = new DataSourceInput()
    {
        name = dataSourceName,
        properties = new DataSourceProperties()
        {
            repositoryId = DefaultServiceHelperUrls.GetDataSourceRepositoryUrl(subscriptionId, resourceGroupName, Constants.StorSimpleProviderName, resourceId),
            dataStoreTypeId = DefaultServiceHelperUrls.GetDataStoreTypeUrl(subscriptionId, resourceGroupName, Constants.ResourceProviderName, resourceName, Constants.StorSimpleDataStoreTypeName),
            state = Constants.State,
            extendedProperties = new DataSourceExtendedProperty()
            {
                resourceId = resourceId
            },
            customerSecrets = new List<DataSourceCustomerSecret>()
            {
                new DataSourceCustomerSecret() {keyIdentifier = Constants.ServiceEncryptionKey, keyValue = encryptedServiceEncryptionKey, algorithm = Constants.Algorithm }
            }
        }

    };

    // Create/Update data source
    log.Info($"Data source ({dataSourceName}) creation initiated.");

    isResourceCreated = dataTransformationJob.CreateDataSource(resourceGroupName, resourceName, dataSourceInput, dataSourceName, out message);
    if (!isResourceCreated)
    {
        log.Info($"Failed to create new Data source.");
        log.Info(message);
        return;
    }
    else
    {
        log.Info($"Data source ({dataSourceName}) created successfully.");
    }


    string encryptedStorageAccountKey = EncryptCustomerSecret(storageAccountKey, publicKeys);
    string encryptedMediaServiceKey = EncryptCustomerSecret(mediaServiceKey, publicKeys);

    // Create StorSimple Data Sink (MediaService / StorageAccount)
    DataSinkInput dataSinkInput = new DataSinkInput()
    {
        name = dataSinkName,
        properties = new DataSinkProperties()
        {
            repositoryId = DefaultServiceHelperUrls.GetMediaServiceDataSinkRepositoryUrl(subscriptionId, resourceGroupName, Constants.MediaProviderName, mediaServiceName),
            dataStoreTypeId = DefaultServiceHelperUrls.GetDataStoreTypeUrl(subscriptionId, resourceGroupName, Constants.ResourceProviderName, resourceName, Constants.MediaDataStoreTypeName),
            state = Constants.State,
            extendedProperties = new DataSinkExtendedProperty()
            {
                storageAccountNameForQueue = storageAccountName
            },
            customerSecrets = new List<DataSinkCustomerSecret>()
                {
                    new DataSinkCustomerSecret() {keyIdentifier = Constants.MediaServicesAccessKey, keyValue = encryptedMediaServiceKey, algorithm = Constants.Algorithm },
                    new DataSinkCustomerSecret() {keyIdentifier = Constants.StorageAccountAccessKeyForQueue, keyValue = encryptedStorageAccountKey, algorithm = Constants.Algorithm }
                }
        }
    };

    // Create/Update data sink
    log.Info($"Data sink ({dataSinkName}) creation initiated.");
    isResourceCreated = dataTransformationJob.CreateDataSink(resourceGroupName, resourceName, dataSinkInput, dataSinkName, out message);
    if (!isResourceCreated)
    {
        log.Info($"Failed to create new Data sink.");
        log.Info(message);
        return;
    }
    else
    {
        log.Info($"Data sink ({dataSinkName}) created successfully.");
    }

    JobDefinitionInput jobDefinitionInput = new JobDefinitionInput()
    {
        name = jobDefinitionName,
        properties = new JobDefinitionProperties()
        {
            dataSourceId = DefaultServiceHelperUrls.GetDataPath(subscriptionId, resourceGroupName, Constants.ResourceProviderName, resourceName, dataSourceName),
            dataSinkId = DefaultServiceHelperUrls.GetDataPath(subscriptionId, resourceGroupName, Constants.ResourceProviderName, resourceName, dataSinkName),
            state = Constants.State,
            userConfirmation = userConfirmation,
            dataServiceInput = new DataServiceProperties()
            {
                backupChoice = backupChoice,
                deviceName = deviceName,
                fileNameFilter = fileNameFilter,
                isDirectoryMode = isDirectoryMode,
                rootDirectories = new List<string>() { rootDirectories },
                volumeNames = new List<string>() { volumeName }
            }
        }
    };

    // Create/Update data sink
    log.Info($"Job definition ({jobDefinitionName}) creation initiated.");
    isResourceCreated = dataTransformationJob.CreateJobDefinition(resourceGroupName, resourceName, jobDefinitionInput, jobDefinitionName, out message);
    if (!isResourceCreated)
    {
        log.Info($"Failed to create new Job definition.");
        log.Info(message);
        return;
    }
    else
    {
        log.Info($"Job definition ({jobDefinitionName}) created successfully.");
    }

    // Read Job definition params
    DataServiceProperties dataServiceInput = dataTransformationJob.GetJobDefinitionParameters(jobDefinitionName).properties.dataServiceInput;

    string retryAfter = string.Empty;
    string trackJobUrl = string.Empty;
    dataTransformationJob.RunJobAsync(jobDefinitionName, dataServiceInput, out trackJobUrl, out retryAfter);
    log.Info($"Job triggered successfully.");
    log.Info($"Job url: {trackJobUrl}");

    //// Update Status
    //appSettings["NEW_JOBDEFINITION_STATUS"] = "Created";
}

/// <summary>
/// This method converts customer secrets into encrypted value
/// </summary>
/// <param name="key">Key.</param>
/// <param name="publicKeys">Public keys.</param>
/// <returns></returns>
private static string EncryptCustomerSecret(string key, PublicKeys publicKeys)
{
    byte[] Level1KeyModulus = Convert.FromBase64String(publicKeys.dataServiceLevel1Key.keyModulus);
    byte[] Level1KeyExponent = Convert.FromBase64String(publicKeys.dataServiceLevel1Key.keyExponent);

    byte[] sekArray = Encoding.UTF8.GetBytes(key);

    string firstPass = EncryptUsingJsonWebKey(sekArray, Level1KeyModulus, Level1KeyExponent);

    byte[] Level2KeyModulus = Convert.FromBase64String(publicKeys.dataServiceLevel2Key.keyModulus);
    byte[] Level2KeyExponent = Convert.FromBase64String(publicKeys.dataServiceLevel2Key.keyExponent);

    string encryptedKey = EncryptUsingJsonWebKey(Encoding.UTF8.GetBytes(firstPass), Level2KeyModulus, Level2KeyExponent);
    return encryptedKey;
}

/// <summary>
/// This method converts customer secrets into encrypted value
/// </summary>
/// <param name="plainTextArray">Plain text byte array</param>
/// <param name="levelKeyModulus">Level key modulus</param>
/// <param name="levelKeyExponent">Level key exponent</param>
/// <returns></returns>
public static string EncryptUsingJsonWebKey(byte[] plainTextArray, byte[] levelKeyModulus, byte[] levelKeyExponent)
{
    int start = 0;
    StringBuilder builder = new StringBuilder();
    List<byte> plainTextList = plainTextArray.ToList();
    int remainingBytes = plainTextArray.Length;

    while (remainingBytes >= 1)
    {
        int chunkLength = remainingBytes > 245 ? 245 : remainingBytes;
        byte[] encryptedText;
        byte[] plainChunkText = plainTextList.GetRange(start, chunkLength).ToArray();

        using (var rsa = new RSACryptoServiceProvider())
        {
            var param = new RSAParameters() { Modulus = levelKeyModulus, Exponent = levelKeyExponent };
            rsa.ImportParameters(param);
            encryptedText = rsa.Encrypt(plainChunkText, false);
        }

        string encryptedSecret = Convert.ToBase64String(encryptedText);
        builder.Append(encryptedSecret);
        builder.Append(":");
        start += chunkLength;
        remainingBytes -= chunkLength;
    }
    builder.Remove(builder.Length - 1, 1);
    return builder.ToString();
}