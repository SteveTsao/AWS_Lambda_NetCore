
using Amazon.Lambda.SQSEvents;
using PuppeteerSharp;
using System.Text.Json;

namespace UrlScreenshot;

/// <summary>
/// 執行快照入口類別
/// </summary>
public class ControllerService : IController
{
    /// <summary>
    /// 隊列位置組態鍵值
    /// </summary>
    private readonly string[] configKeys = new[] { "AWS", "SQS", "QueueUrl" };

    /// <summary>
    /// 快照檔案類型工具物件
    /// </summary>
    private readonly ScreenTypeHelperService screenTypeHelper = new();

    /// <summary>
    /// 日誌紀錄物件
    /// </summary>
    private readonly ILogger logger;

    /// <summary>
    /// 組態管理物件
    /// </summary>
    private readonly IConfigManager configManager;

    /// <summary>
    /// 快照物件
    /// </summary>
    private readonly IScreenshot screenshot;

    /// <summary>
    /// 瀏覽器物件
    /// </summary>
    private readonly IBrowser browser;

    /// <summary>
    /// 通知物件
    /// </summary>
    private readonly INotify notify;

    /// <summary>
    /// SQS託管物件
    /// </summary>
    private readonly ISimpleQueue simpleQueue;

    /// <summary>
    /// S3貯存物件
    /// </summary>
    private readonly IS3Storage s3Storage;

    /// <summary>
    /// 建構子
    /// </summary>
    /// <param name="_logger"> 日誌紀錄物件 </param>
    /// <param name="_configManager"> 組態管理物件 </param>
    /// <param name="_screenshot"> 快照物件 </param>
    /// <param name="_browser"> 瀏覽器物件 </param>
    /// <param name="_notify"> 通知物件 </param>
    /// <param name="_simpleQueue"> SQS託管物件 </param>
    /// <param name="_s3Storage"> S3貯存物件 </param>
    public ControllerService(ILogger _logger, IConfigManager _configManager, IScreenshot _screenshot, IBrowser _browser, INotify _notify, ISimpleQueue _simpleQueue, IS3Storage _s3Storage)
    {
        logger = _logger;
        browser = _browser;
        configManager = _configManager;
        notify = _notify;
        screenshot = _screenshot;
        simpleQueue = _simpleQueue;
        s3Storage = _s3Storage;
    }

    /// <summary>
    /// 處理快照隊列
    /// </summary>
    /// <param name="sqsEvent"> SQS觸發事件 </param>
    /// <returns> 處理結果 </returns>
    public async Task<string> ProcessMessage(SQSEvent sqsEvent)
    {
        logger.LogInformation($"快照 Lambda 開始 , 版號 {configManager.GetVersion()}");

        var recordIndex = 0;
        var successCount = 0;
        var queueUrl = configManager.GetStringValue(configKeys);

        foreach (var record in sqsEvent.Records)
        {
            QueueDTO? queueDTO = null;

            var receiptHandle = record.ReceiptHandle;
            var queueBody = "";
            var queueInfo = $"第 {++recordIndex} 筆佇列 Queue.ReceiptHandle = ${receiptHandle} ; 快照";

            try
            {
                queueDTO = await CreateQueueDTO(record.Body, queueInfo);

                if (queueDTO != null)
                {
                    queueInfo = $"{queueInfo} {GetInfoQueueDTO(queueDTO)}";

                    var isDeleteSuccess = await simpleQueue.IsDeleteSuccess(queueUrl, receiptHandle, queueInfo);
                    var isHandleSuccess = await IsHandleQueue(queueDTO, queueInfo);

                    if (isHandleSuccess)
                    {
                        successCount += 1;

                        logger.LogInformation($"[快照成功] {queueInfo}");

                        continue;
                    }
                    else if (queueDTO.RetryCount >= queueDTO.RetryTimes)
                    {
                        // 超出重試次數，跳出迴圈不再重新 Send Queue

                        await logger.LogError($"[超出重試次數] {queueInfo}");

                        continue;
                    }
                    else if (!isDeleteSuccess)
                    {
                        // 刪除失敗，跳出迴圈不再重新 Send Queue

                        continue;
                    }

                    queueDTO.RetryCount++;

                    queueBody = JsonSerializer.Serialize(queueDTO);
                }
            }
            catch (Exception ex)
            {
                await logger.LogError($"[例外錯誤] {ex.Message} ; {queueInfo} Queue.messageBody = {record.Body}");
            }

            if (queueBody != "")
            {
                await logger.LogError($"[快照重送] {queueInfo} Queue.messageBody = {queueBody}");
                await simpleQueue.IsSendSuccess(queueUrl, queueBody, queueInfo);
            }
        }

        return $"共 {recordIndex} 筆快照 , 成功 {successCount} 筆";
    }

