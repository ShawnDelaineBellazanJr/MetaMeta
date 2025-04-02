using System;
using System.Text;
using System.Threading.Tasks;

namespace MetaMeta.Plugins;

/// <summary>
/// Plugin for text processing operations.
/// </summary>
/// <remarks>
/// Provides common text manipulation functionality such as case conversion, 
/// summarization, and entity extraction.
/// </remarks>
public class TextProcessingPlugin : IPlugin
{
    /// <summary>
    /// Gets the unique identifier for the plugin.
    /// </summary>
    public string Id => "metameta.plugins.textprocessing";

    /// <summary>
    /// Gets the display name of the plugin.
    /// </summary>
    public string Name => "Text Processing Plugin";

    /// <summary>
    /// Gets the description of the plugin's functionality.
    /// </summary>
    public string Description => "Provides text processing capabilities including case conversion and basic text analysis.";

    /// <summary>
    /// Gets the version of the plugin.
    /// </summary>
    public string Version => "1.0.0";

    /// <summary>
    /// Executes text processing operations based on the context.
    /// </summary>
    /// <param name="context">The context containing text data and operation parameters.</param>
    /// <returns>The result of the text processing operation.</returns>
    /// <exception cref="ArgumentException">Thrown when the input is not a string or operation is not supported.</exception>
    public async Task<PluginResult> ExecuteAsync(PluginContext context)
    {
        // Step 1: Validate input type
        if (context.Input is not string textInput)
        {
            return new PluginResult
            {
                Success = false,
                ErrorMessage = "Input must be a string for text processing operations."
            };
        }

        // Step 2: Determine which operation to perform based on parameters
        if (!context.Parameters.TryGetValue("operation", out var operationObj) || operationObj is not string operation)
        {
            return new PluginResult
            {
                Success = false,
                ErrorMessage = "Operation parameter is required and must be a string."
            };
        }

        // Step 3: Process the text based on the requested operation
        try
        {
            string result = operation.ToLowerInvariant() switch
            {
                "uppercase" => textInput.ToUpperInvariant(),
                "lowercase" => textInput.ToLowerInvariant(),
                "titlecase" => ConvertToTitleCase(textInput),
                "wordcount" => CountWords(textInput).ToString(),
                "reverse" => ReverseText(textInput),
                _ => throw new ArgumentException($"Unsupported operation: {operation}")
            };

            // Step 4: Simulate async processing (for demonstration purposes)
            await Task.Delay(100, context.CancellationToken);

            // Step 5: Return the processed result
            return new PluginResult
            {
                Success = true,
                Output = result
            };
        }
        catch (Exception ex)
        {
            return new PluginResult
            {
                Success = false,
                ErrorMessage = $"Text processing failed: {ex.Message}"
            };
        }
    }

    /// <summary>
    /// Converts text to title case (capitalizes the first letter of each word).
    /// </summary>
    /// <param name="text">The text to convert.</param>
    /// <returns>The text in title case.</returns>
    private static string ConvertToTitleCase(string text)
    {
        // Step 1: Handle empty input
        if (string.IsNullOrEmpty(text))
            return text;

        // Step 2: Split the text into words and process each word
        var words = text.Split(' ');
        var result = new StringBuilder();

        foreach (var word in words)
        {
            if (word.Length > 0)
            {
                // Step 3: Capitalize the first letter and keep the rest lowercase
                result.Append(char.ToUpperInvariant(word[0]));
                if (word.Length > 1)
                    result.Append(word.Substring(1).ToLowerInvariant());
                result.Append(' ');
            }
        }

        // Step 4: Return the processed result, trimming any trailing space
        return result.ToString().TrimEnd();
    }

    /// <summary>
    /// Counts the number of words in the text.
    /// </summary>
    /// <param name="text">The text to analyze.</param>
    /// <returns>The number of words in the text.</returns>
    private static int CountWords(string text)
    {
        // Split the text by whitespace and count non-empty segments
        return text.Split(new[] { ' ', '\t', '\n', '\r' }, StringSplitOptions.RemoveEmptyEntries).Length;
    }

    /// <summary>
    /// Reverses the text.
    /// </summary>
    /// <param name="text">The text to reverse.</param>
    /// <returns>The reversed text.</returns>
    private static string ReverseText(string text)
    {
        // Step 1: Convert string to char array
        char[] charArray = text.ToCharArray();
        
        // Step 2: Reverse the array
        Array.Reverse(charArray);
        
        // Step 3: Create a new string from the reversed array
        return new string(charArray);
    }
} 