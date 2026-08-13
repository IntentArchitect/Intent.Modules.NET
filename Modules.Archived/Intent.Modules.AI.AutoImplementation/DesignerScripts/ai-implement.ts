async function execute(taskId: string): Promise<void> {
    
    const providerModelsResult = await getAiProviderModels();
    const settingName = "AI.AutoImplementation";

    // Open a dialog for the user to enter an AI prompt
    let promptResult = await dialogService.openForm({
        title: "AI: Auto-Implement Handler for " + element.getName(),
        icon: Icons.AiImplement,
        fields: [
            {
                id: "prompt",
                fieldType: "textarea",
                label: "Provide any additional context",
                placeholder: "Leave blank if you wish to provide no additional context.",
                hint: "NOTE: Additional context will be combined with the Intent Architect auto-generated prompt to guide the AI Agent."
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

    await launchHostedModuleTask(taskId, 
        [
            application.id, 
            element.id, 
            promptResult.prompt ?? "", 
            providerId, 
            modelId, 
            thinkingLevel
        ], 
        {
            taskName: "AI: Handler for " + element.getName()
        });
}