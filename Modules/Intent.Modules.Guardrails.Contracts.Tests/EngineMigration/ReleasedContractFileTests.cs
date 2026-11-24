using System.IO;
using Intent.Engine;
using Intent.Modules.Common.Drift.EngineMigration;
using Intent.Templates;
using NSubstitute;
using Shouldly;
using Xunit;

namespace Intent.Modules.Guardrails.Contracts.Tests.EngineMigration
{
    /// <summary>
    /// A teammate's first run keeps the contract files: each one their log still lists gets a stand-in the Software Factory can't
    /// overwrite, under the contract entry's correlation id, in the output target the file was generated into.
    /// </summary>
    public class ReleasedContractFileTests
    {
        private static readonly string Root = Path.GetFullPath(Path.Combine(Path.GetTempPath(), "released-contract-file"));
        private static readonly string Order = Path.Combine(Root, "src", "Acme.Domain", "Entities", "Order.cs");

        [Fact]
        public void A_stand_in_is_a_file_it_cant_overwrite_at_the_contract_files_path_under_its_correlation_id()
        {
            var template = new ReleasedContractFileTemplate(
                Target("7b3d", Path.Combine(Root, "src", "Acme.Domain")),
                new ReleasedContractFileTemplate.ReleasedFile(Order, "Intent.Domain.Guardrails.Contracts.EntityContract#1c5f"));

            var config = template.GetTemplateFileConfig();

            config.FileName.ShouldBe("Order");
            config.FileExtension.ShouldBe("cs");
            config.LocationInProject.ShouldBe("Entities");
            config.OverwriteBehaviour.ShouldBe(OverwriteBehaviour.OverwriteDisabled);
            template.GetCorrelationId().ShouldBe("Intent.Domain.Guardrails.Contracts.EntityContract#1c5f");
        }

        [Fact]
        public void A_file_directly_in_its_output_target_has_no_folder()
        {
            var template = new ReleasedContractFileTemplate(
                Target("7b3d", Path.Combine(Root, "src", "Acme.Domain", "Entities")),
                new ReleasedContractFileTemplate.ReleasedFile(Order, "Intent.Domain.Guardrails.Contracts.EntityContract#1c5f"));

            template.GetTemplateFileConfig().LocationInProject.ShouldBe(string.Empty);
        }

        [Fact]
        public void The_output_target_the_log_recorded_is_used_while_it_holds_the_file()
        {
            var root = Target("root", Root);
            var project = Target("7b3d", Path.Combine(Root, "src", "Acme.Domain"));

            ReleasedContractFileRegistration.OutputTargetFor(new[] { project, root }, "root", Order).ShouldBeSameAs(root);
        }

        [Fact]
        public void Without_the_recorded_output_target_the_innermost_one_holding_the_file_is_used()
        {
            var root = Target("root", Root);
            var project = Target("7b3d", Path.Combine(Root, "src", "Acme.Domain"));
            var elsewhere = Target("9f0a", Path.Combine(Root, "src", "Acme.Domain.Tests"));

            ReleasedContractFileRegistration.OutputTargetFor(new[] { root, elsewhere, project }, "gone", Order).ShouldBeSameAs(project);
            ReleasedContractFileRegistration.OutputTargetFor(new[] { root, elsewhere, project }, null, Order).ShouldBeSameAs(project);
        }

        [Fact]
        public void A_recorded_output_target_that_no_longer_holds_the_file_is_passed_over()
        {
            var root = Target("root", Root);
            var moved = Target("7b3d", Path.Combine(Root, "src", "Acme.Core"));

            ReleasedContractFileRegistration.OutputTargetFor(new[] { moved, root }, "7b3d", Order).ShouldBeSameAs(root);
        }

        [Fact]
        public void No_output_target_holding_the_file_means_no_stand_in()
        {
            var elsewhere = Target("9f0a", Path.Combine(Root, "src", "Acme.Domain.Tests"));

            ReleasedContractFileRegistration.OutputTargetFor(new[] { elsewhere }, "9f0a", Order).ShouldBeNull();
        }

        private static IOutputTarget Target(string id, string location)
        {
            var target = Substitute.For<IOutputTarget>();
            target.Id.Returns(id);
            target.Location.Returns(location);
            return target;
        }
    }
}
