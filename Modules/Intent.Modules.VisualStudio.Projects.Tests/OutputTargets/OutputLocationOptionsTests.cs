using System.IO;
using Intent.Modules.VisualStudio.Projects.OutputTargets;
using Shouldly;
using Xunit;

namespace Intent.Modules.VisualStudio.Projects.Tests.OutputTargets
{
    public class OutputLocationOptionsTests
    {
        private static readonly string ContainerDirectory = Path.Combine(Path.GetTempPath(), "Container");
        private static readonly string MyAppDirectory = Path.Combine(ContainerDirectory, "MyApp");

        [Fact]
        public void WhenRelativeLocationUnset_RootDirectory_ShouldBeOutputRootDirectory()
        {
            var sut = new OutputLocationOptions(MyAppDirectory, relativeLocation: "");

            sut.RootDirectory.ShouldBe(MyAppDirectory);
        }

        [Fact]
        public void WhenRelativeLocationUnset_Combine_ShouldReturnValueUnchanged()
        {
            var sut = new OutputLocationOptions(MyAppDirectory, relativeLocation: "");

            sut.Combine("MyProject").ShouldBe("MyProject");
        }

        [Fact]
        public void WhenRelativeLocationIsParentShift_RootDirectory_ShouldResolveToContainer()
        {
            var sut = new OutputLocationOptions(MyAppDirectory, relativeLocation: "..");

            sut.RootDirectory.ShouldBe(ContainerDirectory);
        }

        [Fact]
        public void WhenRelativeLocationIsParentShift_Combine_ShouldPrefixTheShift()
        {
            var sut = new OutputLocationOptions(MyAppDirectory, relativeLocation: "..");

            sut.Combine("MyApp").ShouldBe(Path.Combine("..", "MyApp"));
        }

        [Fact]
        public void WhenRawValueIsExplicit_GetEffectiveRelativeLocation_ShouldBypassTheShift()
        {
            var sut = new OutputLocationOptions(MyAppDirectory, relativeLocation: "..");

            var explicitLocation = Path.Combine(".", "Chain", "Chain");
            sut.GetEffectiveRelativeLocation(rawRelativeLocation: explicitLocation, fallbackName: "Chain")
                .ShouldBe(explicitLocation);
        }

        [Fact]
        public void WhenRawValueIsBlank_GetEffectiveRelativeLocation_ShouldCombineShiftWithName()
        {
            var sut = new OutputLocationOptions(Path.Combine(ContainerDirectory, "Intent.Modules.Foo"), relativeLocation: "..");

            sut.GetEffectiveRelativeLocation(rawRelativeLocation: null, fallbackName: "Intent.Modules.Foo")
                .ShouldBe(Path.Combine("..", "Intent.Modules.Foo"));
        }

        [Fact]
        public void None_GetEffectiveRelativeLocation_ShouldFallBackToNameUnshifted()
        {
            OutputLocationOptions.None.GetEffectiveRelativeLocation(rawRelativeLocation: "", fallbackName: "Intent.Modules.Foo")
                .ShouldBe("Intent.Modules.Foo");
        }
    }
}
