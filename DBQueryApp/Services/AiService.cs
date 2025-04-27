using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using static DBQueryApp.Services.SqlService;

namespace DBQueryApp.Services;

public class AiService
{
    private readonly HttpClient _httpClient;
    private readonly List<TableDefinition> _tableTree;
    private readonly string _serverUrl;

    public AiService(List<TableDefinition> tableTree, string serverUrl = "http://localhost:8080/v1")
    {
        _httpClient = new HttpClient()
        {
            Timeout = Timeout.InfiniteTimeSpan
        };
        _tableTree = tableTree;
        _serverUrl = serverUrl;
    }

    public async Task<string> QueryAsync(string userQuestion)
    {
        var messages = BuildMessages(userQuestion);

        var payload = new ChatCompletionRequest()
        {
            Messages = messages,
            MaxTokens = -1,
            Stream = false,
            // Temperature = 0.2,
            // TopP = 0.9,
            // NPredict = 300
        };

        var json = JsonSerializer.Serialize(payload, JsonSerializerOptions.Default);
        var content = new StringContent(json, Encoding.UTF8, "application/json");

        var response = await _httpClient.PostAsync($"{_serverUrl}/chat/completions", content);
        response.EnsureSuccessStatusCode();

        var responseString = await response.Content.ReadAsStringAsync();
        var result = JsonSerializer.Deserialize<ChatCompletionResponse>(responseString);
        var message = result?.Choices?.FirstOrDefault()?.Message.Content.Trim() ?? string.Empty;
        var query = ParseSql(message);
        Console.WriteLine("返却メッセージ: {0}", message);
        Console.WriteLine("抽出SQL: {0}", query);
        return query;
    }

    private string ParseSql(string s)
    {
        if (s.ToUpperInvariant().StartsWith("SELECT"))
        {
            return s;
        }
        var match = Regex.Match(s, "```sql([^`]+)```");
        return match?.Groups?[1].Value ?? string.Empty;
    }

    private List<Message> BuildMessages(string userInput)
    {
        var messages = new List<Message>
        {
            // 1. システムメッセージ：振る舞い設定
            new Message
            {
                Role = "system",
                Content = "あなたは優秀なSQLエキスパートです。以下のデータベース構造に基づき、正確なSQLクエリだけを作成してください。SQL以外の出力は禁止です。"
            },

            // 2. システムメッセージ：データベース構造
            new Message
            {
                Role = "system",
                Content = BuildDatabaseSchemaContext()
            },

            // 3. ユーザーメッセージ
            new Message
            {
                Role = "user",
                Content = userInput
            }
        };

        return messages;
    }

    private string BuildDatabaseSchemaContext()
    {
        var sb = new StringBuilder();
        sb.AppendLine("データベース構造:");

        foreach (var table in _tableTree)
        {
            sb.Append("\n").Append($"- {table.Name} (");

            if (table.Columns.Count > 0)
            {
                sb.Append(string.Join(", ", table.Columns.Select(child => child.Name)));
            }

            sb.AppendLine(")");
        }
        sb.Append('\n').Append('\n').Append("テーブル名は省略しないで下さい。");

        return sb.ToString();
    }
}

public class ChatCompletionRequest
{
    [JsonPropertyName("messages")]
    public required List<Message> Messages { get; set; }
    [JsonPropertyName("max_tokens")]
    public required int MaxTokens { get; set; }
    [JsonPropertyName("stream")]
    public required bool Stream { get; set; }
    // [JsonPropertyName("temperature")]
    // public required double Temperature { get; set; }
    // [JsonPropertyName("top_p")]
    // public required double TopP { get; set; }
    // [JsonPropertyName("n_predict")]
    // public required double NPredict { get; set; }
}

public class ChatCompletionResponse
{
    [JsonPropertyName("id")]
    public required string Id { get; set; }
    [JsonPropertyName("object")]
    public required string Object { get; set; }
    [JsonPropertyName("created")]
    public required long Created { get; set; }
    [JsonPropertyName("model")]
    public required string Model { get; set; }
    [JsonPropertyName("choices")]
    public required List<Choice> Choices { get; set; }
    [JsonPropertyName("usage")]
    public required Usage Usage { get; set; }
    [JsonPropertyName("system_fingerprint")]
    public required string SystemFingerprint { get; set; }
}

public class Message
{
    [JsonPropertyName("role")]
    public required string Role { get; set; }
    [JsonPropertyName("content")]
    public required string Content { get; set; }
}

public class Choice
{
    [JsonPropertyName("index")]
    public required int Index { get; set; }
    [JsonPropertyName("finish_reason")]
    public required string FinishReason { get; set; }
    [JsonPropertyName("message")]
    public required Message Message { get; set; }
}

public class Usage
{
    [JsonPropertyName("prompt_tokens")]
    public required int PromptTokens { get; set; }
    [JsonPropertyName("completion_tokens")]
    public required int CompletionTokens { get; set; }
    [JsonPropertyName("total_tokens")]
    public required int TotalTokens { get; set; }
}
