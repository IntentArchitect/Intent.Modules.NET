// Shared Services model for every HttpQuery sample app. Run once per app via run_designer_script includedScriptPaths.
// QUERY can't be selected as an Http Settings verb yet, so the QUERY endpoints are modelled as POST
// (body-bound, 200 OK) and the generated endpoint is hand-edited to QUERY.
const pkg = getPackages().find(p => p.getName().endsWith(".Services"));
const product = lookupByName("Product", "Class");
if (!pkg || !product) throw new Error(`Missing - pkg: ${!!pkg}, Product: ${!!product}`);
if (pkg.getChildren("Folder").find(f => f.getName() === "Products")) throw new Error("Products folder already exists - model already applied");

const folder = createElementUnder(pkg, "Folder", "Products");
const field = (owner, name, type, o = {}) => {
    const f = owner.addChild("DTO-Field", name);
    f.setType({ name: type, isNullable: !!o.nullable, isCollection: !!o.collection });
    if (o.source) f.ensureStereotype("Parameter Settings").setProperty("Source", o.source);
    return f;
};
const http = (el, verb, route, code) => {
    const h = el.ensureStereotype("Http Settings");
    h.setProperty("Verb", verb);
    h.setProperty("Route", route);
    if (code) h.setProperty("Success Response Code", code);
};

const dto = createElementUnder(folder, "DTO", "ProductDto");
dto.setBasicMapping(product.id);
for (const n of ["Id", "Name", "Category", "Price"]) {
    const f = dto.addChild("DTO-Field", n);
    const target = n === "Id"
        ? product.getChildren("Attribute").find(c => c.getName() === "Id")
        : product.getChildren("Attribute").find(c => c.getName() === n);
    if (!target) throw new Error(`Product.${n} not found`);
    f.setBasicMapping(target.id);
}

const create = createElementUnder(folder, "Command", "CreateProductCommand");
field(create, "Name", "string");
field(create, "Category", "string");
field(create, "Price", "decimal");
create.setType("guid");
http(create, "POST", "api/products", "201 (Created)");
createAssociation("Create Entity Action", create.id, product.id);

const getById = createElementUnder(folder, "Query", "GetProductByIdQuery");
field(getById, "Id", "guid", { source: "From Route" });
getById.setType("ProductDto");
http(getById, "GET", "api/products/{id}");
createAssociation("Query Entity Action", getById.id, product.id);

// QUERY endpoint, body only. Modelled as POST + 200 until Http Settings supports QUERY.
const search = createElementUnder(folder, "Query", "SearchProductsQuery");
search.setComment("HTTP QUERY endpoint (modelled as POST until Http Settings supports QUERY). Filters products by the criteria in the request body; every criterion is optional.");
field(search, "NameContains", "string", { nullable: true });
field(search, "Categories", "string", { nullable: true, collection: true });
field(search, "MinPrice", "decimal", { nullable: true });
field(search, "MaxPrice", "decimal", { nullable: true });
search.setType({ name: "ProductDto", isCollection: true });
http(search, "POST", "api/products/search", "200 (Ok)");
createAssociation("Query Entity Action", search.id, product.id).setMultiplicity("0..*");

// QUERY endpoint, route parameter + body.
const byCategory = createElementUnder(folder, "Query", "SearchCategoryProductsQuery");
byCategory.setComment("HTTP QUERY endpoint (modelled as POST until Http Settings supports QUERY). Category comes from the route; the remaining criteria from the request body.");
field(byCategory, "Category", "string", { source: "From Route" });
field(byCategory, "NameContains", "string", { nullable: true });
field(byCategory, "MaxPrice", "decimal", { nullable: true });
byCategory.setType({ name: "ProductDto", isCollection: true });
http(byCategory, "POST", "api/categories/{category}/products/search", "200 (Ok)");
createAssociation("Query Entity Action", byCategory.id, product.id).setMultiplicity("0..*");

return folder.getChildren().map(c => c.getName());
