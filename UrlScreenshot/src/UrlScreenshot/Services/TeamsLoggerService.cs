using Amazon.Lambda.Core;
using System.Text;
using System.Text.Json;

namespace UrlScreenshot;

/// <summary>
/// 微軟Teams日誌紀錄類別
/// </summary>
public class TeamsLoggerService : ILogger
{
    /// <summary>
    /// 提示API組態鍵值
    /// </summary>
    private readonly string[] configKeys = new[] { "API", "TeamsWebhookUrl" };

    /// <summary>
    /// 傳送類型
    /// </summary>
    private readonly string contentType = "application/json";

    /// <summary>
    /// 時間格式
    /// </summary>
    private readonly string timeFormat = "yyyy-MM-dd HH:mm:ss";

    /// <summary>
    /// 台北時區
    /// </summary>
    private readonly string timeZoneId = "Asia/Taipei";

    /// <summary>
    /// 日誌紀錄物件
    /// </summary>
    private readonly ILambdaLogger logger;

    /// <summary>
    /// 組態管理物件
    /// </summary>
    private readonly IConfigManager configManager;

    /// <summary>
    /// HTTP客戶端物件
    /// </summary>
    private readonly HttpClient httpClient;

    /// <summary>
    /// 建構子
    /// </summary>
    /// <param name="_logger"> 日誌紀錄物件 </param>
    /// <param name="_configManager"> 組態管理物件 </param>
    /// <param name="_httpClient"> HTTP客戶端物件 </param>
    public TeamsLoggerService(ILambdaLogger _logger, IConfigManager _configManager, HttpClient _httpClient)
    {
        logger = _logger;
        configManager = _configManager;
        httpClient = _httpClient;
    }

    /// <summary>
    /// 一般日誌紀錄
    /// </summary>
    /// <param name="message"> 日誌紀錄訊息 </param>
    public void LogInformation(string message)
    {
        logger.LogInformation(message);
    }

    /// <summary>
    /// 錯誤日誌紀錄
    /// </summary>
    /// <param name="message"> 日誌紀錄訊息 </param>
    public async Task LogError(string message)
    {
        logger.LogError(message);

        await SendMessage(message);
    }

    /// <summary>
    /// 送出日誌紀錄
    /// </summary>
    /// <param name="message"> 日誌紀錄訊息 </param>
    private async Task SendMessage(string message)
    {
        var logs = $"HEADER {contentType}";
        var isSuccess = false;

        try
        {
            var apiUrl = configManager.GetStringValue(configKeys);

            logs = $"{logs} ; API = {apiUrl}";

            var jsonData = JsonSerializer.Serialize(new { text = $"{getTimestamp()} {message}" });

            logs = $"{logs} ; POST JSON = {jsonData}";

            var response = await httpClient.PostAsync(apiUrl, new StringContent(jsonData, Encoding.UTF8, contentType));
            var result = await response.Content.ReadAsStringAsync();

            isSuccess = response.IsSuccessStatusCode;

            logs = $"{logs} ; HTTP STATUS CODE IS SCCUESS = {isSuccess} ; RESPONSE = {result}";
        }
        catch (Exception ex)
        {
            logs = $"{logs} ; EXCEPTION = {ex.Message}";
        }

        if (isSuccess)
        {
            logger.LogInformation($"呼叫 TeamsWebhookUrl 成功 ; {logs}");
        }
        else
        {
            logger.LogError($"呼叫 TeamsWebhookUrl 發生錯誤 ; {logs}");
        }
    }

    /// <summary>
    /// 取得現在日期時間
    /// </summary>
    /// <returns> 現在日期時間 </returns>
    private string getTimestamp()
    {
        return TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, TimeZoneInfo.FindSystemTimeZoneById(timeZoneId)).ToString(timeFormat);
    }
}