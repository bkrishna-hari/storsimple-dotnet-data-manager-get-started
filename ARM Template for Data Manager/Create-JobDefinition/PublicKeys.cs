//---------------------------------------------------------------
//      Copyright (c) Microsoft Corporation. All rights reserved.
//----------------------------------------------------------------

public class PublicKeys
{
    public DataServiceLevel1Key dataServiceLevel1Key { get; set; }
    public DataServiceLevel2Key dataServiceLevel2Key { get; set; }
}
public class DataServiceLevel1Key
{
    public string keyModulus { get; set; }
    public string keyExponent { get; set; }
    public int encryptionChunkSizeInBytes { get; set; }
}

public class DataServiceLevel2Key
{
    public string keyModulus { get; set; }
    public string keyExponent { get; set; }
    public int encryptionChunkSizeInBytes { get; set; }
}