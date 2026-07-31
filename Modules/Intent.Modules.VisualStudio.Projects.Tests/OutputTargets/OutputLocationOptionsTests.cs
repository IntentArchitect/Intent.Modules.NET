using Intent.Modules.VisualStudio.Projects.OutputTargets;
using Shouldly;
using Xunit;

namespace Intent.Modules.VisualStudio.Projects.Tests.OutputTargets
{
    public class OutputLocationOptionsTests
    {
        [Fact]
        public void WhenRelativeLocationUnset_RootDirectory_ShouldBeOutputRootDirectory()
        {
            var sut = new OutputLocationOptions(@"C:\Container\MyApp", relativeLocation: "");

            sut.RootDirectory.ShouldBe(@"C:\Container\MyApp");
        }

        [Fact]
        public void WhenRelativeLocationUnset_Combine_ShouldReturnValueUnchanged()
        {
            var sut = new OutputLocationOptions(@"C:\Container\MyApp", relativeLocation: "");

            sut.Combine("MyProject").ShouldBe("MyProject");
        }

        [Fact]
        public void WhenRelativeLocationIsParentShift_RootDirectory_ShouldResolveToContainer()
        {
            var sut = new OutputLocationOptions(@"C:\Container\MyApp", relativeLocation: "..");

            sut.RootDirectory.ShouldBe(@"C:\Container");
        }

        [Fact]
        public void WhenRelativeLocationIsParentShift_Combine_ShouldPrefixTheShift()
        {
            var sut = new OutputLocationOptions(@"C:\Container\MyApp", relativeLocation: "..");

            sut.Combine("MyApp").ShouldBe(@"..\MyApp");
        }

        [Fact]
        public void WhenRawValueIsExplicit_GetEffectiveRelativeLocation_ShouldBypassTheShift()
        {
            var sut = new OutputLocationOptions(@"C:\Container\MyApp", relativeLocation: "..");

            sut.GetEffectiveRelativeLocation(rawRelativeLocation: @".\Chain\Chain", fallbackName: "Chain")
                .ShouldBe(@".\Chain\Chain");
        }

        [Fact]
        public void WhenRawValueIsBlank_GetEffectiveRelativeLocation_ShouldCombineShiftWithName()
        {
            var sut = new OutputLocationOptions(@"C:\Container\Intent.Modules.Foo", relativeLocation: "..");

            sut.GetEffectiveRelativeLocation(rawRelativeLocation: null, fallbackName: "Intent.Modules.Foo")
                .ShouldBe(@"..\Intent.Modules.Foo");
        }

        [Fact]
        public void None_GetEffectiveRelativeLocation_ShouldFallBackToNameUnshifted()
        {
            OutputLocationOptions.None.GetEffectiveRelativeLocation(rawRelativeLocation: "", fallbackName: "Intent.Modules.Foo")
                .ShouldBe("Intent.Modules.Foo");
        }
    }
}
