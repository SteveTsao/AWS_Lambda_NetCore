namespace UrlScreenshot;

/// <summary>
/// 佇列DTO
/// </summary>
public class QueueDTO
{
    /// <summary>
    /// 檔案名稱
    /// </summary>
    public string FileName { get; set; }

    /// <summary>
    /// 副檔名
    /// </summary>
    public string FileExtension { get; set; }

    /// <summary>
    /// 快照網頁
    /// </summary>
    public string PageUrl { get; set; }

    /// <summary>
    /// 快照通知網址
    /// </summary>
    public string NotifyPostUrl { get; set; }

    /// <summary>
    /// 快照圖片寬度
    /// </summary>
    public int Width { get; set; }

    /// <summary>
    /// 快照圖片高度
    /// </summary>
    public int Height { get; set; }

    /// <summary>
    /// 快照圖片品質
    /// </summary>
    public int? Quality { get; set; }

    /// <summary>
    /// 快照設備比例
    /// </summary>
    public double DeviceScaleFactor { get; set; }

    /// <summary>
    /// 是否手機
    /// </summary>
    public bool IsMobile { get; set; }

    /// <summary>
    /// 是否整頁
    /// </summary>
    public bool IsFullPage { get; set; }

    /// <summary>
    /// 重試次數
    /// </summary>
    public int RetryCount { get; set; }

    /// <summary>
    /// 重試上限次數
    /// </summary>
    public int RetryTimes { get; set; }

    /// <summary>
    /// 建構子
    /// </summary>
    public QueueDTO()
    {
        FileName = "";

        FileExtension = "";

        PageUrl = "";

        NotifyPostUrl = "";

        Width = 1280;

        Height = 720;

        Quality = 75;

        DeviceScaleFactor = 1.0;

        IsMobile = false;

        IsFullPage = true;

        RetryCount = 0;

        RetryTimes = 3;
    }
}
