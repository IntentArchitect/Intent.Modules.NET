using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml.Linq;
using Intent.Engine;
using Intent.Modules.Common.CSharp.Builder;
using Intent.Modules.Common.CSharp.Templates;
using Intent.Modules.Common.Drift.EngineMigration;
using Intent.Modules.Common.FileBuilders;
using Intent.Modules.Services.Guardrails.Contracts.Templates;
using Intent.Templates;
using Microsoft.CodeAnalysis.CSharp;
using NSubstitute;
using Shouldly;
using Xunit;

namespace Intent.Modules.Guardrails.Contracts.Tests.EngineMigration
{
    /// <summary>A contract file handed back to its developer: Intent's merge attributes and their using gone, the rest as written.</summary>
    public class ContractReleaseTests : IDisposable
    {
        private readonly string _root = Path.Combine(Path.GetTempPath(), "contract-release-" + Guid.NewGuid().ToString("N"));

        public ContractReleaseTests() => Directory.CreateDirectory(_root);

        public void Dispose()
        {
            foreach (var file in Directory.GetFiles(_root, "*", System.IO.SearchOption.AllDirectories))
            {
                File.SetAttributes(file, FileAttributes.Normal);
            }

            Directory.Delete(_root, recursive: true);
        }

        [Fact]
        public void A_rendered_contract_file_loses_its_header_and_keeps_its_code()
        {
            var file = new CSharpFile("Acme.Orders", "src").IntentManagedContractMerge();
            file.AddUsing("System");
            file.AddClass("Order", c => c
                .AddProperty("Guid", "Id")
                .AddMethod("void", "Cancel", m => m.AddStatement("throw new NotImplementedException();")));
            var rendered = Render(file);
            rendered.ShouldContain("[assembly: DefaultIntentManaged(Mode.Merge)]");

            var released = ContractRelease.Strip(WithTemplateHeader(rendered, "Intent.Services.Guardrails.Contracts.DtoContract"));

            released.ShouldNotContain("Intent");
            released.ShouldNotContain("[assembly");
            released.ShouldStartWith("using System;\n\nnamespace Acme.Orders");
            released.ShouldContain("public class Order");
            released.ShouldContain("public void Cancel()");
            Parses(released);
        }

        [Fact]
        public void Every_Intent_attribute_on_types_and_members_goes()
        {
            const string code = """
                using System;
                using Intent.RoslynWeaver.Attributes;

                [assembly: DefaultIntentManaged(Mode.Merge)]
                [assembly: IntentTemplate("Intent.Domain.Guardrails.Contracts.EntityContract", Version = "1.0")]

                namespace Acme
                {
                    /// <summary>An order.</summary>
                    [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
                    public class Order
                    {
                        [IntentIgnore]
                        public string Notes { get; set; }

                        [IntentManaged(Mode.Fully, Body = Mode.Ignore), Obsolete("Use Close")]
                        public void Cancel() { }

                        [IntentIgnoreBody] public void Close([IntentMerge] int reason) { }

                        [Intent.RoslynWeaver.Attributes.IntentFully]
                        [IntentManagedAttribute(Mode.Ignore)]
                        public int Total() => 0;
                    }
                }
                """;

            ContractRelease.Strip(code.Replace("\r\n", "\n")).ShouldBe("""
                using System;

                namespace Acme
                {
                    /// <summary>An order.</summary>
                    public class Order
                    {
                        public string Notes { get; set; }

                        [Obsolete("Use Close")]
                        public void Cancel() { }

                        public void Close(int reason) { }

                        public int Total() => 0;
                    }
                }
                """.Replace("\r\n", "\n"));
        }

        [Fact]
        public void An_attribute_of_the_same_name_from_elsewhere_stays()
        {
            const string code = "using Acme.Weaving;\n\nnamespace Acme;\n\n[IntentManaged]\npublic class Order { }\n";

            ContractRelease.Strip(code).ShouldBeSameAs(code);
        }

        [Fact]
        public void A_file_with_nothing_of_Intents_is_the_same_text()
        {
            const string code = "namespace Acme;\n\npublic class Order { }\n";

            ContractRelease.Strip(code).ShouldBeSameAs(code);
        }

