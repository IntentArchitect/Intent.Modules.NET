using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Intent.Modules.Common.Drift;
using Shouldly;
using Xunit;
using static Intent.Modules.Guardrails.Contracts.Tests.Drift;

namespace Intent.Modules.Guardrails.Contracts.Tests
{
    /// <summary>
    /// The six facts the 2026-10-20 authority audit found wrong in a client's service (ClientA), which the old drift check
    /// reported as No drift. The code is copied from ClientA with its name replaced by Acme, the recipes are ClientA's, and
    /// the model is the audit's own dump of ClientA's model: independent of the module's metadata readers.
    /// </summary>
    public class AcmeRegressionTests
    {
        private static readonly string[] CodeFiles =
        {
            "Acme.Data/Model/AcmeContext.cs",
            "Acme.Data/Model/WebCommonDBContext.cs",
            "Acme.API/Controllers/Main/FeatureController.cs"
        };

        [Fact]
        public void The_Domain_check_reports_exactly_the_four_wrong_database_facts()
        {
            var report = Check("Domain", Fixture("Acme/guardrails/domain.drift.json"), DatabaseFacts(), Code());

            report.Wrong(DriftSection.Contradicted).ShouldBe(new[]
            {
                "`Application_Acme_Document_Doc.DocumentLink` text type: model `DEFAULT`, code `VARCHAR`. Location: `Acme.Data/Model/WebCommonDBContext.cs:20`",
                "`Application_Acme_Document_Log.DateAccesptedRead` column type: model (none), code `datetime`. Location: `Acme.Data/Model/WebCommonDBContext.cs:29`",
                "`IntegrationServices_Logs.AuthorizationToken` text type: model `DEFAULT`, code `VARCHAR`. Location: `Acme.Data/Model/WebCommonDBContext.cs:37`",
                "`sapPlants.differential` text type: model `VARCHAR`, code `CHAR`. Location: `Acme.Data/Model/AcmeContext.cs:56`"
            });
            report.Wrong(DriftSection.Unverifiable).ShouldBeEmpty();
            report.Wrong(DriftSection.Missing).ShouldBeEmpty();

            // sapPlantsMapping is configured but not part of this excerpt of the model.
            report.Wrong(DriftSection.Undescribed).ShouldBe(new[]
            {
                "entity configuration `sapPlantsMapping`: not modelled. Location: `Acme.Data/Model/AcmeContext.cs:70`"
            });
        }

        [Fact]
        public void The_Services_check_reports_exactly_the_two_wrong_routes()
        {
            var report = Check("Services", Fixture("Acme/guardrails/services.drift.json"), HttpFacts(), Code());

            report.WrongLines.ShouldBe(new[]
            {
                "`FeatureController.ClearCache` route: model `Cache/Clear/{CacheKey}/{cacheKey}`, code `Cache/Clear/{CacheKey}`. Location: `Acme.API/Controllers/Main/FeatureController.cs:38`",
                "`FeatureController.GetLogDocuments` route: model `Documents/Log/Get/{OneId}/{oneId}`, code `Documents/Log/Get/{OneId}`. Location: `Acme.API/Controllers/Main/FeatureController.cs:56`"
            });
            report.UnusedExceptions.ShouldBe(new[] { "`Acme.API/Controllers/HealthController.cs` (load-balancer probe)" });
        }

        [Fact]
        public void Every_wrong_line_is_an_error_in_the_Services_area_and_the_stale_exception_a_warning()
        {
            var report = Check("Services", Fixture("Acme/guardrails/services.drift.json"), HttpFacts(), Code());

            // No elements, so each finding is about the element's name rather than a live link.
            DriftFindings.Of(report).ShouldBe(new[]
            {
                "Error Model/Code | text:`FeatureController.ClearCache` | Acme.API/Controllers/Main/FeatureController.cs:38 | `FeatureController.ClearCache` route: model `Cache/Clear/{CacheKey}/{cacheKey}`, code `Cache/Clear/{CacheKey}`",
                "Error Model/Code | text:`FeatureController.GetLogDocuments` | Acme.API/Controllers/Main/FeatureController.cs:56 | `FeatureController.GetLogDocuments` route: model `Documents/Log/Get/{OneId}/{oneId}`, code `Documents/Log/Get/{OneId}`",
                "Warning Code | guardrails/services.drift.json | — | The exception `Acme.API/Controllers/HealthController.cs` (load-balancer probe) matches nothing: remove it from `guardrails/services.drift.json`."
            });
        }

