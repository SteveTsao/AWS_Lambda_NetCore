using Amazon.SQS;
using System.Net;

namespace UrlScreenshot;

/// <summary>
/// SQS託管類別
/// </summary>
public class SimpleQueueService : ISimpleQueue
{
    /// <summary>
    /// 日誌紀錄物件
    /// </summary>
    private readonly ILogger logger;

    /// <summary>
    /// SQS客戶端物件
    /// </summary>
    private readonly IAmazonSQS sqsClient;

    /// <summary>
    /// 建構子
    /// </summary>
    /// <param name="_logger"> 日誌紀錄物件 </param>
    /// <param name="_sqsClient"> SQS客戶端物件 </param>
    public SimpleQueueService(ILogger _logger, IAmazonSQS _sqsClient)
    {
        logger = _logger;
        sqsClient = _sqsClient;
    }

    /// <summary>
    /// 刪除佇列是否成功
    /// </summary>
    /// <param name="queueUrl"> 隊列URL </param>
    /// <param name="receiptHandle"> 佇列識別碼 </param>
    /// <param name="execeptionMessage"> 例外訊息 </param>
    /// <returns> 是否刪除成功 </returns>
    public async Task<bool> IsDeleteSuccess(string queueUrl, string receiptHandle, string execeptionMessage = "")
    {
        var message = "";

        try
        {
            var response = await sqsClient.DeleteMessageAsync(queueUrl, receiptHandle);

            if (response.HttpStatusCode == HttpStatusCode.OK)
            {
                return true;
            }
        }
        catch (Exception ex)
        {
            message = $"[例外錯誤] {ex.Message}";
        }

        await logger.LogError($"{message} 刪除 Queue 失敗 , queueUrl = {queueUrl} , receiptHandle = {receiptHandle} ; {execeptionMessage}");

        return false;
    }

    /// <summary>
    /// 發送佇列是否成功
    /// </summary>
    /// <param name="queueUrl"> 隊列URL </param>
    /// <param name="messageBody"> 佇列內容 </param>
    /// <param name="execeptionMessage"> 例外訊息 </param>
    /// <returns> 是否發送成功 </returns>
    public async Task<bool> IsSendSuccess(string queueUrl, string messageBody, string execeptionMessage = "")
    {
        var message = "";

        try
        {
            var response = await sqsClient.SendMessageAsync(queueUrl, messageBody);

            if (response.HttpStatusCode == HttpStatusCode.OK)
            {
                return true;
            }
        }
        catch (Exception ex)
        {
            message = $"[例外錯誤] {ex.Message}";
        }

        await logger.LogError($"{message} 新增 Queue 失敗 , queueUrl = {queueUrl} , messageBody = {messageBody} ; {execeptionMessage}");

        return false;
    }
}