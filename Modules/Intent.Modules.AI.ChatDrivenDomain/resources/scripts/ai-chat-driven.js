async function execute(element) {
    const providerModelsResult = await getAiProviderModels();
    const settingName = "AI.ChatDrivenDomain";
    // Open a dialog for the user to enter an AI prompt
    let promptResult = await dialogService.openForm({
        title: "Execute Prompt",
        fields: [
            {
                id: "prompt",
                fieldType: "textarea",
                label: "Enter AI prompt",
                hint: "NOTE: Intent Architect designer data will also be sent to the AI service provider."
            },
            ...await getAiModelSelectionFields(providerModelsResult, settingName)
        ]
    });
    // Check if the user cancelled or provided an empty prompt
    if (!promptResult || promptResult.prompt.trim() === "") {
        return;
    }
    // Gather data about the current class model
    const currentClasses = lookupTypesOf("Class").map(clazz => {
        // Get all associations where this class is either source or target
        const associations = clazz.getAssociations().map(assoc => {
            //console.log(`classId = ${clazz.id}; className = ${clazz.getName()}; assoc = {classId: ${assoc.typeReference.getTypeId()}, name: ${assoc.getName()}, isSourceEnd: ${assoc.isSourceEnd()}, isCollection: ${assoc.typeReference.isCollection}}`)
            // Determine if this class is the source or target of the association
            const isSource = assoc.isSourceEnd(); //assoc.getParent().id === clazz.id;
            const sourceEnd = isSource ? assoc : assoc.getOtherEnd();
            if (isSource && !sourceEnd.typeReference.isNavigable) {
                return null;
            }
            const targetEnd = isSource ? assoc.getOtherEnd() : assoc;
            // Get type references for both ends
            const sourceTypeRef = sourceEnd.typeReference;
            const targetTypeRef = targetEnd.typeReference;
            // Build the relationship string (e.g., "1 -> *")
            const sourceMultiplicity = sourceTypeRef.getIsCollection() ? "*" :
                sourceTypeRef.getIsNullable() ? "0..1" : "1";
            const targetMultiplicity = targetTypeRef.getIsCollection() ? "*" :
                targetTypeRef.getIsNullable() ? "0..1" : "1";
            const relationship = `${sourceMultiplicity} -> ${targetMultiplicity}`;
            return {
                id: assoc.id,
                name: assoc.getName(),
                classId: assoc.typeReference.getTypeId(),
                type: relationship,
                specializationEndType: isSource ? "Source End" : "Target End",
                // Add these for compatibility with the C# model
                relationship: relationship,
                associationEndType: isSource ? "Source End" : "Target End",
                isNullable: assoc.typeReference.isNullable,
                isCollection: assoc.typeReference.isCollection
            };
        })
            .filter(x => x != null);
        return {
            id: clazz.id,
            name: clazz.getName(),
            comment: clazz.getComment(),
            attributes: clazz.getChildren("Attribute")
                .filter(attr => !attr.hasMetadata("is-managed-key") && !attr.hasMetadata("set-by-infrastructure"))
                .map(attr => ({
                id: attr.id,
                name: attr.getName(),
                type: attr.typeReference ? attr.typeReference.getType().getName() : null,
                isNullable: attr.typeReference ? attr.typeReference.getIsNullable() : false,
                isCollection: attr.typeReference ? attr.typeReference.getIsCollection() : false,
                comment: attr.getComment()
            })),
            associations: associations
        };
    });
    const { providerId, modelId, thinkingLevel: thinkingLevel } = await collectAndPersistAiSettingsFromPromptResult(promptResult, providerModelsResult, settingName);
    const input = { prompt: promptResult.prompt, classes: currentClasses, providerId: providerId, modelId: modelId, thinkingLevel: thinkingLevel };
    // Execute the AI module task
    let outputStr;
    try {
        outputStr = await executeModuleTask("Intent.Modules.ChatDrivenDomain.Tasks.ChatCompletionTask", JSON.stringify(input));
    }
    catch (error) {
        dialogService.error(`Failed to execute AI task: ${error.message}`);
        return;
    }
    // Parse the result from the AI task
    let updatedClasses;
    try {
        updatedClasses = JSON.parse(outputStr);
    }
    catch (error) {
        dialogService.error(`Failed to parse AI task result: ${error.message}`);
        return;
    }
    // Check for errors in the AI response
    if (updatedClasses.errorMessage) {
        dialogService.error(updatedClasses.errorMessage);
        return;
    }
    // Create a lookup of type names to IDs
    const types = lookupTypesOf("Type-Definition");
    const typesLookup = Object.fromEntries(types.map(el => [el.getName(), el.id]));
    // Create maps for existing classes and updated classes
    const existingClassesMap = new Map(lookupTypesOf("Class").map(clazz => [clazz.id, clazz]));
    const packageId = element.id;
    const updatedClassesMap = new Map(updatedClasses.map((jClass) => [jClass.id, jClass]));
    // Process class updates and additions
    updatedClasses.forEach((jClass) => {
        let clazz = existingClassesMap.get(jClass.id);
        if (!clazz) {
            // Add new class
            clazz = createElement("Class", jClass.name, packageId);
            existingClassesMap.set(jClass.id, clazz);
        }
        // Update class properties
        clazz.setName(jClass.name, true);
        clazz.setComment(jClass.comment);
        // Process attributes - first delete all existing attributes
        const existingAttributes = clazz.getChildren("Attribute");
        existingAttributes.forEach(attr => attr.delete());
        // Add all attributes from the updated model
        if (jClass.attributes && jClass.attributes.length > 0) {
            jClass.attributes.forEach((jAttr) => {
                const attr = createElement("Attribute", jAttr.name, clazz.id);
                attr.setComment(jAttr.comment || "");
                if (jAttr.type && typesLookup[jAttr.type]) {
                    attr.typeReference.setType(typesLookup[jAttr.type]);
                    attr.typeReference.setIsCollection(jAttr.isCollection || false);
                    attr.typeReference.setIsNullable(jAttr.isNullable || false);
                }
            });
        }
    });
    // Delete classes not in the updated model
    existingClassesMap.forEach((clazz, id) => {
        if (!updatedClassesMap.has(id)) {
            clazz.delete();
        }
    });
    // Clear all existing associations
    const existingAssociations = lookupTypesOf("Association");
    existingAssociations.forEach(assoc => {
        assoc.delete();
    });
    // Create a map to track which associations we've already created
    // We'll use a compound key of sourceClassId + targetClassId to uniquely identify each association
    const createdAssociations = new Map();
    // For each class in our updated model
    updatedClasses.forEach((jClass) => {
        // Process all associations for this class
        jClass.associations.forEach((jAssoc) => {
            const thisClassId = jClass.id;
            const otherClassId = jAssoc.classId;
            const thisClass = existingClassesMap.get(thisClassId);
            const otherClass = existingClassesMap.get(otherClassId);
            if (!thisClass || !otherClass)
                return;
            // Determine the source and target classes based on associationEndType
            let sourceClassId, targetClassId;
            if (jAssoc.associationEndType === "Target End") {
                // This class is the source
                sourceClassId = thisClassId;
                targetClassId = otherClassId;
            }
            else if (jAssoc.associationEndType === "Source End") {
                // This class is the target
                sourceClassId = otherClassId;
                targetClassId = thisClassId;
            }
            else {
                return; // Skip if associationEndType is not recognized
            }
            // Create a compound key to identify this association
            const associationKey = sourceClassId + "->" + targetClassId;
            // Only create the association if we haven't already created it
            if (!createdAssociations.has(associationKey)) {
                const sourceClass = existingClassesMap.get(sourceClassId);
                const targetClass = existingClassesMap.get(targetClassId);
                // Create the association
                const association = createAssociation("Association", sourceClass.id, targetClass.id);
                // Mark this association as created
                createdAssociations.set(associationKey, association);
            }
            // Get the association we just created or previously created
            const association = createdAssociations.get(associationKey);
            // Now apply the properties to the appropriate end
            if (jAssoc.associationEndType === "Target End") {
                // Set properties on the target end
                const targetEnd = association;
                targetEnd.setName(jAssoc.name || "");
                targetEnd.typeReference.setIsCollection(jAssoc.isCollection || false);
                targetEnd.typeReference.setIsNullable(jAssoc.isNullable || false);
            }
            else if (jAssoc.associationEndType === "Source End") {
                // Set properties on the source end
                // In this case, the association itself is the source end
                const sourceEnd = association.getOtherEnd();
                sourceEnd.setName(jAssoc.name || "");
                sourceEnd.typeReference.setIsCollection(jAssoc.isCollection || false);
                sourceEnd.typeReference.setIsNullable(jAssoc.isNullable || false);
            }
        });
    });
    // Show a completion message
    dialogService.info("Domain model updated based on AI response.");
}
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
class Icons {
}
Icons.AiUnitTests = "data:image/svg+xml;base64,PD94bWwgdmVyc2lvbj0iMS4wIiBlbmNvZGluZz0iVVRGLTgiPz4KPHN2ZyBpZD0iYnJhY2tldF9zeW1ib2wtYmx1ZSIgeG1sbnM9Imh0dHA6Ly93d3cudzMub3JnLzIwMDAvc3ZnIiB2ZXJzaW9uPSIxLjEiIHhtbG5zOnhsaW5rPSJodHRwOi8vd3d3LnczLm9yZy8xOTk5L3hsaW5rIiB2aWV3Qm94PSIwIDAgNDIuOSA0Mi45Ij4KICA8IS0tIEdlbmVyYXRvcjogQWRvYmUgSWxsdXN0cmF0b3IgMjkuNi4xLCBTVkcgRXhwb3J0IFBsdWctSW4gLiBTVkcgVmVyc2lvbjogMi4xLjEgQnVpbGQgOSkgIC0tPgogIDxkZWZzPgogICAgPGxpbmVhckdyYWRpZW50IGlkPSJsaW5lYXItZ3JhZGllbnQiIHgxPSItMTE1MjIuNyIgeTE9Ii02NjU3LjciIHgyPSItMTE1MjIuNyIgeTI9Ii02NjM5LjQiIGdyYWRpZW50VHJhbnNmb3JtPSJ0cmFuc2xhdGUoLTY2MzYgLTExNDk1LjIpIHJvdGF0ZSgtOTApIHNjYWxlKDEgLTEpIiBncmFkaWVudFVuaXRzPSJ1c2VyU3BhY2VPblVzZSI+CiAgICAgIDxzdG9wIG9mZnNldD0iMCIgc3RvcC1jb2xvcj0iI2ZmZiIvPgogICAgICA8c3RvcCBvZmZzZXQ9Ii40IiBzdG9wLWNvbG9yPSIjMDljNGZmIi8+CiAgICAgIDxzdG9wIG9mZnNldD0iMSIgc3RvcC1jb2xvcj0iIzAwNzBjMCIvPgogICAgPC9saW5lYXJHcmFkaWVudD4KICAgIDxsaW5lYXJHcmFkaWVudCBpZD0ibGluZWFyLWdyYWRpZW50MSIgeDE9Ii0xNTYxMi4yIiB5MT0iLTE1MzciIHgyPSItMTU2MTIuMiIgeTI9Ii0xNTE4LjkiIGdyYWRpZW50VHJhbnNmb3JtPSJ0cmFuc2xhdGUoLTEyMDk5LjggLTk5MjguNykgcm90YXRlKC0xMzUpIHNjYWxlKDEgLTEpIiBncmFkaWVudFVuaXRzPSJ1c2VyU3BhY2VPblVzZSI+CiAgICAgIDxzdG9wIG9mZnNldD0iMCIgc3RvcC1jb2xvcj0iI2ZmZiIvPgogICAgICA8c3RvcCBvZmZzZXQ9Ii40IiBzdG9wLWNvbG9yPSIjMDljNGZmIi8+CiAgICAgIDxzdG9wIG9mZnNldD0iMSIgc3RvcC1jb2xvcj0iIzAwNzBjMCIvPgogICAgPC9saW5lYXJHcmFkaWVudD4KICAgIDxsaW5lYXJHcmFkaWVudCBpZD0ibGluZWFyLWdyYWRpZW50MiIgeDE9Ii0xNDg4Mi42IiB5MT0iNDk3NS40IiB4Mj0iLTE0ODgyLjYiIHkyPSI0OTkzLjYiIGdyYWRpZW50VHJhbnNmb3JtPSJ0cmFuc2xhdGUoLTE0ODU1LjIgLTQ5NTcuMikgcm90YXRlKC0xODApIHNjYWxlKDEgLTEpIiBncmFkaWVudFVuaXRzPSJ1c2VyU3BhY2VPblVzZSI+CiAgICAgIDxzdG9wIG9mZnNldD0iMCIgc3RvcC1jb2xvcj0iI2ZmZiIvPgogICAgICA8c3RvcCBvZmZzZXQ9Ii40IiBzdG9wLWNvbG9yPSIjMDljNGZmIi8+CiAgICAgIDxzdG9wIG9mZnNldD0iMSIgc3RvcC1jb2xvcj0iIzAwNzBjMCIvPgogICAgPC9saW5lYXJHcmFkaWVudD4KICAgIDxsaW5lYXJHcmFkaWVudCBpZD0ibGluZWFyLWdyYWRpZW50MyIgeDE9Ii05NzYyLjMiIHkxPSI5MDY0LjUiIHgyPSItOTc2Mi4zIiB5Mj0iOTA4Mi43IiBncmFkaWVudFRyYW5zZm9ybT0idHJhbnNsYXRlKC0xMzI4OC43IDUwNi43KSByb3RhdGUoMTM1KSBzY2FsZSgxIC0xKSIgZ3JhZGllbnRVbml0cz0idXNlclNwYWNlT25Vc2UiPgogICAgICA8c3RvcCBvZmZzZXQ9IjAiIHN0b3AtY29sb3I9IiNmZmYiLz4KICAgICAgPHN0b3Agb2Zmc2V0PSIuNCIgc3RvcC1jb2xvcj0iIzA5YzRmZiIvPgogICAgICA8c3RvcCBvZmZzZXQ9IjEiIHN0b3AtY29sb3I9IiMwMDcwYzAiLz4KICAgIDwvbGluZWFyR3JhZGllbnQ+CiAgICA8bGluZWFyR3JhZGllbnQgaWQ9ImxpbmVhci1ncmFkaWVudDQiIHgxPSItMzI0OS41IiB5MT0iODMzNS40IiB4Mj0iLTMyNDkuNSIgeTI9IjgzNTMuNiIgZ3JhZGllbnRUcmFuc2Zvcm09InRyYW5zbGF0ZSgtODMxNy4xIDMyNjIuMSkgcm90YXRlKDkwKSBzY2FsZSgxIC0xKSIgZ3JhZGllbnRVbml0cz0idXNlclNwYWNlT25Vc2UiPgogICAgICA8c3RvcCBvZmZzZXQ9IjAiIHN0b3AtY29sb3I9IiNmZmYiLz4KICAgICAgPHN0b3Agb2Zmc2V0PSIuNCIgc3RvcC1jb2xvcj0iIzA5YzRmZiIvPgogICAgICA8c3RvcCBvZmZzZXQ9IjEiIHN0b3AtY29sb3I9IiMwMDcwYzAiLz4KICAgIDwvbGluZWFyR3JhZGllbnQ+CiAgICA8bGluZWFyR3JhZGllbnQgaWQ9ImxpbmVhci1ncmFkaWVudDUiIHgxPSI4MzkuNCIgeTE9IjMyMTQuOCIgeDI9IjgzOS40IiB5Mj0iMzIzMyIgZ3JhZGllbnRUcmFuc2Zvcm09InRyYW5zbGF0ZSgtMjg1My4zIDE2OTUuNSkgcm90YXRlKDQ1KSBzY2FsZSgxIC0xKSIgZ3JhZGllbnRVbml0cz0idXNlclNwYWNlT25Vc2UiPgogICAgICA8c3RvcCBvZmZzZXQ9IjAiIHN0b3AtY29sb3I9IiNmZmYiLz4KICAgICAgPHN0b3Agb2Zmc2V0PSIuNCIgc3RvcC1jb2xvcj0iIzA5YzRmZiIvPgogICAgICA8c3RvcCBvZmZzZXQ9IjEiIHN0b3AtY29sb3I9IiMwMDcwYzAiLz4KICAgIDwvbGluZWFyR3JhZGllbnQ+CiAgICA8bGluZWFyR3JhZGllbnQgaWQ9ImxpbmVhci1ncmFkaWVudDYiIHgxPSIxMTAuNSIgeTE9Ii0zMjk3LjciIHgyPSIxMTAuNSIgeTI9Ii0zMjc5LjQiIGdyYWRpZW50VHJhbnNmb3JtPSJ0cmFuc2xhdGUoLTk3LjkgLTMyNzYpIHNjYWxlKDEgLTEpIiBncmFkaWVudFVuaXRzPSJ1c2VyU3BhY2VPblVzZSI+CiAgICAgIDxzdG9wIG9mZnNldD0iMCIgc3RvcC1jb2xvcj0iI2ZmZiIvPgogICAgICA8c3RvcCBvZmZzZXQ9Ii40IiBzdG9wLWNvbG9yPSIjMDljNGZmIi8+CiAgICAgIDxzdG9wIG9mZnNldD0iMSIgc3RvcC1jb2xvcj0iIzAwNzBjMCIvPgogICAgPC9saW5lYXJHcmFkaWVudD4KICAgIDxsaW5lYXJHcmFkaWVudCBpZD0ibGluZWFyLWdyYWRpZW50NyIgeDE9Ii01MDEwLjYiIHkxPSItNzM4Ni43IiB4Mj0iLTUwMTAuNiIgeTI9Ii03MzY4LjYiIGdyYWRpZW50VHJhbnNmb3JtPSJ0cmFuc2xhdGUoLTE2NjQuNSAtODczOS44KSByb3RhdGUoLTQ1KSBzY2FsZSgxIC0xKSIgZ3JhZGllbnRVbml0cz0idXNlclNwYWNlT25Vc2UiPgogICAgICA8c3RvcCBvZmZzZXQ9IjAiIHN0b3AtY29sb3I9IiNmZmYiLz4KICAgICAgPHN0b3Agb2Zmc2V0PSIuNCIgc3RvcC1jb2xvcj0iIzA5YzRmZiIvPgogICAgICA8c3RvcCBvZmZzZXQ9IjEiIHN0b3AtY29sb3I9IiMwMDcwYzAiLz4KICAgIDwvbGluZWFyR3JhZGllbnQ+CiAgICA8bGluZWFyR3JhZGllbnQgaWQ9ImxpbmVhci1ncmFkaWVudDgiIHgxPSIzMS4yIiB5MT0iLTIwNjgiIHgyPSIzMS4yIiB5Mj0iLTIwNTAuNiIgZ3JhZGllbnRUcmFuc2Zvcm09InRyYW5zbGF0ZSgwIC0yMDI4KSBzY2FsZSgxIC0xKSIgZ3JhZGllbnRVbml0cz0idXNlclNwYWNlT25Vc2UiPgogICAgICA8c3RvcCBvZmZzZXQ9IjAiIHN0b3AtY29sb3I9IiM0YTk5MjQiLz4KICAgICAgPHN0b3Agb2Zmc2V0PSIxIiBzdG9wLWNvbG9yPSIjODZkZTMzIi8+CiAgICA8L2xpbmVhckdyYWRpZW50PgogIDwvZGVmcz4KICA8cGF0aCBkPSJNMjEuNywyMS43YzAtLjQuNCwzLjctMS41LDYuOC0xLjgsMy4xLTQuMSw0LTcuNyw0LjgtMiwuNS0zLjIsMC00LjgtLjUtMi4yLS43LTMuMy0yLjUtMy43LTMuMXMtLjgtMS45LS41LTMuMmMuNS0yLjIsMi0zLjMsNC0zLjkuNCwwLDIuMi0uNSwzLjkuNHMyLjIsMS41LDMuNywxLjljMS41LjQsMy4xLDAsMy43LS40LDEuNy0uNiwyLjktMi40LDMuMS0zLjF2LjJoLS4yWiIgZmlsbD0idXJsKCNsaW5lYXItZ3JhZGllbnQpIi8+CiAgPHBhdGggZD0iTTIyLjQsMTkuOWMtLjMtLjMsMi45LDIuNCwzLjcsNS44cy4yLDUuOC0yLDguOWMtMS4yLDEuOC0yLjMsMi40LTMuOCwzLjItMi4xLDEtNC4yLjYtNC43LjVzLTEuOS0uNy0yLjYtMS45Yy0xLjEtMS45LTEtMy44LDAtNS42LjMtLjMsMS4xLTEuOSwzLTIuNSwxLjgtLjYsMi42LS41LDQtMS4zczIuMS0yLjEsMi40LTIuOWMuNy0xLjcuNC0zLjcsMC00LjNoLjItLjJaIiBmaWxsPSJ1cmwoI2xpbmVhci1ncmFkaWVudDEpIi8+CiAgPHBhdGggZD0iTTIxLjcsMTguM2MtLjQsMCwzLjctLjQsNi44LDEuNSwzLjEsMS44LDQuMyw0LDQuOCw3LjcuNCwyLDAsMy4yLS41LDQuOC0uNywyLjItMi41LDMuMy0zLjEsMy43cy0xLjkuOC0zLjIuNWMtMi4yLS41LTMuMy0yLTMuOS00LDAtLjQtLjUtMi4yLjQtMy45czEuNS0yLjIsMS45LTMuNywwLTMuMS0uNC0zLjdjLS42LTEuNy0yLjQtMi45LTMuMS0zLjFoLjJ2LjJoLjFaIiBmaWxsPSJ1cmwoI2xpbmVhci1ncmFkaWVudDIpIi8+CiAgPHBhdGggZD0iTTE5LjksMTcuNmMtLjMuMywyLjQtMi45LDUuOC0zLjdzNS44LS4yLDguOSwyYzEuOCwxLjIsMi40LDIuMywzLjIsMy44LDEsMi4xLjYsNC4yLjUsNC43cy0uNywxLjktMS45LDIuNmMtMS45LDEuMS0zLjgsMS01LjYsMC0uMy0uMy0xLjktMS4xLTIuNS0zcy0uNS0yLjYtMS4zLTQtMi4xLTIuMS0yLjktMi40Yy0xLjctLjctMy43LS40LTQuMywwaDB2LS4yaDB2LjJzLjEsMCwuMSwwWiIgZmlsbD0idXJsKCNsaW5lYXItZ3JhZGllbnQzKSIvPgogIDxwYXRoIGQ9Ik0xOC4zLDE4LjNjMCwuNC0uNC0zLjcsMS41LTYuOHM0LTQuMyw3LjctNC44YzItLjQsMy4yLDAsNC44LjUsMi4yLjcsMy4zLDIuNSwzLjcsMy4xcy44LDEuOS41LDMuMmMtLjUsMi4yLTIsMy4zLTQsMy45LS40LDAtMi4yLjUtMy45LS40cy0yLjItMS41LTMuNy0xLjktMy4xLDAtMy43LjRjLTEuNy42LTIuOSwyLjQtMy4xLDMuMXYtLjJoLjJaIiBmaWxsPSJ1cmwoI2xpbmVhci1ncmFkaWVudDQpIi8+CiAgPHBhdGggZD0iTTE3LjYsMTkuOWMuMy4zLTIuOS0yLjQtMy43LTUuOHMtLjItNS44LDItOC45YzEuMi0xLjgsMi4zLTIuNCwzLjgtMy4yLDIuMS0xLDQuMi0uNiw0LjctLjVzMS45LjcsMi42LDEuOWMxLjEsMS45LDEsMy44LDAsNS42LS4zLjMtMS4xLDEuOS0zLDIuNXMtMi42LjUtNCwxLjNjLTEuMy44LTIuMSwyLjEtMi40LDIuOS0uNywxLjctLjQsMy43LDAsNC4zaC0uMi4yLDBaIiBmaWxsPSJ1cmwoI2xpbmVhci1ncmFkaWVudDUpIi8+CiAgPHBhdGggZD0iTTE4LjMsMjEuN2MuNCwwLTMuNy40LTYuOC0xLjVzLTQuMy00LTQuOC03LjdjLS40LTIsMC0zLjIuNS00LjguNy0yLjIsMi41LTMuMywzLjEtMy43czEuOS0uOCwzLjItLjVjMi4yLjUsMy4zLDIsMy45LDQsMCwuNC41LDIuMi0uNCwzLjlzLTEuNSwyLjItMS45LDMuN2MtLjQsMS41LDAsMy4xLjQsMy43LjYsMS43LDIuNCwyLjksMy4xLDMuMWgtLjJ2LS4yaC0uMVoiIGZpbGw9InVybCgjbGluZWFyLWdyYWRpZW50NikiLz4KICA8cGF0aCBkPSJNMTkuOSwyMi40Yy4zLS4zLTIuNCwyLjktNS44LDMuN3MtNS44LjItOC45LTJjLTEuOC0xLjItMi40LTIuMy0zLjItMy44LTEtMi4xLS42LTQuMi0uNS00LjdzLjctMS45LDEuOS0yLjZjMS45LTEuMSwzLjgtMSw1LjYsMCwuMy4zLDEuOSwxLjEsMi41LDMsLjYsMS44LjUsMi42LDEuMyw0LC44LDEuMywyLjEsMi4xLDIuOSwyLjQsMS43LjcsMy43LjQsNC4zLDBoMHYuMmgwdi0uMnMtLjEsMC0uMSwwWiIgZmlsbD0idXJsKCNsaW5lYXItZ3JhZGllbnQ3KSIvPgogIDxyZWN0IHg9IjI4LjQiIHk9IjE4LjkiIHdpZHRoPSI2IiBoZWlnaHQ9IjE1LjkiIGZpbGw9IiNmZmYiLz4KICA8cGF0aCBkPSJNMzQuNywzMC4zdi05LjhjLjgsMCwuOC0uMy44LS44di0uOGMwLS40LS4yLS44LS43LS44aC02LjhjLS40LDAtLjguMy0uOC44di44YzAsLjguOC44LjguOHY5LjhsLTUuOCw4LjdjLS4zLjUtLjQsMS4yLDAsMS43cy45LjksMS41LjloMTUuMWMuNiwwLDEuMS0uMywxLjQtLjhzLjItLjYuMi0uOCwwLS41LS4zLS44bC01LjQtOC44aDBaTTI4LjQsMzMuMmwxLjYtMi41di0xMC4yaDIuNnYxMC4xbDEuNSwyLjZzLTUuNywwLTUuNywwWiIgZmlsbD0idXJsKCNsaW5lYXItZ3JhZGllbnQ4KSIgc3Ryb2tlPSIjZmZmIiBzdHJva2UtbWl0ZXJsaW1pdD0iMTAiIHN0cm9rZS13aWR0aD0iLjgiLz4KPC9zdmc+";
