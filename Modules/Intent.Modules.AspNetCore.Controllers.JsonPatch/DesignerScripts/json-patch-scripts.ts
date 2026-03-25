const HTTP_SETTINGS_STEREOTYPE = "Http Settings";
const HTTP_VERB_PROPERTY = "Verb";
const PATCH_HTTP_VERB = "PATCH";
const DTO_FIELD_CHILD_TYPE = "DTO-Field";
const PARAMETER_CHILD_TYPE = "Parameter";
const DTO_SPECIALIZATION = "DTO";
const UPDATE_NAME_PART = "Update";
const EDIT_NAME_PART = "Edit";
const PATCH_NAME_PART = "Patch";

function convertToJsonPatchCommand(element: MacroApi.Context.IElementApi) {
    setHttpVerbToPatch(element);
    renameElementToPatch(element);

    // Recursively scan all the DTOs starting from the Command's Fields.
    const commandFieldsAsDtos = getChildDtos(element, DTO_FIELD_CHILD_TYPE);
    const dtos = collectDtos(commandFieldsAsDtos);
    for (const dto of dtos) {
        renameElementToPatch(dto);
    }
}

function convertToJsonPatchServiceOperation(element: MacroApi.Context.IElementApi) {
    // element will be the Service Operation. we can extract the HTTP Verb information from it but for its DTOs we'll need to traverse
    // its parameters first.
    setHttpVerbToPatch(element);
    renameElementToPatch(element);

    // Recursively scan all the DTOs starting from the Service Operation's Parameters.
    const serviceOperationParametersAsDtos = getChildDtos(element, PARAMETER_CHILD_TYPE);
    const dtos = collectDtos(serviceOperationParametersAsDtos);
    for (const dto of dtos) {
        renameElementToPatch(dto);
    }
}

function setHttpVerbToPatch(element: MacroApi.Context.IElementApi): void {
    const httpVerbProperty = element.getStereotype(HTTP_SETTINGS_STEREOTYPE).getProperty(HTTP_VERB_PROPERTY);
    httpVerbProperty.setValue(PATCH_HTTP_VERB);
}

function getPatchName(originalName: string): string {
    const originalNameToLower = originalName.toLocaleLowerCase();

    if (originalNameToLower.indexOf(UPDATE_NAME_PART.toLocaleLowerCase()) > -1) {
        return originalName.replace(UPDATE_NAME_PART, PATCH_NAME_PART);
    }

    if (originalNameToLower.indexOf(EDIT_NAME_PART.toLocaleLowerCase()) > -1) {
        return originalName.replace(EDIT_NAME_PART, PATCH_NAME_PART);
    }

    return PATCH_NAME_PART + originalName;
}

function renameElementToPatch(element: MacroApi.Context.IElementApi): void {
    const newName = getPatchName(element.getName());
    element.setName(newName, true);
}

function getChildDtos(element: MacroApi.Context.IElementApi, childType: string): MacroApi.Context.IElementApi[] {
    return element.getChildren(childType)
        .filter(x => x?.typeReference.getType()?.specialization === DTO_SPECIALIZATION)
        .map(x => x.typeReference.getType()!);
}

function collectDtos(seedDtos: MacroApi.Context.IElementApi[]): Set<MacroApi.Context.IElementApi> {
    const dtos = new Set<MacroApi.Context.IElementApi>();
    const dtoStack = new Array<MacroApi.Context.IElementApi>(...seedDtos);

    while (dtoStack.length > 0) {
        const currentDto = dtoStack.pop()!;
        if (dtos.has(currentDto)) {
            continue;
        }

        dtos.add(currentDto);
        const childDtos = getChildDtos(currentDto, DTO_FIELD_CHILD_TYPE);
        dtoStack.push(...childDtos);
    }

    return dtos;
}