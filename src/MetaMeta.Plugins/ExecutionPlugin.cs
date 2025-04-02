using System;
using System.ComponentModel;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.SemanticKernel;

namespace MetaMeta.Plugins;

/// <summary>
/// Plugin for executing external commands and API calls.
/// </summary>
public class ExecutionPlugin
{
    private readonly HttpClient _httpClient;

    /// <summary>
    /// Initializes a new instance of the ExecutionPlugin class.
    /// </summary>
    public ExecutionPlugin()
    {
        _httpClient = new HttpClient();
    }

    /// <summary>
    /// Executes a simple HTTP GET request to the specified URL.
    /// </summary>
    /// <param name="url">The URL to send the GET request to.</param>
    /// <returns>The response content as a string.</returns>
    [KernelFunction, Description("Executes an HTTP GET request to the specified URL and returns the response.")]
    public async Task<string> HttpGetAsync(
        [Description("The URL to send the GET request to")] string url)
    {
        try
        {
            var response = await _httpClient.GetAsync(url);
            response.EnsureSuccessStatusCode();
            return await response.Content.ReadAsStringAsync();
        }
        catch (Exception ex)
        {
            return $"Error executing GET request: {ex.Message}";
        }
    }

    /// <summary>
    /// Executes an HTTP POST request to the specified URL with the provided JSON payload.
    /// </summary>
    /// <param name="url">The URL to send the POST request to.</param>
    /// <param name="jsonPayload">The JSON payload to include in the request body.</param>
    /// <returns>The response content as a string.</returns>
    [KernelFunction, Description("Executes an HTTP POST request with JSON payload to the specified URL and returns the response.")]
    public async Task<string> HttpPostJsonAsync(
        [Description("The URL to send the POST request to")] string url,
        [Description("The JSON payload to include in the request body")] string jsonPayload)
    {
        try
        {
            var content = new StringContent(jsonPayload, Encoding.UTF8, "application/json");
            var response = await _httpClient.PostAsync(url, content);
            response.EnsureSuccessStatusCode();
            return await response.Content.ReadAsStringAsync();
        }
        catch (Exception ex)
        {
            return $"Error executing POST request: {ex.Message}";
        }
    }

    /// <summary>
    /// Extracts specific information from a JSON string using a JSON path expression.
    /// </summary>
    /// <param name="json">The JSON string to extract from.</param>
    /// <param name="path">The JSON path expression (e.g., "data.items[0].name").</param>
    /// <returns>The extracted value as a string, or an error message if extraction fails.</returns>
    [KernelFunction, Description("Extracts specific information from a JSON string using a JSON path expression.")]
    public string ExtractJsonValue(
        [Description("The JSON string to extract from")] string json,
        [Description("The JSON path expression (e.g., \"data.items[0].name\")")] string path)
    {
        try
        {
            // Very simple JSON path implementation for demonstration purposes
            JsonDocument document = JsonDocument.Parse(json);
            JsonElement root = document.RootElement;

            string[] segments = path.Split('.');
            JsonElement current = root;

            foreach (string segment in segments)
            {
                // Handle array indexing (e.g., "items[0]")
                if (segment.Contains('[') && segment.Contains(']'))
                {
                    int bracketIndex = segment.IndexOf('[');
                    string propertyName = segment.Substring(0, bracketIndex);
                    string indexStr = segment.Substring(bracketIndex + 1, segment.Length - bracketIndex - 2);

                    if (int.TryParse(indexStr, out int index) && current.TryGetProperty(propertyName, out JsonElement arrayElement))
                    {
                        if (arrayElement.ValueKind == JsonValueKind.Array && index < arrayElement.GetArrayLength())
                        {
                            current = arrayElement[index];
                        }
                        else
                        {
                            return $"Invalid array or index: {segment}";
                        }
                    }
                    else
                    {
                        return $"Property not found or invalid index: {segment}";
                    }
                }
                else if (current.TryGetProperty(segment, out JsonElement property))
                {
                    current = property;
                }
                else
                {
                    return $"Property not found: {segment}";
                }
            }

            return current.ToString();
        }
        catch (Exception ex)
        {
            return $"Error extracting JSON value: {ex.Message}";
        }
    }
} 