    /// <summary>
    /// 處理快照流程是否成功
    /// </summary>
    /// <param name="queueDTO"> 佇列DTO </param>
    /// <param name="message"> 佇列資訊 </param>
    /// <returns> 快照流程是否成功 </returns>
    private async Task<bool> IsHandleQueue(QueueDTO queueDTO, string message)
    {
        var screenType = screenTypeHelper.ConvertToScreenshopType(queueDTO.FileExtension);

        if (screenType == null)
        {
            await logger.LogError($"{message} 圖片類型不支援");

            return false;
        }

        var screenshotDTO = new ScreenshotDTO(
              (ScreenshotType)screenType,
                queueDTO.PageUrl,
                queueDTO.Width,
                queueDTO.Height,
                queueDTO.Quality,
                queueDTO.DeviceScaleFactor,
                queueDTO.IsMobile,
                queueDTO.IsFullPage
            );

        var screenshotFile = await screenshot.LaunchPage(screenshotDTO, browser.GetInitialBrowserPath(), browser.GetBrowserArguments());

        if (screenshotFile == "")
        {
            return false;
        }

        var fullFileName = screenTypeHelper.CombineFileName(screenshotDTO.Type, queueDTO.FileName);
        var bucketName = configManager.GetStringValue(new[] { "AWS", "S3", "BucketName" });
        var fileDestination = $"screenshot/{fullFileName}";
        var contentType = screenTypeHelper.GetContentType(screenshotDTO.Type);

        return await s3Storage.IsUpload(bucketName, screenshotFile, fileDestination, contentType) && (queueDTO.NotifyPostUrl == "" || await notify.IsSendSuccess(queueDTO.NotifyPostUrl, bucketName, fileDestination));
    }

    /// <summary>
    /// 建立佇列DTO
    /// </summary>
    /// <param name="queueDTO"> 佇列DTO </param>
    /// <param name="message"> 佇列資訊 </param>
    /// <returns> 佇列DTO </returns>
    private async Task<QueueDTO?> CreateQueueDTO(string body, string message)
    {
        var queueDTO = JsonSerializer.Deserialize<QueueDTO>(body);

        if (queueDTO == null)
        {
            await logger.LogError($"{message} 轉換 QueueDTO 失敗 ; Queue.messageBody = {body}");

            return null;
        }
        else if (queueDTO.PageUrl == "")
        {
            await logger.LogError($"{message} {GetInfoQueueDTO(queueDTO)} 未傳入快照網址 ; Queue.messageBody = {body}");

            return null;
        }
        else if (queueDTO.FileExtension == "")
        {
            await logger.LogError($"{message} {GetInfoQueueDTO(queueDTO)} 未傳入圖片類型 ; Queue.messageBody = {body}");

            return null;
        }

        return queueDTO;
    }

    /// <summary>
    /// 取得佇列DTO內容
    /// </summary>
    /// <param name="queueDTO"> 佇列DTO </param>
    /// <returns> 佇列內容 </returns>
    private string GetInfoQueueDTO(QueueDTO queueDTO)
    {
        return $"第 {queueDTO.RetryCount} 次執行 , 重試上限 {queueDTO.RetryTimes} 次 , 圖片位置 {queueDTO.FileName} , 圖片類型 {queueDTO.FileExtension} , 圖片寬度 {queueDTO.Width} , 圖片高度 {queueDTO.Height} , 圖片品質 {queueDTO.Quality} , 設備比例 {queueDTO.DeviceScaleFactor} , 快照網址 {queueDTO.PageUrl} ;";
    }
}