//---------------------------------------------------------------
//      Copyright (c) Microsoft Corporation. All rights reserved.
//----------------------------------------------------------------

using Newtonsoft.Json;
using System.Collections.Generic;

public class DataSinkInput
{
    public string name { get; set; }
    public string id { get; set; }
    public string type { get; set; }
    public DataSinkProperties properties { get; set; }
}
public class DataSinkProperties
{
    public string repositoryId { get; set; }
    public string dataStoreTypeId { get; set; }
    public List<DataSinkCustomerSecret> customerSecrets { get; set; }
    public DataSinkExtendedProperty extendedProperties { get; set; }
    public string state { get; set; }
}
public class DataSinkExtendedProperty
{
    public string storageAccountNameForQueue { get; set; }
}
public class DataSinkCustomerSecret
{
    public string keyIdentifier { get; set; }
    public string keyValue { get; set; }
    public string algorithm { get; set; }
}