        /// <summary>
        /// C0 M22 of the 2026-10-20 adversarial test: the same route, read from a constant. The recipe's default stood
        /// in, so the identical route was reported as contradicted ("code (none)"), which invited a wrong model fix.
        /// </summary>
        [Fact]
        public void A_route_read_from_a_constant_can_not_be_verified_rather_than_contradicting_the_model()
        {
            const string controller = """
            namespace Acme.API.Controllers.Sales
            {
                internal static class AcmeRoutes
                {
                    public const string OriginItemDistances = "Get/OriginItemDistances";
                }

                [Authorization]
                [Produces("application/json")]
                [ApiController]
                [Route("v1/[controller]")]
                public class AcmeController : ControllerBase
                {
                    /// <summary>Get all origin to-from item distances.</summary>
                    [Route(AcmeRoutes.OriginItemDistances)]
                    [HttpGet]
                    public IActionResult GetOriginItemDistances()
                    {
                        return Ok(originItemDistanceBO.Get());
                    }

                    /// <summary>Get specific origin to-from item distance.</summary>
                    [Route("Get/OriginItemDistance/{origin}/{item}")]
                    [HttpGet]
                    public IActionResult GetOriginItemDistance(string origin, string item)
                    {
                        return Ok(originItemDistanceBO.GetDistance(origin, item));
                    }
                }
            }
            """;
            var model = new List<ModelFact>
            {
                Fact(FactKinds.ServiceRoute, "AcmeController", "v1/Acme", ("Service", "AcmeController")),
                Operation(FactKinds.OperationVerb, "AcmeController", "GetOriginItemDistances", "GET"),
                Operation(FactKinds.OperationRoute, "AcmeController", "GetOriginItemDistances", "Get/OriginItemDistances"),
                Operation(FactKinds.Security, "AcmeController", "GetOriginItemDistances", "Secured"),
                Operation(FactKinds.OperationVerb, "AcmeController", "GetOriginItemDistance", "GET", "origin", "item"),
                Operation(FactKinds.OperationRoute, "AcmeController", "GetOriginItemDistance", "Get/OriginItemDistance/{origin}/{item}", "origin", "item"),
                Operation(FactKinds.Security, "AcmeController", "GetOriginItemDistance", "Secured", "origin", "item"),
                Parameter("AcmeController", "GetOriginItemDistance", "origin", "From Route"),
                Parameter("AcmeController", "GetOriginItemDistance", "item", "From Route")
            };

            var report = Check("Services", Fixture("Acme/guardrails/services.drift.json"), model,
                new Dictionary<string, string> { ["Acme.API/Controllers/Sales/AcmeController.cs"] = controller });

            report.WrongLines.ShouldBe(new[]
            {
                "`AcmeController.GetOriginItemDistances` route: stated in a form the recipe can't read: `[Route(AcmeRoutes.OriginItemDistances)]`. Location: `Acme.API/Controllers/Sales/AcmeController.cs:15`"
            });
            report.Wrong(DriftSection.Unverifiable).Count.ShouldBe(1);
        }

