using System.Collections.Generic;
using System.Linq;
using Intent.Modules.Common.Drift;
using Intent.Modules.Common.Drift.EngineMigration;
using Shouldly;
using Xunit;

namespace Intent.Modules.Guardrails.Contracts.Tests.EngineMigration
{
    /// <summary>
    /// The exceptions that keep the converted opt-in rows to what v1 reported: a row is a glob, where v1's convention reported
    /// only what its find matched, and the member row reports public fields, which v1 never did.
    /// </summary>
    public class V1CoverageTests
    {
        private const string Services = """
            {
              "files": ["Api/**/*.cs", "Dto/**/*.cs"],
              "own": [
                { "kind": "endpoint", "find": "class with :ControllerBase > method with [\\[,]Http(?:Get|Post)\\b", "name": "{Class}.{Method}" },
                { "kind": "DTO", "find": "class with \\bpublic class \\w+", "files": "Dto/**/*.cs" }
              ]
            }
            """;

        private static readonly Dictionary<string, string> Code = new()
        {
            ["Api/OrdersController.cs"] = """
                public class OrdersController : ControllerBase
                {
                    [HttpGet]
                    public OrderDto Get() => new OrderDto();
                }
                """,
            ["Api/NewController.cs"] = """
                public class NewController : ControllerBase
                {
                    [HttpPost]
                    public void Post() { }
                }
                """,
            ["Api/Startup.cs"] = "public class Startup { public void Configure() { } }",
            ["Api/ICache.cs"] = "public interface ICache { } public class Cache : ICache { }",
            ["Dto/OrderDto.cs"] = """
                public class OrderDto
                {
                    public int Id { get; set; }
                    public string note;
                    public LineDto line;
                }

                public class LineDto { }

                internal sealed class Helper { }

                public class RoleAttribute : System.Attribute { }
                """,
            ["Dto/Unmodelled.cs"] = "public class UnmodelledDto { }",
            ["Other/Tool.cs"] = "public class Tool { }"
        };

        private static readonly ModelledType[] Model =
        {
            new("OrdersController", "Service", new[] { "Get" }),
            new("OrderDto", "DTO", new[] { "Id", "line" }),
            new("LineDto", "DTO", new string[0])
        };

        [Fact]
        public void A_type_no_convention_found_is_excepted_with_the_conventions_named()
        {
            var rows = Exceptions(Services, Code, Model);

            var api = rows.Single(x => x.Reason == "v1's conventions over these files (endpoint) didn't report it");
            api.Rule.ShouldBe(RecipeConversion.UnmodelledType);
            api.Matches.ShouldBe(new[] { "ICache", "Cache", "Startup" }, ignoreOrder: true);

            // The endpoint row's recipe files take in the DTO folder too, where the DTO convention's row is.
            rows.Single(x => x.Reason == "v1's conventions over these files (endpoint, DTO) didn't report it").Matches.ShouldBe(new[] { "Helper" });
        }

        [Fact]
        public void A_type_a_convention_found_stays_reported_as_v1_reported_it()
        {
            var rows = Exceptions(Services, Code, Model);

            rows.SelectMany(x => x.Matches).ShouldNotContain("NewController");
            rows.SelectMany(x => x.Matches).ShouldNotContain("UnmodelledDto");
        }

        [Fact]
        public void A_modelled_type_gets_no_exception()
        {
            var rows = Exceptions(Services, Code, Model);

            rows.SelectMany(x => x.Matches).ShouldNotContain("OrdersController");
            rows.SelectMany(x => x.Matches).ShouldNotContain("OrderDto");
            rows.SelectMany(x => x.Matches).ShouldNotContain("LineDto");
        }

        [Fact]
        public void A_type_v1_counted_as_modelled_for_sharing_a_contract_file_is_excepted()
        {
            var rows = Exceptions(Services, Code, Model, contractFiles: new[] { "Dto/OrderDto.cs" });

            rows.Single(x => x.Reason == V1Coverage.ContractFileReason).Matches.ShouldBe(new[] { "RoleAttribute" });
        }

