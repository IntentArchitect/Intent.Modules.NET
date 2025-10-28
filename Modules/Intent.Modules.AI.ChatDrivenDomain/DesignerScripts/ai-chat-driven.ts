async function execute(element: MacroApi.Context.IElementApi): Promise<void> {
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
            const isSource = assoc.isSourceEnd() //assoc.getParent().id === clazz.id;
            const sourceEnd = isSource ? assoc : assoc.getOtherEnd();

            if (isSource && !sourceEnd.typeReference.isNavigable) { return null; }

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

    const { providerId, modelId, thinkingLevel: thinkingLevel } = await collectAndPersistAiSettingsFromPromptResult(
        promptResult, providerModelsResult, settingName);

    const input = { prompt: promptResult.prompt, classes: currentClasses, providerId: providerId, modelId: modelId, thinkingLevel: thinkingLevel };

    // Execute the AI module task
    let outputStr;
    try {
        outputStr = await executeModuleTask("Intent.Modules.ChatDrivenDomain.Tasks.ChatCompletionTask", JSON.stringify(input));
    } catch (error) {
        dialogService.error(`Failed to execute AI task: ${error.message}`);
        return;
    }

    // Parse the result from the AI task
    let updatedClasses: any;
    try {
        updatedClasses = JSON.parse(outputStr);
    } catch (error) {
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
    const updatedClassesMap = new Map(updatedClasses.map((jClass: any) => [jClass.id, jClass]));

    // Process class updates and additions
    updatedClasses.forEach((jClass: any) => {
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
            jClass.attributes.forEach((jAttr: any) => {
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
    updatedClasses.forEach((jClass: any) => {
        // Process all associations for this class
        jClass.associations.forEach((jAssoc: any) => {
            const thisClassId = jClass.id;
            const otherClassId = jAssoc.classId;

            const thisClass = existingClassesMap.get(thisClassId);
            const otherClass = existingClassesMap.get(otherClassId);

            if (!thisClass || !otherClass) return;

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