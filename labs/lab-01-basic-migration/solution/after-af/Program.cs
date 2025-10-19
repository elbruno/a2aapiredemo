using Microsoft.Agents.AI;
using Microsoft.Extensions.AI;
using Microsoft.Extensions.Configuration;
using OpenAI;
using OpenAI.Chat;
using System.ClientModel;
using System.Linq;
using System.Reflection;

Console.OutputEncoding = System.Text.Encoding.UTF8;
Console.WriteLine("=== Agent Framework Weather Bot ===");
Console.WriteLine("Type 'exit' to quit.\n");

Console.WriteLine("ChatClientAgentOptions properties: " + string.Join(", ", typeof(ChatClientAgentOptions).GetProperties().Select(p => p.Name)));
Console.WriteLine("ChatClientAgent methods: " + string.Join(", ", typeof(ChatClientAgent).GetMethods().Select(m => m.Name).Distinct().OrderBy(n => n)));
foreach (var method in typeof(ChatClientAgent).GetMethods(BindingFlags.Public | BindingFlags.Instance))
{
    Console.WriteLine($"ChatClientAgent method: {method}");
}
Console.WriteLine("ChatClientAgent properties: " + string.Join(", ", typeof(ChatClientAgent).GetProperties().Select(p => p.Name)));
Console.WriteLine("ChatOptions properties: " + string.Join(", ", typeof(ChatOptions).GetProperties().Select(p => p.Name)));
var debugOptions = new ChatOptions();
Console.WriteLine($"ChatOptions.Tools type: {debugOptions.Tools?.GetType().FullName ?? "null"}");
Console.WriteLine("ChatOptions methods: " + string.Join(", ", typeof(ChatOptions).GetMethods().Select(m => m.Name).Distinct().OrderBy(n => n)));
var toolTypes = typeof(ChatOptions).Assembly.GetTypes()
    .Where(t => t.Name.Contains("Tool", StringComparison.OrdinalIgnoreCase))
    .Select(t => t.FullName)
    .OrderBy(n => n)
    .ToArray();
Console.WriteLine("Types containing 'Tool' in Microsoft.Extensions.AI assembly:\n - " + string.Join("\n - ", toolTypes));
var hostedFunctionToolType = typeof(ChatOptions).Assembly.GetType("Microsoft.Extensions.AI.HostedFunctionTool");
if (hostedFunctionToolType is not null)
{
    Console.WriteLine("HostedFunctionTool constructors: " + string.Join("; ", hostedFunctionToolType.GetConstructors(BindingFlags.Public | BindingFlags.Instance | BindingFlags.NonPublic).Select(c => c.ToString())));
    Console.WriteLine("HostedFunctionTool methods: " + string.Join("; ", hostedFunctionToolType.GetMethods(BindingFlags.Public | BindingFlags.Instance | BindingFlags.Static | BindingFlags.NonPublic).Select(m => m.ToString())));
}
Console.WriteLine("AITool factory methods: " + string.Join(", ", typeof(AITool).GetMethods().Where(m => m.IsStatic).Select(m => m.ToString())));
Console.WriteLine("AITool constructors: " + string.Join(", ", typeof(AITool).GetConstructors().Select(c => c.ToString())));
Console.WriteLine("AITool properties: " + string.Join(", ", typeof(AITool).GetProperties().Select(p => $"{p.PropertyType.Name} {p.Name}")));
Console.WriteLine("AITool non-public constructors: " + string.Join(", ", typeof(AITool).GetConstructors(BindingFlags.Instance | BindingFlags.NonPublic).Select(c => c.ToString())));
Console.WriteLine("AITool non-public static methods: " + string.Join(", ", typeof(AITool).GetMethods(BindingFlags.Static | BindingFlags.NonPublic).Select(m => m.ToString())));
var functionTypes = typeof(ChatOptions).Assembly.GetTypes()
    .Where(t => t.Name.Contains("Function", StringComparison.OrdinalIgnoreCase))
    .Select(t => t.FullName)
    .OrderBy(n => n)
    .ToArray();
Console.WriteLine("Types containing 'Function' in Microsoft.Extensions.AI assembly:\n - " + string.Join("\n - ", functionTypes));
Console.WriteLine("AIFunctionFactory methods: " + string.Join(", ", typeof(AIFunctionFactory).GetMethods(BindingFlags.Public | BindingFlags.Static).Select(m => m.ToString())));
Console.WriteLine($"ChatOptions.Tools property type: {typeof(ChatOptions).GetProperty("Tools")?.PropertyType.FullName ?? "unknown"}");
Console.WriteLine($"AIFunction base type: {typeof(AIFunction).BaseType?.FullName ?? "unknown"}");
var toolFactories = typeof(ChatOptions).Assembly.GetTypes()
    .SelectMany(t => t.GetMethods(BindingFlags.Public | BindingFlags.Static))
    .Where(m => m.ReturnType == typeof(AITool))
    .Select(m => $"{m.DeclaringType?.FullName}.{m.Name}")
    .OrderBy(n => n)
    .ToArray();
