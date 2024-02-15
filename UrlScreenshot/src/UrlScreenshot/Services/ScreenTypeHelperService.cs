using PuppeteerSharp;

namespace UrlScreenshot;

/// <summary>
/// 快照檔案類型工具類別
/// </summary>
public class ScreenTypeHelperService
{
    /// <summary>
    /// 副檔名對應快照檔案類型列舉
    /// </summary>
    private readonly Dictionary<string, ScreenshotType> mappingTypes = new()
    {
        {"jpeg", ScreenshotType.Jpeg },
        {"jpg", ScreenshotType.Jpeg },
        {"png", ScreenshotType.Png },
        {"webp", ScreenshotType.Webp }
    };

    /// <summary>
    /// 組合完整檔名
    /// </summary>
    /// <param name="screentType"> 快照檔案類型列舉 </param>
    /// <param name="fileName"> 檔案名稱 </param>
    /// <returns> 完整檔名 </returns>
    public string CombineFileName(ScreenshotType screentType, string fileName)
    {
        var fileExtension = (new string[] { "jpg", "png", "webp" })[(int)screentType];

        return $"{fileName}.{fileExtension}";
    }

    /// <summary>
    /// 取得檔案傳輸類型
    /// </summary>
    /// <param name="screentType"> 快照檔案類型列舉 </param>
    /// <returns> 檔案傳輸類型 </returns>
    public string GetContentType(ScreenshotType screentType)
    {
        var contentType = (new string[] { "jpeg", "png", "webp" })[(int)screentType];

        return $"image/{contentType}";
    }

    /// <summary>
    /// 轉換快照檔案類型列舉
    /// </summary>
    /// <param name="fileExtension"> 副檔名 </param>
    /// <returns> 快照檔案類型列舉 </returns>
    public ScreenshotType? ConvertToScreenshopType(string fileExtension)
    {
        foreach (var item in mappingTypes)
        {
            if (item.Key == fileExtension.ToLower())
            {
                return item.Value;
            }
        }

        return null;
    }
}