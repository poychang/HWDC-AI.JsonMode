using Library;
using System.ComponentModel;

Console.WriteLine($"\x1b[33m===== HERE IS CLIENT SIDE =====\x1b[0m");
Console.WriteLine($"\x1b[33mPress ENTER to run...\x1b[0m");
Console.ReadLine();

var request = new RequestToAI
{
    Instruction = "請幫我解 2x+1=10 這個問題",
    ResponseModelName = nameof(QnA),
    ResponseModelSchema = typeof(QnA).ToJsonSchema(description: "回答問題的格式"),
};

var httpClient = HttpLogger.GetHttpClient(isShowLogging: true);
var response = httpClient.PostAsync(
    new Uri("http://localhost:5170/request-to-ai"),
    new StringContent(request.ToJson(), System.Text.Encoding.UTF8, "application/json")
).GetAwaiter().GetResult();

Console.WriteLine($"\x1b[34mRequest:\x1b[0m  {request.Instruction}");
Console.WriteLine($"\x1b[32mResponse:\x1b[0m \n{response.Content.ReadAsStringAsync().GetAwaiter().GetResult().ToIndented()}");

/*

Request:  請幫我解 2x+1=10 這個問題
Response:
{
  "Question": "請幫我解 2x+1=10 這個問題",
  "Answer": "解題步驟：\n1) 2x + 1 = 10\n2) 移項：2x = 10 - 1 = 9\n3) 兩邊同除以 2：x = 9/2 = 4.5\n檢查：2×4.5 + 1 = 9 + 1 = 10，正確。\n答案：x = 9/2（或 4.5）"
}

 */

public class QnA
{
    public required string Question { get; set; }
    public required string Answer { get; set; }
}

/*

Request:  請幫我解 2x+1=10 這個問題
Response:
{
  "Question": "解方程 2x+1=10",
  "Answer": "2x+1=10 → 2x=9 → x=9/2=4.5",
  "Remark": {
    "Level": "Easy",
    "Description": "一次線性方程，移項後除以係數即可。"
  }
}

 */

public class QnA2
{
    public required string Question { get; set; }
    public required string Answer { get; set; }
    public required ExtraInfo Remark { get; set; }

    public class ExtraInfo
    {
        [Description("標註困難度，分別為'Easy'、'Medium'、'Hard'")]
        public required string Level { get; set; }
        public required string Description { get; set; }
    }
}
