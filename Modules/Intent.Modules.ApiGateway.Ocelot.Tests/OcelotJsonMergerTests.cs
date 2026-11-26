using System;
using System.Text.Json;
using Intent.Modules.ApiGateway.Ocelot.Templates.OcelotJson;
using Shouldly;
using Xunit;

namespace Intent.Modules.ApiGateway.Ocelot.Tests
{
    public class OcelotJsonMergerTests
    {
        private static string Merge(string? existing, string generated)
        {
            var existingDocument = existing is null ? null : JsoncParser.Parse(existing);
            var generatedRoot = (JsoncObject)JsoncParser.Parse(generated)!.Root;
            return JsoncWriter.Write(OcelotJsonMerger.Merge(existingDocument, generatedRoot)).Replace(Environment.NewLine, "\n");
        }

        private static string Route(string key, string upstream, string verb = "Get", string extra = "") =>
            $$"""{ "Key": "{{key}}", "UpstreamPathTemplate": "{{upstream}}", "UpstreamHttpMethod": ["{{verb}}"], "DownstreamPathTemplate": "{{upstream}}", "DownstreamHttpMethod": "{{verb}}", "DownstreamHostAndPorts": [{ "Host": "{Svc}" }]{{extra}} }""";

        private static string Generated(params string[] routes) =>
            "{ \"Routes\": [" + string.Join(",", routes) + "] }";

        private static JsoncObject Parse(string output) => (JsoncObject)JsoncParser.Parse(output)!.Root;

        private static JsoncArray Routes(string output) => (JsoncArray)Parse(output).Get("Routes")!;

        private static string?[] Keys(string output) =>
            Routes(output).Items.ConvertAll(i => ((JsoncObject)i.Value).GetString("Key")).ToArray();

        [Fact]
        public void Missing_file_yields_the_generated_content()
        {
            var output = Merge(null, Generated(Route("A", "/a")));

            Keys(output).ShouldBe(["A"]);
        }

        [Fact]
        public void File_with_only_comments_yields_the_generated_content()
        {
            Keys(Merge("// nothing\n", Generated(Route("A", "/a")))).ShouldBe(["A"]);
        }

        [Fact]
        public void Empty_file_observed_behaviour()
        {
            // The merger treats a null existing document as a missing file. What the parser does with "" is pinned in JsoncTests.
            var existing = JsoncParserOrNull("");
            Keys(JsoncWriter.Write(OcelotJsonMerger.Merge(existing, (JsoncObject)JsoncParser.Parse(Generated(Route("A", "/a")))!.Root)))
                .ShouldBe(["A"]);

            static JsoncDocument? JsoncParserOrNull(string text)
            {
                try { return JsoncParser.Parse(text); }
                catch (JsonException) { return null; }
            }
        }

        [Fact]
        public void Adds_a_modelled_route_the_file_lacks()
        {
            var output = Merge(Generated(Route("A", "/a")), Generated(Route("A", "/a"), Route("B", "/b")));

            Keys(output).ShouldBe(["A", "B"]);
        }

        [Fact]
        public void Updates_modelled_values_of_a_matched_route()
        {
            var existing = Generated(Route("Old", "/a"));
            var generated = Generated(Route("New", "/a", extra: ", \"Priority\": 2"));

            var route = (JsoncObject)Routes(Merge(existing, generated)).Items[0].Value;

            route.GetString("Key").ShouldBe("New");
            ((JsoncScalar)route.Get("Priority")!).RawText.ShouldBe("2");
        }

        [Fact]
        public void Removes_a_modelled_value_the_model_no_longer_has()
        {
            var existing = Generated(Route("A", "/a", extra: ", \"Priority\": 2"));

            var route = (JsoncObject)Routes(Merge(existing, Generated(Route("A", "/a")))).Items[0].Value;

            route.Find("Priority").ShouldBeNull();
        }

