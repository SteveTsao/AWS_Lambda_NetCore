namespace UrlScreenshot;

/// <summary>
/// 通知介面
/// </summary>
public interface INotify
{
    /// <summary>
    /// 通知是否發送成功
    /// </summary>
    /// <param name="apiUrl"> 通知位置 </param>
    /// <param name="bucketName"> 存儲對象容器 </param>
    /// <param name="imagePath"> 存儲圖片位置 </param>
    /// <returns> 是否發送成功 </returns>
    public Task<bool> IsSendSuccess(string apiUrl, string bucketName, string imagePath);
}