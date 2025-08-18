using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Intent.Modules.AI.Blazor.Tasks.Config
{
    public static class TemplateGuesser
    {
        public sealed class GuessResult
        {
            public string TemplateId { get; init; }
            public double Score { get; init; }
            public double Confidence { get; init; } // 0..1 against that template’s max positive weight
            public string Leaf { get; init; } = "";
            public IReadOnlyList<string> Tokens { get; init; } = Array.Empty<string>();
            public IReadOnlyList<(string TemplateId, double Score, int Priority)> TopCandidates { get; init; } = Array.Empty<(string, double, int)>();
        }

        /// <summary>
        /// Scores templates using token matches from the LEAF (last path segment).
        /// Set includeGroupTokens=true to also include tokens from path segments before the leaf.
        /// </summary>
        public static GuessResult Guess(PromptConfig cfg, string nameOrPath, bool includeGroupTokens = false)
        {
            if (cfg is null) throw new ArgumentNullException(nameof(cfg));
            if (cfg.Templates is null || cfg.Templates.Count == 0)
                return new GuessResult { TemplateId = null, Score = 0, Confidence = 0, Leaf = GetLeaf(nameOrPath) };

            var (leaf, tokens) = BuildTokens(nameOrPath, includeGroupTokens);

            // Precompute a set for O(1) membership checks
            var tokenSet = new HashSet<string>(tokens, StringComparer.OrdinalIgnoreCase);

            // Score each template: sum(keywords in tokens) - sum(negatives in tokens)
            var scored = cfg.Templates.Select(t =>
            {
                var pos = (t.Match?.Keywords ?? new()).Sum(k => tokenSet.Contains((k.Word ?? "").ToLowerInvariant()) ? k.Weight : 0);
                var neg = (t.Match?.Negatives ?? new()).Sum(k => tokenSet.Contains((k.Word ?? "").ToLowerInvariant()) ? k.Weight : 0);
                var score = pos - neg;
                var priority = t.Match?.Priority ?? 0;

                // For confidence, normalize by the max achievable positive weight for THIS template
                var maxPos = (t.Match?.Keywords ?? new()).Sum(k => k.Weight);
                var confidence = maxPos > 0 ? Clamp01(score / maxPos) : 0;

                return new
                {
                    TemplateId = t.Id,
                    Score = score,
                    Priority = priority,
                    Confidence = confidence
                };
            })
            .OrderByDescending(x => x.Score)
            .ThenByDescending(x => x.Priority) // deterministic tie-breaker
            .ToList();

            var best = scored.First();

            if (best.Confidence == 0 || best.Score == 0)
                return null;

            return new GuessResult
            {
                TemplateId = best.TemplateId,
                Score = best.Score,
                Confidence = Clamp01(best.Confidence),
                Leaf = leaf,
                Tokens = tokens,
                TopCandidates = scored
                    .Take(3)
                    .Select(x => (x.TemplateId, Math.Round(x.Score, 2), x.Priority))
                    .ToList()
            };
        }

        // ---------- helpers ----------

        private static double Clamp01(double v) => v < 0 ? 0 : (v > 1 ? 1 : v);

        private static string GetLeaf(string nameOrPath)
        {
            var s = nameOrPath ?? "";
            var parts = s.Split('/', StringSplitOptions.RemoveEmptyEntries);
            return parts.Length == 0 ? s : parts[^1];
        }

        private static (string leaf, List<string> tokens) BuildTokens(string nameOrPath, bool includeGroupTokens)
        {
            var s = nameOrPath ?? "";
            var parts = s.Split('/', StringSplitOptions.RemoveEmptyEntries);
            var leaf = parts.Length == 0 ? s : parts[^1];

            var tokens = SplitTokens(leaf);

            if (includeGroupTokens && parts.Length > 1)
            {
                for (int i = 0; i < parts.Length - 1; i++)
                    tokens.AddRange(SplitTokens(parts[i]));
            }

            // lower-case everything for matching against config words
            for (int i = 0; i < tokens.Count; i++)
                tokens[i] = tokens[i].ToLowerInvariant();

            return (leaf, tokens);
        }

        /// <summary>
        /// Casing/number aware tokenizer: splits on spaces/_/-/., camel humps, and letter↔digit boundaries.
        /// Examples:
        ///  - "AddOrder"   -> ["Add","Order"]
        ///  - "OrderAdd"   -> ["Order","Add"]
        ///  - "Customer-Create" -> ["Customer","Create"]
        ///  - "Customer1A" -> ["Customer","1","A"]
        /// </summary>
        private static List<string> SplitTokens(string segment)
        {
            var t = (segment ?? "").Replace('_', ' ').Replace('-', ' ').Replace('.', ' ');
            var tokens = new List<string>();
            int start = 0;

            bool IsBoundary(int i)
            {
                if (i <= 0 || i >= t.Length) return false;
                char prev = t[i - 1], curr = t[i];
                if (prev == ' ' || curr == ' ') return true;                          // separators
                if (char.IsLower(prev) && char.IsUpper(curr)) return true;            // fooBar
                if (char.IsLetter(prev) && char.IsDigit(curr)) return true;           // Foo1
                if (char.IsDigit(prev) && char.IsLetter(curr)) return true;           // 1Foo
                                                                                      // XMLParser → split before 'P': XML | Parser
                if (char.IsUpper(prev) && char.IsUpper(curr) && i + 1 < t.Length && char.IsLower(t[i + 1])) return true;
                return false;
            }

            void Push(int i)
            {
                var part = t.AsSpan(start, i - start).ToString().Trim();
                if (part.Length > 0) tokens.Add(part);
                start = i;
            }

            for (int i = 1; i < t.Length; i++)
                if (IsBoundary(i)) Push(i);
            Push(t.Length);

            // collapse multi-spaces and drop empties
            return tokens
                .SelectMany(x => x.Split(' ', StringSplitOptions.RemoveEmptyEntries))
                .ToList();
        }
    }
}