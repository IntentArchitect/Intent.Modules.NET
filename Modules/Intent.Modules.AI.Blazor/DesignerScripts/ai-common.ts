interface IModelRecord {
    providerId: string;
    modelName: string;
    providerName: string;
    thinkingType: ThinkingType;
}

type ThinkingType = "None" | "Unknown" | "ThinkingLevels";

interface IProviderModelsResult {
    providerModels: IModelRecord[];
    modelLookup: { [key: string]: IModelRecord };
}

interface ICollectedAiSettings {
    providerId: string;
    modelId: string;
    thinkingLevel: string | null;
}

async function getAiProviderModels(): Promise<IProviderModelsResult> {
    const moduleTaskResult = await executeModuleTask("Intent.Modules.Common.AI.Tasks.ProviderModelsTask");

    const providerModels = JSON.parse(moduleTaskResult) as IModelRecord[];
    const modelLookup = providerModels.reduce((acc: any, item) => {
        acc[`${item.providerId}--${item.modelName}`] = item;
        return acc;
    }, {});

    return { providerModels, modelLookup };
}

async function getAiModelSelectionFields(providerModelsResult: IProviderModelsResult, aiSettingKeyPrefix: string): Promise<MacroApi.Context.IDynamicFormFieldConfig[]> {
    const globalSettings = await userSettings.loadGlobalAsync();
    const autoImplementationAiModelId = globalSettings.get(`${aiSettingKeyPrefix}.ModelId`);
    const autoImplementationAiThinkingLevel = globalSettings.get(`${aiSettingKeyPrefix}.ThinkingLevel`);

    const { providerModels, modelLookup } = providerModelsResult;

    return [
        {
            id: "model",
            fieldType: "select",
            label: "Model",
            isRequired: true,
            hint: providerModels.length === 0 ? "Not seeing any AI Models? Learn how to configure or add models [here](https://docs.intentarchitect.com/articles/modules-common/intent-common-ai/intent-common-ai.html)." : "",
            selectOptions: 
                Object.entries(modelLookup)
                .map(([key, value]: [string, IModelRecord]) => { 
                    return { 
                        id: key, 
                        description: value.modelName, 
                        additionalInfo: value.providerName
                    }; 
                }),
            value: autoImplementationAiModelId,
            onChange: async (config) => {
                const curThinkingType = modelLookup[config.getField("model").value as string].thinkingType;
                const thinkingField = config.getField("thinking");

                thinkingField.isHidden = curThinkingType === "None";
                thinkingField.selectOptions = getApplicableThinkingOptions(curThinkingType);

                if (curThinkingType === "ThinkingLevels") {
                    thinkingField.value = "low";
                } else if (curThinkingType === "Unknown") {
                    thinkingField.value = "none";
                } else {
                    thinkingField.value = null;
                }
            }
        },
        {
            id: "thinking",
            fieldType: "select",
            label: "Thinking/reasoning mode",
            isHidden: autoImplementationAiThinkingLevel == null || providerModels.length === 0,
            value: autoImplementationAiThinkingLevel,
            selectOptions: getApplicableThinkingOptions(modelLookup[autoImplementationAiModelId].thinkingType)
        }
    ];

    function getApplicableThinkingOptions(thinkingType: ThinkingType | null): MacroApi.Context.IDynamicFormFieldSelectOption[] {
        if (thinkingType === "ThinkingLevels") {
            return [
                {
                    id: "low",
                    description: "Low",
                    additionalInfo: "Thinks less, quicker"
                },
                {
                    id: "high",
                    description: "High",
                    additionalInfo: "Thinks more, slower"
                }
            ];
        } else if (thinkingType === "Unknown") {
            return [
                {
                    id: "none",
                    description: "None",
                    additionalInfo: "No thinking/reasoning"
                },
                {
                    id: "low",
                    description: "Low",
                    additionalInfo: "Thinks less, quicker"
                },
                {
                    id: "high",
                    description: "High",
                    additionalInfo: "Thinks more, slower"
                }
            ];
        } else {
            return [];
        }
    }
}

async function collectAndPersistAiSettingsFromPromptResult(promptResult: any, providerModelsResult: IProviderModelsResult, aiSettingKeyPrefix: string): Promise<ICollectedAiSettings> {
    const providerId = providerModelsResult.modelLookup[promptResult.model].providerId;
    const modelId = providerModelsResult.modelLookup[promptResult.model].modelName;
    const thinkingLevel = promptResult.thinking;

    const globalSettings = await userSettings.loadGlobalAsync();
    globalSettings.set(`${aiSettingKeyPrefix}.ModelId`, `${providerId}--${modelId}`);
    globalSettings.set(`${aiSettingKeyPrefix}.ThinkingLevel`, thinkingLevel);

    return { providerId, modelId, thinkingLevel: thinkingLevel };
}