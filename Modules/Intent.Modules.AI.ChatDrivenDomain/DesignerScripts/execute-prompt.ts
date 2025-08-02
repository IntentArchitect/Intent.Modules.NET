async function executePrompt(element: MacroApi.Context.IElementApi) {
    // Open a dialog for the user to enter an AI prompt
    let promptResult = await dialogService.openForm({
        title: "Execute Prompt",
        fields: [
            {
                id: "prompt",
                fieldType: "textarea",
                label: "Enter AI prompt",
                hint: "NOTE: Intent Architect designer data will also be sent to the AI service provider."
            }
        ]
    });

    // Check if the user cancelled or provided an empty prompt
    if (!promptResult || promptResult.prompt.trim() === "") {
        return;
    }

    // Extract elements (classes and their attributes) - SEPARATE from associations
    const currentElements = lookupTypesOf("Class").map(clazz => ({
            id: clazz.id,
        specialization: clazz.specialization,
        specializationId: clazz.specializationId,
            name: clazz.getName(),
            comment: clazz.getComment(),
        value: clazz.getValue(),
        isAbstract: clazz.getIsAbstract(),
        isStatic: clazz.getIsStatic(),
        order: clazz.getOrder(),
        typeReference: null as any, // Classes don't have typeReference
        children: [
            // Extract attributes ONLY - associations are separate
            ...clazz.getChildren("Attribute")
                .filter(attr => !attr.hasMetadata("is-managed-key") && !attr.hasMetadata("set-by-infrastructure"))
                .map(attr => ({
                    id: attr.id,
                    specialization: attr.specialization,
                    specializationId: attr.specializationId,
                    name: attr.getName(),
                    comment: attr.getComment(),
                    value: attr.getValue(),
                    isAbstract: false,
                    isStatic: false,
                    order: attr.getOrder(),
                    typeReference: attr.typeReference ? {
                        typeId: attr.typeReference.getTypeId(),
                        isNavigable: attr.typeReference.isNavigable,
                        isNullable: attr.typeReference.isNullable,
                        isCollection: attr.typeReference.isCollection,
                        display: attr.typeReference.display
                    } : null,
                    children: [] as any[]
                }))
        ]
    }));

    // Extract associations separately from elements
    const currentAssociations = lookupTypesOf("Class").flatMap(clazz => 
        clazz.getAssociations()
            .filter(assoc => assoc.isSourceEnd()) // Only extract source ends to avoid duplicates
            .map(assoc => {
                const otherEnd = assoc.getOtherEnd();
                return {
                    id: assoc.id, // Association ID
                    sourceElementId: clazz.id,
                    targetElementId: assoc.typeReference.getTypeId(),
                    sourceEndName: assoc.getName(),
                    targetEndName: otherEnd.getName(),
                    sourceCardinality: assoc.typeReference.isCollection ? "*" : (assoc.typeReference.isNullable ? "0..1" : "1"),
                    targetCardinality: otherEnd.typeReference.isCollection ? "*" : (otherEnd.typeReference.isNullable ? "0..1" : "1"),
                    associationType: "Composite" // Default - AI can override this
                };
            })
    );

    // Prepare input for the AI module task with corrected separated structure
    const input = { 
        prompt: promptResult.prompt, 
        elements: currentElements,      // Elements only (classes + attributes)
        associations: currentAssociations, // Associations separately
        classes: [] as any[]            // Empty - using new structure only
    };

    // Execute the AI module task
    let outputStr;
    try {
        outputStr = await executeModuleTask("Intent.Modules.ChatDrivenDomain.Tasks.ChatCompletionTask", JSON.stringify(input));
    } catch (error) {
        dialogService.error(`Failed to execute AI task: ${error.message}`);
        return;
    }

    // Parse the result as ExecuteResult
    let executeResult: any;
    try {
        executeResult = JSON.parse(outputStr);
    } catch (error) {
        dialogService.error(`Failed to parse AI task result: ${error.message}`);
        return;
    }

    // Check for errors in the ExecuteResult
    if (executeResult.errors && executeResult.errors.length > 0) {
        const errorMessage = executeResult.errors.join('\n');
        dialogService.error(`AI task errors:\n${errorMessage}`);
        console.error(`AI task errors:\n${errorMessage}`);
        return;
    }

    // Extract the updated elements and associations from the result
    const aiResult = executeResult.result;
    if (!aiResult || (!aiResult.elements && !Array.isArray(aiResult))) {
        dialogService.error("AI task did not return a valid result structure.");
        console.error("AI task did not return a valid result structure.");
        return;
    }

    // Handle both new structure {elements, associations} and legacy array structure
    const updatedElements = aiResult.elements || aiResult;
    const updatedAssociations = aiResult.associations || [];
    
    if (!Array.isArray(updatedElements)) {
        dialogService.error("AI task did not return valid elements.");
        console.error("AI task did not return valid elements.");
        return;
    }

    // Optionally show warnings
    if (executeResult.warnings && executeResult.warnings.length > 0) {
        const warningMessage = executeResult.warnings.join('\n');
        dialogService.warn(`AI task warnings:\n${warningMessage}`);
        console.warn(`AI task warnings:\n${warningMessage}`);
    }

    // SYNC REQUIRED: AI tools work with in-memory model, must sync to Intent Architect
    if (updatedElements.length > 0 || updatedAssociations.length > 0) {
        try {
            await applyAiModelChanges(updatedElements, updatedAssociations, element);
            dialogService.info(`Domain model updated successfully: ${updatedElements.length} elements, ${updatedAssociations.length} associations synced.`);
        } catch (error) {
            dialogService.error(`Failed to sync model changes: ${error}`);
            console.error(`Failed to sync model changes: ${error}`);
        }
    } else {
        dialogService.info("AI task completed but no model changes were returned.");
    }
}

