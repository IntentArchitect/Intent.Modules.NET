using System;
using System.Collections.Generic;
using System.Linq;

namespace Intent.Modules.ApiGateway.Ocelot.Templates.OcelotJson
{
    /// <summary>
    /// Merges the <c>ocelot.json</c> the template generates into the one on disk, so what was written there by hand survives
    /// regeneration. The template only owns what it writes:
    /// <list type="bullet">
    /// <item>Each route's <c>Key</c>, paths, verbs and <c>DownstreamHostAndPorts</c>, plus <c>Priority</c> and
    /// <c>AuthenticationOptions.AllowAnonymous</c>, which it writes or removes as the model requires.</item>
    /// <item><c>GlobalConfiguration.AuthenticationOptions.AuthenticationProviderKeys</c>, and the shared rate limit rule's
    /// <c>EnableRateLimiting</c> and <c>RouteKeys</c>. The rule's other settings are seeded once and then left alone.</item>
    /// </list>
    /// Everything else, including comments, is kept. A member marked <c>// IntentIgnore</c> is left exactly as it is.
    /// </summary>
    internal static class OcelotJsonMerger
    {
        private const string Routes = "Routes";
        private const string GlobalConfiguration = "GlobalConfiguration";
        private const string AuthenticationOptions = "AuthenticationOptions";
        private const string RateLimitOptions = "RateLimitOptions";

        /// <summary>Route keys the template writes from the model. Any other key on a route is a hand edit and is kept.</summary>
        private static readonly string[] RouteKeysFromModel =
            [
            "Key", "UpstreamPathTemplate", "UpstreamHttpMethod", "DownstreamPathTemplate", "DownstreamHttpMethod", "DownstreamHostAndPorts", "Priority"
            ];

        /// <summary>Shared rate limit rule keys the template writes from the model.</summary>
        private static readonly string[] RateLimitKeysFromModel = ["EnableRateLimiting", "RouteKeys"];

        public static JsoncDocument Merge(JsoncDocument? existing, JsoncObject generated)
        {
            if (existing?.Root is not JsoncObject existingRoot)
            {
                return new JsoncDocument(generated);
            }

            MergeMember(existingRoot, generated, Routes, (existingRoutes, generatedRoutes) =>
                existingRoutes is JsoncArray existingArray && generatedRoutes is JsoncArray generatedArray
                ? MergeRoutes(existingArray, generatedArray)
                : generatedRoutes);

            MergeMember(existingRoot, generated, GlobalConfiguration, (existingGlobal, generatedGlobal) =>
                existingGlobal is JsoncObject existingObject && generatedGlobal is JsoncObject generatedObject
                ? MergeGlobalConfiguration(existingObject, generatedObject)
                : generatedGlobal);

            return existing;
        }

        /// <summary>
        /// Routes are matched on <c>UpstreamPathTemplate</c> + <c>DownstreamHttpMethod</c>, which identify a route in the model,
        /// so hand edits stay with their route when other routes are added, removed or renamed. The result follows the model's
        /// order. A route on disk that the model no longer has is removed, unless it is marked <c>// IntentIgnore</c>; then it
        /// stays after the route it followed.
        /// </summary>
        private static JsoncArray MergeRoutes(JsoncArray existing, JsoncArray generated)
        {
            var unmatched = existing.Items.ToList();
            var result = new JsoncArray();
            var placed = new HashSet<JsoncMember>();

            foreach (var generatedItem in generated.Items)
            {
                var match = unmatched.FirstOrDefault(item => IsSameRoute(item.Value, generatedItem.Value));
                if (match is null)
                {
                    result.Items.Add(generatedItem);
                    continue;
                }

                unmatched.Remove(match);
                if (!match.IsIgnored && match.Value is JsoncObject existingRoute && generatedItem.Value is JsoncObject generatedRoute)
                {
                    MergeRoute(existingRoute, generatedRoute);
                }

                result.Items.Add(match);
                placed.Add(match);
            }

            // Hand-kept routes the model does not have go back after the route they followed on disk.
            JsoncMember? anchor = null;
            var insertAt = 0;
            foreach (var item in existing.Items)
            {
                if (placed.Contains(item))
                {
                    anchor = item;
                    insertAt = result.Items.IndexOf(anchor) + 1;
                    continue;
                }

                if (!unmatched.Contains(item) || !item.IsIgnored)
                {
                    continue;
                }

                result.Items.Insert(insertAt, item);
                insertAt++;
            }

            result.TrailingComments.AddRange(existing.TrailingComments);
            return result;
        }

