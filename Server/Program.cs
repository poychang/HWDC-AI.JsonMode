using Library;
using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.ChatCompletion;
using Microsoft.SemanticKernel.Connectors.AzureOpenAI;
using Server;

Console.WriteLine($"\x1b[33m===== HERE IS SERVER SIDE =====\x1b[0m\n\n");

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

var app = builder.Build();

// Configure the HTTP request pipeline.

app.UseHttpsRedirection();

app.MapPost("/request-to-ai", async (RequestToAI request) =>
{
    var secrets = app.Configuration.Get<Secrets>()
        ?? throw new InvalidOperationException("無法載入 Secrets 設定，請確認 user secrets 或環境變數是否已設定。");

    Kernel kernel = Kernel.CreateBuilder()
        .AddAzureOpenAIChatCompletion(
            deploymentName: secrets.AzureOpenAI.DeploymentName,
            endpoint: secrets.AzureOpenAI.Endpoint,
            apiKey: secrets.AzureOpenAI.APIKey,
            httpClient: HttpLogger.GetHttpClient(isShowLogging: true)
        )
        .Build();

    var chatCompletionService = kernel.GetRequiredService<IChatCompletionService>();

    var executionSettings = new AzureOpenAIPromptExecutionSettings()
    {
        ResponseFormat = OpenAI.Chat.ChatResponseFormat.CreateJsonSchemaFormat(
            request.ResponseModelName,
            BinaryData.FromString(request.ResponseModelSchema),
            jsonSchemaIsStrict: true),
    };
    var response = await chatCompletionService.GetChatMessageContentAsync(request.Instruction, executionSettings);

    Console.WriteLine($"\x1b[34mRequest:\x1b[0m  {request.Instruction}");
    Console.WriteLine($"\x1b[32mResponse:\x1b[0m {response}");

    return response.ToString();
});

app.Run();