        [Fact]
        public void Releasing_again_changes_nothing()
        {
            var path = Write("Order.cs", "using Intent.RoslynWeaver.Attributes;\n\n[assembly: DefaultIntentManaged(Mode.Merge)]\n\nnamespace Acme;\n\npublic class Order { }\n", Encoding.UTF8);

            ContractRelease.ReleaseFile(path).ShouldBeTrue();
            var once = File.ReadAllBytes(path);
            var written = File.GetLastWriteTimeUtc(path);

            ContractRelease.ReleaseFile(path).ShouldBeFalse();
            File.ReadAllBytes(path).ShouldBe(once);
            File.GetLastWriteTimeUtc(path).ShouldBe(written);
        }

        [Fact]
        public void Line_endings_and_a_byte_order_mark_are_kept()
        {
            var path = Write("Order.cs", "using System;\r\nusing Intent.RoslynWeaver.Attributes;\r\n\r\n[assembly: DefaultIntentManaged(Mode.Merge)]\r\n\r\nnamespace Acme;\r\n\r\npublic class Order { }\r\n", new UTF8Encoding(true));

            ContractRelease.ReleaseFile(path).ShouldBeTrue();

            var bytes = File.ReadAllBytes(path);
            bytes.Take(3).ShouldBe(new byte[] { 0xEF, 0xBB, 0xBF });
            Encoding.UTF8.GetString(bytes, 3, bytes.Length - 3).ShouldBe("using System;\r\n\r\nnamespace Acme;\r\n\r\npublic class Order { }\r\n");
        }

        [Fact]
        public void A_file_without_a_byte_order_mark_gets_none()
        {
            var path = Write("Order.cs", "using Intent.RoslynWeaver.Attributes;\n\n[assembly: DefaultIntentManaged(Mode.Merge)]\n\nnamespace Acme;\n", new UTF8Encoding(false));

            ContractRelease.ReleaseFile(path).ShouldBeTrue();

            File.ReadAllBytes(path).ShouldBe(Encoding.UTF8.GetBytes("namespace Acme;\n"));
        }

        [Fact]
        public void A_file_with_no_Intent_attributes_isnt_written()
        {
            var path = Write("Order.cs", "namespace Acme;\n\npublic class Order { }\n", Encoding.UTF8);
            var written = File.GetLastWriteTimeUtc(path);

            ContractRelease.ReleaseFile(path).ShouldBeFalse();
            File.GetLastWriteTimeUtc(path).ShouldBe(written);
        }

        [Fact]
        public void Contract_files_are_found_by_the_template_their_header_names()
        {
            const string contract = "using Intent.RoslynWeaver.Attributes;\n\n[assembly: IntentTemplate(\"Intent.Domain.Guardrails.Contracts.EntityContract\", Version = \"1.0\")]\n\nnamespace Acme;\n";
            Directory.CreateDirectory(Path.Combine(_root, "src", "Domain"));
            Directory.CreateDirectory(Path.Combine(_root, "src", "bin", "Debug"));
            Directory.CreateDirectory(Path.Combine(_root, "Other", "Domain"));
            Write(Path.Combine("src", "Domain", "Order.cs"), contract, Encoding.UTF8);
            Write(Path.Combine("src", "bin", "Debug", "Order.cs"), contract, Encoding.UTF8);
            Write(Path.Combine("src", "Program.cs"), contract.Replace("Intent.Domain.Guardrails.Contracts.EntityContract", "Intent.AspNetCore.Program"), Encoding.UTF8);
            Write(Path.Combine("src", "Domain", "OrderSkill.cs"), contract.Replace("EntityContract", "SyncModelSkill"), Encoding.UTF8);
            Write(Path.Combine("Other", "Domain", "Customer.cs"), contract, Encoding.UTF8);

            // Other is another application's output folder, which may still be on v1.
            ContractRelease.FindContractFiles(_root, new[] { Path.Combine(_root, "Other") }).ShouldBe(new[] { "src/Domain/Order.cs" });
        }

        [Fact]
        public void A_file_no_longer_on_disk_is_skipped()
        {
            ContractRelease.ReleaseFile(Path.Combine(_root, "Gone.cs")).ShouldBeFalse();
        }

        [Fact]
        public void A_read_only_file_fails_the_release()
        {
            var path = Write("Order.cs", "using Intent.RoslynWeaver.Attributes;\n\n[assembly: DefaultIntentManaged(Mode.Merge)]\n\nnamespace Acme;\n", Encoding.UTF8);
            File.SetAttributes(path, FileAttributes.ReadOnly);

            Should.Throw<UnauthorizedAccessException>(() => ContractRelease.ReleaseFile(path));
        }

