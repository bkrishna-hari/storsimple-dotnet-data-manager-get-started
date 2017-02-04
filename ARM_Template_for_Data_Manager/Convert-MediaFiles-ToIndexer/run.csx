#load "QueueData.cs"

using System;
using Microsoft.WindowsAzure.MediaServices.Client;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Configuration;
using System.Threading;

private static CloudMediaContext cloudMediaContext = null;

public static void Run(QueueItem myQueueItem, TraceWriter log)
{
    string assetId = myQueueItem.TargetLocation.Substring(myQueueItem.TargetLocation.LastIndexOf("/") + 1).Replace("asset-", "nb:cid:UUID:");
    
    // Read Asset details & initiate Media Analytics
    ReadMediaAssetAndRunEncoding(assetId, log);
}

public static void ReadMediaAssetAndRunEncoding(string assetId, TraceWriter log)
{    
    string keyIdentifier = ConfigurationManager.AppSettings["MEDIA_ACCOUNT_NAME"];
    string keyValue = ConfigurationManager.AppSettings["MEDIA_ACCOUNT_KEY"];
    
    MediaServicesCredentials _cachedCredentials = new MediaServicesCredentials(keyIdentifier, keyValue);
    cloudMediaContext = new CloudMediaContext(_cachedCredentials);
    
    var assetInstance = from a in cloudMediaContext.Assets where a.Id == assetId select a;
    IAsset asset = assetInstance.FirstOrDefault();

    log.Info($"Asset {asset}");
    log.Info($"Asset Id: {asset.Id}");
    log.Info($"Asset name: {asset.Name}");
    log.Info($"Asset files: ");

    foreach (IAssetFile fileItem in asset.AssetFiles)
    {
        log.Info($"    Name: {fileItem.Name}");
        log.Info($"    Size: {fileItem.ContentFileSize}");
    }

    //submit job
    EncodeToAdaptiveBitrateMP4s(asset, AssetCreationOptions.None, log);

    log.Info($"Encoding launched - function done");
}

static public void EncodeToAdaptiveBitrateMP4s(IAsset asset, AssetCreationOptions options, TraceWriter log)
{

    // Prepare a job with a single task to transcode the specified asset
    // into a multi-bitrate asset MP4 720p preset.
    var encodingPreset = "H264 Multiple Bitrate 720p";

    IJob job = cloudMediaContext.Jobs.Create("Encoding " + asset.Name + " to " + encodingPreset);
    
    log.Info($"Job created");
    
    IMediaProcessor mesEncoder = (from p in cloudMediaContext.MediaProcessors where p.Name == "Media Encoder Standard" select p).ToList().OrderBy(mes => new Version(mes.Version)).LastOrDefault();
    
    log.Info($"MES encoder");
    
    ITask encodeTask = job.Tasks.AddNew("Encoding", mesEncoder, encodingPreset, TaskOptions.None);
    encodeTask.InputAssets.Add(asset);
    encodeTask.OutputAssets.AddNew(asset.Name + " as " + encodingPreset, AssetCreationOptions.None);

    log.Info($"Submit job encoder");
    job.Submit();

    //todo - change to queue based notifications
    job.GetExecutionProgressTask(CancellationToken.None).Wait();
    
    
}


public static string GetDynamicStreamingUrl(IAsset outputAsset)
{
    var daysForWhichStreamingUrlIsActive = 365;
    
    var accessPolicy = cloudMediaContext.AccessPolicies.Create(outputAsset.Name, TimeSpan.FromDays(daysForWhichStreamingUrlIsActive), AccessPermissions.Read | AccessPermissions.List);
    var assetFiles = outputAsset.AssetFiles.ToList();
    
    var assetFile = assetFiles.Where(f => f.Name.ToLower().EndsWith(".ism")).FirstOrDefault();
    if (assetFile != null)
    {
        var locator = cloudMediaContext.Locators.CreateLocator(LocatorType.OnDemandOrigin, outputAsset, accessPolicy);
        Uri smoothUri = new Uri(locator.Path + assetFile.Name + "/manifest");
        
         
        return smoothUri.ToString();
    }

    return string.Empty;
}

public static bool RunIndexingJob(IAsset asset, TraceWriter log, string configurationFile = "")
{
    // Declare a new job.
    var jobName = string.Format("Media Indexing of {0}", asset.Name);
    IJob job = cloudMediaContext.Jobs.Create(jobName);
    job.Priority = 10;

    // Get a reference to the Azure Media Indexer.
    string MediaProcessorName = "Azure Media Indexer";
    IMediaProcessor processor = GetLatestMediaProcessorByName(MediaProcessorName);

    // Read configuration from file if specified.
    string configuration = string.IsNullOrEmpty(configurationFile) ? "" : File.ReadAllText(configurationFile);

    // Create a task with the encoding details, using a string preset.
    ITask task = job.Tasks.AddNew(jobName, processor, configuration, TaskOptions.None);

    // Specify the input asset to be indexed.
    task.InputAssets.Add(asset);

    // Add an output asset to contain the results of the job.
    task.OutputAssets.AddNew(string.Format("{0} - Indexed", asset.Name), AssetCreationOptions.None);

    // Launch the job.
    job.Submit();
    log.Info($"Media Indexer submitted (Job name: {jobName})");

    return true;
}

public static IMediaProcessor GetLatestMediaProcessorByName(string mediaProcessorName)
{
    var processor = cloudMediaContext.MediaProcessors.Where(p => p.Name == mediaProcessorName).ToList().OrderBy(p => new Version(p.Version)).LastOrDefault();

    if (processor == null)
        throw new ArgumentException(string.Format("Unknown media processor", mediaProcessorName));

    return processor;
}