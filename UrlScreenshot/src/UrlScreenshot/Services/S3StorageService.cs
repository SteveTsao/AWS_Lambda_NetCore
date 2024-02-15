using Amazon.S3;
using Amazon.S3.Model;
using System.Net;

namespace UrlScreenshot;

/// <summary>
/// S3貯存類別
/// </summary>
public class S3StorageService : IS3Storage
{
    /// <summary>
    /// 日誌紀錄物件
    /// </summary>
    private readonly ILogger logger;

    /// <summary>
    /// S3貯存客戶端物件
    /// </summary>
    private readonly IAmazonS3 s3Client;

    /// <summary>
    /// 建構子
    /// </summary>
    /// <param name="logger"> 日誌紀錄物件 </param>
    /// <param name="s3Client"> S3貯存客戶端物件 </param>
    public S3StorageService(ILogger logger, IAmazonS3 s3Client)
    {
        this.logger = logger;
        this.s3Client = s3Client;
    }

    /// <summary>
    /// 檔案上傳是否成功
    /// </summary>
    /// <param name="bucketName"> 存儲對象容器 </param>
    /// <param name="fileSource"> 來源檔案位置 </param>
    /// <param name="fileDestination"> 目標檔案位置 </param>
    /// <param name="contentType"> 上傳檔案類型 </param>
    /// <returns> 上傳是否成功 </returns>
    public async Task<bool> IsUpload(string bucketName, string fileSource, string fileDestination, string contentType)
    {
        logger.LogInformation($"[上傳S3初始設定] BuckName = {bucketName} , source FilePath = {fileSource} , destination FilePath = {fileDestination} , content type = {contentType}");

        var request = new PutObjectRequest
        {
            BucketName = bucketName,
            Key = fileDestination,
            ContentType = contentType,
            FilePath = fileSource,
            CannedACL = S3CannedACL.PublicRead
        };

        var response = await s3Client.PutObjectAsync(request);

        if (response.HttpStatusCode == HttpStatusCode.OK)
        {
            logger.LogInformation($"[上傳S3成功] {fileSource} => {fileDestination} ; response http status code = {response.HttpStatusCode}");

            return true;
        }
        else
        {
            await logger.LogError($"[上傳S3失敗] {fileSource} => {fileDestination} ; response http status code = {response.HttpStatusCode}");

            return false;
        }
    }
}