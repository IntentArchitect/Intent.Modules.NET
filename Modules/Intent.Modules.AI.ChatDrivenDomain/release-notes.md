### Version 0.0.3

- Improvement: Moved the AI config from being on a context menu item (that stores in a config file on Hard drive) to an Intent Application Setting.
- Improvement: Added ability to use Azure Open AI and Ollama too.
- Improvement: Semantic Kernel logs will output to SF logger.
- Improvement: LLM Prompt has been improved for better results.
- Improvement: Configure the Max Tokens for better results.
- Fixed: When canceling a prompt it won't try to execute it.

### Version 0.0.2

- Improvement: Telling the model not to be lazy so that it can provide greater modeling support.
- Improvement: Increased number of tokens and told the AI to return results without whitespace to reduce token usage (possibly).

### Version 0.0.1

- New Feature: AI Assistant to model your Domain with chat prompts. By default, uses Open AI and requires an API Key. Register [here](https://platform.openai.com/api-keys) for an API Key. In the Domain designer, right click on a package and select `Execute Prompt`.