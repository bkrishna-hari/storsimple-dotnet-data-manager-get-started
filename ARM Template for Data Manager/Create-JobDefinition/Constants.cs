//---------------------------------------------------------------
//      Copyright (c) Microsoft Corporation. All rights reserved.
//----------------------------------------------------------------

/// <summary>
/// This class contains solution wide constants.
/// </summary>
internal class Constants
{
    public const int ConnectionLimitMultiplier = 48;
    public const int MaxServicePointIdleTime = 90000;
    public const string TokenProvider = "TokenProvider";
    public const string AccessToken = "_accessToken";
    public const string AccessTokenType = "_accessTokenType";
    public const string Expiration = "_expiration";
    public const string Authorization = "Authorization";
    public const string AuthorizationDelimiter = " ";
    public const string POST = "POST";
    public const string GET = "GET";
    public const string PUT = "PUT";
    public const int IndentSpaces = 4;
    public const string ApiVersion = "api-version";
    public const string ContentTypeJson = "application/json";
    public const string AzureAsyncOperation = "Azure-AsyncOperation";
    public const string Location = "Location";
    public const string Properties = "properties";
    public const string JobStatus = "jobStatus";
    public const string ItemsProcessed = "itemsProcessed";
    public const string JobStages = "jobStages";
    public const string Details = "details";
    public const string ErrorDetails = "errorDetails";
    public const string DataServiceInput = "dataServiceInput";
    public const string DataTransformationJobInput = "dataTransformationJobInput";
    public const string DataServiceLevel1Key = "dataServiceLevel1Key";
    public const string DataServiceLevel2Key = "dataServiceLevel2Key";
    public const string State = "Enabled";
    public const string ServiceEncryptionKey = "ServiceEncryptionKey";
    public const string StorageAccountAccessKey = "StorageAccountAccessKey";
    public const string MediaServicesAccessKey = "MediaServicesAccessKey";
    public const string StorageAccountAccessKeyForQueue = "StorageAccountAccessKeyForQueue";
    public const string Algorithm = "RSA1_5";
    public const int MinimumClaimValiditiyDurationInMinutes = 10;

    // DOGFOOD/PARTNER endpoint
    public const string FrontdoorUrl = "https://management.azure.com";

    // TEST AAD endpoint
    public const string AadUrl = "https://login.microsoftonline.com";
    public const string TokenUrl = "https://management.core.windows.net";

    // Resource provider settings
    public const string ResourceProviderName = "Microsoft.HybridData";
    public const string StorSimpleProviderName = "Microsoft.StorSimple";
    public const string MediaProviderName = "Microsoft.Media";
    public const string StorageProviderName = "Microsoft.Storage";
    public const string StorSimpleDataStoreTypeName = "Storsimple8000Series";
    public const string MediaDataStoreTypeName = "AzureMediaServicesAccount";
    public const string StorageDataStoreTypeName = "AzureStorageAccount";
    public const string ResourceProviderApiVersion = "2016-06-01";
}