        /// <summary>
        /// C0 M16: a new DTO beside the modelled ones in <c>Acme.DTO/Sales</c>. No recipe owned DTOs, so nothing was
        /// reported. ClientA's DTOs follow a convention (every <c>*DTO</c> class in the DTO project), which the recipe
        /// file now records.
        /// </summary>
        [Fact]
        public void A_new_DTO_that_follows_the_DTO_convention_is_not_modelled()
        {
            const string itemsDto = """
            namespace Acme.DTO.Sales
            {
                public class ItemsDTO
                {
                    public string Code { get; set; }
                    public string Description { get; set; }
                    public string Province { get; set; }
                    public decimal? CapacityTons { get; set; }
                }
            }
            """;
            const string itemSummaryDto = """
            namespace Acme.DTO.Sales
            {
                public class ItemSummaryDTO
                {
                    public string Code { get; set; }
                    public int AlertCount { get; set; }
                }
            }
            """;

            const string recipe = """
            { "own": [{ "kind": "DTO", "find": "class with \\bclass \\w+DTO\\b", "files": "Acme.DTO/**/*.cs" }] }
            """;

            var report = Check("Services", recipe, new List<ModelFact>(),
                new Dictionary<string, string>
                {
                    ["Acme.DTO/Sales/ItemsDTO.cs"] = itemsDto,
                    ["Acme.DTO/Sales/ItemSummaryDTO.cs"] = itemSummaryDto
                },
                new[] { new GeneratedFile { RelativePath = "Acme.DTO/Sales/ItemsDTO.cs", Text = itemsDto } });

            report.WrongLines.ShouldBe(new[]
            {
                "DTO `ItemSummaryDTO`: not modelled. Location: `Acme.DTO/Sales/ItemSummaryDTO.cs:3`"
            });
        }

        /// <summary>
        /// C0 M17: a new business class beside the modelled ones. The recipe's base-type pattern names it; the modelled
        /// business classes it also finds are generated, so they count as described.
        /// </summary>
        [Fact]
        public void A_new_business_class_is_found_by_its_base_type_and_the_modelled_ones_are_described()
        {
            const string items = """
            namespace Acme.Business.BusinessObject.Sales
            {
                public class Items : BaseBusinessObject
                {
                    public Items(ITokenService tokenService) : base(tokenService)
                    {
                    }

                    public int CountEnabled() => Repo.Find<items>(x => x.enabled).Count();
                }
            }
            """;
            const string generatedItems = """
            namespace Acme.Business.BusinessObject.Sales
            {
                public class Items : BaseBusinessObject
                {
                    public Items(ITokenService tokenService) : base(tokenService)
                    {
                    }

                    public int CountEnabled()
                    {
                    }
                }
            }
            """;
            const string itemReport = """
            using System.Linq;
            using Acme.Business.Helper;
            using Acme.Data.Model.Entities;

            namespace Acme.Business.BusinessObject.Sales
            {
                public class ItemReport : BaseBusinessObject
                {
                    public ItemReport(ITokenService tokenService) : base(tokenService)
                    {
                    }

                    public int CountProvinces() => Repo.Find<items>(x => x.enabled).Select(s => s.province).Distinct().Count();
                }
            }
            """;
            const string recipe = """
            {
              "files": "Acme.Business/**/*.cs",
              "own": [{ "kind": "business class", "find": "class with :BaseBusinessObject\\b" }]
            }
            """;

            var report = Check("Services", recipe, new List<ModelFact>(),
                new Dictionary<string, string>
                {
                    ["Acme.Business/BusinessObject/Sales/Items.cs"] = items,
                    ["Acme.Business/BusinessObject/Sales/ItemReport.cs"] = itemReport
                },
                new[] { new GeneratedFile { RelativePath = "Acme.Business/BusinessObject/Sales/Items.cs", Text = generatedItems } });

            report.WrongLines.ShouldBe(new[]
            {
                "business class `ItemReport`: not modelled. Location: `Acme.Business/BusinessObject/Sales/ItemReport.cs:7`"
            });
        }

