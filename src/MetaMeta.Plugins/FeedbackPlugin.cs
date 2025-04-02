using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.SemanticKernel;

namespace MetaMeta.Plugins;

/// <summary>
/// Plugin for handling feedback and improvement suggestions.
/// </summary>
public class FeedbackPlugin
{
    // Simple in-memory storage for feedback
    private static readonly List<FeedbackItem> _feedbackStore = new();

    /// <summary>
    /// Submits user feedback about an agent response.
    /// </summary>
    /// <param name="agentName">The name of the agent that provided the response.</param>
    /// <param name="rating">A numerical rating from 1-5.</param>
    /// <param name="comments">Optional comments elaborating on the feedback.</param>
    /// <param name="responseId">The identifier of the response being rated.</param>
    /// <returns>A confirmation message.</returns>
    [KernelFunction, Description("Submits user feedback about an agent response.")]
    public string SubmitFeedback(
        [Description("The name of the agent that provided the response")] string agentName,
        [Description("A numerical rating from 1-5")] int rating,
        [Description("Optional comments elaborating on the feedback")] string comments = "",
        [Description("The identifier of the response being rated")] string responseId = "")
    {
        // Validate rating
        if (rating < 1 || rating > 5)
        {
            return "Invalid rating. Please provide a rating between 1 and 5.";
        }

        var feedback = new FeedbackItem
        {
            Id = Guid.NewGuid().ToString(),
            Timestamp = DateTime.UtcNow,
            AgentName = agentName,
            Rating = rating,
            Comments = comments,
            ResponseId = responseId
        };

        _feedbackStore.Add(feedback);
        
        return $"Thank you for your feedback! Your feedback has been recorded with ID: {feedback.Id}";
    }

    /// <summary>
    /// Gets feedback statistics for a specific agent.
    /// </summary>
    /// <param name="agentName">The name of the agent to get statistics for.</param>
    /// <returns>A JSON summary of feedback statistics.</returns>
    [KernelFunction, Description("Gets feedback statistics for a specific agent.")]
    public string GetFeedbackStats(
        [Description("The name of the agent to get statistics for")] string agentName)
    {
        var agentFeedback = _feedbackStore.FindAll(f => f.AgentName.Equals(agentName, StringComparison.OrdinalIgnoreCase));
        
        if (agentFeedback.Count == 0)
        {
            return $"No feedback found for agent: {agentName}";
        }

        // Calculate statistics
        double averageRating = 0;
        int count1Star = 0;
        int count2Star = 0;
        int count3Star = 0;
        int count4Star = 0;
        int count5Star = 0;

        foreach (var feedback in agentFeedback)
        {
            averageRating += feedback.Rating;
            switch (feedback.Rating)
            {
                case 1: count1Star++; break;
                case 2: count2Star++; break;
                case 3: count3Star++; break;
                case 4: count4Star++; break;
                case 5: count5Star++; break;
            }
        }

        averageRating /= agentFeedback.Count;

        var recentComments = agentFeedback
            .Where(f => !string.IsNullOrEmpty(f.Comments))
            .OrderByDescending(f => f.Timestamp)
            .Take(3)
            .Select(f => f.Comments)
            .ToList();

        var stats = new
        {
            AgentName = agentName,
            TotalFeedback = agentFeedback.Count,
            AverageRating = Math.Round(averageRating, 2),
            Distribution = new
            {
                OneStar = count1Star,
                TwoStar = count2Star,
                ThreeStar = count3Star,
                FourStar = count4Star,
                FiveStar = count5Star
            },
            RecentComments = recentComments
        };

        return System.Text.Json.JsonSerializer.Serialize(stats, new System.Text.Json.JsonSerializerOptions
        {
            WriteIndented = true
        });
    }

    /// <summary>
    /// Suggests improvements based on feedback patterns.
    /// </summary>
    /// <param name="agentName">The name of the agent to suggest improvements for.</param>
    /// <returns>A list of suggested improvements.</returns>
    [KernelFunction, Description("Suggests improvements based on feedback patterns.")]
    public string SuggestImprovements(
        [Description("The name of the agent to suggest improvements for")] string agentName)
    {
        var agentFeedback = _feedbackStore.FindAll(f => f.AgentName.Equals(agentName, StringComparison.OrdinalIgnoreCase));
        
        if (agentFeedback.Count < 5)
        {
            return $"Insufficient feedback for {agentName} to suggest improvements. Need at least 5 feedback items.";
        }

        // In a real implementation, we would analyze feedback patterns using NLP
        // For demonstration, we'll return simulated suggestions
        var suggestions = new List<string>
        {
            "Consider adding more detailed explanations in responses",
            "Focus on providing more actionable steps",
            "Include relevant examples when addressing complex topics"
        };

        return System.Text.Json.JsonSerializer.Serialize(new { AgentName = agentName, Suggestions = suggestions }, 
            new System.Text.Json.JsonSerializerOptions { WriteIndented = true });
    }

    // Internal feedback data structure
    private class FeedbackItem
    {
        public string Id { get; set; } = string.Empty;
        public DateTime Timestamp { get; set; }
        public string AgentName { get; set; } = string.Empty;
        public int Rating { get; set; }
        public string Comments { get; set; } = string.Empty;
        public string ResponseId { get; set; } = string.Empty;
    }
} 