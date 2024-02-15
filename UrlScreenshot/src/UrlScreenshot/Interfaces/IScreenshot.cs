namespace UrlScreenshot;

/// <summary>
/// 快照介面
/// </summary>
public interface IScreenshot
{
    /// <summary>
    /// 啟動頁面
    /// </summary>
    /// <param name="screenshotDTO"> 快照DTO </param>
    /// <param name="browserPath"> 瀏覽器執行檔位置 </param>
    /// <param name="browserArguments"> 瀏覽器參數設定 </param>
    /// <returns> 啟動頁面結果 </returns>
    public Task<string> LaunchPage(ScreenshotDTO screenshotDTO, string browserPath, string[] browserArguments);
}