        [Fact]
        public void Without_its_contract_file_a_type_a_convention_found_stays_reported()
        {
            var rows = Exceptions(Services, Code, Model);

            rows.SelectMany(x => x.Matches).ShouldNotContain("RoleAttribute");
        }

        [Fact]
        public void A_type_outside_every_type_row_or_out_of_scope_gets_no_exception()
        {
            var rows = Exceptions(Services.Replace("\"own\": [", "\"exceptions\": [{ \"match\": \"Api/Startup.cs\", \"reason\": \"Hosting\" }], \"own\": ["), Code, Model);

            rows.SelectMany(x => x.Matches).ShouldNotContain("Tool");
            rows.SelectMany(x => x.Matches).ShouldNotContain("Startup");
        }

        [Fact]
        public void A_modelled_types_unmodelled_public_field_is_excepted_and_a_modelled_one_isnt()
        {
            var rows = Exceptions(Services, Code, Model);

            var row = rows.Single(x => x.Reason == V1Coverage.FieldReason);
            row.Rule.ShouldBe(RecipeConversion.UnmodelledMember);
            row.Matches.ShouldBe(new[] { "OrderDto.note" });
        }

        [Fact]
        public void A_service_with_no_type_of_its_name_is_its_interface_and_the_classes_implementing_it()
        {
            var model = Model.Append(new ModelledType("Cache", "Service", new string[0])).ToArray();
            var code = new Dictionary<string, string>(Code) { ["Api/ICache.cs"] = "public interface ICache { } public class RedisCache : ICache { } public class Other { }" };

            var matches = Exceptions(Services, code, model).SelectMany(x => x.Matches).ToList();
            matches.ShouldNotContain("ICache");
            matches.ShouldNotContain("RedisCache");
            matches.ShouldContain("Other");
        }

        [Fact]
        public void A_partial_type_a_convention_found_in_one_of_its_files_stays_reported()
        {
            var code = new Dictionary<string, string>(Code)
            {
                ["Api/ReportsController.cs"] = "public partial class ReportsController : ControllerBase { [HttpGet] public void Get() { } }",
                ["Api/ReportsControllerExt.cs"] = "public partial class ReportsController { private void Helper() { } }"
            };

            Exceptions(Services, code, Model).SelectMany(x => x.Matches).ShouldNotContain("ReportsController");
        }

        [Fact]
        public void A_modelled_classs_interfaces_and_its_base_classs_are_its_code()
        {
            var code = new Dictionary<string, string>(Code)
            {
                ["Api/OrdersController.cs"] = """
                    public interface IOrders : IAudited { }
                    public interface IAudited { }
                    public interface IVersioned { }
                    public abstract class ApiController : ControllerBase, IVersioned { }
                    public class OrdersController : ApiController, IOrders
                    {
                        [HttpGet]
                        public OrderDto Get() => new OrderDto();
                    }
                    """
            };

            var matches = Exceptions(Services, code, Model).SelectMany(x => x.Matches).ToList();
            matches.ShouldNotContain("IOrders");
            matches.ShouldNotContain("IAudited");
            matches.ShouldNotContain("IVersioned");
            matches.ShouldContain("ApiController");
        }

        private static IReadOnlyList<ExceptionRowSpec> Exceptions(string services, Dictionary<string, string> code, ModelledType[] model, string[] contractFiles = null)
        {
            var recipes = new[] { new RecipeSource("services.drift.json", services) };
            var rows = RecipeConversion.Convert(recipes);
            var notes = new List<string>();
            var result = V1Coverage.Exceptions(new CodeFiles(code), recipes, rows, contractFiles ?? new string[0], model, notes);
            notes.ShouldBeEmpty();
            return result;
        }
    }
}
