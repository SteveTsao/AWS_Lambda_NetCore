using Amazon.Lambda.SQSEvents;

namespace UrlScreenshot;

/// <summary>
/// 執行快照入口介面
/// </summary>
public interface IController
{
    /// <summary>
    /// 處理快照隊列
    /// </summary>
    /// <param name="sqsEvent"> SQS觸發事件 </param>
    /// <returns> 處理結果 </returns>
    public Task<string> ProcessMessage(SQSEvent sqsEvent);
}