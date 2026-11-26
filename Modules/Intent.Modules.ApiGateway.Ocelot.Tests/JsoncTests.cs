using System;
using System.Linq;
using System.Text.Json;
using Intent.Modules.ApiGateway.Ocelot.Templates.OcelotJson;
using Shouldly;
using Xunit;

namespace Intent.Modules.ApiGateway.Ocelot.Tests
{
    public class JsoncTests
    {
        private static JsoncObject ParseObject(string text) =>
            (JsoncObject)JsoncParser.Parse(text)!.Root;

        [Fact]
        public void Parses_a_plain_object()
        {
            var root = ParseObject("""{ "a": 1, "b": "two", "c": true, "d": false, "e": null }""");

            root.Members.Select(m => m.Name).ShouldBe(["a", "b", "c", "d", "e"]);
            ((JsoncScalar)root.Get("a")!).Kind.ShouldBe(JsonValueKind.Number);
            root.GetString("b").ShouldBe("two");
            ((JsoncScalar)root.Get("c")!).Kind.ShouldBe(JsonValueKind.True);
            ((JsoncScalar)root.Get("d")!).Kind.ShouldBe(JsonValueKind.False);
            ((JsoncScalar)root.Get("e")!).Kind.ShouldBe(JsonValueKind.Null);
        }

        [Fact]
        public void Keeps_the_raw_text_of_scalars()
        {
            var root = ParseObject("{ \"n\": 1.50, \"e\": 1E3, \"s\": \"a\\u0041\\n\" }");

            ((JsoncScalar)root.Get("n")!).RawText.ShouldBe("1.50");
            ((JsoncScalar)root.Get("e")!).RawText.ShouldBe("1E3");
            var s = (JsoncScalar)root.Get("s")!;
            s.RawText.ShouldBe("\"a\\u0041\\n\"");
            s.StringValue.ShouldBe("aA\n");
        }

        [Fact]
        public void Parses_nested_objects_and_arrays()
        {
            var root = ParseObject("""{ "o": { "arr": [1, [2, 3], { "x": "y" }] } }""");

            var arr = (JsoncArray)((JsoncObject)root.Get("o")!).Get("arr")!;
            arr.Items.Count.ShouldBe(3);
            arr.Items.ShouldAllBe(i => i.Name == null);
            ((JsoncArray)arr.Items[1].Value).Items.Count.ShouldBe(2);
            ((JsoncObject)arr.Items[2].Value).GetString("x").ShouldBe("y");
        }

        [Fact]
        public void Attaches_line_comments_above_a_member_to_that_member()
        {
            var root = ParseObject("{\n  // first\n  // second\n  \"a\": 1\n}");

            root.Find("a")!.Comments.ShouldBe(["// first", "// second"]);
        }

        [Fact]
        public void Attaches_block_comments_above_a_member_to_that_member()
        {
            var root = ParseObject("{\n  /* block */\n  \"a\": 1\n}");

            root.Find("a")!.Comments.ShouldBe(["/* block */"]);
        }

        [Fact]
        public void Keeps_a_multi_line_block_comment_whole()
        {
            var root = ParseObject("{\n  /* one\n     two */\n  \"a\": 1\n}");

            root.Find("a")!.Comments.Single().ShouldBe("/* one\n     two */");
        }

        [Fact]
        public void Keeps_a_comment_on_the_same_line_beside_the_previous_member()
        {
            var root = ParseObject("{\n  \"a\": 1, // beside a\n  \"b\": 2\n}");

            root.Find("a")!.LineComment.ShouldBe("// beside a");
            root.Find("b")!.Comments.ShouldBeEmpty();
        }

        [Fact]
        public void A_comment_on_the_next_line_belongs_to_the_next_member()
        {
            var root = ParseObject("{\n  \"a\": 1,\n  // above b\n  \"b\": 2\n}");

            root.Find("a")!.LineComment.ShouldBeNull();
            root.Find("b")!.Comments.ShouldBe(["// above b"]);
        }

        [Fact]
        public void Keeps_comments_after_the_last_member_as_trailing_comments()
        {
            var root = ParseObject("{\n  \"a\": 1\n  // trailing\n}");

            root.TrailingComments.ShouldBe(["// trailing"]);
        }

        [Fact]
        public void Keeps_a_comment_between_a_name_and_its_value_above_the_member()
        {
            var root = ParseObject("{\n  \"a\": /* odd */ 1\n}");

            root.Find("a")!.Comments.ShouldBe(["/* odd */"]);
            ((JsoncScalar)root.Get("a")!).RawText.ShouldBe("1");
        }

        [Fact]
        public void Keeps_leading_and_trailing_document_comments()
        {
            var document = JsoncParser.Parse("// head\n{ \"a\": 1 }\n// tail\n")!;

            document.LeadingComments.ShouldBe(["// head"]);
            document.TrailingComments.ShouldBe(["// tail"]);
        }