Console.WriteLine("Public static methods returning AITool:\n - " + string.Join("\n - ", toolFactories));
var agentsAssemblyToolTypes = typeof(ChatClientAgent).Assembly.GetTypes()
    .Where(t => t.Name.Contains("Tool", StringComparison.OrdinalIgnoreCase))
    .Select(t => t.FullName)
    .OrderBy(n => n)
    .ToArray();
Console.WriteLine("Types containing 'Tool' in Microsoft.Agents.AI assembly:\n - " + string.Join("\n - ", agentsAssemblyToolTypes));
var createAgentMethods = typeof(IChatClient).Assembly.GetTypes()
    .SelectMany(t => t.GetMethods(BindingFlags.Public | BindingFlags.Static | BindingFlags.Instance))
    .Where(m => m.Name.Contains("CreateAIAgent", StringComparison.OrdinalIgnoreCase))
    .Select(m => m.ToString())
    .OrderBy(n => n)
    .ToArray();
Console.WriteLine("Methods containing CreateAIAgent:");
foreach (var signature in createAgentMethods)
{
    Console.WriteLine(" - " + signature);
}
var aiAgentTypes = typeof(ChatClientAgent).Assembly.GetTypes()
    .Where(t => t.Name.Contains("AIAgent", StringComparison.OrdinalIgnoreCase))
    .Select(t => t.FullName)
    .OrderBy(n => n)
    .ToArray();
Console.WriteLine("Types containing 'AIAgent' in Microsoft.Agents.AI assembly:\n - " + string.Join("\n - ", aiAgentTypes));
var agentBuilderMethods = typeof(AIAgentBuilderExtensions).GetMethods(BindingFlags.Public | BindingFlags.Static)
    .Select(m => m.ToString())
    .OrderBy(n => n)
    .ToArray();
Console.WriteLine("AIAgentBuilderExtensions methods:\n - " + string.Join("\n - ", agentBuilderMethods));
var agentBuilderInstanceMethods = typeof(AIAgentBuilder).GetMethods(BindingFlags.Public | BindingFlags.Instance)
    .Select(m => m.ToString())
    .OrderBy(n => n)
    .ToArray();
Console.WriteLine("AIAgentBuilder instance methods:\n - " + string.Join("\n - ", agentBuilderInstanceMethods));
var agentExtensionsMethods = typeof(AIAgentExtensions).GetMethods(BindingFlags.Public | BindingFlags.Static)
    .Select(m => m.ToString())
    .OrderBy(n => n)
    .ToArray();
Console.WriteLine("AIAgentExtensions methods:\n - " + string.Join("\n - ", agentExtensionsMethods));
return;

var configuration = new ConfigurationBuilder()
    .SetBasePath(Directory.GetCurrentDirectory())
    .AddJsonFile("appsettings.json", optional: true)
    .AddUserSecrets<Program>()
    .Build();

var githubToken = configuration["GITHUB_TOKEN"]
    ?? throw new InvalidOperationException("Configure GITHUB_TOKEN using dotnet user-secrets.");
var deploymentName = configuration["OpenAI:ChatDeploymentName"] ?? "gpt-4o-mini";
var endpoint = configuration["OpenAI:Endpoint"] ?? "https://models.github.ai/inference";

IChatClient chatClient = new ChatClient(
        deploymentName,
        new ApiKeyCredential(githubToken),
        new OpenAIClientOptions { Endpoint = new Uri(endpoint) })
    .AsIChatClient();

// Local functions replace the old plugin class.
string GetWeather(string city) => $"Weather in {city}: Sunny, 72°F";
string ConvertToFahrenheit(double celsius)
{
    var fahrenheit = (celsius * 9 / 5) + 32;
    return $"{celsius:F1}°C = {fahrenheit:F1}°F";
}

var agent = new ChatClientAgent(
    chatClient,
    new ChatClientAgentOptions
    {
        Name = "WeatherBot",
        Instructions = "You are a helpful weather assistant. Use the provided tools to answer questions."
    });

Console.WriteLine("Agent option properties: " + string.Join(", ", typeof(ChatClientAgentOptions).GetProperties().Select(p => p.Name)));

while (true)
{
    Console.Write("You: ");
    var input = Console.ReadLine();

    if (string.IsNullOrWhiteSpace(input) || input.Equals("exit", StringComparison.OrdinalIgnoreCase))
    {
        Console.WriteLine("WeatherBot: Goodbye!");
        break;
    }

    try
    {
        AgentRunResponse response = await agent.RunAsync(input);
        Console.WriteLine($"WeatherBot: {response.Text}\n");
    }
    catch (Exception ex)
    {
        Console.WriteLine($"Error: {ex.Message}\n");
    }
}