        /// <summary>
        /// eShop10's contract files, one per template it has (13 of the 18), as the weaver merged them, read in place. Each loses
        /// only Intent's lines and the blank lines a removal left doubled, and still parses.
        /// </summary>
        [Fact]
        public void Each_real_merged_contract_file_loses_only_Intents_lines()
        {
            const string root = "D:/Dev/Temp/eShop10";
            var log = Path.Combine(root, "intent", "eShop", ".intent", "output-log.xml");
            if (!File.Exists(log))
            {
                return;
            }

            var files = XDocument.Load(log).Root!.Element("files")!.Elements("file")
                .Where(x => V1Footprint.IsContractCorrelation((string)x.Element("correlationId")))
                .GroupBy(x => ((string)x.Element("correlationId"))!.Split('#')[0])
                .Select(x => (Template: x.Key, Path: Path.Combine(root, (string)x.First().Attribute("relativePath")!)))
                .Where(x => File.Exists(x.Path))
                .ToList();
            files.Count.ShouldBe(13);

            foreach (var (template, path) in files)
            {
                var original = File.ReadAllText(path);
                var released = ContractRelease.Strip(original);

                released.ShouldNotContain("Intent.RoslynWeaver", customMessage: template);
                released.ShouldNotContain("[assembly: DefaultIntentManaged", customMessage: template);
                released.ShouldNotContain("[assembly: IntentTemplate", customMessage: template);
                ContractRelease.Strip(released).ShouldBeSameAs(released, template);
                Parses(released);

                // What went is Intent's lines and blank lines; what stayed is in the same order, unchanged.
                var kept = released.Split('\n');
                var removed = Removed(original.Split('\n'), kept);
                removed.ShouldAllBe(x => x.Trim().Length == 0 || x.Contains("Intent"), template);
                original.Split('\n').Count(x => x.Contains("Intent")).ShouldBe(removed.Count(x => x.Contains("Intent")), template);
            }
        }

        private static List<string> Removed(string[] original, string[] kept)
        {
            var removed = new List<string>();
            var k = 0;
            foreach (var line in original)
            {
                if (k < kept.Length && kept[k] == line)
                {
                    k++;
                }
                else
                {
                    removed.Add(line);
                }
            }

            k.ShouldBe(kept.Length, "the released file has a line the original doesn't");
            return removed;
        }

        private static void Parses(string code) =>
            CSharpSyntaxTree.ParseText(code).GetDiagnostics().Where(x => x.Severity == Microsoft.CodeAnalysis.DiagnosticSeverity.Error).ShouldBeEmpty();

        /// <summary>
        /// What the template adds when the Software Factory writes the file: the attributes' using, after the file's own, and the
        /// template id attribute, after the file's own assembly attributes.
        /// </summary>
        private static string WithTemplateHeader(string rendered, string templateId)
        {
            var lines = rendered.Replace("\r\n", "\n").Split('\n').ToList();
            var assembly = lines.FindLastIndex(x => x.StartsWith("[assembly:", StringComparison.Ordinal));
            lines.Insert(assembly + 1, $"[assembly: IntentTemplate(\"{templateId}\", Version = \"1.0\")]");
            lines.Insert(lines.FindLastIndex(x => x.StartsWith("using ", StringComparison.Ordinal)) + 1, "using Intent.RoslynWeaver.Attributes;");
            return string.Join("\n", lines);
        }

        /// <summary>A file built as the Software Factory builds a file builder template's, then rendered.</summary>
        private static string Render(CSharpFile file)
        {
            var template = Substitute.For<IFileBuilderTemplate>();
            template.File.Returns(file);
            template.CanRunTemplate().Returns(true);
            var outputTarget = Substitute.For<IOutputTarget>();
            outputTarget.TemplateInstances.Returns(new List<ITemplate> { template });
            var context = Substitute.For<ISoftwareFactoryExecutionContext>();
            context.OutputTargets.Returns(new[] { outputTarget });
            FileBuilderHelper.PerformConfiguration(context, isForAfterBuild: false);
            FileBuilderHelper.PerformConfiguration(context, isForAfterBuild: true);
            return file.ToString().Replace("\r\n", "\n");
        }

        private string Write(string name, string text, Encoding encoding)
        {
            var path = Path.Combine(_root, name);
            File.WriteAllText(path, text, encoding);
            return path;
        }
    }
}
