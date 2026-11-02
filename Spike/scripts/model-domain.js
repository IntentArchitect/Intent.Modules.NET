// Shared Domain model for every HttpQuery sample app. Run once per app via run_designer_script includedScriptPaths.
const pkg = getPackages().find(p => p.getName().endsWith(".Domain"));
if (!pkg) throw new Error("No *.Domain package");

let folder = pkg.getChildren("Folder").find(f => f.getName() === "Products");
if (folder) throw new Error("Products folder already exists - model already applied");
folder = createElementUnder(pkg, "Folder", "Products");

const product = createElementUnder(folder, "Class", "Product");
const attr = (name, type) => { const a = product.addChild("Attribute", name); a.setType(type); return a; };
attr("Name", "string");
attr("Category", "string");
attr("Price", "decimal");

return { product: product.getName(), attributes: product.getChildren("Attribute").map(a => a.getName()) };
