// Import namespaces
using Microsoft.SemanticKernel.ChatCompletion;
using Microsoft.SemanticKernel;
using Humanizer;

// Disable specific compiler warnings
#pragma warning disable SKEXP0001, SKEXP0010, SKEXP0070

// Define the endpoint URI for the chat model
var endpoint = new Uri("http://localhost:8080");

// Define the model to be used for chat completion
const string Model = "lmstudio-community/Meta-Llama-3.1-8B-Instruct-GGUF";

// Print a header to the console
Console.WriteLine("======== Llama 3.1 - Text Generation - Raw Streaming ========");

// Create a new instance of the Kernel with the specified model and endpoint
Kernel kernel = Kernel.CreateBuilder()
    .AddHuggingFaceChatCompletion(model: Model, endpoint: endpoint)
    .Build();

string? userInput;
do
{
    // Collect user input from the console
    Console.ForegroundColor = ConsoleColor.Cyan;
    Console.Write("User > ");
    userInput = Console.ReadLine();
    Console.ResetColor();

    // Get the chat completion service from the kernel
    var chatCompletion = kernel.GetRequiredService<IChatCompletionService>();

    // Initialize chat history with a system message
    var chatHistory = new ChatHistory("You are a helpful and intelligent assistant.")
    {
        new ChatMessageContent(AuthorRole.User, userInput) // Add user input to chat history
    };

    AuthorRole? role = null;

    // Stream chat message contents asynchronously
    await foreach (var chatMessageChunk in chatCompletion.GetStreamingChatMessageContentsAsync(chatHistory))
    {
        if (role is null)
        {
            // Set the role and print it to the console
            role = chatMessageChunk.Role;
            Console.ForegroundColor = ConsoleColor.Green; // Set output color to green
            Console.Write($"{role.ToString().Humanize()} > ");
            Console.ResetColor(); // Reset output color
        }
        // Print the chat message content to the console
        Console.ForegroundColor = ConsoleColor.Green; // Set output color to green
        Console.Write(chatMessageChunk.Content);
        Console.ResetColor(); // Reset output color
    }

    // Print a separator line to the console
    Console.WriteLine();
    Console.WriteLine("--------------------------------------------------------------");

} while (userInput is not null); // Continue until user input is null
