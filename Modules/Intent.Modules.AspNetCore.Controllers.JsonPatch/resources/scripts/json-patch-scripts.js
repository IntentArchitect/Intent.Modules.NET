const HTTP_SETTINGS_STEREOTYPE = "Http Settings";
const HTTP_VERB_PROPERTY = "Verb";
const PATCH_HTTP_VERB = "PATCH";
const DTO_FIELD_CHILD_TYPE = "DTO-Field";
const PARAMETER_CHILD_TYPE = "Parameter";
const DTO_SPECIALIZATION = "DTO";
const UPDATE_NAME_PART = "Update";
const EDIT_NAME_PART = "Edit";
const PATCH_NAME_PART = "Patch";
function convertToJsonPatchCommand(element) {
    setHttpVerbToPatch(element);
    renameElementToPatch(element);
    // Recursively scan all the DTOs starting from the Command's Fields.
    const commandFieldsAsDtos = getChildDtos(element, DTO_FIELD_CHILD_TYPE);
    const dtos = collectDtos(commandFieldsAsDtos);
    for (const dto of dtos) {
        renameElementToPatch(dto);
    }
}
function convertToJsonPatchServiceOperation(element) {
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
function setHttpVerbToPatch(element) {
    const httpVerbProperty = element.getStereotype(HTTP_SETTINGS_STEREOTYPE).getProperty(HTTP_VERB_PROPERTY);
    httpVerbProperty.setValue(PATCH_HTTP_VERB);
}
function getPatchName(originalName) {
    const originalNameToLower = originalName.toLocaleLowerCase();
    if (originalNameToLower.indexOf(UPDATE_NAME_PART.toLocaleLowerCase()) > -1) {
        return originalName.replace(UPDATE_NAME_PART, PATCH_NAME_PART);
    }
    if (originalNameToLower.indexOf(EDIT_NAME_PART.toLocaleLowerCase()) > -1) {
        return originalName.replace(EDIT_NAME_PART, PATCH_NAME_PART);
    }
    return PATCH_NAME_PART + originalName;
}
function renameElementToPatch(element) {
    const newName = getPatchName(element.getName());
    element.setName(newName, true);
}
function getChildDtos(element, childType) {
    return element.getChildren(childType)
        .filter(x => { var _a; return ((_a = x === null || x === void 0 ? void 0 : x.typeReference.getType()) === null || _a === void 0 ? void 0 : _a.specialization) === DTO_SPECIALIZATION; })
        .map(x => x.typeReference.getType());
}
function collectDtos(seedDtos) {
    const dtos = new Set();
    const dtoStack = new Array(...seedDtos);
    while (dtoStack.length > 0) {
        const currentDto = dtoStack.pop();
        if (dtos.has(currentDto)) {
            continue;
        }
        dtos.add(currentDto);
        const childDtos = getChildDtos(currentDto, DTO_FIELD_CHILD_TYPE);
        dtoStack.push(...childDtos);
    }
    return dtos;
}
