using Intent.Engine;
using Intent.Modules.VisualStudio.Projects.FactoryExtensions;
using NSubstitute;
using Shouldly;
using Xunit;

namespace Intent.Modules.VisualStudio.Projects.Tests.Templates.SlnxFile
{
    public class VisualStudioSolutionSlnxWeaverTests
    {
        [Fact]
        public void Transform_WhenSwitchedFromSlnToSlnx_ShouldIgnoreOldSlnHistoryAndGenerateFresh()
        {
            // Real-world case reported against v4.1.6: a brand new solution defaults to .sln, then the
            // developer switches its "Solution File Format" setting to .slnx. VisualStudioSolutionTemplate
            // and VisualStudioSolutionSlnxTemplate share the same output Id, so PreviousFilePath still
            // resolves to the OLD "MyApp.sln" file - whose content is classic .sln text, not XML at all.
            const string previousSlnContent = """
                Microsoft Visual Studio Solution File, Format Version 12.00
                # Visual Studio Version 17
                Project("{FAE04EC0-301F-11D3-BF4B-00C04F79EFBC}") = "MyApp.Domain", "MyApp.Domain\MyApp.Domain.csproj", "{11111111-1111-1111-1111-111111111111}"
                EndProject
                """;

            const string generated = """
                <Solution>
                  <Folder Name="/3 - Domain/">
                    <Project Path="MyApp.Domain/MyApp.Domain.csproj" />
                  </Folder>
                </Solution>
                """;

            var output = Substitute.For<IOutputFile>();
            output.Content.Returns(generated);
            output.PreviousFilePath.Returns(@"C:\MyApp\MyApp.sln");
            output.TargetFilePath.Returns(@"C:\MyApp\MyApp.slnx");
            output.GetPreviousFilePathContent().Returns(previousSlnContent);
            output.GetPreviousTemplateOutput().Returns(previousSlnContent);

            var weaver = new VisualStudioSolutionSlnxWeaver();

            Should.NotThrow(() => weaver.Transform(output));

            output.Received(1).ChangeContent(Arg.Is<string>(c => c.Contains("MyApp.Domain.csproj")));
        }

        [Fact]
        public void Transform_WhenNotAFormatSwitch_ShouldStillMergeAgainstExistingSlnxHistory()
        {
            // Sanity check the guard doesn't also suppress genuine, same-format merge history.
            const string previousOutput = """
                <Solution>
                  <Folder Name="/3 - Domain/">
                    <Project Id="11111111-1111-1111-1111-111111111111" Path="MyApp.Domain/MyApp.Domain.csproj" />
                  </Folder>
                </Solution>
                """;

            const string existing = """
                <Solution>
                  <Folder Name="/3 - Domain/">
                    <Project Path="MyApp.Domain/MyApp.Domain.csproj" />
                  </Folder>
                </Solution>
                """;

            const string generated = """
                <Solution>
                  <Folder Name="/3 - Core/">
                    <Project Id="11111111-1111-1111-1111-111111111111" Path="MyApp.Core/MyApp.Core.csproj" />
                  </Folder>
                </Solution>
                """;

            var output = Substitute.For<IOutputFile>();
            output.Content.Returns(generated);
            output.PreviousFilePath.Returns(@"C:\MyApp\MyApp.slnx");
            output.TargetFilePath.Returns(@"C:\MyApp\MyApp.slnx");
            output.GetPreviousFilePathContent().Returns(existing);
            output.GetPreviousTemplateOutput().Returns(previousOutput);

            var weaver = new VisualStudioSolutionSlnxWeaver();

            weaver.Transform(output);

            output.Received(1).ChangeContent(Arg.Is<string>(c => c.Contains("MyApp.Core.csproj") && !c.Contains("MyApp.Domain.csproj")));
        }
    }
}
