function convertToJsonPatchCommand(element: MacroApi.Context.IElementApi) {
    let httpVerbProperty = element.getStereotype("Http Settings").getProperty("Verb");
    httpVerbProperty.setValue("PATCH");

    const originalCommandName = element.getName();
    const originalCommandNameToLower = originalCommandName.toLocaleLowerCase();

    let newCommandName = originalCommandName;
    if (originalCommandNameToLower.indexOf("update") > -1) {
        newCommandName = newCommandName.replace("Update", "Patch");
    } else if (originalCommandNameToLower.indexOf("edit") > -1) {
        newCommandName = newCommandName.replace("Edit", "Patch");
    } else {
        newCommandName = "Patch" + newCommandName;
    }

    element.setName(newCommandName, true);

    // Recursively scan all the DTOs starting from the Command's Fields.

    let commandFieldsAsDtos = element.getChildren("DTO-Field")
        .filter(x => x?.typeReference.getType()?.specialization === "DTO")
        .map(x => x.typeReference.getType()!);

    const dtos = new Set<MacroApi.Context.IElementApi>();
    const dtoStack = new Array<MacroApi.Context.IElementApi>(...commandFieldsAsDtos);

    while (dtoStack.length > 0) {
        const currentDto = dtoStack.pop()!;
        if (dtos.has(currentDto)) {
            continue;
        }
        dtos.add(currentDto);
        const childDtos = currentDto.getChildren("DTO-Field")
            .filter(x => x?.typeReference.getType()?.specialization === "DTO")
            .map(x => x.typeReference.getType()!);
        dtoStack.push(...childDtos);
    }

    
    for (let dto of dtos) {
        // Also update the DTO name if it contains Update or Edit
        const originalDtoName = dto.getName();
        const originalDtoNameToLower = originalDtoName.toLocaleLowerCase();
        let newDtoName = originalDtoName;
        if (originalDtoNameToLower.indexOf("update") > -1) {
            newDtoName = newDtoName.replace("Update", "Patch");
        } else if (originalDtoNameToLower.indexOf("edit") > -1) {
            newDtoName = newDtoName.replace("Edit", "Patch");
        } else {
            newDtoName = "Patch" + newDtoName;
        }
        dto.setName(newDtoName, true);
    }
}

function convertToJsonPatchServiceOperation(element: MacroApi.Context.IElementApi) {
    // element will be the Service Operation. we can extract the HTTP Verb information from it but for its DTOs we'll need to traverse
    // its parameters first.
    let httpVerbProperty = element.getStereotype("Http Settings").getProperty("Verb");
    httpVerbProperty.setValue("PATCH");

    const originalServiceOperationName = element.getName();
    const originalServiceOperationNameToLower = originalServiceOperationName.toLocaleLowerCase();
    let newServiceOperationName = originalServiceOperationName;
    if (originalServiceOperationNameToLower.indexOf("update") > -1) {
        newServiceOperationName = newServiceOperationName.replace("Update", "Patch");
    } else if (originalServiceOperationNameToLower.indexOf("edit") > -1) {
        newServiceOperationName = newServiceOperationName.replace("Edit", "Patch");
    } else {
        newServiceOperationName = "Patch" + newServiceOperationName;
    }
    element.setName(newServiceOperationName, true);

    // Recursively scan all the DTOs starting from the Service Operation's Parameters.

    let serviceOperationParametersAsDtos = element.getChildren("Parameter")
        .filter(x => x?.typeReference.getType()?.specialization === "DTO")
        .map(x => x.typeReference.getType()!);
    
    const dtos = new Set<MacroApi.Context.IElementApi>();
    const dtoStack = new Array<MacroApi.Context.IElementApi>(...serviceOperationParametersAsDtos);

    while (dtoStack.length > 0) {
        const currentDto = dtoStack.pop()!;
        if (dtos.has(currentDto)) {
            continue;
        }
        dtos.add(currentDto);
        const childDtos = currentDto.getChildren("DTO-Field")
            .filter(x => x?.typeReference.getType()?.specialization === "DTO")
            .map(x => x.typeReference.getType()!);
        dtoStack.push(...childDtos);
    }

    for (let dto of dtos) {
        // Also update the DTO name if it contains Update or Edit
        const originalDtoName = dto.getName();
        const originalDtoNameToLower = originalDtoName.toLocaleLowerCase();
        let newDtoName = originalDtoName;
        if (originalDtoNameToLower.indexOf("update") > -1) {
            newDtoName = newDtoName.replace("Update", "Patch");
        } else if (originalDtoNameToLower.indexOf("edit") > -1) {
            newDtoName = newDtoName.replace("Edit", "Patch");
        } else {
            newDtoName = "Patch" + newDtoName;
        }
        dto.setName(newDtoName, true);
    }
}