async function applyAiModelChanges(aiElements: any[], aiAssociations: any[], targetPackage: MacroApi.Context.IElementApi): Promise<void> {
    // Create ID mapping to track AI temporary IDs -> Intent Architect real IDs
    const idMapping = new Map<string, string>();
    
    console.log(`Starting sync: ${aiElements.length} elements, ${aiAssociations.length} associations`);
    
    // Phase 1: Sync elements (classes and their attributes)
    for (const aiElement of aiElements) {
        if (aiElement.specialization === "Class") {
            // Get fresh Intent Architect model state for each class (includes previously created ones)
            const currentClasses = lookupTypesOf("Class");
            await syncClass(aiElement, currentClasses, idMapping, targetPackage);
        }
    }
    
    // Phase 2: Sync associations (after all classes exist)
    if (aiAssociations.length > 0) {
        await syncAssociations(aiAssociations, idMapping);
    }
}

async function syncClass(aiClass: any, currentClasses: MacroApi.Context.IElementApi[], idMapping: Map<string, string>, targetPackage: MacroApi.Context.IElementApi): Promise<void> {
    // Find existing class by ID or create new one
    let intentClass = currentClasses.find(c => c.id === aiClass.id);
    
    console.log(`Syncing class ${aiClass.name} with ID ${aiClass.id}`);
    console.log(`Found existing class: ${intentClass ? 'YES' : 'NO'}`);
    
    if (!intentClass) {
        console.log(`Creating new class ${aiClass.name} in package ${targetPackage.id}`);
        // Create new class - use correct API: createElement(specialization, name, parentId)
        // Use the target package ID as the parent
        intentClass = createElement("Class", aiClass.name, targetPackage.id);
        intentClass.setComment(aiClass.comment || "");
        
        console.log(`Created class ${aiClass.name} with new ID ${intentClass.id}`);
        
        // Map AI ID to real Intent Architect ID (should be the same if AI tools worked correctly)
        idMapping.set(aiClass.id, intentClass.id);
    } else {
        console.log(`Updating existing class ${aiClass.name}`);
        // Update existing class
        if (intentClass.getName() !== aiClass.name) {
            intentClass.setName(aiClass.name, false);
        }
        if (intentClass.getComment() !== aiClass.comment) {
            intentClass.setComment(aiClass.comment || "");
        }
        
        // Ensure mapping exists for existing elements too
        idMapping.set(aiClass.id, intentClass.id);
    }
    
    // Sync attributes (only attributes, not associations)
    await syncAttributes(aiClass, intentClass, idMapping);
}

