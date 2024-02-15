using Microsoft.Extensions.Configuration;

namespace UrlScreenshot;

/// <summary>
/// 組態設定管理類別
/// </summary>
public class ConfigManagerService : IConfigManager
{
    /// <summary>
    /// 版本號鍵值
    /// </summary>
    private readonly string[] keyVersion = new[] { "version" };

    /// <summary>
    /// 設定檔讀取物件
    /// </summary>
    private readonly IConfigurationRoot configurationRoot;

    /// <summary>
    /// 建構子
    /// </summary>
    /// <param name="_configurationRoot"> 設定檔讀取物件 </param>
    public ConfigManagerService(IConfigurationRoot _configurationRoot)
    {
        configurationRoot = _configurationRoot;
    }

    /// <summary>
    /// 取得版本號
    /// </summary>
    /// <returns> 版本號 </returns>
    public string GetVersion()
    {
        return GetStringValue(keyVersion);
    }

    /// <summary>
    /// 取得設定的字串型別內容，查無回空值
    /// </summary>
    /// <param name="keys"> 設定檔對應鍵值 </param>
    /// <returns> 設定內容 </returns>
    public string GetStringValue(string[] keys)
    {
        var key = string.Join(":", keys);
        var value = configurationRoot[key];

        if (value == null || value is not string)
        {
            throw new Exception($"設定檔查無字串內容 {key}");
        }

        return value;
    }
}