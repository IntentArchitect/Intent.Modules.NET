# Intent.AI.AutoImplementation

The **Intent.AI.AutoImplementation** module uses  AI to automatically implement the `Handler` method of a `Command` or `Query`, based on the full context provided by generated code and model metadata from Intent Architect.

> [!NOTE]
> To use this feature, ensure that the required [User Settings](https://docs.intentarchitect.com/articles/modules-common/intent-common-ai/intent-common-ai.html#user-settings) have been completed — including a valid API key for your selected AI provider.

## Implement with AI

To auto-implement a `Command` or `Query` handler, simply right-click on the element and select **Implement with AI**:

![Auto Implement Menu](images/auto-implement-menu.png)

### Influencing Factors

The quality and relevance of the generated implementation depend on several factors.

#### Intent Modeling

Before running **Implement with AI**, ensure the following:

- **Generated Code is up-to-date**: Run the Software Factory to apply all outstanding code changes.
- **Command/Query is mapped**: Ensure the `Command` or `Query` is associated with the appropriate `Entity` using a `Create Entity`, `Update Entity`, or `Query Entity` action (a dotted line should appear between the elements).

#### Adjusting the Prompt

While Intent Architect supplies a default prompt and relevant file context to the AI provider, you can optionally provide additional context to refine the result.

Enter extra prompt details here:

![Additional Prompt](images/additional-prompt.png)

> 💡 It’s recommended to try the default implementation first. If needed, rerun with added context to improve results.

> [!NOTE]
>
> AI responses are **not deterministic** — each execution may produce different results. Use the [additional context prompt](#adjusting-the-prompt) to guide the AI toward your desired implementation.

### Code Changes

Once the AI Agent completes the task, suggested code changes will be displayed for review:

![Recommended Changes](images/suggest-code-changes.png)

You can review and apply the changes through the familiar _Software Factory_ interface. If the results aren’t satisfactory, rerun the feature with revised prompt guidance.

### Execution Output

Full logs of the execution, including the AI prompt and any errors, are available in the **Execution** tab:

![Recommended Changes](images/execution-logs.png)
