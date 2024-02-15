using PuppeteerSharp;
using System.Net;

namespace UrlScreenshot;

/// <summary>
/// 快照類別
/// </summary>
public class ScreenshotService : IScreenshot
{
    /// <summary>
    /// 快照檔案類型工具物件
    /// </summary>
    private readonly ScreenTypeHelperService screenTypeHelper = new();

    /// <summary>
    /// 日誌紀錄物件
    /// </summary>
    private readonly ILogger logger;

    /// <summary>
    /// 建構子
    /// </summary>
    /// <param name="_logger"> 日誌紀錄物件 </param>
    public ScreenshotService(ILogger _logger)
    {
        logger = _logger;
    }

    /// <summary>
    /// 啟動頁面
    /// </summary>
    /// <param name="screenshotDTO"> 快照DTO </param>
    /// <param name="browserPath"> 瀏覽器執行檔位置 </param>
    /// <param name="browserArguments"> 瀏覽器參數設定 </param>
    /// <returns> 啟動頁面結果 </returns>
    public async Task<string> LaunchPage(ScreenshotDTO screenshotDTO, string browserPath, string[] browserArguments)
    {
        var options = new LaunchOptions()
        {
            ExecutablePath = browserPath,
            Args = browserArguments,
            Headless = true
        };

        var newFileName = screenTypeHelper.CombineFileName(screenshotDTO.Type, Guid.NewGuid().ToString());
        var screenshotPath = $"/tmp/{newFileName}";

        logger.LogInformation($"快照初始設定 : FilePath = {screenshotPath} , Width = {screenshotDTO.Width} , Height = {screenshotDTO.Height} , DeviceScaleFactor = {screenshotDTO.DeviceScaleFactor} , IsMobile = {screenshotDTO.IsMobile} , Quality = {screenshotDTO.Quality} , FullPage = {screenshotDTO.IsFullPage}");

        using (var browser = await Puppeteer.LaunchAsync(options))
        {
            var page = await browser.NewPageAsync();

            await page.SetViewportAsync(new ViewPortOptions
            {
                Width = screenshotDTO.Width, // 寬度
                Height = screenshotDTO.Height, // 高度
                DeviceScaleFactor = screenshotDTO.DeviceScaleFactor,
                IsMobile = screenshotDTO.IsMobile
            });

            var response = await page.GoToAsync(screenshotDTO.PageUrl);

            if (response.Status == HttpStatusCode.OK)
            {
                var screenshotOptions = new ScreenshotOptions
                {
                    Type = screenshotDTO.Type,
                    Quality = screenshotDTO.Quality,
                    FullPage = screenshotDTO.IsFullPage
                };

                await page.ScreenshotAsync(screenshotPath, screenshotOptions);

                logger.LogInformation($"快照執行成功 FilePath = {screenshotPath}");
            }
            else
            {
                screenshotPath = "";

                await logger.LogError($"快照執行失敗 HTTP Code Error {response.Status}");
            }
        }

        return screenshotPath;
    }
}