        private static bool IsSameRoute(JsoncValue existing, JsoncValue generated)
        {
            return existing is JsoncObject existingRoute &&
                generated is JsoncObject generatedRoute &&
                string.Equals(existingRoute.GetString("UpstreamPathTemplate"), generatedRoute.GetString("UpstreamPathTemplate"), StringComparison.OrdinalIgnoreCase) &&
                string.Equals(existingRoute.GetString("DownstreamHttpMethod"), generatedRoute.GetString("DownstreamHttpMethod"), StringComparison.OrdinalIgnoreCase);
        }

        private static void MergeRoute(JsoncObject existing, JsoncObject generated)
        {
            foreach (var key in RouteKeysFromModel)
            {
                SetOrRemove(existing, generated, key);
            }

            // AllowAnonymous belongs to the model (an Unsecured route); any other authentication option on a route is a hand edit.
            MergeMember(existing, generated, AuthenticationOptions, (existingAuth, generatedAuth) =>
            {
                if (existingAuth is not JsoncObject existingObject)
                {
                    return generatedAuth;
                }

                SetOrRemove(existingObject, generatedAuth as JsoncObject ?? new JsoncObject(), "AllowAnonymous");
                return existingObject;
            });
        }

        private static JsoncObject MergeGlobalConfiguration(JsoncObject existing, JsoncObject generated)
        {
            MergeMember(existing, generated, AuthenticationOptions, (existingAuth, generatedAuth) =>
            {
                if (existingAuth is not JsoncObject existingObject)
                {
                    return generatedAuth;
                }

                SetOrRemove(existingObject, generatedAuth as JsoncObject ?? new JsoncObject(), "AuthenticationProviderKeys");
                return existingObject;
            });

            // The rule is the model's while a route is marked, and goes when none is: Ocelot applies a rule without RouteKeys to
            // every route. Its values (Limit, Period, ...) are seeded once and then belong to whoever edits the file.
            MergeMember(existing, generated, RateLimitOptions, (existingRule, generatedRule) =>
            {
                if (existingRule is not JsoncObject existingObject || generatedRule is not JsoncObject generatedObject)
                {
                    return generatedRule;
                }

                foreach (var key in RateLimitKeysFromModel)
                {
                    SetOrRemove(existingObject, generatedObject, key);
                }

                foreach (var seeded in generatedObject.Members.Where(member => !RateLimitKeysFromModel.Contains(member.Name)))
                {
                    if (existingObject.Find(seeded.Name!) is null)
                    {
                        Insert(existingObject, generatedObject, seeded);
                    }
                }

                return existingObject;
            });

            return existing;
        }

        /// <summary>
        /// Brings the member <paramref name="name"/> of <paramref name="existing"/> in line with <paramref name="generated"/>:
        /// added where only the model has it, removed where only the file has it, otherwise replaced by <paramref name="merge"/>'s
        /// result. A member marked <c>// IntentIgnore</c> is left alone. An object the merge leaves empty is removed.
        /// </summary>
        private static void MergeMember(JsoncObject existing, JsoncObject generated, string name, Func<JsoncValue, JsoncValue?, JsoncValue?> merge)
        {
            var existingMember = existing.Find(name);
            var generatedMember = generated.Find(name);
            if (existingMember?.IsIgnored == true)
            {
                return;
            }

            if (existingMember is null)
            {
                if (generatedMember is not null)
                {
                    Insert(existing, generated, generatedMember);
                }

                return;
            }

            var merged = merge(existingMember.Value, generatedMember?.Value);
            if (merged is null || merged is JsoncObject { Members.Count: 0, TrailingComments.Count: 0 } && generatedMember is null)
            {
                existing.Members.Remove(existingMember);
                return;
            }

            existingMember.Value = merged;
        }

        /// <summary>A value the model owns: written as the model has it, or removed when the model has none.</summary>
        private static void SetOrRemove(JsoncObject existing, JsoncObject generated, string name)
        {
            MergeMember(existing, generated, name, (_, generatedValue) => generatedValue);
        }

        /// <summary>Adds a member the file lacks after the closest member that precedes it in the generated object.</summary>
        private static void Insert(JsoncObject existing, JsoncObject generated, JsoncMember member)
        {
            var index = 0;
            foreach (var preceding in generated.Members.TakeWhile(m => m.Name != member.Name))
            {
                var position = existing.Members.FindIndex(m => m.Name == preceding.Name);
                if (position >= 0)
                {
                    index = position + 1;
                }
            }

            existing.Members.Insert(index, member);
        }
    }
}