        [Fact]
        public void Matches_routes_case_insensitively_on_path_and_verb()
        {
            var existing = Generated(Route("A", "/Api/Things", extra: ", \"Custom\": 1"));
            var generated = Generated(Route("A", "/api/things", verb: "GET"));

            var routes = Routes(Merge(existing, generated));

            routes.Items.Count.ShouldBe(1);
            ((JsoncObject)routes.Items[0].Value).Find("Custom").ShouldNotBeNull();
        }

        [Fact]
        public void The_same_path_with_a_different_verb_is_a_different_route()
        {
            var output = Merge(Generated(Route("Get", "/a", "Get")), Generated(Route("Post", "/a", "Post")));

            Keys(output).ShouldBe(["Post"]);
        }

        [Fact]
        public void Keeps_hand_written_keys_and_comments_on_a_matched_route()
        {
            var existing = "{ \"Routes\": [\n  // my note\n  " +
                Route("A", "/a", extra: ", \"AllowedScopes\": [\"x\"]") + " // beside\n] }";

            var output = Merge(existing, Generated(Route("A", "/a")));

            output.ShouldContain("// my note");
            output.ShouldContain("\"AllowedScopes\"");
        }

        [Fact]
        public void Keeps_the_order_of_the_model()
        {
            var existing = Generated(Route("A", "/a"), Route("B", "/b"));
            var generated = Generated(Route("B", "/b"), Route("A", "/a"));

            Keys(Merge(existing, generated)).ShouldBe(["B", "A"]);
        }

        [Fact]
        public void Removes_an_unmodelled_route_without_IntentIgnore()
        {
            var existing = Generated(Route("A", "/a"), Route("Hand", "/hand"));

            Keys(Merge(existing, Generated(Route("A", "/a")))).ShouldBe(["A"]);
        }

        [Fact]
        public void Keeps_an_unmodelled_route_marked_IntentIgnore_with_its_comments()
        {
            var existing = "{ \"Routes\": [\n" + Route("A", "/a") + ",\n  // IntentIgnore\n  // keep me\n" +
                Route("Hand", "/hand") + ",\n" + Route("B", "/b") + "\n] }";

            var output = Merge(existing, Generated(Route("A", "/a"), Route("B", "/b")));

            Keys(output).ShouldBe(["A", "Hand", "B"]);
            output.ShouldContain("// IntentIgnore");
            output.ShouldContain("// keep me");
        }

        [Fact]
        public void Block_comment_IntentIgnore_is_honoured()
        {
            var existing = "{ \"Routes\": [\n  /* IntentIgnore */\n" + Route("Hand", "/hand") + "\n] }";

            // With no matched route to follow, the kept route goes first.
            Keys(Merge(existing, Generated(Route("A", "/a")))).ShouldBe(["Hand", "A"]);
        }

        [Fact]
        public void An_ignored_route_that_matches_a_modelled_one_is_left_as_written()
        {
            var existing = "{ \"Routes\": [\n  // IntentIgnore\n" + Route("Mine", "/a") + "\n] }";

            var output = Merge(existing, Generated(Route("Model", "/a")));

            Keys(output).ShouldBe(["Mine"]);
        }

        [Fact]
        public void An_ignored_route_stays_after_the_route_it_followed()
        {
            var existing = "{ \"Routes\": [\n" + Route("A", "/a") + ",\n  // IntentIgnore\n" + Route("Hand", "/hand") + "\n] }";

            Keys(Merge(existing, Generated(Route("B", "/b"), Route("A", "/a")))).ShouldBe(["B", "A", "Hand"]);
        }

        [Fact]
        public void Removes_all_routes_when_the_model_has_none()
        {
            var output = Merge(Generated(Route("A", "/a")), "{ \"Routes\": [] }");

            Routes(output).Items.ShouldBeEmpty();
        }

        [Fact]
        public void Keeps_trailing_comments_of_the_routes_array()
        {
            var existing = "{ \"Routes\": [\n" + Route("A", "/a") + "\n // after routes\n] }";

            Merge(existing, Generated(Route("A", "/a"))).ShouldContain("// after routes");
        }

