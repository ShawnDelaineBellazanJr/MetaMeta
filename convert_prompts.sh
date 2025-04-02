#!/bin/bash

# Script to convert and organize prompt files

# Create directory structure if it doesn't exist
mkdir -p src/MetaMeta.Orchestration/Prompts/Library/{AgentDesign,ContentCreation,ProductMarketing,TechnicalDocs}

# Function to convert .prompt to .prompty
convert_prompt() {
  local src=$1
  local dest=$2
  local name=$3
  local desc=$4
  
  if [ -f "$src" ]; then
    echo "Converting: $src -> $dest"
    echo "---" > "$dest"
    echo "name: $name" >> "$dest"
    echo "description: $desc" >> "$dest"
    echo "model:" >> "$dest"
    echo "  api: chat" >> "$dest"
    echo "---" >> "$dest"
    echo "" >> "$dest"
    cat "$src" >> "$dest"
  else
    echo "Source file not found: $src"
  fi
}

# Function to move existing .prompty files
move_prompty() {
  local src=$1
  local dest=$2
  
  if [ -f "$src" ]; then
    echo "Moving: $src -> $dest"
    cp "$src" "$dest"
  else
    echo "Source file not found: $src"
  fi
}

# Move existing .prompty files
AGENT_DESIGN_DIR="src/MetaMeta.Orchestration/Prompts/Library/AgentDesign"
CONTENT_CREATION_DIR="src/MetaMeta.Orchestration/Prompts/Library/ContentCreation"

# Agent design prompts
move_prompty "src/MetaMeta.Orchestration/Prompts/ChatCompletionAgent.prompty" "$AGENT_DESIGN_DIR/ChatCompletionAgent.prompty"
move_prompty "src/MetaMeta.Orchestration/Prompts/ClaimEvaluatorAgent.prompty" "$AGENT_DESIGN_DIR/ClaimEvaluatorAgent.prompty"
move_prompty "src/MetaMeta.Orchestration/Prompts/ExecutiveAgent.prompty" "$AGENT_DESIGN_DIR/ExecutiveAgent.prompty"
move_prompty "src/MetaMeta.Orchestration/Prompts/MemoryAgent.prompty" "$AGENT_DESIGN_DIR/MemoryAgent.prompty"
move_prompty "src/MetaMeta.Orchestration/Prompts/PlannerAgent.prompty" "$AGENT_DESIGN_DIR/PlannerAgent.prompty"
move_prompty "src/MetaMeta.Orchestration/Prompts/PromptRefinementAgent.prompty" "$AGENT_DESIGN_DIR/PromptRefinementAgent.prompty"
move_prompty "src/MetaMeta.Orchestration/Prompts/ReasoningAgent.prompty" "$AGENT_DESIGN_DIR/ReasoningAgent.prompty"
move_prompty "src/MetaMeta.Orchestration/Prompts/SummaryAgent.prompty" "$AGENT_DESIGN_DIR/SummaryAgent.prompty"
move_prompty "src/MetaMeta.Orchestration/Prompts/ToolAgent.prompty" "$AGENT_DESIGN_DIR/ToolAgent.prompty"

# Content creation prompts
move_prompty "src/MetaMeta.Orchestration/Prompts/ContentAgent.prompty" "$CONTENT_CREATION_DIR/ContentAgent.prompty"
move_prompty "src/MetaMeta.Orchestration/Prompts/ContentOutlineAgent.prompty" "$CONTENT_CREATION_DIR/ContentOutlineAgent.prompty"

# Convert remaining .prompt files to .prompty format
convert_prompt "src/MetaMeta.Orchestration/Prompts/ChatCompletion.prompt" "$AGENT_DESIGN_DIR/ChatCompletionBaseAgent.prompty" "ChatCompletionBaseAgent" "Base prompt for chat completion functionality"
convert_prompt "src/MetaMeta.Orchestration/Prompts/ExecutiveStrategy.prompt" "$AGENT_DESIGN_DIR/ExecutiveStrategyAgent.prompty" "ExecutiveStrategyAgent" "Determines the optimal approach for achieving a goal"
convert_prompt "src/MetaMeta.Orchestration/Prompts/MemoryRecall.prompt" "$AGENT_DESIGN_DIR/MemoryRecallAgent.prompty" "MemoryRecallAgent" "Helps retrieve and organize information from a knowledge base"
convert_prompt "src/MetaMeta.Orchestration/Prompts/ResultIntegration.prompt" "$AGENT_DESIGN_DIR/ResultIntegrationAgent.prompty" "ResultIntegrationAgent" "Combines outputs from multiple processing steps into a coherent result"
convert_prompt "src/MetaMeta.Orchestration/Prompts/TechnicalChatCompletion.prompt" "$AGENT_DESIGN_DIR/TechnicalChatCompletionAgent.prompty" "TechnicalChatCompletionAgent" "Technical chat assistant specialized for technical queries"
convert_prompt "src/MetaMeta.Orchestration/Prompts/GenericSummarization.prompt" "$CONTENT_CREATION_DIR/GenericSummarizationAgent.prompty" "GenericSummarizationAgent" "Condenses information while preserving key insights"

echo "Conversion complete" 