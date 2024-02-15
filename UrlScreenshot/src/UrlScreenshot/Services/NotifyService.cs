namespace UrlScreenshot;

/// <summary>
/// 通知物件
/// </summary>
public class NotifyService : INotify
{
    /// <summary>
    /// 日誌紀錄物件
    /// </summary>
    private readonly ILogger logger;

    /// <summary>
    /// HTTP客戶端物件
    /// </summary>
    private readonly HttpClient httpClient;

    /// <summary>
    /// 建構子
    /// </summary>
    /// <param name="_logger"> 日誌紀錄物件 </param>
    /// <param name="_httpClient"> HTTP客戶端物件 </param>
    public NotifyService(ILogger _logger, HttpClient _httpClient)
    {
        logger = _logger;
        httpClient = _httpClient;
    }

    /// <summary>
    /// 通知是否發送成功
    /// </summary>
    /// <param name="apiUrl"> 通知位置 </param>
    /// <param name="bucketName"> 存儲對象容器 </param>
    /// <param name="imagePath"> 存儲圖片位置 </param>
    /// <returns> 是否發送成功 </returns>
    public async Task<bool> IsSendSuccess(string apiUrl, string bucketName, string imagePath)
    {
        var isSuccess = false;
        var message = $"API = ${apiUrl} ; POST FORM : ";

        try
        {
            var formData = new Dictionary<string, string>() {
                {"BucketName", bucketName},
                {"ImagePath", imagePath}
            };

            var formContent = new FormUrlEncodedContent(formData);

            message += await formContent.ReadAsStringAsync();

            var response = await httpClient.PostAsync(apiUrl, formContent);
            var result = await response.Content.ReadAsStringAsync();

            message = $"{message} ; HTTP STATUS CODE IS SCCUESS = {response.IsSuccessStatusCode} ; RESPONSE = {result}";

            if (response.IsSuccessStatusCode)
            {
                isSuccess = !string.IsNullOrEmpty(result);
            }
        }
        catch (Exception ex)
        {
            message = $"{message} ; EXCEPTION = {ex.Message}";
        }

        if (isSuccess)
        {
            logger.LogInformation($"發送快照通知成功 ; {message}");
        }
        else
        {
            await logger.LogError($"發送快照通知失敗 ; {message}");
        }

        return isSuccess;
    }
}