        [Fact]
        public void Keeps_top_level_settings_the_model_does_not_write()
        {
            var existing = "// head\n{ \"Aggregates\": [], \"Custom\": { \"x\": 1 },\n // about routes\n \"Routes\": [] }";

            var output = Merge(existing, Generated(Route("A", "/a")));

            output.ShouldContain("\"Aggregates\"");
            output.ShouldContain("\"Custom\"");
            output.ShouldContain("// head");
            output.ShouldContain("// about routes");
            Keys(output).ShouldBe(["A"]);
        }

        [Fact]
        public void Keeps_hand_written_GlobalConfiguration_keys_and_updates_provider_keys()
        {
            var existing = """{ "Routes": [], "GlobalConfiguration": { "BaseUrl": "https://gw", "AuthenticationOptions": { "AuthenticationProviderKeys": ["Old"], "Other": 1 } } }""";
            var generated = """{ "Routes": [], "GlobalConfiguration": { "AuthenticationOptions": { "AuthenticationProviderKeys": ["Bearer"] } } }""";

            var global = (JsoncObject)Parse(Merge(existing, generated)).Get("GlobalConfiguration")!;

            global.GetString("BaseUrl").ShouldBe("https://gw");
            var auth = (JsoncObject)global.Get("AuthenticationOptions")!;
            auth.Find("Other").ShouldNotBeNull();
            ((JsoncScalar)((JsoncArray)auth.Get("AuthenticationProviderKeys")!).Items[0].Value).StringValue.ShouldBe("Bearer");
        }

        [Fact]
        public void Adds_GlobalConfiguration_when_the_file_lacks_it()
        {
            var generated = """{ "Routes": [], "GlobalConfiguration": { "BaseUrl": "x" } }""";

            Parse(Merge("""{ "Routes": [] }""", generated)).Find("GlobalConfiguration").ShouldNotBeNull();
        }

        [Fact]
        public void Keeps_hand_keys_but_removes_the_owned_auth_keys_when_the_model_writes_an_empty_GlobalConfiguration()
        {
            // The template always writes GlobalConfiguration, empty when nothing is secured or limited.
            var existing = """{ "Routes": [], "GlobalConfiguration": { "BaseUrl": "x", "AuthenticationOptions": { "AuthenticationProviderKeys": ["Bearer"] } } }""";

            var global = (JsoncObject)Parse(Merge(existing, """{ "Routes": [], "GlobalConfiguration": {} }""")).Get("GlobalConfiguration")!;

            global.Find("BaseUrl").ShouldNotBeNull();
            global.Find("AuthenticationOptions").ShouldBeNull();
        }

        [Fact]
        public void Observed_a_model_with_no_GlobalConfiguration_member_deletes_the_whole_hand_written_one()
        {
            // Not reachable from the template (it always writes GlobalConfiguration), but it is what the merger does with such input.
            var output = Merge("""{ "Routes": [], "GlobalConfiguration": { "BaseUrl": "x" } }""", """{ "Routes": [] }""");

            Parse(output).Find("GlobalConfiguration").ShouldBeNull();
        }

        [Fact]
        public void A_member_marked_IntentIgnore_is_left_alone()
        {
            var existing = "{ \"Routes\": [],\n  // IntentIgnore\n  \"GlobalConfiguration\": { \"Mine\": 1 } }";
            var generated = """{ "Routes": [], "GlobalConfiguration": { "BaseUrl": "x" } }""";

            var global = (JsoncObject)Parse(Merge(existing, generated)).Get("GlobalConfiguration")!;

            global.Find("Mine").ShouldNotBeNull();
            global.Find("BaseUrl").ShouldBeNull();
        }

