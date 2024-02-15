namespace UrlScreenshot;

/// <summary>
/// SQS託管介面
/// </summary>
public interface ISimpleQueue
{
    /// <summary>
    /// 刪除佇列是否成功
    /// </summary>
    /// <param name="queueUrl"> 隊列URL </param>
    /// <param name="receiptHandle"> 佇列識別碼 </param>
    /// <param name="execeptionMessage"> 例外訊息 </param>
    /// <returns> 是否刪除成功 </returns>
    public Task<bool> IsDeleteSuccess(string queueUrl, string receiptHandle, string execeptionMessage = "");

    /// <summary>
    /// 發送佇列是否成功
    /// </summary>
    /// <param name="queueUrl"> 隊列URL </param>
    /// <param name="messageBody"> 佇列內容 </param>
    /// <param name="execeptionMessage"> 例外訊息 </param>
    /// <returns> 是否發送成功 </returns>
    public Task<bool> IsSendSuccess(string queueUrl, string messageBody, string execeptionMessage = "");
}