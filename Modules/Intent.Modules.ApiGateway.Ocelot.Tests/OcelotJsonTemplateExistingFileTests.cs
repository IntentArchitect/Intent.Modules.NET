using System;
using System.Collections.Generic;
using System.Reflection;
using Intent.Engine;
using Intent.Metadata.ApiGateway.Api;
using Intent.Modules.ApiGateway.Ocelot.Templates.OcelotJson;
using Intent.Modules.Common.Templates;
using NSubstitute;
using Shouldly;
using Xunit;

namespace Intent.Modules.ApiGateway.Ocelot.Tests
{
    /// <summary>
    /// How <see cref="OcelotJsonTemplate"/> reads the ocelot.json on disk. The template cannot find a real file without a
    /// Software Factory run, so the content the base class would have read is placed in its private cache by reflection.
    /// </summary>
    public class OcelotJsonTemplateExistingFileTests
    {
        private static OcelotJsonTemplate CreateTemplate(string? existingContent)
        {
            var target = Substitute.For<IOutputTarget>();
            var template = new OcelotJsonTemplate(target, new List<ApiGatewayRouteModel>());
            typeof(IntentTemplateBase)
                .GetField("_tryGetExistingFileContentCache", BindingFlags.NonPublic | BindingFlags.Instance)!
                .SetValue(template, new Lazy<(bool, string)>(() => (existingContent is not null, existingContent ?? string.Empty)));
            return template;
        }

        private static JsoncDocument? ReadExistingFile(OcelotJsonTemplate template)
        {
            try
            {
                return (JsoncDocument?)typeof(OcelotJsonTemplate)
                    .GetMethod("ReadExistingFile", BindingFlags.NonPublic | BindingFlags.Instance)!
                    .Invoke(template, null);
            }
            catch (TargetInvocationException ex)
            {
                throw ex.InnerException!;
            }
        }

        [Fact]
        public void No_file_on_disk_gives_no_existing_document()
        {
            ReadExistingFile(CreateTemplate(null)).ShouldBeNull();
        }

        [Fact]
        public void A_valid_file_is_parsed_with_its_comments()
        {
            var document = ReadExistingFile(CreateTemplate("// hi\n{ \"Routes\": [] }"));

            document.ShouldNotBeNull();
            document.LeadingComments.ShouldBe(["// hi"]);
        }

        [Fact]
        public void PlausibleBug_invalid_JSON_with_no_modelled_routes_throws_InvalidOperationException_not_ElementException()
        {
            // The friendly ElementException needs a package, taken from the first route in the model. With no routes there is
            // none, so the fallback is a plain InvalidOperationException (a raw stack trace in the Software Factory output),
            // even though the cause is the user's file. The message is the same, and the JsonException is the inner exception.
            var ex = Should.Throw<InvalidOperationException>(() => ReadExistingFile(CreateTemplate("{ \"Routes\": [ ")));

            ex.GetType().ShouldBe(typeof(InvalidOperationException));
            ex.Message.ShouldStartWith("ocelot.json is not valid JSON");
            ex.Message.ShouldContain("Fix the file, or delete it");
            ex.InnerException.ShouldBeAssignableTo<System.Text.Json.JsonException>();
        }

        [Fact]
        public void Invalid_JSON_message_reports_the_line_as_an_editor_shows_it()
        {
            var ex = Should.Throw<InvalidOperationException>(() => ReadExistingFile(CreateTemplate("{\n  \"a\": 1,\n  \"b\": ,\n}")));

            ex.Message.ShouldContain("at line 3");
        }
    }
}