        /// <summary>C0 M20: an index renamed and made unique. The model's <c>Index</c> stereotypes had no facts, so nothing was reported.</summary>
        [Fact]
        public void An_index_renamed_and_made_unique_contradicts_the_model_and_the_new_one_is_not_modelled()
        {
            const string context = """
            public partial class AcmeContext : DbContext
            {
                protected override void OnModelCreating(ModelBuilder modelBuilder)
                {
                    modelBuilder.Entity<acmeLinkedMembers>(entity =>
                    {
                        entity.HasIndex(e => new { e.Username, e.ClientNumber }, "IX_acmeLinkedMembers_UsernameClientNumber");
                    });

                    modelBuilder.Entity<transaction>(entity =>
                    {
                        entity.HasKey(e => e.number).HasName("PK_Transactions");

                        entity.ToTable("transactions", "sale");

                        entity.HasIndex(e => e.fillID, "IX_Transactions_FillID");

                        entity.Property(e => e.number).ValueGeneratedNever();
                    });
                }
            }
            """;
            static ModelFact Index(FactKind kind, string entity, string index, string value) =>
                Fact(kind, $"{entity}.{index}", value, ("Entity", entity), ("Index", index));
            var model = new List<ModelFact>
            {
                Index(FactKinds.IndexColumns, "acmeLinkedMembers", "IX_acmeLinkedMembers_UsernameClientNumber", "Username,ClientNumber"),
                Index(FactKinds.IndexUnique, "acmeLinkedMembers", "IX_acmeLinkedMembers_UsernameClientNumber", "false"),
                Index(FactKinds.IndexColumns, "transaction", "IX_Transactions_FillID", "fillID"),
                Index(FactKinds.IndexUnique, "transaction", "IX_Transactions_FillID", "false"),
                Table(FactKinds.TableName, "transaction", "transactions")
            };
            var recipe = Fixture("Acme/guardrails/domain.drift.json");
            static Dictionary<string, string> Code(string text) => new() { ["Acme.Data/Model/AcmeContext.cs"] = text };

            Check("Domain", recipe, model, Code(context)).WrongLines.ShouldBeEmpty();

            var report = Check("Domain", recipe, model, Code(context.Replace(
                "entity.HasIndex(e => e.fillID, \"IX_Transactions_FillID\");",
                "entity.HasIndex(e => e.fillID, \"IX_Transactions_FillID_U\").IsUnique();")));

            report.WrongLines.ShouldBe(new[]
            {
                "`transaction.IX_Transactions_FillID` columns: model `fillID`, code (none) (the code doesn't state it). Near: `Acme.Data/Model/AcmeContext.cs:10`",
                "`transaction.IX_Transactions_FillID` unique: model `false`, code (none) (the code doesn't state it). Near: `Acme.Data/Model/AcmeContext.cs:10`",
                "index `IX_Transactions_FillID_U`: not modelled. Location: `Acme.Data/Model/AcmeContext.cs:16`"
            });
        }

        /// <summary>
        /// C0 M13's side line: once the model's primary key was removed from a key column, its required fact read the
        /// column as optional, although EF Core always requires a key column.
        /// </summary>
        [Fact]
        public void A_key_column_is_read_as_required()
        {
            const string context = """
            public partial class AcmeContext : DbContext
            {
                protected override void OnModelCreating(ModelBuilder modelBuilder)
                {
                    modelBuilder.Entity<originItemDistance>(entity =>
                    {
                        entity.HasKey(e => new { e.originCode, e.itemCode });

                        entity.ToTable("originItemDistance", "sales");

                        entity.Property(e => e.itemCode)
                            .HasMaxLength(10)
                            .IsUnicode(false);
                    });
                }
            }
            """;
            var model = new List<ModelFact>
            {
                Table(FactKinds.Key, "originItemDistance", "originCode"),
                Column(FactKinds.ColumnRequired, "originItemDistance", "itemCode", "true"),
                Column(FactKinds.ColumnRequired, "originItemDistance", "originCode", "true")
            };

            var report = Check("Domain", Fixture("Acme/guardrails/domain.drift.json"), model,
                new Dictionary<string, string> { ["Acme.Data/Model/AcmeContext.cs"] = context });

            report.WrongLines.ShouldBe(new[]
            {
                "`originItemDistance` primary key: model `originCode`, code `originCode,itemCode`. Location: `Acme.Data/Model/AcmeContext.cs:7`"
            });
        }

