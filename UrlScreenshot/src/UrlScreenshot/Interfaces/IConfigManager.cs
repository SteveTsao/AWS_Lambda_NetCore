namespace UrlScreenshot;

/// <summary>
/// 組態設定管理介面
/// </summary>
public interface IConfigManager
{
    /// <summary>
    /// 取得版本號
    /// </summary>
    /// <returns> 版本號 </returns>
    public string GetVersion();

    /// <summary>
    /// 取得字串類型組態設定
    /// </summary>
    /// <param name="keys"> 組態鍵值 </param>
    /// <returns> 字串類型組態設定 </returns>
    public string GetStringValue(string[] keys);
}