async function getAiProviderModels() {
    const moduleTaskResult = await executeModuleTask("Intent.Modules.Common.AI.Tasks.ProviderModelsTask");
    const providerModels = JSON.parse(moduleTaskResult);
    const modelLookup = providerModels.reduce((acc, item) => {
        acc[`${item.providerId}--${item.modelName}`] = item;
        return acc;
    }, {});
    return { providerModels, modelLookup };
}
async function getAiModelSelectionFields(providerModelsResult, aiSettingKeyPrefix) {
    var _a;
    const globalSettings = await userSettings.loadGlobalAsync();
    const settingModelId = globalSettings.get(`${aiSettingKeyPrefix}.ModelId`);
    const settingThinkingLevel = globalSettings.get(`${aiSettingKeyPrefix}.ThinkingLevel`);
    const { providerModels, modelLookup } = providerModelsResult;
    const initialThinkingType = (_a = modelLookup[settingModelId]) === null || _a === void 0 ? void 0 : _a.thinkingType;
    return [
        {
            id: "model",
            fieldType: "select",
            label: "Model",
            isRequired: true,
            hint: getModelHint(providerModels, initialThinkingType),
            selectOptions: Object.entries(modelLookup)
                .map(([key, value]) => {
                return {
                    id: key,
                    description: value.modelName,
                    additionalInfo: value.providerName
                };
            }),
            value: settingModelId,
            onChange: async (config) => {
                const curThinkingType = modelLookup[config.getField("model").value].thinkingType;
                const thinkingField = config.getField("thinking");
                thinkingField.isHidden = curThinkingType === "None";
                thinkingField.selectOptions = getApplicableThinkingOptions(curThinkingType);
                thinkingField.hint = getModelHint(providerModels, curThinkingType);
                if (curThinkingType === "ThinkingLevels") {
                    thinkingField.value = "low";
                }
                else if (curThinkingType === "Unknown") {
                    thinkingField.value = "none";
                }
                else {
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
    function getModelHint(providerModels, thinkingType) {
        if (providerModels.length === 0) {
            return "Not seeing any AI Models? Learn how to configure or add models [here](https://docs.intentarchitect.com/articles/modules-common/intent-common-ai/intent-common-ai.html).";
        }
        else if (thinkingType == "Unknown") {
            return "Thinking level for model is unknown; none is selected by default.";
        }
        else {
            return "";
        }
    }
    function getApplicableThinkingOptions(thinkingType) {
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
        }
        else if (thinkingType === "Unknown") {
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
        }
        else {
            return [];
        }
    }
}
async function collectAndPersistAiSettingsFromPromptResult(promptResult, providerModelsResult, aiSettingKeyPrefix) {
    const providerId = providerModelsResult.modelLookup[promptResult.model].providerId;
    const modelId = providerModelsResult.modelLookup[promptResult.model].modelName;
    const thinkingLevel = promptResult.thinking;
    const globalSettings = await userSettings.loadGlobalAsync();
    globalSettings.set(`${aiSettingKeyPrefix}.ModelId`, `${providerId}--${modelId}`);
    globalSettings.set(`${aiSettingKeyPrefix}.ThinkingLevel`, thinkingLevel);
    return { providerId, modelId, thinkingLevel: thinkingLevel };
}
async function execute(taskId) {
    var _a;
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
    const { providerId, modelId, thinkingLevel: thinkingLevel } = await collectAndPersistAiSettingsFromPromptResult(promptResult, providerModelsResult, settingName);
    let result = await launchHostedModuleTask(taskId, [
        application.id,
        element.id,
        (_a = promptResult.prompt) !== null && _a !== void 0 ? _a : "",
        providerId,
        modelId,
        thinkingLevel
    ], {
        taskName: "AI: Unit tests for " + element.getName()
    });
    const unitTestStereotype = "4965bed2-6320-49d1-beba-0fbc6fd4dfe6";
    if (!element.hasStereotype(unitTestStereotype)) {
        element.addStereotype(unitTestStereotype); // Unit Test stereotype
    }
    return result;
}
class Icons {
}
Icons.AiUnitTests = "data:image/svg+xml;base64,PD94bWwgdmVyc2lvbj0iMS4wIiBlbmNvZGluZz0iVVRGLTgiPz4KPHN2ZyBpZD0iYnJhY2tldF9zeW1ib2wtYmx1ZSIgeG1sbnM9Imh0dHA6Ly93d3cudzMub3JnLzIwMDAvc3ZnIiB2ZXJzaW9uPSIxLjEiIHhtbG5zOnhsaW5rPSJodHRwOi8vd3d3LnczLm9yZy8xOTk5L3hsaW5rIiB2aWV3Qm94PSIwIDAgNDIuOSA0Mi45Ij4KICA8IS0tIEdlbmVyYXRvcjogQWRvYmUgSWxsdXN0cmF0b3IgMjkuNi4xLCBTVkcgRXhwb3J0IFBsdWctSW4gLiBTVkcgVmVyc2lvbjogMi4xLjEgQnVpbGQgOSkgIC0tPgogIDxkZWZzPgogICAgPGxpbmVhckdyYWRpZW50IGlkPSJsaW5lYXItZ3JhZGllbnQiIHgxPSItMTE1MjIuNyIgeTE9Ii02NjU3LjciIHgyPSItMTE1MjIuNyIgeTI9Ii02NjM5LjQiIGdyYWRpZW50VHJhbnNmb3JtPSJ0cmFuc2xhdGUoLTY2MzYgLTExNDk1LjIpIHJvdGF0ZSgtOTApIHNjYWxlKDEgLTEpIiBncmFkaWVudFVuaXRzPSJ1c2VyU3BhY2VPblVzZSI+CiAgICAgIDxzdG9wIG9mZnNldD0iMCIgc3RvcC1jb2xvcj0iI2ZmZiIvPgogICAgICA8c3RvcCBvZmZzZXQ9Ii40IiBzdG9wLWNvbG9yPSIjMDljNGZmIi8+CiAgICAgIDxzdG9wIG9mZnNldD0iMSIgc3RvcC1jb2xvcj0iIzAwNzBjMCIvPgogICAgPC9saW5lYXJHcmFkaWVudD4KICAgIDxsaW5lYXJHcmFkaWVudCBpZD0ibGluZWFyLWdyYWRpZW50MSIgeDE9Ii0xNTYxMi4yIiB5MT0iLTE1MzciIHgyPSItMTU2MTIuMiIgeTI9Ii0xNTE4LjkiIGdyYWRpZW50VHJhbnNmb3JtPSJ0cmFuc2xhdGUoLTEyMDk5LjggLTk5MjguNykgcm90YXRlKC0xMzUpIHNjYWxlKDEgLTEpIiBncmFkaWVudFVuaXRzPSJ1c2VyU3BhY2VPblVzZSI+CiAgICAgIDxzdG9wIG9mZnNldD0iMCIgc3RvcC1jb2xvcj0iI2ZmZiIvPgogICAgICA8c3RvcCBvZmZzZXQ9Ii40IiBzdG9wLWNvbG9yPSIjMDljNGZmIi8+CiAgICAgIDxzdG9wIG9mZnNldD0iMSIgc3RvcC1jb2xvcj0iIzAwNzBjMCIvPgogICAgPC9saW5lYXJHcmFkaWVudD4KICAgIDxsaW5lYXJHcmFkaWVudCBpZD0ibGluZWFyLWdyYWRpZW50MiIgeDE9Ii0xNDg4Mi42IiB5MT0iNDk3NS40IiB4Mj0iLTE0ODgyLjYiIHkyPSI0OTkzLjYiIGdyYWRpZW50VHJhbnNmb3JtPSJ0cmFuc2xhdGUoLTE0ODU1LjIgLTQ5NTcuMikgcm90YXRlKC0xODApIHNjYWxlKDEgLTEpIiBncmFkaWVudFVuaXRzPSJ1c2VyU3BhY2VPblVzZSI+CiAgICAgIDxzdG9wIG9mZnNldD0iMCIgc3RvcC1jb2xvcj0iI2ZmZiIvPgogICAgICA8c3RvcCBvZmZzZXQ9Ii40IiBzdG9wLWNvbG9yPSIjMDljNGZmIi8+CiAgICAgIDxzdG9wIG9mZnNldD0iMSIgc3RvcC1jb2xvcj0iIzAwNzBjMCIvPgogICAgPC9saW5lYXJHcmFkaWVudD4KICAgIDxsaW5lYXJHcmFkaWVudCBpZD0ibGluZWFyLWdyYWRpZW50MyIgeDE9Ii05NzYyLjMiIHkxPSI5MDY0LjUiIHgyPSItOTc2Mi4zIiB5Mj0iOTA4Mi43IiBncmFkaWVudFRyYW5zZm9ybT0idHJhbnNsYXRlKC0xMzI4OC43IDUwNi43KSByb3RhdGUoMTM1KSBzY2FsZSgxIC0xKSIgZ3JhZGllbnRVbml0cz0idXNlclNwYWNlT25Vc2UiPgogICAgICA8c3RvcCBvZmZzZXQ9IjAiIHN0b3AtY29sb3I9IiNmZmYiLz4KICAgICAgPHN0b3Agb2Zmc2V0PSIuNCIgc3RvcC1jb2xvcj0iIzA5YzRmZiIvPgogICAgICA8c3RvcCBvZmZzZXQ9IjEiIHN0b3AtY29sb3I9IiMwMDcwYzAiLz4KICAgIDwvbGluZWFyR3JhZGllbnQ+CiAgICA8bGluZWFyR3JhZGllbnQgaWQ9ImxpbmVhci1ncmFkaWVudDQiIHgxPSItMzI0OS41IiB5MT0iODMzNS40IiB4Mj0iLTMyNDkuNSIgeTI9IjgzNTMuNiIgZ3JhZGllbnRUcmFuc2Zvcm09InRyYW5zbGF0ZSgtODMxNy4xIDMyNjIuMSkgcm90YXRlKDkwKSBzY2FsZSgxIC0xKSIgZ3JhZGllbnRVbml0cz0idXNlclNwYWNlT25Vc2UiPgogICAgICA8c3RvcCBvZmZzZXQ9IjAiIHN0b3AtY29sb3I9IiNmZmYiLz4KICAgICAgPHN0b3Agb2Zmc2V0PSIuNCIgc3RvcC1jb2xvcj0iIzA5YzRmZiIvPgogICAgICA8c3RvcCBvZmZzZXQ9IjEiIHN0b3AtY29sb3I9IiMwMDcwYzAiLz4KICAgIDwvbGluZWFyR3JhZGllbnQ+CiAgICA8bGluZWFyR3JhZGllbnQgaWQ9ImxpbmVhci1ncmFkaWVudDUiIHgxPSI4MzkuNCIgeTE9IjMyMTQuOCIgeDI9IjgzOS40IiB5Mj0iMzIzMyIgZ3JhZGllbnRUcmFuc2Zvcm09InRyYW5zbGF0ZSgtMjg1My4zIDE2OTUuNSkgcm90YXRlKDQ1KSBzY2FsZSgxIC0xKSIgZ3JhZGllbnRVbml0cz0idXNlclNwYWNlT25Vc2UiPgogICAgICA8c3RvcCBvZmZzZXQ9IjAiIHN0b3AtY29sb3I9IiNmZmYiLz4KICAgICAgPHN0b3Agb2Zmc2V0PSIuNCIgc3RvcC1jb2xvcj0iIzA5YzRmZiIvPgogICAgICA8c3RvcCBvZmZzZXQ9IjEiIHN0b3AtY29sb3I9IiMwMDcwYzAiLz4KICAgIDwvbGluZWFyR3JhZGllbnQ+CiAgICA8bGluZWFyR3JhZGllbnQgaWQ9ImxpbmVhci1ncmFkaWVudDYiIHgxPSIxMTAuNSIgeTE9Ii0zMjk3LjciIHgyPSIxMTAuNSIgeTI9Ii0zMjc5LjQiIGdyYWRpZW50VHJhbnNmb3JtPSJ0cmFuc2xhdGUoLTk3LjkgLTMyNzYpIHNjYWxlKDEgLTEpIiBncmFkaWVudFVuaXRzPSJ1c2VyU3BhY2VPblVzZSI+CiAgICAgIDxzdG9wIG9mZnNldD0iMCIgc3RvcC1jb2xvcj0iI2ZmZiIvPgogICAgICA8c3RvcCBvZmZzZXQ9Ii40IiBzdG9wLWNvbG9yPSIjMDljNGZmIi8+CiAgICAgIDxzdG9wIG9mZnNldD0iMSIgc3RvcC1jb2xvcj0iIzAwNzBjMCIvPgogICAgPC9saW5lYXJHcmFkaWVudD4KICAgIDxsaW5lYXJHcmFkaWVudCBpZD0ibGluZWFyLWdyYWRpZW50NyIgeDE9Ii01MDEwLjYiIHkxPSItNzM4Ni43IiB4Mj0iLTUwMTAuNiIgeTI9Ii03MzY4LjYiIGdyYWRpZW50VHJhbnNmb3JtPSJ0cmFuc2xhdGUoLTE2NjQuNSAtODczOS44KSByb3RhdGUoLTQ1KSBzY2FsZSgxIC0xKSIgZ3JhZGllbnRVbml0cz0idXNlclNwYWNlT25Vc2UiPgogICAgICA8c3RvcCBvZmZzZXQ9IjAiIHN0b3AtY29sb3I9IiNmZmYiLz4KICAgICAgPHN0b3Agb2Zmc2V0PSIuNCIgc3RvcC1jb2xvcj0iIzA5YzRmZiIvPgogICAgICA8c3RvcCBvZmZzZXQ9IjEiIHN0b3AtY29sb3I9IiMwMDcwYzAiLz4KICAgIDwvbGluZWFyR3JhZGllbnQ+CiAgICA8bGluZWFyR3JhZGllbnQgaWQ9ImxpbmVhci1ncmFkaWVudDgiIHgxPSIzMS4yIiB5MT0iLTIwNjgiIHgyPSIzMS4yIiB5Mj0iLTIwNTAuNiIgZ3JhZGllbnRUcmFuc2Zvcm09InRyYW5zbGF0ZSgwIC0yMDI4KSBzY2FsZSgxIC0xKSIgZ3JhZGllbnRVbml0cz0idXNlclNwYWNlT25Vc2UiPgogICAgICA8c3RvcCBvZmZzZXQ9IjAiIHN0b3AtY29sb3I9IiM0YTk5MjQiLz4KICAgICAgPHN0b3Agb2Zmc2V0PSIxIiBzdG9wLWNvbG9yPSIjODZkZTMzIi8+CiAgICA8L2xpbmVhckdyYWRpZW50PgogIDwvZGVmcz4KICA8cGF0aCBkPSJNMjEuNywyMS43YzAtLjQuNCwzLjctMS41LDYuOC0xLjgsMy4xLTQuMSw0LTcuNyw0LjgtMiwuNS0zLjIsMC00LjgtLjUtMi4yLS43LTMuMy0yLjUtMy43LTMuMXMtLjgtMS45LS41LTMuMmMuNS0yLjIsMi0zLjMsNC0zLjkuNCwwLDIuMi0uNSwzLjkuNHMyLjIsMS41LDMuNywxLjljMS41LjQsMy4xLDAsMy43LS40LDEuNy0uNiwyLjktMi40LDMuMS0zLjF2LjJoLS4yWiIgZmlsbD0idXJsKCNsaW5lYXItZ3JhZGllbnQpIi8+CiAgPHBhdGggZD0iTTIyLjQsMTkuOWMtLjMtLjMsMi45LDIuNCwzLjcsNS44cy4yLDUuOC0yLDguOWMtMS4yLDEuOC0yLjMsMi40LTMuOCwzLjItMi4xLDEtNC4yLjYtNC43LjVzLTEuOS0uNy0yLjYtMS45Yy0xLjEtMS45LTEtMy44LDAtNS42LjMtLjMsMS4xLTEuOSwzLTIuNSwxLjgtLjYsMi42LS41LDQtMS4zczIuMS0yLjEsMi40LTIuOWMuNy0xLjcuNC0zLjcsMC00LjNoLjItLjJaIiBmaWxsPSJ1cmwoI2xpbmVhci1ncmFkaWVudDEpIi8+CiAgPHBhdGggZD0iTTIxLjcsMTguM2MtLjQsMCwzLjctLjQsNi44LDEuNSwzLjEsMS44LDQuMyw0LDQuOCw3LjcuNCwyLDAsMy4yLS41LDQuOC0uNywyLjItMi41LDMuMy0zLjEsMy43cy0xLjkuOC0zLjIuNWMtMi4yLS41LTMuMy0yLTMuOS00LDAtLjQtLjUtMi4yLjQtMy45czEuNS0yLjIsMS45LTMuNywwLTMuMS0uNC0zLjdjLS42LTEuNy0yLjQtMi45LTMuMS0zLjFoLjJ2LjJoLjFaIiBmaWxsPSJ1cmwoI2xpbmVhci1ncmFkaWVudDIpIi8+CiAgPHBhdGggZD0iTTE5LjksMTcuNmMtLjMuMywyLjQtMi45LDUuOC0zLjdzNS44LS4yLDguOSwyYzEuOCwxLjIsMi40LDIuMywzLjIsMy44LDEsMi4xLjYsNC4yLjUsNC43cy0uNywxLjktMS45LDIuNmMtMS45LDEuMS0zLjgsMS01LjYsMC0uMy0uMy0xLjktMS4xLTIuNS0zcy0uNS0yLjYtMS4zLTQtMi4xLTIuMS0yLjktMi40Yy0xLjctLjctMy43LS40LTQuMywwaDB2LS4yaDB2LjJzLjEsMCwuMSwwWiIgZmlsbD0idXJsKCNsaW5lYXItZ3JhZGllbnQzKSIvPgogIDxwYXRoIGQ9Ik0xOC4zLDE4LjNjMCwuNC0uNC0zLjcsMS41LTYuOHM0LTQuMyw3LjctNC44YzItLjQsMy4yLDAsNC44LjUsMi4yLjcsMy4zLDIuNSwzLjcsMy4xcy44LDEuOS41LDMuMmMtLjUsMi4yLTIsMy4zLTQsMy45LS40LDAtMi4yLjUtMy45LS40cy0yLjItMS41LTMuNy0xLjktMy4xLDAtMy43LjRjLTEuNy42LTIuOSwyLjQtMy4xLDMuMXYtLjJoLjJaIiBmaWxsPSJ1cmwoI2xpbmVhci1ncmFkaWVudDQpIi8+CiAgPHBhdGggZD0iTTE3LjYsMTkuOWMuMy4zLTIuOS0yLjQtMy43LTUuOHMtLjItNS44LDItOC45YzEuMi0xLjgsMi4zLTIuNCwzLjgtMy4yLDIuMS0xLDQuMi0uNiw0LjctLjVzMS45LjcsMi42LDEuOWMxLjEsMS45LDEsMy44LDAsNS42LS4zLjMtMS4xLDEuOS0zLDIuNXMtMi42LjUtNCwxLjNjLTEuMy44LTIuMSwyLjEtMi40LDIuOS0uNywxLjctLjQsMy43LDAsNC4zaC0uMi4yLDBaIiBmaWxsPSJ1cmwoI2xpbmVhci1ncmFkaWVudDUpIi8+CiAgPHBhdGggZD0iTTE4LjMsMjEuN2MuNCwwLTMuNy40LTYuOC0xLjVzLTQuMy00LTQuOC03LjdjLS40LTIsMC0zLjIuNS00LjguNy0yLjIsMi41LTMuMywzLjEtMy43czEuOS0uOCwzLjItLjVjMi4yLjUsMy4zLDIsMy45LDQsMCwuNC41LDIuMi0uNCwzLjlzLTEuNSwyLjItMS45LDMuN2MtLjQsMS41LDAsMy4xLjQsMy43LjYsMS43LDIuNCwyLjksMy4xLDMuMWgtLjJ2LS4yaC0uMVoiIGZpbGw9InVybCgjbGluZWFyLWdyYWRpZW50NikiLz4KICA8cGF0aCBkPSJNMTkuOSwyMi40Yy4zLS4zLTIuNCwyLjktNS44LDMuN3MtNS44LjItOC45LTJjLTEuOC0xLjItMi40LTIuMy0zLjItMy44LTEtMi4xLS42LTQuMi0uNS00LjdzLjctMS45LDEuOS0yLjZjMS45LTEuMSwzLjgtMSw1LjYsMCwuMy4zLDEuOSwxLjEsMi41LDMsLjYsMS44LjUsMi42LDEuMyw0LC44LDEuMywyLjEsMi4xLDIuOSwyLjQsMS43LjcsMy43LjQsNC4zLDBoMHYuMmgwdi0uMnMtLjEsMC0uMSwwWiIgZmlsbD0idXJsKCNsaW5lYXItZ3JhZGllbnQ3KSIvPgogIDxyZWN0IHg9IjI4LjQiIHk9IjE4LjkiIHdpZHRoPSI2IiBoZWlnaHQ9IjE1LjkiIGZpbGw9IiNmZmYiLz4KICA8cGF0aCBkPSJNMzQuNywzMC4zdi05LjhjLjgsMCwuOC0uMy44LS44di0uOGMwLS40LS4yLS44LS43LS44aC02LjhjLS40LDAtLjguMy0uOC44di44YzAsLjguOC44LjguOHY5LjhsLTUuOCw4LjdjLS4zLjUtLjQsMS4yLDAsMS43cy45LjksMS41LjloMTUuMWMuNiwwLDEuMS0uMywxLjQtLjhzLjItLjYuMi0uOCwwLS41LS4zLS44bC01LjQtOC44aDBaTTI4LjQsMzMuMmwxLjYtMi41di0xMC4yaDIuNnYxMC4xbDEuNSwyLjZzLTUuNywwLTUuNywwWiIgZmlsbD0idXJsKCNsaW5lYXItZ3JhZGllbnQ4KSIgc3Ryb2tlPSIjZmZmIiBzdHJva2UtbWl0ZXJsaW1pdD0iMTAiIHN0cm9rZS13aWR0aD0iLjgiLz4KPC9zdmc+";
