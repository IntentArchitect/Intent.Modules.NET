### Version 0.0.5

- Improvement: Upgraded to use the `Intent.Common.AI` module which moves API Key settings into the User's settings.

### Version 0.0.4

- Improvement: The LLM will no longer attempt to generate the domain as part of its output but instead rely on tools to carve out a domain from an in-memory model whereby then the model gets sent to the designer to be updated from there. It does run quite long and is more expensive, but it does seem to handle larger and more complex domains a lot better.

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