        [Fact]
        public void Keeps_comments_in_arrays()
        {
            var array = (JsoncArray)ParseObject("{ \"r\": [\n // one\n 1, // beside\n 2\n // end\n] }").Get("r")!;

            array.Items[0].Comments.ShouldBe(["// one"]);
            array.Items[0].LineComment.ShouldBe("// beside");
            array.TrailingComments.ShouldBe(["// end"]);
        }

        [Fact]
        public void Accepts_trailing_commas_in_objects_and_arrays()
        {
            var root = ParseObject("""{ "a": [1, 2,], "b": 3, }""");

            ((JsoncArray)root.Get("a")!).Items.Count.ShouldBe(2);
            root.Members.Count.ShouldBe(2);
        }

        [Fact]
        public void Strings_may_contain_comment_like_text()
        {
            var root = ParseObject("""{ "url": "http://host/path", "block": "/* not a comment */", "line": "// nor this" }""");

            root.GetString("url").ShouldBe("http://host/path");
            root.GetString("block").ShouldBe("/* not a comment */");
            root.GetString("line").ShouldBe("// nor this");
            root.Members.ShouldAllBe(m => m.Comments.Count == 0 && m.LineComment == null);
        }

        [Fact]
        public void Strings_may_contain_escaped_quotes_and_backslashes()
        {
            var root = ParseObject("""{ "a": "say \"hi\" \\ // x" }""");

            root.GetString("a").ShouldBe("say \"hi\" \\ // x");
        }

        [Fact]
        public void Strips_a_leading_byte_order_mark()
        {
            var document = JsoncParser.Parse("﻿{ \"a\": 1 }");

            document.ShouldNotBeNull();
            ((JsoncObject)document.Root).Find("a").ShouldNotBeNull();
        }

        [Fact]
        public void Parses_a_non_object_root()
        {
            JsoncParser.Parse("[1, 2]")!.Root.ShouldBeOfType<JsoncArray>();
            JsoncParser.Parse("42")!.Root.ShouldBeOfType<JsoncScalar>();
        }

        [Fact]
        public void A_file_with_only_comments_has_no_value()
        {
            JsoncParser.Parse("// nothing here\n").ShouldBeNull();
        }

        [Theory]
        [InlineData("{ \"a\": 1")]
        [InlineData("{ \"a\": }")]
        [InlineData("{ \"a\" 1 }")]
        [InlineData("{ a: 1 }")]
        [InlineData("[1, 2")]
        [InlineData("{ \"a\": 1 } extra")]
        [InlineData("{ \"a\": 'x' }")]
        public void Invalid_input_throws_JsonException(string content)
        {
            Should.Throw<JsonException>(() => JsoncParser.Parse(content));
        }

        [Fact]
        public void An_invalid_file_reports_its_line()
        {
            var ex = Should.Throw<JsonException>(() => JsoncParser.Parse("{\n  \"a\": 1,\n  \"b\": ,\n}"));

            ex.LineNumber.ShouldBe(2); // 0-based; the template reports line 3
        }

        [Fact]
        public void Observed_an_empty_or_blank_file_throws_JsonException_rather_than_returning_null()
        {
            // So a 0-byte ocelot.json stops the Software Factory as "not valid JSON" (the parser's null is only for comment-only files).
            Should.Throw<JsonException>(() => JsoncParser.Parse(""));
            Should.Throw<JsonException>(() => JsoncParser.Parse("   \n"));
        }

        [Fact]
        public void Writer_normalises_to_two_space_indentation_and_drops_trailing_commas()
        {
            var document = JsoncParser.Parse("{\"a\":[1,2,],\"b\":{\"c\":true,},\"d\":[],\"e\":{}}")!;

            JsoncWriter.Write(document).Replace(Environment.NewLine, "\n").ShouldBe(
                "{\n  \"a\": [\n    1,\n    2\n  ],\n  \"b\": {\n    \"c\": true\n  },\n  \"d\": [],\n  \"e\": {}\n}\n");
        }

        [Fact]
        public void Writer_round_trips_comments()
        {
            const string text = "// head\n{\n  // above a\n  \"a\": 1, // beside a\n  /* above b */\n  \"b\": [\n    // item\n    2\n    // end of b\n  ]\n  // end\n}\n// tail\n";

            JsoncWriter.Write(JsoncParser.Parse(text)!).Replace(Environment.NewLine, "\n").ShouldBe(text);
        }

        [Fact]
        public void Writer_is_idempotent()
        {
            var once = JsoncWriter.Write(JsoncParser.Parse("{\"a\": 1, // c\n\"b\": [1,2,]}")!);
            var twice = JsoncWriter.Write(JsoncParser.Parse(once)!);

            twice.ShouldBe(once);
        }
    }
}
