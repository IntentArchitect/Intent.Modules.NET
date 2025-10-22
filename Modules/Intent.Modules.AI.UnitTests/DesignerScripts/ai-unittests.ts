async function execute(taskId: string) {
    const providerModelsResult = await getAiProviderModels();
    const settingName = "AI.UnitTests";

    // Open a dialog for the user to enter an AI prompt
    let promptResult = await dialogService.openForm({
        title: "AI: Generate Unit Test for " + element.getName(),
        icon: Icons.AiUnitTests,
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

    let result = await launchHostedModuleTask(taskId, 
        [
            application.id, 
            element.id, 
            promptResult.prompt ?? "",
            providerId, 
            modelId, 
            thinkingLevel
        ], 
        {
            taskName: "AI: Unit tests for " + element.getName()
        });

    const unitTestStereotype = "4965bed2-6320-49d1-beba-0fbc6fd4dfe6";
    if(!element.hasStereotype(unitTestStereotype))
    {
        element.addStereotype(unitTestStereotype); // Unit Test stereotype
    }

    return result;
}