async function syncAttributes(aiClass: any, intentClass: MacroApi.Context.IElementApi, idMapping: Map<string, string>): Promise<void> {
    const currentAttributes = intentClass.getChildren("Attribute");
    
    // Find attributes in AI model
    const aiAttributes = aiClass.children?.filter((child: any) => child.specialization === "Attribute") || [];
    
    for (const aiAttr of aiAttributes) {
        let intentAttr = currentAttributes.find((attr: any) => attr.id === aiAttr.id);
        
        if (!intentAttr) {
            // Create new attribute using correct API: createElement(specialization, name, parentId)
            intentAttr = createElement("Attribute", aiAttr.name, intentClass.id);
            intentAttr.setComment(aiAttr.comment || "");
            
            // Map AI temporary ID to real Intent Architect ID
            idMapping.set(aiAttr.id, intentAttr.id);
            
            // Set type reference - use mapped ID if it's a custom type
            if (aiAttr.typeReference) {
                const typeRef = intentAttr.typeReference;
                // Check if typeId is mapped (for custom types created by AI)
                const mappedTypeId = idMapping.get(aiAttr.typeReference.typeId) || aiAttr.typeReference.typeId;
                typeRef.setType(mappedTypeId);
                typeRef.setIsNullable(aiAttr.typeReference.isNullable);
                typeRef.setIsCollection(aiAttr.typeReference.isCollection);
            }
        } else {
            // Update existing attribute
            if (intentAttr.getName() !== aiAttr.name) {
                intentAttr.setName(aiAttr.name, false);
            }
            if (intentAttr.getComment() !== aiAttr.comment) {
                intentAttr.setComment(aiAttr.comment || "");
            }
            
            // Ensure mapping exists for existing elements too
            idMapping.set(aiAttr.id, intentAttr.id);
            
            // Update type reference if changed
            if (aiAttr.typeReference) {
                const typeRef = intentAttr.typeReference;
                // Check if typeId is mapped (for custom types created by AI)
                const mappedTypeId = idMapping.get(aiAttr.typeReference.typeId) || aiAttr.typeReference.typeId;
                if (typeRef.getTypeId() !== mappedTypeId) {
                    typeRef.setType(mappedTypeId);
                }
                if (typeRef.isNullable !== aiAttr.typeReference.isNullable) {
                    typeRef.setIsNullable(aiAttr.typeReference.isNullable);
                }
                if (typeRef.isCollection !== aiAttr.typeReference.isCollection) {
                    typeRef.setIsCollection(aiAttr.typeReference.isCollection);
                }
            }
        }
    }
}

async function syncAssociations(aiAssociations: any[], idMapping: Map<string, string>): Promise<void> {
    console.log(`Syncing ${aiAssociations.length} associations`);
    
    for (const aiAssoc of aiAssociations) {
        console.log(`Syncing association: ${aiAssoc.sourceEndName} -> ${aiAssoc.targetEndName}`);
        
        // Map source and target element IDs
        const mappedSourceId = idMapping.get(aiAssoc.sourceElementId) || aiAssoc.sourceElementId;
        const mappedTargetId = idMapping.get(aiAssoc.targetElementId) || aiAssoc.targetElementId;
        
        console.log(`Mapped IDs: ${aiAssoc.sourceElementId} -> ${mappedSourceId}, ${aiAssoc.targetElementId} -> ${mappedTargetId}`);
        
        // Check if association already exists
        const sourceClass = lookupTypesOf("Class").find(c => c.id === mappedSourceId);
        if (!sourceClass) {
            console.error(`Source class with ID ${mappedSourceId} not found`);
            continue;
        }
        
        const targetClass = lookupTypesOf("Class").find(c => c.id === mappedTargetId);
        if (!targetClass) {
            console.error(`Target class with ID ${mappedTargetId} not found`);
            continue;
        }
        
        // Check if this association already exists
        const existingAssoc = sourceClass.getAssociations().find(assoc => 
            assoc.typeReference.getTypeId() === mappedTargetId && 
            assoc.getName() === aiAssoc.sourceEndName
        );
        
        if (!existingAssoc) {
            console.log(`Creating new association between ${sourceClass.getName()} and ${targetClass.getName()}`);
            
            // Create bidirectional association using global createAssociation
            const intentAssoc = createAssociation("Association", mappedSourceId, mappedTargetId);
            
            // Set source end name (this end)
            intentAssoc.setName(aiAssoc.sourceEndName);
            
            // Set source end cardinality
            const sourceTypeRef = intentAssoc.typeReference;
            const sourceCardinality = parseCardinality(aiAssoc.sourceCardinality);
            sourceTypeRef.setIsNullable(sourceCardinality.isNullable);
            sourceTypeRef.setIsCollection(sourceCardinality.isCollection);
            
            // Set target end name and cardinality
            const otherEnd = intentAssoc.getOtherEnd();
            if (otherEnd) {
                otherEnd.setName(aiAssoc.targetEndName);
                const targetTypeRef = otherEnd.typeReference;
                const targetCardinality = parseCardinality(aiAssoc.targetCardinality);
                targetTypeRef.setIsNullable(targetCardinality.isNullable);
                targetTypeRef.setIsCollection(targetCardinality.isCollection);
            }
            
            console.log(`Created association: ${aiAssoc.sourceEndName} (${aiAssoc.sourceCardinality}) -> ${aiAssoc.targetEndName} (${aiAssoc.targetCardinality})`);
        } else {
            console.log(`Association already exists: ${aiAssoc.sourceEndName} -> ${aiAssoc.targetEndName}`);
        }
    }
}

function parseCardinality(cardinality: string): { isNullable: boolean, isCollection: boolean } {
    switch (cardinality) {
        case "1": return { isNullable: false, isCollection: false };
        case "0..1": return { isNullable: true, isCollection: false };
        case "*": return { isNullable: false, isCollection: true };
        default: return { isNullable: false, isCollection: false };
    }
}