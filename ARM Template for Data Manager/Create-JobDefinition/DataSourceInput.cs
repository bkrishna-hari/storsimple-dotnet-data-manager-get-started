//---------------------------------------------------------------
//      Copyright (c) Microsoft Corporation. All rights reserved.
//----------------------------------------------------------------

using Newtonsoft.Json;
using System.Collections.Generic;

public class DataSourceInput
{
    public string name { get; set; }
    public string id { get; set; }
    public string type { get; set; }
    public DataSourceProperties properties { get; set; }
}
public class DataSourceProperties
{
    public string repositoryId { get; set; }
    public string dataStoreTypeId { get; set; }
    public string state { get; set; }
    public DataSourceExtendedProperty extendedProperties { get; set; }
    public List<DataSourceCustomerSecret> customerSecrets { get; set; }
}
public class DataSourceExtendedProperty
{
    public string resourceId { get; set; }
}
public class DataSourceCustomerSecret
{
    public string keyIdentifier { get; set; }
    public string keyValue { get; set; }
    public string algorithm { get; set; }
}
