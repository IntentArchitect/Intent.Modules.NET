using System;
using Intent.Modules.ApiGateway.Ocelot.Templates.OcelotJson;
using Intent.Modules.Common.FileBuilders.DataFileBuilder;
using Shouldly;
using Xunit;

namespace Intent.Modules.ApiGateway.Ocelot.Tests
{
    public class OcelotJsonWriterTests
    {
        private static string Write(string? existing, DataFileObjectValue generated)
        {
            var writer = new OcelotJsonWriter(() => existing is null ? null : JsoncParser.Parse(existing));
            writer.Visit(generated);
            return writer.ToString()!;
        }

        private static DataFileObjectValue Generated()
        {
            var root = new DataFileObjectValue();
            root.WithArray("Routes", routes =>
            {
                var route = new DataFileObjectValue();
                route.WithValue("Key", "A");
                route.WithValue("Priority", 2);
                route.WithValue("AllowAnonymous", true);
                routes.Add(route);
            });
            return root;
        }

        [Fact]
        public void Writes_the_generated_tree_with_two_space_indentation()
        {
            var output = Write(null, Generated()).Replace(Environment.NewLine, "\n");

            output.ShouldBe("{\n  \"Routes\": [\n    {\n      \"Key\": \"A\",\n      \"Priority\": 2,\n      \"AllowAnonymous\": true\n    }\n  ]\n}\n");
        }

        [Fact]
        public void Output_is_stable_when_written_over_its_own_output()
        {
            var first = Write(null, Generated());
            var second = Write(first, Generated());

            second.ShouldBe(first);
        }

        [Fact]
        public void Writes_a_data_file_comment_above_its_member()
        {
            var root = new DataFileObjectValue();
            root.WithValue("a", "b");
            foreach (var (_, member) in root)
            {
                member.Comment = "about a";
            }

            Write(null, root).ShouldContain("// about a");
        }

        [Fact]
        public void Merges_into_the_file_on_disk()
        {
            var existing = "{ \"Routes\": [], \"Custom\": 1 }";

            Write(existing, Generated()).ShouldContain("\"Custom\": 1");
        }

        [Fact]
        public void Only_the_root_object_can_be_visited()
        {
            var writer = new OcelotJsonWriter(() => null);

            Should.Throw<InvalidOperationException>(() => writer.Visit(new DataFileScalarValue("x")));
            Should.Throw<InvalidOperationException>(() => writer.Visit(new DataFileArrayValue()));
        }

        [Fact]
        public void PlausibleBug_line_endings_follow_Environment_NewLine_not_a_fixed_value()
        {
            // JsoncWriter uses StringBuilder.AppendLine, so the same model gives CRLF on Windows and LF on Linux/macOS. The
            // file written on one OS differs, byte for byte, from the one written on the other.
            var output = Write(null, Generated());

            var expected = Environment.NewLine;
            output.Replace(expected, string.Empty).ShouldNotContain("\n");
            output.Replace(expected, string.Empty).ShouldNotContain("\r");
            output.ShouldEndWith(expected);

            var document = JsoncWriter.Write(JsoncParser.Parse("// c\n{ \"a\": 1 }\n// t")!);
            document.Replace(expected, string.Empty).ShouldNotContain("\n");
        }

        [Fact]
        public void PlausibleBug_input_line_endings_are_normalised_to_Environment_NewLine_on_output()
        {
            var crlf = JsoncWriter.Write(JsoncParser.Parse("{\r\n  // c\r\n  \"a\": 1\r\n}\r\n")!);
            var lf = JsoncWriter.Write(JsoncParser.Parse("{\n  // c\n  \"a\": 1\n}\n")!);

            crlf.ShouldBe(lf);
        }
    }
}