        [Fact]
        public void Rate_limit_values_are_seeded_once_and_then_left_alone()
        {
            const string generated = """{ "Routes": [], "GlobalConfiguration": { "RateLimitOptions": { "EnableRateLimiting": true, "RouteKeys": ["A"], "Limit": 100, "Period": "1m" } } }""";
            const string existing = """{ "Routes": [], "GlobalConfiguration": { "RateLimitOptions": { "EnableRateLimiting": true, "RouteKeys": ["Old"], "Limit": 5 } } }""";

            var rule = (JsoncObject)((JsoncObject)Parse(Merge(existing, generated)).Get("GlobalConfiguration")!).Get("RateLimitOptions")!;

            ((JsoncScalar)rule.Get("Limit")!).RawText.ShouldBe("5");
            rule.GetString("Period").ShouldBe("1m");
            ((JsoncArray)rule.Get("RouteKeys")!).Items[0].Value.ShouldBeOfType<JsoncScalar>()
                .StringValue.ShouldBe("A");
        }

        [Fact]
        public void The_rate_limit_rule_goes_when_no_route_is_marked()
        {
            const string existing = """{ "Routes": [], "GlobalConfiguration": { "BaseUrl": "x", "RateLimitOptions": { "EnableRateLimiting": true, "RouteKeys": ["A"], "Limit": 5 } } }""";
            const string generated = """{ "Routes": [], "GlobalConfiguration": { "BaseUrl": "x" } }""";

            var global = (JsoncObject)Parse(Merge(existing, generated)).Get("GlobalConfiguration")!;

            global.Find("RateLimitOptions").ShouldBeNull();
        }

        [Fact]
        public void AllowAnonymous_follows_the_model_and_other_route_auth_options_are_kept()
        {
            var existing = Generated(Route("A", "/a", extra: ", \"AuthenticationOptions\": { \"AllowAnonymous\": true, \"AllowedScopes\": [\"s\"] }"));

            var withoutAnonymous = (JsoncObject)Routes(Merge(existing, Generated(Route("A", "/a")))).Items[0].Value;
            var auth = (JsoncObject)withoutAnonymous.Get("AuthenticationOptions")!;
            auth.Find("AllowAnonymous").ShouldBeNull();
            auth.Find("AllowedScopes").ShouldNotBeNull();

            var onlyAnonymous = Generated(Route("A", "/a", extra: ", \"AuthenticationOptions\": { \"AllowAnonymous\": true }"));
            var cleared = (JsoncObject)Routes(Merge(onlyAnonymous, Generated(Route("A", "/a")))).Items[0].Value;
            cleared.Find("AuthenticationOptions").ShouldBeNull();
        }

        [Fact]
        public void Merging_twice_is_stable()
        {
            var existing = "// head\n{ \"Routes\": [\n // note\n" + Route("A", "/a") + "\n], \"Other\": 1 }";
            var generated = Generated(Route("A", "/a"), Route("B", "/b"));

            var once = Merge(existing, generated);
            Merge(once, generated).ShouldBe(once);
        }

        // ---- Behaviour the review flagged as plausible bugs. These pin what the code does TODAY; they are not endorsements. ----

        [Theory]
        [InlineData("[]")]
        [InlineData("[1, 2]")]
        [InlineData("42")]
        [InlineData("\"text\"")]
        public void PlausibleBug_a_non_object_root_is_overwritten_silently_with_the_generated_content(string existing)
        {
            var output = Merge(existing, Generated(Route("A", "/a")));

            Keys(output).ShouldBe(["A"]);
            output.ShouldNotContain(existing + "\n");
        }

        [Fact]
        public void PlausibleBug_a_non_object_root_loses_its_comments_too()
        {
            var output = Merge("// precious\n[]\n// more", Generated(Route("A", "/a")));

            output.ShouldNotContain("precious");
            output.ShouldNotContain("more");
        }

        [Fact]
        public void A_non_object_Routes_value_is_replaced_by_the_generated_routes()
        {
            Keys(Merge("""{ "Routes": "nope" }""", Generated(Route("A", "/a")))).ShouldBe(["A"]);
        }

        [Fact]
        public void The_parser_rejects_invalid_JSON_rather_than_returning_a_document()
        {
            Should.Throw<JsonException>(() => JsoncParser.Parse("{ \"Routes\": [ "));
        }
    }
}
