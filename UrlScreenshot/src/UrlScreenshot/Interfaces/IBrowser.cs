namespace UrlScreenshot;

/// <summary>
/// 瀏覽器介面
/// </summary>
public interface IBrowser
{
    /// <summary>
    /// 取得初始化瀏覽器執行檔位置
    /// </summary>
    /// <returns> 瀏覽器執行檔位置 </returns>
    public string GetInitialBrowserPath();

    /// <summary>
    /// 取得瀏覽器參數設定
    /// </summary>
    /// <returns> 瀏覽器參數設定 </returns>
    public string[] GetBrowserArguments();
}