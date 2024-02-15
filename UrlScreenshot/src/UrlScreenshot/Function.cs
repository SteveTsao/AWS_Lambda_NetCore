using Amazon.Lambda.Core;
using Amazon.Lambda.SQSEvents;
using Amazon.S3;
using Amazon.SQS;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

// Assembly attribute to enable the Lambda function's JSON input to be converted into a .NET class.
[assembly: LambdaSerializer(typeof(Amazon.Lambda.Serialization.SystemTextJson.DefaultLambdaJsonSerializer))]

namespace UrlScreenshot;

public class Function
{
    /// <summary>
    /// 相依注入服務集合物件
    /// </summary>
    private readonly ServiceCollection serviceProvider = new();

    /// <summary>
    /// 設定檔物件
    /// </summary>
    private readonly ConfigurationBuilder configBuilder = new();

    /// <summary>
    /// 版號環境變數鍵值
    /// </summary>
    private readonly string envirmentVariable = "ENV";

    /// <summary>
    /// This method is called for every Lambda invocation. This method takes in an SQS event object and can be used 
    /// to respond to SQS messages.
    /// </summary>
    /// <param name="evnt"> SQS觸發事件 </param>
    /// <param name="context"> Lambda 物件 </param>
    /// <returns></returns>
    public async Task FunctionHandler(SQSEvent evnt, ILambdaContext context)
    {
        try
        {
            var errorLoadConfig = GetErrorAfterLoadConfigFile();

            if (errorLoadConfig != "")
            {
                OutputFailResult(context.Logger, $"[載入設定檔] {errorLoadConfig}");

                return;
            }

            var controllerResult = "取得依賴注入 ControllerService 發生錯誤";
            var isSuccess = false;

            using (var builderProvider = BuildServiceProvider(context.Logger))
            {
                var controller = builderProvider.GetService<IController>();

                if (controller != null)
                {
                    controllerResult = await controller.ProcessMessage(evnt);

                    isSuccess = true;
                }
            }

            OutputResult(context.Logger, controllerResult, isSuccess);
        }
        catch (Exception ex)
        {
            OutputFailResult(context.Logger, $"[例外錯誤] {ex.Message}");
        }
    }

    /// <summary>
    /// 取得載入設定檔的錯誤訊息
    /// </summary>
    /// <returns> 載入設定檔的錯誤訊息 </returns>
    private string GetErrorAfterLoadConfigFile()
    {
        var env = Environment.GetEnvironmentVariable(envirmentVariable);

        if (env == null)
        {
            return $"查無環境變數 {envirmentVariable} 設定";
        }

        var basePath = Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location) ?? "";
        var configFiles = new string[] { $"appsettings.{env}.json" };

        configBuilder.SetBasePath(basePath);

        foreach (string configFile in configFiles)
        {
            var srcConfigFile = Path.Combine(basePath, configFile);

            if (!File.Exists(srcConfigFile))
            {
                return $"查無設定檔案 {srcConfigFile}";
            }

            configBuilder.AddJsonFile(srcConfigFile);
        }

        return "";
    }

    /// <summary>
    /// 建置相依注入服務集合物件
    /// </summary>
    /// <param name="logger"> Lambda Log 物件 </param>
    /// <returns> 相依注入服務集合建置物件 </returns>
    private ServiceProvider BuildServiceProvider(ILambdaLogger logger)
    {
        serviceProvider.AddScoped<IConfigurationRoot>(serviceProvider => configBuilder.Build());
        serviceProvider.AddScoped<ILambdaLogger>(serviceProvider => logger);
        serviceProvider.AddTransient<HttpClient>();
        serviceProvider.AddTransient<IBrowser, ChromeBrowserService>();
        serviceProvider.AddTransient<IConfigManager, ConfigManagerService>();
        serviceProvider.AddTransient<IController, ControllerService>();
        serviceProvider.AddTransient<ILogger, TeamsLoggerService>();
        serviceProvider.AddTransient<IS3Storage, S3StorageService>();
        serviceProvider.AddTransient<IScreenshot, ScreenshotService>();
        serviceProvider.AddTransient<ISimpleQueue, SimpleQueueService>();
        serviceProvider.AddTransient<INotify, NotifyService>();
        serviceProvider.AddTransient<IAmazonS3, AmazonS3Client>();
        serviceProvider.AddTransient<IAmazonSQS, AmazonSQSClient>();

        return serviceProvider.BuildServiceProvider();
    }

    /// <summary>
    /// 輸出訊息
    /// </summary>
    /// <param name="logger"> Log 物件 </param>
    /// <param name="message"> 輸出訊息 </param>
    /// <param name="isSuccessExecuted"> 是否執行成功 </param>
    private void OutputResult(ILambdaLogger logger, string message, bool isSuccessExecuted)
    {
        if (isSuccessExecuted)
        {
            logger.LogInformation($"[執行結果] {message}");

            return;
        }

        OutputFailResult(logger, message);
    }

    /// <summary>
    /// 輸出錯誤訊息
    /// </summary>
    /// <param name="logger"> Log 物件 </param>
    /// <param name="message"> 錯誤訊息 </param>
    private void OutputFailResult(ILambdaLogger logger, string message)
    {
        logger.LogError($"[執行失敗] {message}");
    }
}