// Headless check of how an API-reference UI shows the QUERY operations, and whether its Try-it sends a real QUERY.
// Usage: node ui-check.js <swagger|scalar> <pageUrl> <screenshotPath>
// Needs playwright-core (installed under C:\Dev\.tools\hq-ui); drives the installed Microsoft Edge, headless.
const { chromium } = require(process.env.PLAYWRIGHT_CORE || "C:/Dev/.tools/hq-ui/node_modules/playwright-core");

(async () => {
    const [kind, url, shot] = process.argv.slice(2);
    const browser = await chromium.launch({ channel: "msedge", headless: true });
    const page = await browser.newPage({ viewport: { width: 1400, height: 1800 } });
    const sent = [];
    page.on("request", r => { if (/\/api\//.test(r.url())) sent.push(`${r.method()} ${new URL(r.url()).pathname}`); });
    // "swagger-cdn" and "scalar-cdn" serve a same-origin page (so the document fetch needs no CORS) that loads the
    // latest UI from a CDN; url must be "<origin>|<documentPath>".
    if (kind === "swagger-cdn" || kind === "scalar-cdn") {
        const [origin, docPath] = url.split("|");
        // Experiment: relabel the served document's "openapi" version (e.g. 3.2.0) without changing anything else.
        if (process.env.OPENAPI_VERSION_OVERRIDE) {
            await page.route(`${origin}${docPath}`, async route => {
                const response = await route.fetch();
                const json = await response.json();
                json.openapi = process.env.OPENAPI_VERSION_OVERRIDE;
                await route.fulfill({ response, json });
            });
        }
        const html = kind === "swagger-cdn"
            ? `<!doctype html><html><head><link rel="stylesheet" href="https://unpkg.com/swagger-ui-dist@${process.env.SWAGGER_UI_VERSION || "latest"}/swagger-ui.css"></head><body><div id="swagger-ui"></div>
            <script src="https://unpkg.com/swagger-ui-dist@${process.env.SWAGGER_UI_VERSION || "latest"}/swagger-ui-bundle.js"></script>
            <script>window.ui = SwaggerUIBundle({ url: "${docPath}", dom_id: "#swagger-ui" });</script></body></html>`
            : `<!doctype html><html><body><script id="api-reference" data-url="${docPath}"></script>
            <script src="https://cdn.jsdelivr.net/npm/@scalar/api-reference@${process.env.SCALAR_VERSION || "latest"}"></script></body></html>`;
        await page.route(`${origin}/ui-check.html`, route => route.fulfill({ contentType: "text/html", body: html }));
        await page.goto(`${origin}/ui-check.html`, { waitUntil: "networkidle" });
    } else {
        await page.goto(url, { waitUntil: "networkidle" });
    }
    await page.waitForTimeout(2500);

    const report = { kind, url };
    if (kind === "swagger" || kind === "swagger-cdn") {
        report.swaggerUiVersion = await page.evaluate(() => (window.versions && window.versions.swaggerUi && window.versions.swaggerUi.version) || null);
        report.methodLabels = await page.$$eval(".opblock .opblock-summary-method", els => els.map(e => e.textContent.trim()));
        report.paths = await page.$$eval(".opblock .opblock-summary-path", els => els.map(e => e.textContent.trim()));
        // The body-only operation: the route+body one needs a path value before Execute sends anything.
        const queryBlock = page.locator(".opblock", { has: page.locator(".opblock-summary-method", { hasText: /^QUERY$/i }) })
            .filter({ has: page.locator(".opblock-summary-path", { hasText: /^\/api\/products\/search$/ }) }).first();
        report.queryShown = await queryBlock.count() > 0;
        if (report.queryShown) {
            try {
                if (!(await queryBlock.locator(".try-out__btn").count())) {
                    await queryBlock.locator(".opblock-summary").click();
                }
                await queryBlock.locator(".try-out__btn").first().click({ timeout: 10000 });
                await queryBlock.locator(".execute").first().click({ timeout: 10000 });
                await page.waitForTimeout(2000);
            } catch (e) {
                report.tryItError = e.message.split("\n")[0];
            }
        }
    } else {
        const body = await page.locator("body").innerText();
        report.queryShown = /\bQUERY\b/i.test(body);
        report.searchPathShown = body.includes("/api/products/search");
        if (report.queryShown) {
            // Open the QUERY operation in Scalar's client and send it.
            const section = page.locator("section, div").filter({ hasText: /^QUERY\s*\/api\/products\/search/ }).last();
            const testButton = page.getByRole("button", { name: /Test Request/i }).last();
            await (await section.count() ? section.getByRole("button", { name: /Test Request/i }).first() : testButton).click().catch(() => { });
            await page.waitForTimeout(1500);
            await page.getByRole("button", { name: /^Send/i }).first().click().catch(() => { });
            await page.waitForTimeout(2000);
        }
    }
    report.requestsSent = sent;
    report.tryItSentQuery = sent.some(s => s.startsWith("QUERY "));
    await page.screenshot({ path: shot, fullPage: true });
    console.log(JSON.stringify(report, null, 2));
    await browser.close();
})().catch(e => { console.error(e); process.exit(1); });