        private static Dictionary<string, string> Code() => CodeFiles.ToDictionary(x => x, x => Fixture(Path.Combine("Acme", x)));

        /// <summary>The audit's dump of the Domain model, as the facts the model states.</summary>
        private static List<ModelFact> DatabaseFacts()
        {
            var facts = new List<ModelFact>();
            var lines = Fixture("Acme/model-db.txt").Split('\n').Select(x => x.TrimEnd('\r')).Where(x => x.Length > 0 && !x.StartsWith("#")).Select(x => x.Split('|')).ToList();
            foreach (var entity in lines.Where(x => x[0] == "E"))
            {
                var name = entity[1];
                facts.Add(Table(FactKinds.TableName, name, entity[3]));
                facts.Add(Table(FactKinds.TableSchema, name, entity[4]));
                facts.Add(Table(FactKinds.TableKind, name, entity[2]));
                var attributes = lines.Where(x => x[0] == "A" && x[1] == name).ToList();
                facts.Add(Table(FactKinds.Key, name, string.Join(",", attributes.Where(x => x[5] == "pk").Select(x => x[2]))));
                foreach (var a in attributes)
                {
                    var (attribute, type, nullable, isKey) = (a[2], a[3], a[4] == "null", a[5] == "pk");
                    facts.Add(Column(FactKinds.ColumnName, name, attribute, a[11]));
                    facts.Add(Column(FactKinds.ColumnType, name, attribute, a[10]));
                    if (type == "string")
                    {
                        facts.Add(Column(FactKinds.ColumnTextType, name, attribute, a[6].Length > 0 ? a[6] : "DEFAULT"));
                        facts.Add(Column(FactKinds.ColumnMaxLength, name, attribute, a[7]));
                        if (!isKey)
                        {
                            facts.Add(Column(FactKinds.ColumnRequired, name, attribute, nullable ? "false" : "true"));
                        }
                    }

                    if (type == "decimal")
                    {
                        facts.Add(Column(FactKinds.ColumnPrecision, name, attribute, a[8]));
                        facts.Add(Column(FactKinds.ColumnScale, name, attribute, a[9]));
                    }
                }
            }

            return facts;
        }

        /// <summary>The audit's dump of the Services model, as the facts the model states.</summary>
        private static List<ModelFact> HttpFacts()
        {
            static string Security(string value) => value switch { "sec" => "Secured", "unsec" => "Unsecured", _ => "None" };

            var facts = new List<ModelFact>();
            var lines = Fixture("Acme/model-http.txt").Split('\n').Select(x => x.TrimEnd('\r')).Where(x => x.Length > 0 && !x.StartsWith("#")).Select(x => x.Split('|')).ToList();
            foreach (var service in lines.Where(x => x[0] == "S"))
            {
                facts.Add(Fact(FactKinds.ServiceRoute, service[1], service[2], ("Service", service[1])));
                foreach (var operation in lines.Where(x => x[0] == "O" && x[1] == service[1]))
                {
                    var parameters = operation[6].Split(',', StringSplitOptions.RemoveEmptyEntries).Select(x => x.Split(':')).ToList();
                    var names = parameters.Select(x => x[0]).ToArray();
                    facts.Add(Operation(FactKinds.OperationVerb, service[1], operation[2], operation[3], names));
                    facts.Add(Operation(FactKinds.OperationRoute, service[1], operation[2], operation[4], names));
                    facts.Add(Operation(FactKinds.Security, service[1], operation[2], Security(operation[5] != "-" ? operation[5] : service[3]), names));
                    foreach (var parameter in parameters)
                    {
                        facts.Add(Parameter(service[1], operation[2], parameter[0], parameter[1] == "default" ? "Default" : parameter[1]));
                    }
                }
            }

            return facts;
        }
    }
}
