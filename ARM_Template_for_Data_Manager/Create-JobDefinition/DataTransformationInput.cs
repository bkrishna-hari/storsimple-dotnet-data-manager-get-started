//---------------------------------------------------------------
//      Copyright (c) Microsoft Corporation. All rights reserved.
//----------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

public class Configuration
{
    public string subscriptionId { get; set; }
    public string tenantId { get; set; }
    public string clientId { get; set; }
    public string activeDirectoryKey { get; set; }
}

public class ResourceGroup
{
    public string name { get; set; }
    public string location { get; set; }
}

public class DataSource
{
    public string name { get; set; }
    public string resourceId { get; set; }
    public string serviceEncryptionKey { get; set; }
}

public class DataSink
{
    public string name { get; set; }
    public string type { get; set; }
    public string storageAccountName { get; set; }
    public string storageAccountKey { get; set; }
    public string mediaServiceName { get; set; }
    public string mediaServiceKey { get; set; }
}

public class JobDefinition
{
    public string azureStorageType { get; set; }
    public string backupChoice { get; set; }
    public string containerName { get; set; }
    public string deviceName { get; set; }
    public string fileNameFilter { get; set; }
    public string isDirectoryMode { get; set; }
    public string name { get; set; }
    public string rootDirectories { get; set; }
    public string userConfirmation { get; set; }
    public string volumeName { get; set; }
}

public class DataTransformation
{
    public string name { get; set; }
    public DataSource dataSource { get; set; }
    public DataSink dataSink { get; set; }
    public JobDefinition jobDefinition { get; set; }
}

public class DataTransformationInput
{
    public Configuration configuration { get; set; }
    public ResourceGroup resourceGroup { get; set; }
    public DataTransformation dataTransformation { get; set; }
}