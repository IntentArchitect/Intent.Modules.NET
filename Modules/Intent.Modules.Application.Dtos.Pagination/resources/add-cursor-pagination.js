/// <reference path="../../typings/elementmacro.context.api.d.ts" />
class PaginateHelper {
    static showCursorPaginateMenuItem(element) {
        var _a;
        var packages = lookupTypesOf("Domain Package");
        if (packages.length == 0) {
            return false;
        }
        var providers = lookupTypesOf("Document Db Provider");
        // if no providers could be found, it means the reference is not added or an older version of the module is installed
        // default to not showing the menu
        if (providers.length == 0) {
            return false;
        }
        // if there is only 1 package, we will try find the selected provider
        if (packages.length == 1) {
            return PaginateHelper.packageQualifiesForPagination(packages[0], providers, "Cursor Enabled");
        }
        // if more than one package, then need to look at the entity linked to the query
        let generalizations = element.getAssociations().filter(x => { var _a; return x.isTargetEnd() && ((_a = x === null || x === void 0 ? void 0 : x.typeReference.getType()) === null || _a === void 0 ? void 0 : _a.specialization) == "Class"; });
        if (generalizations.length == 0) {
            var anyEnabled = packages.some(domainPackage => PaginateHelper.packageQualifiesForPagination(domainPackage, providers, "Cursor Enabled"));
            return anyEnabled;
        }
        else {
            if (PaginateHelper.packageQualifiesForPagination((_a = generalizations[0]) === null || _a === void 0 ? void 0 : _a.typeReference.getType().getPackage(), providers, "Cursor Enabled") == false) {
                return false;
            }
            else {
                return true;
            }
        }
    }
    static showOffsetPaginateMenuItem(element) {
        var _a;
        const documentStoreId = "8b68020c-6652-484b-85e8-6c33e1d8031f";
        var packages = lookupTypesOf("Domain Package");
        if (packages.length == 0) {
            return false;
        }
        var providers = lookupTypesOf("Document Db Provider");
        // if no providers could be found, means only EF installed, and normal offset paging  should be shown
        if (providers.length == 0) {
            return true;
        }
        // if there is only 1 package, and its a document store, then try find the setting
        if (packages.length == 1 && packages[0].hasStereotype(documentStoreId)) {
            return PaginateHelper.packageQualifiesForPagination(packages[0], providers, "Offset Enabled");
        }
        // if more than one package, then need to look at the entity linked to the query
        let generalizations = element.getAssociations().filter(x => { var _a; return x.isTargetEnd() && ((_a = x === null || x === void 0 ? void 0 : x.typeReference.getType()) === null || _a === void 0 ? void 0 : _a.specialization) == "Class"; });
        if (generalizations.length == 0) {
            return true;
        }
        else {
            var linkedPackage = (_a = generalizations[0]) === null || _a === void 0 ? void 0 : _a.typeReference.getType().getPackage();
            // if not a document db, then show the menu item
            if (!linkedPackage.hasStereotype(documentStoreId)) {
                return true;
            }
            // else, actually check the values to see if it should be shown
            if (PaginateHelper.packageQualifiesForPagination(linkedPackage, providers, "Offset Enabled") == false) {
                return false;
            }
            else {
                return true;
            }
        }
    }
    // looks at the domain package and tries to determine based on a number of factors, if the menu option should be shown
    static packageQualifiesForPagination(domainPackage, providers, paginationPropertyName) {
        var _a, _b;
        const documentStoreId = "8b68020c-6652-484b-85e8-6c33e1d8031f";
        const paginationSupportId = "03385443-bbd6-46d9-87f2-6813f7833a38";
        // make sure its a documentDB store for now. This is the only one supported
        if (!domainPackage.hasStereotype(documentStoreId)) {
            return false;
        }
        // get the provider selected on the package
        var docDatabase = domainPackage.getStereotype(documentStoreId);
        var selectedProvider = docDatabase.getProperty("Provider");
        // if no provider selected, then take the first provider of type "Document Db Provider".
        var provider = null;
        if ((selectedProvider === null || selectedProvider === void 0 ? void 0 : selectedProvider.getValue()) == '' || (selectedProvider === null || selectedProvider === void 0 ? void 0 : selectedProvider.getValue()) == null) {
            provider = providers[0];
        }
        else if (providers.filter(p => p.id == (selectedProvider === null || selectedProvider === void 0 ? void 0 : selectedProvider.getValue())).length > 0) {
            // if we have a "Document Db Provider" for the selected value
            provider = providers.filter(p => p.id == (selectedProvider === null || selectedProvider === void 0 ? void 0 : selectedProvider.getValue()))[0];
        }
        if (provider == null) {
            return false;
        }
        if (!provider.hasStereotype(paginationSupportId)) {
            return false;
        }
        // if cursor is not enabled OR it is enabled AND offset is enabled
        if (((_a = provider.getStereotype(paginationSupportId).getProperty(paginationPropertyName)) === null || _a === void 0 ? void 0 : _a.getValue()) == null ||
            ((_b = provider.getStereotype(paginationSupportId).getProperty(paginationPropertyName)) === null || _b === void 0 ? void 0 : _b.getValue()) != "true") {
            return false;
        }
        return true;
    }
    static getCursorFiltering(element) {
        var _a;
        var packages = lookupTypesOf("Domain Package");
        if (packages.length == 0) {
            return new CursorFiltering(false, false);
        }
        var providers = lookupTypesOf("Document Db Provider");
        if (packages.length == 1) {
            return PaginateHelper.getPackageCursorFilter(packages[0], providers);
        }
        // if more than one package, then need to look at the entity linked to the query
        let generalizations = element.getAssociations().filter(x => { var _a; return x.isTargetEnd() && ((_a = x === null || x === void 0 ? void 0 : x.typeReference.getType()) === null || _a === void 0 ? void 0 : _a.specialization) == "Class"; });
        if (generalizations.length == 0) {
            return new CursorFiltering(false, false);
        }
        else {
            return PaginateHelper.getPackageCursorFilter((_a = generalizations[0]) === null || _a === void 0 ? void 0 : _a.typeReference.getType().getPackage(), providers);
        }
    }
    static getPackageCursorFilter(domainPackage, providers) {
        var _a;
        const documentStoreId = "8b68020c-6652-484b-85e8-6c33e1d8031f";
        const paginationSupportId = "03385443-bbd6-46d9-87f2-6813f7833a38";
        // make sure its a documentDB store for now. This is the only one supported
        if (!domainPackage.hasStereotype(documentStoreId)) {
            return new CursorFiltering(false, false);
        }
        // get the provider selected on the package
        var docDatabase = domainPackage.getStereotype(documentStoreId);
        var selectedProvider = docDatabase.getProperty("Provider");
        // if no provider selected, then take the first provider of type "Document Db Provider".
        var provider = null;
        if ((selectedProvider === null || selectedProvider === void 0 ? void 0 : selectedProvider.getValue()) == '' || (selectedProvider === null || selectedProvider === void 0 ? void 0 : selectedProvider.getValue()) == null) {
            provider = providers[0];
        }
        else if (providers.filter(p => p.id == (selectedProvider === null || selectedProvider === void 0 ? void 0 : selectedProvider.getValue())).length > 0) {
            // if we have a "Document Db Provider" for the selected value
            provider = providers.filter(p => p.id == (selectedProvider === null || selectedProvider === void 0 ? void 0 : selectedProvider.getValue()))[0];
        }
        if (provider == null) {
            return new CursorFiltering(false, false);
        }
        if (!provider.hasStereotype(paginationSupportId)) {
            return new CursorFiltering(false, false);
        }
        var cursorFiltering = (_a = provider.getStereotype(paginationSupportId).getProperty("Cursor Filtering")) === null || _a === void 0 ? void 0 : _a.getValue();
        if (cursorFiltering == "Disabled") {
            return new CursorFiltering(false, false);
        }
        if (cursorFiltering == "Enabled (Nullable)") {
            return new CursorFiltering(true, true);
        }
        if (cursorFiltering == "Enabled (Not Nullable)") {
            return new CursorFiltering(true, false);
        }
        return new CursorFiltering(false, false);
    }
}
class CursorFiltering {
    constructor(enabled, nullable) {
        this.enabled = enabled;
        this.nullable = nullable;
    }
}
/// <reference path="../../typings/elementmacro.context.api.d.ts" />
function getSurrogateKeyType() {
    var _a, _b, _c;
    const commonTypes = {
        guid: "6b649125-18ea-48fd-a6ba-0bfff0d8f488",
        long: "33013006-E404-48C2-AC46-24EF5A5774FD",
        int: "fb0a362d-e9e2-40de-b6ff-5ce8167cbe74"
    };
    const javaTypes = {
        long: "e9e575eb-f8de-4ce4-9838-2d09665a752d",
        int: "b3e5cb3b-8a26-4346-810b-9789afa25a82"
    };
    const typeNameToIdMap = new Map();
    typeNameToIdMap.set("guid", commonTypes.guid);
    typeNameToIdMap.set("int", lookup(javaTypes.int) != null ? javaTypes.int : commonTypes.int);
    typeNameToIdMap.set("long", lookup(javaTypes.long) != null ? javaTypes.long : commonTypes.long);
    let typeName = (_c = (_b = (_a = application.getSettings("ac0a788e-d8b3-4eea-b56d-538608f1ded9")) === null || _a === void 0 ? void 0 : _a.getField("Key Type")) === null || _b === void 0 ? void 0 : _b.value) !== null && _c !== void 0 ? _c : "int";
    if (typeNameToIdMap.has(typeName)) {
        return typeNameToIdMap.get(typeName);
    }
    return typeNameToIdMap.get("guid");
}
;
/// <reference path="getSurrogateKeyType.ts"/>
/// <reference path="attributeWithMapPath.ts"/>
class DomainHelper {
    static isAggregateRoot(element) {
        let result = !element.getAssociations("Association")
            .some(x => x.isSourceEnd() && !x.typeReference.isCollection && !x.typeReference.isNullable);
        return result;
    }
    static getCommandOperations(entity) {
        const queryOperationNames = ["Get", "Find", "Filter", "Query", "Is", "Must", "Can"];
        const operations = entity.getChildren("Operation").filter(operation => operation.typeReference.getType() == null ||
            !queryOperationNames.some(allowedOperationName => operation.getName().startsWith(allowedOperationName)));
        return operations;
    }
    static isComplexType(element) {
        return (element === null || element === void 0 ? void 0 : element.specialization) === "Data Contract" ||
            (element === null || element === void 0 ? void 0 : element.specialization) === "Value Object" ||
            (element === null || element === void 0 ? void 0 : element.specialization) === "Class";
    }
    static isComplexTypeById(typeId) {
        let element = lookup(typeId);
        return DomainHelper.isComplexType(element);
    }
    static getOwningAggregate(entity) {
        var _a;
        if (!entity || entity.specialization != "Class") {
            return null;
        }
        let invalidAssociations = entity.getAssociations("Association").filter(x => x.typeReference.getType() == null);
        if (invalidAssociations.length > 0) {
            console.warn("Invalid associations found:");
            invalidAssociations.forEach(x => {
                console.warn("Invalid associations: " + x.getName());
            });
        }
        let result = (_a = entity.getAssociations("Association")
            .filter(x => this.isAggregateRoot(x.typeReference.getType()) && isOwnedBy(x) &&
            // Let's only target collections for now as part of the nested compositional crud support
            // as one-to-one relationships are more expensive to address and possibly not going to
            // be needed.
            x.getOtherEnd().typeReference.isCollection)[0]) === null || _a === void 0 ? void 0 : _a.typeReference.getType();
        return result;
        function isOwnedBy(association) {
            return association.isSourceEnd() &&
                !association.typeReference.isNullable &&
                !association.typeReference.isCollection;
        }
    }
    static ownerIsAggregateRoot(entity) {
        let result = DomainHelper.getOwningAggregate(entity);
        return result ? true : false;
    }
    static hasPrimaryKey(entity) {
        let keys = entity.getChildren("Attribute").filter(x => x.hasStereotype("Primary Key"));
        return keys.length > 0;
    }
    static getPrimaryKeys(entity) {
        if (!entity) {
            throw new Error("entity not specified");
        }
        let primaryKeys = DomainHelper.getPrimaryKeysMap(entity);
        return Object.values(primaryKeys);
    }
    static isUserSuppliedPrimaryKey(pk) {
        if (pk == null)
            return false;
        if (!pk.hasStereotype("Primary Key"))
            return false;
        var pkStereotype = pk.getStereotype("Primary Key");
        if (!pkStereotype.hasProperty("Data source")) {
            return false;
        }
        return pkStereotype.getProperty("Data source").value == "User supplied";
    }
    static getPrimaryKeysMap(entity) {
        let keydict = Object.create(null);
        let keys = entity.getChildren("Attribute").filter(x => x.hasStereotype("Primary Key"));
        keys.forEach(key => keydict[key.id] = {
            id: key.id,
            name: key.getName(),
            typeId: key.typeReference.typeId,
            mapPath: [key.id],
            isNullable: false,
            isCollection: false
        });
        traverseInheritanceHierarchyForPrimaryKeys(keydict, entity, []);
        return keydict;
        function traverseInheritanceHierarchyForPrimaryKeys(keydict, curEntity, generalizationStack) {
            if (!curEntity) {
                return;
            }
            let generalizations = curEntity.getAssociations("Generalization").filter(x => x.isTargetEnd());
            if (generalizations.length == 0) {
                return;
            }
            let generalization = generalizations[0];
            generalizationStack.push(generalization.id);
            let nextEntity = generalization.typeReference.getType();
            let baseKeys = nextEntity.getChildren("Attribute").filter(x => x.hasStereotype("Primary Key"));
            baseKeys.forEach(key => {
                keydict[key.id] = {
                    id: key.id,
                    name: key.getName(),
                    typeId: key.typeReference.typeId,
                    mapPath: generalizationStack.concat([key.id]),
                    isNullable: key.typeReference.isNullable,
                    isCollection: key.typeReference.isCollection
                };
            });
            traverseInheritanceHierarchyForPrimaryKeys(keydict, nextEntity, generalizationStack);
        }
    }
    static getForeignKeys(entity, owningAggregate) {
        var _a;
        if (!entity) {
            throw new Error("entity not specified");
        }
        if (!owningAggregate) {
            throw new Error("nestedCompOwner not specified");
        }
        // Use the new Associated property on the FK stereotype method for FK Attribute lookup
        let foreignKeys = [];
        for (let attr of entity.getChildren("Attribute").filter(x => x.hasStereotype("Foreign Key"))) {
            let associationId = (_a = attr.getStereotype("Foreign Key").getProperty("Association")) === null || _a === void 0 ? void 0 : _a.getValue();
            if (owningAggregate.getAssociations("Association").some(x => x.id == associationId)) {
                foreignKeys.push(attr);
            }
        }
        // Backward compatible lookup method
        if (foreignKeys.length == 0) {
            let foundFk = entity.getChildren("Attribute")
                .filter(x => x.getName().toLowerCase().indexOf(owningAggregate.getName().toLowerCase()) >= 0 && x.hasStereotype("Foreign Key"))[0];
            if (foundFk) {
                foreignKeys.push(foundFk);
            }
        }
        return foreignKeys.map(x => ({
            name: DomainHelper.getAttributeNameFormat(x.getName()),
            typeId: x.typeReference.typeId,
            id: x.id,
            mapPath: [x.id],
            isCollection: x.typeReference.isCollection,
            isNullable: x.typeReference.isNullable,
            element: x
        }));
    }
    /**
     * Returns true if the attribute is a foreign key on a compositional one-to-many relationship (i.e. is managed by the DB and should not be set).
     * @param attribute
     * @returns
     */
    static isManagedForeignKey(attribute) {
        var _a, _b;
        let fkAssociation = (_b = (_a = attribute.getStereotype("Foreign Key")) === null || _a === void 0 ? void 0 : _a.getProperty("Association")) === null || _b === void 0 ? void 0 : _b.getSelected();
        return fkAssociation != null && !fkAssociation.getOtherEnd().typeReference.getIsCollection() && !fkAssociation.getOtherEnd().typeReference.getIsNullable();
    }
    static getChildrenOfType(entity, type) {
        let attrDict = Object.create(null);
        let attributes = entity.getChildren(type);
        attributes.forEach(attr => attrDict[attr.id] = {
            id: attr.id,
            name: attr.getName(),
            typeId: attr.typeReference.typeId,
            mapPath: [attr.id],
            isNullable: attr.typeReference.isNullable,
            isCollection: attr.typeReference.isCollection
        });
        return Object.values(attrDict);
    }
    static getAttributesWithMapPath(entity) {
        let attrDict = Object.create(null);
        let attributes = entity
            .getChildren("Attribute")
            .filter(x => {
            var _a;
            return !x.hasStereotype("Primary Key") &&
                !DomainHelper.legacyPartitionKey(x) &&
                (x["hasMetadata"] && (!x.hasMetadata("set-by-infrastructure") || ((_a = x.getMetadata("set-by-infrastructure")) === null || _a === void 0 ? void 0 : _a.toLocaleLowerCase()) != "true"));
        });
        attributes.forEach(attr => attrDict[attr.id] = {
            id: attr.id,
            name: attr.getName(),
            typeId: attr.typeReference.typeId,
            mapPath: [attr.id],
            isNullable: false,
            isCollection: false
        });
        traverseInheritanceHierarchyForAttributes(attrDict, entity, []);
        return Object.values(attrDict);
        function traverseInheritanceHierarchyForAttributes(attrDict, curEntity, generalizationStack) {
            if (!curEntity) {
                return;
            }
            let generalizations = curEntity.getAssociations("Generalization").filter(x => x.isTargetEnd());
            if (generalizations.length == 0) {
                return;
            }
            let generalization = generalizations[0];
            generalizationStack.push(generalization.id);
            let nextEntity = generalization.typeReference.getType();
            let baseKeys = nextEntity.getChildren("Attribute").filter(x => !x.hasStereotype("Primary Key") && !DomainHelper.legacyPartitionKey(x));
            baseKeys.forEach(attr => {
                attrDict[attr.id] = {
                    id: attr.id,
                    name: attr.getName(),
                    typeId: attr.typeReference.typeId,
                    mapPath: generalizationStack.concat([attr.id]),
                    isNullable: attr.typeReference.isNullable,
                    isCollection: attr.typeReference.isCollection
                };
            });
            traverseInheritanceHierarchyForAttributes(attrDict, nextEntity, generalizationStack);
        }
    }
    static getMandatoryAssociationsWithMapPath(entity) {
        return traverseInheritanceHierarchy(entity, [], []);
        function traverseInheritanceHierarchy(entity, results, generalizationStack) {
            entity
                .getAssociations("Association")
                .filter(x => !x.typeReference.isCollection && !x.typeReference.isNullable && x.typeReference.isNavigable &&
                !x.getOtherEnd().typeReference.isCollection && !x.getOtherEnd().typeReference.isNullable)
                .forEach(association => {
                return results.push({
                    id: association.id,
                    name: association.getName(),
                    typeId: null,
                    mapPath: generalizationStack.concat([association.id]),
                    isNullable: false,
                    isCollection: false
                });
            });
            let generalizations = entity.getAssociations("Generalization").filter(x => x.isTargetEnd());
            if (generalizations.length == 0) {
                return results;
            }
            let generalization = generalizations[0];
            generalizationStack.push(generalization.id);
            return traverseInheritanceHierarchy(generalization.typeReference.getType(), results, generalizationStack);
        }
    }
    static getAttributeNameFormat(str) {
        let convention = DomainHelper.getDomainAttributeNamingConvention();
        switch (convention) {
            case "pascal-case":
                return toPascalCase(str);
            case "camel-case":
                return toCamelCase(str);
        }
        return str;
    }
    static getDomainAttributeNamingConvention() {
        var _a, _b, _c;
        const domainSettingsId = "c4d1e35c-7c0d-4926-afe0-18f17563ce17";
        return (_c = (_b = (_a = application.getSettings(domainSettingsId)) === null || _a === void 0 ? void 0 : _a.getField("Attribute Naming Convention")) === null || _b === void 0 ? void 0 : _b.value) !== null && _c !== void 0 ? _c : "pascal-case";
    }
    static getSurrogateKeyType() {
        return getSurrogateKeyType();
    }
    // Just in case someone still uses this convention. Used to filter out those attributes when mapping
    // to domain entities that are within a Cosmos DB paradigm.
    static legacyPartitionKey(attribute) {
        return attribute.hasStereotype("Partition Key") && attribute.getName() === "PartitionKey";
    }
    static requiresForeignKey(associationEnd) {
        return DomainHelper.isManyToVariantsOfOne(associationEnd) || DomainHelper.isSelfReferencingZeroToOne(associationEnd);
    }
    static isManyToVariantsOfOne(associationEnd) {
        return !associationEnd.typeReference.isCollection && associationEnd.getOtherEnd().typeReference.isCollection;
    }
    static isSelfReferencingZeroToOne(associationEnd) {
        return !associationEnd.typeReference.isCollection && associationEnd.typeReference.isNullable &&
            associationEnd.typeReference.typeId == associationEnd.getOtherEnd().typeReference.typeId;
    }
    static getOwningAggregateRecursive(entity) {
        let owners = DomainHelper.getOwnersRecursive(entity);
        if (owners.length == 0)
            return null;
        const uniqueIds = new Set(owners.map(item => item.id));
        if (uniqueIds.size !== 1) {
            throw new Error(`Entity : '${entity.getName()}' has more than 1 owner.`);
        }
        return owners[0];
    }
    static getOwnersRecursive(entity) {
        if (!entity || entity.specialization != "Class") {
            return null;
        }
        let results = entity.getAssociations("Association").filter(x => DomainHelper.isOwnedByAssociation(x));
        let result = [];
        for (let i = 0; i < results.length; i++) {
            let owner = results[i].typeReference.getType();
            if (DomainHelper.isAggregateRoot(owner)) {
                result.push(owner);
            }
            else {
                result.push(...DomainHelper.getOwnersRecursive(owner));
            }
        }
        return result;
    }
    static isOwnedByAssociation(association) {
        return association.isSourceEnd() &&
            !association.typeReference.isNullable &&
            !association.typeReference.isCollection;
    }
    static getOwningAggregateKeyChain(entity) {
        if (!entity || entity.specialization != "Class") {
            return null;
        }
        let results = entity.getAssociations("Association").filter(x => DomainHelper.isOwnedByAssociation(x));
        let result = [];
        if (results.length == 0)
            return result;
        let owner = results[0].typeReference.getType();
        let pks = DomainHelper.getPrimaryKeys(owner);
        pks.forEach(pk => {
            let attribute = lookup(pk.id);
            //expectedName would typically be CountryId if you have a Agg: Country with a Pk: Id
            let expectedName = attribute.getParent().getName();
            if (!attribute.getName().startsWith(expectedName)) {
                expectedName += attribute.getName();
            }
            else {
                expectedName = attribute.getName();
            }
            result.push({ attribute: attribute, expectedName: expectedName });
        });
        if (!DomainHelper.isAggregateRoot(owner)) {
            result.unshift(...DomainHelper.getOwningAggregateKeyChain(owner));
        }
        return result;
    }
}
/// <reference path="../common/domainHelper.ts" />
class CrudConstants {
}
CrudConstants.mapFromDomainMappingSettingId = "1f747d14-681c-4a20-8c68-34223f41b825";
CrudConstants.mapToDomainConstructorForDtosSettingId = "8d1f6a8a-77c8-43a2-8e60-421559725419";
CrudConstants.dtoFromEntityMappingId = "1f747d14-681c-4a20-8c68-34223f41b825";
class CrudHelper {
    // Super basic selection dialog.
    static async openBasicSelectEntityDialog(options) {
        let classes = lookupTypesOf("Class").filter(x => CrudHelper.filterClassSelection(x, options));
        if (classes.length == 0) {
            await dialogService.info("No Domain types could be found. Please ensure that you have a reference to the Domain package and that at least one class exists in it.");
            return null;
        }
        let classId = await dialogService.lookupFromOptions(classes.map((x) => ({
            id: x.id,
            name: getFriendlyDisplayNameForClassSelection(x),
            additionalInfo: `(${x.getParents().map(item => item.getName()).join("/")})`
        })));
        if (classId == null) {
            await dialogService.error(`No class found with id "${classId}".`);
            return null;
        }
        let foundEntity = lookup(classId);
        return foundEntity;
        function getFriendlyDisplayNameForClassSelection(element) {
            let aggregateEntity = DomainHelper.getOwningAggregate(element);
            return !aggregateEntity ? element.getName() : `${element.getName()} (${aggregateEntity.getName()})`;
        }
    }
    static async openCrudCreationDialog(options) {
        var _a, _b;
        let classes = lookupTypesOf("Class").filter(x => CrudHelper.filterClassSelection(x, options));
        if (classes.length == 0) {
            await dialogService.info("No Domain types could be found. Please ensure that you have a reference to the Domain package and that at least one class exists in it.");
            return null;
        }
        let dialogResult = await dialogService.openForm({
            title: "CRUD Creation Options",
            fields: [
                {
                    id: "entityId",
                    fieldType: "select",
                    label: "Entity for CRUD operations",
                    selectOptions: classes.map(x => {
                        return {
                            id: x.id,
                            description: x.getName(),
                            additionalInfo: getClassAdditionalInfo(x)
                        };
                    }),
                    isRequired: true
                },
                {
                    id: "create",
                    fieldType: "checkbox",
                    label: "Create",
                    value: "true",
                    hint: "Generate the \"Create\" operation"
                },
                {
                    id: "update",
                    fieldType: "checkbox",
                    label: "Update",
                    value: "true",
                    hint: "Generate the \"Update\" operation"
                },
                {
                    id: "queryById",
                    fieldType: "checkbox",
                    label: "Query By Id",
                    value: "true",
                    hint: "Generate the \"Query By Id\" operation"
                },
                {
                    id: "queryAll",
                    fieldType: "checkbox",
                    label: "Query All",
                    value: "true",
                    hint: "Generate the \"Query All\" operation"
                },
                {
                    id: "delete",
                    fieldType: "checkbox",
                    label: "Delete",
                    value: "true",
                    hint: "Generate the \"Delete\" operation"
                },
                {
                    id: "domain",
                    fieldType: "checkbox",
                    label: "Domain Operations",
                    value: "true",
                    hint: "Generate operations for Domain Entity operations"
                }
            ]
        });
        let foundEntity = lookup(dialogResult.entityId);
        var result = {
            selectedEntity: foundEntity,
            diagramId: dialogResult.diagramId,
            canCreate: dialogResult.create == "true",
            canUpdate: dialogResult.update == "true",
            canQueryById: dialogResult.queryById == "true",
            canQueryAll: dialogResult.queryAll == "true",
            canDelete: dialogResult.delete == "true",
            canDomain: dialogResult.domain == "true",
            selectedDomainOperationIds: []
        };
        if (result.canDomain && foundEntity.getChildren("Operation").length > 0) {
            dialogResult = await dialogService.openForm({
                title: "Select Domain Operations",
                fields: [
                    {
                        id: "tree",
                        fieldType: "tree-view",
                        label: "Domain Operations",
                        hint: "Generate operations from selected domain entity operations",
                        treeViewOptions: {
                            rootId: foundEntity.id,
                            submitFormTriggers: ["double-click", "enter"],
                            isMultiSelect: true,
                            selectableTypes: [
                                {
                                    specializationId: "Class",
                                    autoExpand: true,
                                    autoSelectChildren: false,
                                    isSelectable: (x) => false
                                },
                                {
                                    specializationId: "Operation",
                                    isSelectable: (x) => true
                                }
                            ]
                        }
                    }
                ]
            });
            result.selectedDomainOperationIds = (_b = (_a = dialogResult.tree) === null || _a === void 0 ? void 0 : _a.filter((x) => x != "0")) !== null && _b !== void 0 ? _b : [];
        }
        return result;
        function getClassAdditionalInfo(element) {
            let aggregateEntity = DomainHelper.getOwningAggregate(element);
            let prefix = aggregateEntity ? `: ${aggregateEntity.getName()}  ` : "";
            return `${prefix}(${element.getParents().map(item => item.getName()).join("/")})`;
        }
    }
    static filterClassSelection(element, options) {
        var _a;
        if (!((_a = options === null || options === void 0 ? void 0 : options.allowAbstract) !== null && _a !== void 0 ? _a : false) && element.getIsAbstract()) {
            return false;
        }
        if (element.hasStereotype("Repository")) {
            return true;
        }
        if ((options === null || options === void 0 ? void 0 : options.includeOwnedRelationships) != false && DomainHelper.ownerIsAggregateRoot(element)) {
            return DomainHelper.hasPrimaryKey(element);
        }
        if (DomainHelper.isAggregateRoot(element)) {
            let generalizations = element.getAssociations("Generalization").filter(x => x.isTargetEnd());
            if (generalizations.length == 0) {
                return true;
            }
            let generalization = generalizations[0];
            let parentEntity = generalization.typeReference.getType();
            //Could propagate options here but then we need to update compositional crud to support inheritance and it's already a bit of a hack
            return CrudHelper.filterClassSelection(parentEntity, { includeOwnedRelationships: false, allowAbstract: true });
        }
        return false;
    }
    static getName(command, mappedElement, dtoPrefix = null) {
        if (mappedElement.typeReference != null)
            mappedElement = mappedElement.typeReference.getType();
        let originalVerb = (command.getName().split(/(?=[A-Z])/))[0];
        let domainName = mappedElement.getName();
        let baseName = command.getMetadata("baseName")
            ? `${command.getMetadata("baseName")}${domainName}`
            : domainName;
        let dtoName = `${originalVerb}${baseName}`;
        if (dtoPrefix)
            dtoName = `${dtoPrefix}${dtoName}`;
        return dtoName;
    }
    static getOrCreateCrudDto(dtoName, mappedElement, autoAddPrimaryKey, mappingTypeSettingId, folder, inbound = false) {
        let dto = CrudHelper.getOrCreateDto(dtoName, folder);
        //dtoField.typeReference.setType(dto.id);
        const entityCtor = mappedElement
            .getChildren("Class Constructor")
            .sort((a, b) => {
            // In descending order:
            return b.getChildren("Parameter").length - a.getChildren("Parameter").length;
        })[0];
        if (inbound && entityCtor != null) {
            dto.setMapping([mappedElement.id, entityCtor.id], CrudConstants.mapToDomainConstructorForDtosSettingId);
            CrudHelper.addDtoFieldsForCtor(autoAddPrimaryKey, entityCtor, dto, folder);
        }
        else {
            dto.setMapping(mappedElement.id, mappingTypeSettingId);
            CrudHelper.addDtoFields(autoAddPrimaryKey, mappedElement, dto, folder);
        }
        return dto;
    }
    static getOrCreateDto(elementName, parentElement) {
        const expectedDtoName = elementName.replace(/Dto$/, "") + "Dto";
        let existingDto = parentElement.getChildren("DTO").filter(x => x.getName() === expectedDtoName)[0];
        if (existingDto) {
            return existingDto;
        }
        let dto = createElement("DTO", expectedDtoName, parentElement.id);
        return dto;
    }
    static addDtoFieldsForCtor(autoAddPrimaryKey, ctor, dto, folder) {
        let childrenToAdd = DomainHelper.getChildrenOfType(ctor, "Parameter").filter(x => x.typeId != null && lookup(x.typeId).specialization !== "Domain Service");
        childrenToAdd.forEach(e => {
            if (e.mapPath != null) {
                if (dto.getChildren("Parameter").some(x => { var _a, _b; return ((_b = (_a = x.getMapping()) === null || _a === void 0 ? void 0 : _a.getElement()) === null || _b === void 0 ? void 0 : _b.id) == e.id; })) {
                    return;
                }
            }
            else if (ctor.getChildren("Parameter").some(x => x.getName().toLowerCase() === e.name.toLowerCase())) {
                return;
            }
            let field = createElement("DTO-Field", toPascalCase(e.name), dto.id);
            field.setMapping(e.mapPath);
            if (DomainHelper.isComplexTypeById(e.typeId)) {
                let dtoName = dto.getName().replace(/Dto$/, "") + field.getName() + "Dto";
                let newDto = CrudHelper.getOrCreateCrudDto(dtoName, field.getMapping().getElement().typeReference.getType(), autoAddPrimaryKey, CrudConstants.mapFromDomainMappingSettingId, folder, true);
                field.typeReference.setType(newDto.id);
            }
            else {
                field.typeReference.setType(e.typeId);
            }
            field.typeReference.setIsCollection(e.isCollection);
            field.typeReference.setIsNullable(e.isNullable);
        });
        dto.collapse();
    }
    static addDtoFields(autoAddPrimaryKey, mappedElement, dto, folder) {
        var _a, _b;
        let dtoUpdated = false;
        let domainElement = mappedElement;
        let attributesWithMapPaths = CrudHelper.getAttributesWithMapPath(domainElement);
        let isCreateMode = ((_b = (_a = dto.getMetadata("originalVerb")) === null || _a === void 0 ? void 0 : _a.toLowerCase()) === null || _b === void 0 ? void 0 : _b.startsWith("create")) == true;
        for (var keyName of Object.keys(attributesWithMapPaths)) {
            let entry = attributesWithMapPaths[keyName];
            if (isCreateMode && CrudHelper.isOwnerForeignKey(entry.name, domainElement)) {
                continue;
            }
            if (dto.getChildren("DTO-Field").some(x => x.getName() == entry.name)) {
                continue;
            }
            let field = createElement("DTO-Field", entry.name, dto.id);
            field.setMapping(entry.mapPath);
            if (DomainHelper.isComplexTypeById(entry.typeId)) {
                let dtoName = dto.getName().replace(/Dto$/, "") + field.getName() + "Dto";
                let newDto = CrudHelper.getOrCreateCrudDto(dtoName, field.getMapping().getElement().typeReference.getType(), autoAddPrimaryKey, CrudConstants.mapFromDomainMappingSettingId, folder, true);
                field.typeReference.setType(newDto.id);
            }
            else {
                field.typeReference.setType(entry.typeId);
            }
            field.typeReference.setIsNullable(entry.isNullable);
            field.typeReference.setIsCollection(entry.isCollection);
            dtoUpdated = true;
        }
        if (autoAddPrimaryKey && !isCreateMode) {
            CrudHelper.addPrimaryKeys(dto, domainElement, true);
        }
        if (dtoUpdated) {
            dto.collapse();
        }
    }
    static isOwnerForeignKey(attributeName, domainElement) {
        for (let association of domainElement.getAssociations().filter(x => x.isSourceEnd() && !x.typeReference.isCollection && !x.typeReference.isNullable)) {
            if (attributeName.toLowerCase().indexOf(association.getName().toLowerCase()) >= 0) {
                return true;
            }
        }
        return false;
    }
    static addPrimaryKeys(dto, entity, map) {
        const primaryKeys = CrudHelper.getPrimaryKeysWithMapPath(entity);
        for (const primaryKey of primaryKeys) {
            const name = CrudHelper.getDomainAttributeNameFormat(primaryKey.name);
            if (dto.getChildren("DTO-Field").some(x => x.getName().toLowerCase() == name.toLowerCase())) {
                continue;
            }
            const dtoField = createElement("DTO-Field", CrudHelper.getFieldFormat(name), dto.id);
            dtoField.typeReference.setType(primaryKey.typeId);
            if (map && primaryKey.mapPath != null) {
                console.log(`Doing mapping for ${dtoField.id}`);
                dtoField.setMapping(primaryKey.mapPath);
            }
        }
    }
    static getPrimaryKeysWithMapPath(entity) {
        let keydict = Object.create(null);
        let keys = entity.getChildren("Attribute").filter(x => x.hasStereotype("Primary Key"));
        keys.forEach(key => keydict[key.id] = {
            id: key.id,
            name: key.getName(),
            typeId: key.typeReference.typeId,
            mapPath: [key.id],
            isNullable: false,
            isCollection: false
        });
        traverseInheritanceHierarchyForPrimaryKeys(keydict, entity, []);
        return Object.values(keydict);
        function traverseInheritanceHierarchyForPrimaryKeys(keydict, curEntity, generalizationStack) {
            if (!curEntity) {
                return;
            }
            let generalizations = curEntity.getAssociations("Generalization").filter(x => x.isTargetEnd());
            if (generalizations.length == 0) {
                return;
            }
            let generalization = generalizations[0];
            generalizationStack.push(generalization.id);
            let nextEntity = generalization.typeReference.getType();
            let baseKeys = nextEntity.getChildren("Attribute").filter(x => x.hasStereotype("Primary Key"));
            baseKeys.forEach(key => {
                keydict[key.id] = {
                    id: key.id,
                    name: key.getName(),
                    typeId: key.typeReference.typeId,
                    mapPath: generalizationStack.concat([key.id]),
                    isNullable: key.typeReference.isNullable,
                    isCollection: key.typeReference.isCollection
                };
            });
            traverseInheritanceHierarchyForPrimaryKeys(keydict, nextEntity, generalizationStack);
        }
    }
    static getAttributesWithMapPath(entity) {
        let attrDict = Object.create(null);
        let attributes = entity.getChildren("Attribute")
            .filter(x => {
            var _a;
            return !x.hasStereotype("Primary Key") &&
                !DomainHelper.isManagedForeignKey(x) && // essentially also an attribute set by infrastructure
                !CrudHelper.legacyPartitionKey(x) &&
                (x["hasMetadata"] && (!x.hasMetadata("set-by-infrastructure") || ((_a = x.getMetadata("set-by-infrastructure")) === null || _a === void 0 ? void 0 : _a.toLocaleLowerCase()) != "true"));
        });
        attributes.forEach(attr => attrDict[attr.id] = {
            id: attr.id,
            name: attr.getName(),
            typeId: attr.typeReference.typeId,
            mapPath: [attr.id],
            isNullable: false,
            isCollection: false
        });
        traverseInheritanceHierarchyForAttributes(attrDict, entity, []);
        return attrDict;
        function traverseInheritanceHierarchyForAttributes(attrDict, curEntity, generalizationStack) {
            if (!curEntity) {
                return;
            }
            let generalizations = curEntity.getAssociations("Generalization").filter(x => x.isTargetEnd());
            if (generalizations.length == 0) {
                return;
            }
            let generalization = generalizations[0];
            generalizationStack.push(generalization.id);
            let nextEntity = generalization.typeReference.getType();
            let baseKeys = nextEntity.getChildren("Attribute").filter(x => !x.hasStereotype("Primary Key") && !CrudHelper.legacyPartitionKey(x));
            baseKeys.forEach(attr => {
                attrDict[attr.id] = {
                    id: attr.id,
                    name: attr.getName(),
                    typeId: attr.typeReference.typeId,
                    mapPath: generalizationStack.concat([attr.id]),
                    isNullable: attr.typeReference.isNullable,
                    isCollection: attr.typeReference.isCollection
                };
            });
            traverseInheritanceHierarchyForAttributes(attrDict, nextEntity, generalizationStack);
        }
    }
    static getFieldFormat(str) {
        return toPascalCase(str);
    }
    static getDomainAttributeNameFormat(str) {
        let convention = CrudHelper.getDomainAttributeNamingConvention();
        switch (convention) {
            case "pascal-case":
                return toPascalCase(str);
            case "camel-case":
                return toCamelCase(str);
            default:
                return str;
        }
    }
    static getDomainAttributeNamingConvention() {
        var _a, _b, _c;
        const domainSettingsId = "c4d1e35c-7c0d-4926-afe0-18f17563ce17";
        return (_c = (_b = (_a = application.getSettings(domainSettingsId)) === null || _a === void 0 ? void 0 : _a.getField("Attribute Naming Convention")) === null || _b === void 0 ? void 0 : _b.value) !== null && _c !== void 0 ? _c : "pascal-case";
    }
    // Just in case someone still uses this convention. Used to filter out those attributes when mapping
    // to domain entities that are within a Cosmos DB paradigm.
    static legacyPartitionKey(attribute) {
        return attribute.hasStereotype("Partition Key") && attribute.getName() === "PartitionKey";
    }
}
/// <reference path="../../../typings/elementmacro.context.api.d.ts" />
/// <reference path="../../common/paginateHelper.ts" />
/// <reference path="../../common/crudHelper.ts" />
class CursorPaginateApi {
    static changeReturnType(element) {
        const pagedResultType = "2a11c92d-d27f-4faa-b6fb-c33b93a6ff12";
        let currentReturnType = element.typeReference.typeId;
        element.typeReference.setType(pagedResultType, [{ typeId: currentReturnType, isCollection: false, isNullable: false }]);
        element.typeReference.setIsCollection(false);
        element.typeReference.setIsNullable(false);
    }
    static addPagingParameters(element, childElementType) {
        const commonTypes = {
            guid: "6b649125-18ea-48fd-a6ba-0bfff0d8f488",
            long: "33013006-E404-48C2-AC46-24EF5A5774FD",
            int: "fb0a362d-e9e2-40de-b6ff-5ce8167cbe74",
            string: "d384db9c-a279-45e1-801e-e4e8099625f2"
        };
        var filterSettings = PaginateHelper.getCursorFiltering(element);
        if (filterSettings.enabled && !element.getChildren(childElementType).find(x => x.getName().toLowerCase() == "partitionKey")) {
            let partKey = createElement(childElementType, "PartitionKey", element.id);
            partKey.typeReference.setType(commonTypes.string);
            partKey.typeReference.setIsNullable(filterSettings.nullable);
            let queryAssociation = element.getAssociations().find(x => { var _a; return x.isTargetEnd() && ((_a = x === null || x === void 0 ? void 0 : x.typeReference.getType()) === null || _a === void 0 ? void 0 : _a.specialization) == "Class"; });
            if (queryAssociation != null) {
                const entity = queryAssociation.typeReference.getType();
                const entityPartKey = entity.getChildren("Attribute").find(c => c.getName() == "PartitionKey");
                if (entityPartKey != null) {
                    const mapping = queryAssociation.createAdvancedMapping(element.id, entity.id);
                    mapping.addMappedEnd("01d09a7f-0e7c-4670-b7bc-395d7e893ef2", [partKey.id], [entityPartKey.id]);
                }
            }
        }
        if (!element.getChildren(childElementType).find(x => x.getName().toLowerCase() == "pageSize")) {
            let pageSize = createElement(childElementType, "PageSize", element.id);
            pageSize.typeReference.setType(commonTypes.int);
        }
        if (!element.getChildren(childElementType).find(x => x.getName().toLowerCase() == "cursorToken")) {
            let cursorToken = createElement(childElementType, "CursorToken", element.id);
            cursorToken.typeReference.setType(commonTypes.string);
            cursorToken.typeReference.setIsNullable(true);
        }
    }
    static addServicePagination(element) {
        CursorPaginateApi.addPagingParameters(element, "Parameter");
        CursorPaginateApi.changeReturnType(element);
    }
    static addCqrsPagination(element) {
        CursorPaginateApi.addPagingParameters(element, "DTO-Field");
        CursorPaginateApi.changeReturnType(element);
    }
}
