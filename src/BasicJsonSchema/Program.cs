using Microsoft.Extensions.Configuration;
using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.ChatCompletion;
using Microsoft.SemanticKernel.Connectors.AzureOpenAI;
using Server;

var secrets = new ConfigurationBuilder()
    .AddUserSecrets<Program>(optional: false, reloadOnChange: false)
    .Build()
    .Get<Secrets>() ?? throw new InvalidOperationException("無法載入 Secrets 設定，請確認 user secrets 或環境變數是否已設定。");

Kernel kernel = Kernel.CreateBuilder()
    .AddAzureOpenAIChatCompletion(
        deploymentName: secrets.AzureOpenAI.DeploymentName,
        endpoint: secrets.AzureOpenAI.Endpoint,
        apiKey: secrets.AzureOpenAI.APIKey
    )
    .Build();

var chatCompletionService = kernel.GetRequiredService<IChatCompletionService>();
var executionSettings = new AzureOpenAIPromptExecutionSettings()
{
    ResponseFormat = typeof(QnA),
};
var request = "請幫我解 2x+1=10 這個問題";
var response = await chatCompletionService.GetChatMessageContentAsync(request, executionSettings);

Console.WriteLine($"\x1b[34mRequest:\x1b[0m  {request}");
Console.WriteLine($"\x1b[32mResponse:\x1b[0m {response}");

/* Console Output

Request:  請幫我解 2x+1=10 這個問題
Response: {
    "Question": "請幫我解 2x+1=10 這個問題",
    "Answer": "方程式：2x+1=10\n步驟：\n1) 兩邊同時減去 1：2x = 9\n2) 兩邊同時除以 2：x = 9/2 = 4.5\n因此解為 x = 4.5。"
}

 */

public class QnA
{
    public required string Question { get; set; }
    public required string Answer { get; set; }
}