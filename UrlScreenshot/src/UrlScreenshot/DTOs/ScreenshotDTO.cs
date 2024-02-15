using PuppeteerSharp;

namespace UrlScreenshot;

/// <summary>
/// 快照DTO
/// </summary>
public class ScreenshotDTO
{
    /// <summary>
    /// 快照檔案類型
    /// </summary>
    public ScreenshotType Type { get; }

    /// <summary>
    /// 快照網頁
    /// </summary>
    public string PageUrl { get; }

    /// <summary>
    /// 快照圖片寬度
    /// </summary>
    public int Width { get; }

    /// <summary>
    /// 快照圖片高度
    /// </summary>
    public int Height { get; }

    /// <summary>
    /// 快照圖片品質
    /// </summary>
    public int? Quality { get; }

    /// <summary>
    /// 快照設備比例
    /// </summary>
    public double DeviceScaleFactor { get; }

    /// <summary>
    /// 是否手機
    /// </summary>
    public bool IsMobile { get; }

    /// <summary>
    /// 是否整頁
    /// </summary>
    public bool IsFullPage { get; }

    /// <summary>
    /// 建構子
    /// </summary>
    /// <param name="_type"> 快照檔案類型 </param>
    /// <param name="_pageUrl"> 快照網頁 </param>
    /// <param name="_width"> 快照圖片寬度 </param>
    /// <param name="_height"> 快照圖片高度 </param>
    /// <param name="_quality"> 快照圖片品質 </param>
    /// <param name="_deviceScaleFactor"> 快照設備比例 </param>
    /// <param name="_isMobile"> 是否手機 </param>
    /// <param name="_isFullPage"> 是否整頁 </param>
    public ScreenshotDTO(ScreenshotType _type, string _pageUrl, int _width = 1280, int _height = 720, int? _quality = 75, double _deviceScaleFactor = 1.0, bool _isMobile = false, bool _isFullPage = true)
    {
        Type = _type;

        PageUrl = _pageUrl;

        Width = _width;

        Height = _height;

        Quality = _quality;

        DeviceScaleFactor = _deviceScaleFactor;

        IsMobile = _isMobile;

        IsFullPage = _isFullPage;
    }
}
