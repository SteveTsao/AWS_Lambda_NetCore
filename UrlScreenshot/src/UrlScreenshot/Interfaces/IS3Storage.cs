namespace UrlScreenshot;

/// <summary>
/// S3貯存介面
/// </summary>
public interface IS3Storage
{
    /// <summary>
    /// 檔案上傳是否成功
    /// </summary>
    /// <param name="bucketName"> 存儲對象容器 </param>
    /// <param name="fileSource"> 來源檔案位置 </param>
    /// <param name="fileDestination"> 目標檔案位置 </param>
    /// <param name="contentType"> 上傳檔案類型 </param>
    /// <returns> 上傳是否成功 </returns>
    public Task<bool> IsUpload(string bucketName, string fileSource, string fileDestination, string contentType);
}