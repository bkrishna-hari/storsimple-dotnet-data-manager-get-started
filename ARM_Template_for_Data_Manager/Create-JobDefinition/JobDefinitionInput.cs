//---------------------------------------------------------------
//      Copyright (c) Microsoft Corporation. All rights reserved.
//----------------------------------------------------------------

using Newtonsoft.Json;
using System.Collections.Generic;

public class DataServiceProperties
{
    public string azureStorageType { get; set; }
    public string deviceName { get; set; }
    public string containerName { get; set; }
    public string fileNameFilter { get; set; }
    public List<string> rootDirectories { get; set; }
    public List<string> volumeNames { get; set; }
    public string backupChoice { get; set; }
    public string isDirectoryMode { get; set; }
}

public class JobDefinitionProperties
{
    public string dataSourceId { get; set; }
    public string dataSinkId { get; set; }
    public string state { get; set; }
    public string userConfirmation { get; set; }
    public DataServiceProperties dataServiceInput { get; set; }
}

public class JobDefinitionInput
{
    public string name { get; set; }
    public string id { get; set; }
    public string type { get; set; }
    public JobDefinitionProperties properties { get; set; }
}
