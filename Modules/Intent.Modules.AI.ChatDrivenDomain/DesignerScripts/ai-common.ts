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
    const settingModelId = globalSettings.get(`${aiSettingKeyPrefix}.ModelId`);
    const settingThinkingLevel = globalSettings.get(`${aiSettingKeyPrefix}.ThinkingLevel`);

    const { providerModels, modelLookup } = providerModelsResult;
    const initialThinkingType = modelLookup[settingModelId]?.thinkingType;

    return [
        {
            id: "model",
            fieldType: "select",
            label: "Model",
            isRequired: true,
            hint: getModelHint(providerModels, initialThinkingType),
            selectOptions: 
                Object.entries(modelLookup)
                .map(([key, value]: [string, IModelRecord]) => { 
                    return { 
                        id: key, 
                        description: value.modelName, 
                        additionalInfo: value.providerName
                    }; 
                }),
            value: settingModelId,
            onChange: async (config) => {
                const curThinkingType = modelLookup[config.getField("model").value as string].thinkingType;
                const thinkingField = config.getField("thinking");

                thinkingField.isHidden = curThinkingType === "None";
                thinkingField.selectOptions = getApplicableThinkingOptions(curThinkingType);
                thinkingField.hint = getModelHint(providerModels, curThinkingType);

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
            isHidden: settingThinkingLevel == null || providerModels.length === 0,
            value: settingThinkingLevel,
            selectOptions: getApplicableThinkingOptions(initialThinkingType)
        }
    ];

    function getModelHint(providerModels: IModelRecord[], thinkingType: ThinkingType | null): string {
        if (providerModels.length === 0) { 
            return "Not seeing any AI Models? Learn how to configure or add models [here](https://docs.intentarchitect.com/articles/modules-common/intent-common-ai/intent-common-ai.html).";
        } else if (thinkingType == "Unknown") {
            return "Thinking level for model is unknown; none is selected by default.";
        } else {
            return "";
        }
    }

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