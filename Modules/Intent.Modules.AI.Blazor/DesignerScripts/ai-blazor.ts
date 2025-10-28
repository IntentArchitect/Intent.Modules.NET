async function execute(): Promise<void> {
    const providerModelsResult = await getAiProviderModels();
    const settingName = "AI.Blazor";

    let promptTemplatesString = await executeModuleTask(
        "Intent.Modules.AI.Blazor.GetPromptTemplates",
        application.id,
        element.getParent().getName() + "/" + element.getName());

    let promptTemplates = JSON.parse(promptTemplatesString) as any[];
    let defaultPromptTemplate = promptTemplates.find(t => t.recommenedDefault);

    // Open a dialog for the user to enter an AI prompt
    let promptResult = await dialogService.openForm({
        title: "AI: Implement " + element.getName(),
        icon: Icons.AiBlazor,
        fields: [
            {
                id: "prompt",
                fieldType: "textarea",
                label: "Provide any additional context",
                placeholder: "Leave blank if you wish to provide no additional context.",
                hint: "This additional context will be combined with the pre-engineered prompt to guide the AI Agent.",
                value: defaultPromptTemplate?.defaultUserPrompt
            },
            {
                id: "templateId",
                fieldType: "select",
                label: "Prompt Template",
                placeholder: "Select Template",
                selectOptions: promptTemplates,
                hint: "Select a Prompt Template to guide the the LLM.",
                value: defaultPromptTemplate?.id
            },
            {
                id: "exampleComponentIds",
                fieldType: "multi-select",
                label: "Example Components",
                placeholder: "Select components",
                selectOptions: lookupTypesOf("Component").filter(x => x.id != element.id).map(x => {
                    return {
                        id: x.id,
                        description: x.getName(),
                        icon: x.getIcon(),
                        additionalInfo: x.getParents().map(x => x.getName()).join("/")
                    }
                }),
                hint: "Provide the LLM with examples of existing components that it should base its implementation on."
            },
            ...await getAiModelSelectionFields(providerModelsResult, settingName)
        ],
        submitButtonText: "Execute",
        minWidth: "750px"
    });
    // Check if the user cancelled
    if (!promptResult) {
        return;
    }

    const { providerId, modelId, thinkingLevel: thinkingLevel } = await collectAndPersistAiSettingsFromPromptResult(
        promptResult, providerModelsResult, settingName);

    await launchHostedModuleTask("Intent.Modules.AI.Blazor.Generate",
        [
            application.id,
            element.id,
            promptResult.prompt ?? "",
            JSON.stringify(promptResult.exampleComponentIds ?? []) ?? "",
            providerId,
            modelId,
            thinkingLevel,
            promptResult.templateId
        ],
        {
            taskName: "AI: Blazor for " + element.getName()
        });
}