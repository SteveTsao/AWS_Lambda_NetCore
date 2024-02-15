namespace UrlScreenshot;

/// <summary>
/// Chrome瀏覽器類別
/// </summary>
public class ChromeBrowserService : IBrowser
{
    /// <summary>
    /// 容器暫存位置
    /// </summary>
    private static readonly string awsTmpPath = "/tmp";

    /// <summary>
    /// 瀏覽器執行檔位置
    /// </summary>
    private readonly string chromiumPath = $"{awsTmpPath}/chromium";

    /// <summary>
    /// 專案暫存檔案位置
    /// </summary>
    private readonly string sourceTmpPath = "./Vendor/tmp";

    /// <summary>
    /// 瀏覽器參數設定
    /// </summary>
    private readonly string[] browserArguments = new[] {
        "--disable-background-timer-throttling",
        "--disable-breakpad",
        "--disable-client-side-phishing-detection",
        "--disable-cloud-import",
        "--disable-default-apps",
        "--disable-dev-shm-usage",
        "--disable-extensions",
        "--disable-gesture-typing",
        "--disable-hang-monitor",
        "--disable-infobars",
        "--disable-notifications",
        "--disable-offer-store-unmasked-wallet-cards",
        "--disable-offer-upload-credit-cards",
        "--disable-popup-blocking",
        "--disable-print-preview",
        "--disable-prompt-on-repost",
        "--disable-setuid-sandbox",
        "--disable-speech-api",
        "--disable-sync",
        "--disable-tab-for-desktop-share",
        "--disable-translate",
        "--disable-voice-input",
        "--disable-wake-on-wifi",
        "--disk-cache-size=33554432",
        "--enable-async-dns",
        "--enable-simple-cache-backend",
        "--enable-tcp-fast-open",
        "--enable-webgl",
        "--hide-scrollbars",
        "--ignore-gpu-blacklist",
        "--media-cache-size=33554432",
        "--metrics-recording-only",
        "--mute-audio",
        "--no-default-browser-check",
        "--no-first-run",
        "--no-pings",
        "--no-sandbox", // 必設定，不然會噴錯
        "--no-zygote",
        "--password-store=basic",
        "--prerender-from-omnibox=disabled",
        "--use-gl=swiftshader",
        "--use-mock-keychain",
        "--single-process", // 必設定，不然會噴錯
    };

    /// <summary>
    /// 環境變數設定
    /// </summary>
    private readonly Dictionary<string, string> envirmentVariables = new()
    {
        { "HOME", "/var/task/Vendor/home" },
        { "FONTCONFIG_PATH", "/var/task/Vendor/home" },
        { "LD_LIBRARY_PATH", $"{awsTmpPath}/lib" }
    };

    /// <summary>
    /// 日誌紀錄物件
    /// </summary>
    private readonly ILogger logger;

    /// <summary>
    /// 建構子
    /// </summary>
    /// <param name="_logger"> 日誌紀錄物件 </param>
    public ChromeBrowserService(ILogger _logger)
    {
        logger = _logger;
    }

    /// <summary>
    /// 取得初始化瀏覽器執行檔位置
    /// </summary>
    /// <returns> 瀏覽器執行檔位置 </returns>
    public string GetInitialBrowserPath()
    {
        logger.LogInformation("initial browser");

        if (!Directory.Exists(chromiumPath))
        {
            logger.LogInformation(CopyDirectory(sourceTmpPath, awsTmpPath));
        }

        logger.LogInformation(SetEnvironmentVariables());

        return chromiumPath;
    }

    /// <summary>
    /// 取得瀏覽器參數設定
    /// </summary>
    /// <returns> 瀏覽器參數設定 </returns>
    public string[] GetBrowserArguments()
    {
        return browserArguments;
    }

    /// <summary>
    /// 設定環境變數
    /// </summary>
    /// <returns> 輸出設定訊息 </returns>
    private string SetEnvironmentVariables()
    {
        var message = "Set Envirment Variables \n";

        foreach (var envirmentVariable in envirmentVariables)
        {
            var envirmentValue = Environment.GetEnvironmentVariable(envirmentVariable.Key);

            if (string.IsNullOrEmpty(envirmentValue) || envirmentValue != envirmentVariable.Value)
            {
                message += $" , {envirmentVariable.Key} = {envirmentVariable.Value} \n";

                Environment.SetEnvironmentVariable(envirmentVariable.Key, envirmentVariable.Value);
            }
        }

        return message;
    }

    /// <summary>
    /// 複製目錄下的子目錄與檔案
    /// </summary>
    /// <param name="source"> 來源位置 </param>
    /// <param name="destination"> 目標位置 </param>
    /// <returns> 輸出複製訊息 </returns>
    private string CopyDirectory(string source, string destination)
    {
        var sourceDir = new DirectoryInfo(source);
        var destDir = new DirectoryInfo(destination);

        string message = $"directory copy {sourceDir.FullName} to {destDir.FullName} \n";

        if (!destDir.Exists)
        {
            destDir.Create();
        }

        foreach (FileInfo file in sourceDir.GetFiles())
        {
            var filePath = Path.Combine(destDir.FullName, file.Name);

            message += $"file copy {filePath} \n";

            file.CopyTo(filePath, true);
        }

        foreach (DirectoryInfo subDir in sourceDir.GetDirectories())
        {
            var subDirectoryPath = Path.Combine(destDir.FullName, subDir.Name);

            message += CopyDirectory(subDir.FullName, subDirectoryPath);
        }

        return message;
    }
}
