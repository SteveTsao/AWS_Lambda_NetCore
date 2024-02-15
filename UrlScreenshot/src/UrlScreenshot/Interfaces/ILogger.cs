namespace UrlScreenshot;

/// <summary>
/// 日誌紀錄介面
/// </summary>
public interface ILogger
{
    /// <summary>
    /// 一般日誌紀錄
    /// </summary>`
    /// <param name="message"> 日誌紀錄訊息 </param>
    public void LogInformation(string message);

    /// <summary>
    /// 錯誤日誌紀錄
    /// </summary>
    /// <param name="message"> 日誌紀錄訊息 </param>
    public Task LogError(string message);
}