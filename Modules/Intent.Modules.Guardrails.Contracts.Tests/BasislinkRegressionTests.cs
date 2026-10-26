using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Intent.Modules.Domain.Guardrails.Contracts.Drift;
using Shouldly;
using Xunit;
using static Intent.Modules.Guardrails.Contracts.Tests.Drift;

namespace Intent.Modules.Guardrails.Contracts.Tests
{
    /// <summary>
    /// The six facts the 2026-10-20 authority audit found wrong in BasislinkService, which the old drift check
    /// reported as No drift. The code is copied from Basislink, the recipes are Basislink's, and the model is the
    /// audit's own dump of the Basislink model: independent of the module's metadata readers.
    /// </summary>
    public class BasislinkRegressionTests
    {
        private static readonly string[] CodeFiles =
        {
            "Basislink.Data/Model/BasislinkContext.cs",
            "Basislink.Data/Model/WebCommonDBContext.cs",
            "Basislink.API/Controllers/Main/FeatureController.cs"
        };

        [Fact]
        public void The_Domain_check_reports_exactly_the_four_wrong_database_facts()
        {
            var report = Check("Domain", Fixture("Basislink/guardrails/domain.drift.json"), DatabaseFacts(), Code());

            report.Wrong(DriftSection.Contradicted).ShouldBe(new[]
            {
                "`Application_Basislink_Document_Doc.DocumentLink` text type: model `DEFAULT`, code `VARCHAR`. Location: `Basislink.Data/Model/WebCommonDBContext.cs:20`",
                "`Application_Basislink_Document_Log.DateAccesptedRead` column type: model (none), code `datetime`. Location: `Basislink.Data/Model/WebCommonDBContext.cs:29`",
                "`IntegrationServices_Logs.AuthorizationToken` text type: model `DEFAULT`, code `VARCHAR`. Location: `Basislink.Data/Model/WebCommonDBContext.cs:37`",
                "`sapPlants.differential` text type: model `VARCHAR`, code `CHAR`. Location: `Basislink.Data/Model/BasislinkContext.cs:56`"
            });
            report.Wrong(DriftSection.Unverifiable).ShouldBeEmpty();
            report.Wrong(DriftSection.Missing).ShouldBeEmpty();

            // sapPlantsMapping is configured but not part of this excerpt of the model.
            report.Wrong(DriftSection.Undescribed).ShouldBe(new[]
            {
                "entity configuration `sapPlantsMapping`: not modelled. Location: `Basislink.Data/Model/BasislinkContext.cs:70`"
            });
        }

        [Fact]
        public void The_Services_check_reports_exactly_the_two_wrong_routes()
        {
            var report = Check("Services", Fixture("Basislink/guardrails/services.drift.json"), HttpFacts(), Code());

            report.WrongLines.ShouldBe(new[]
            {
                "`FeatureController.ClearCache` route: model `Cache/Clear/{CacheKey}/{cacheKey}`, code `Cache/Clear/{CacheKey}`. Location: `Basislink.API/Controllers/Main/FeatureController.cs:38`",
                "`FeatureController.GetLogDocuments` route: model `Documents/Log/Get/{OneId}/{oneId}`, code `Documents/Log/Get/{OneId}`. Location: `Basislink.API/Controllers/Main/FeatureController.cs:56`"
            });
            report.UnusedExceptions.ShouldBe(new[] { "`Basislink.API/Controllers/HealthController.cs` (load-balancer probe)" });
        }

        [Fact]
        public void The_report_and_the_failure_list_every_wrong_line()
        {
            var report = Check("Services", Fixture("Basislink/guardrails/services.drift.json"), HttpFacts(), Code());

            // No application, so the model elements are their names rather than live links.
            report.ToMarkdown().ShouldBe("""
                # Services drift report

                ## Model and code drift

                | Model element | Code file | Drift |
                | --- | --- | --- |
                | `FeatureController.ClearCache` | <FileRef path="Basislink.API/Controllers/Main/FeatureController.cs" line={38} /> | `FeatureController.ClearCache` route: model `Cache/Clear/{CacheKey}/{cacheKey}`, code `Cache/Clear/{CacheKey}` |
                | `FeatureController.GetLogDocuments` | <FileRef path="Basislink.API/Controllers/Main/FeatureController.cs" line={56} /> | `FeatureController.GetLogDocuments` route: model `Documents/Log/Get/{OneId}/{oneId}`, code `Documents/Log/Get/{OneId}` |

                ## Warnings

                | Warning | Detail |
                | --- | --- |
                | Exception matches nothing | `Basislink.API/Controllers/HealthController.cs` (load-balancer probe): remove it from `guardrails/services.drift.json`. |

                """);
            report.FailureMessage(@"intent\.cache\guardrails\BasislinkService\ServicesDriftReport.mdx", listLines: false).ShouldBe(
                "The Services model and the code have drifted: 2 drifts detected and listed in the drift report (intent/.cache/guardrails/BasislinkService/ServicesDriftReport.mdx).");
            report.FailureMessage("unused on CI", listLines: true).ShouldStartWith(
                "The Services model and the code have drifted: 2 drifts detected:\n\n" +
                "- `FeatureController.ClearCache` route: model `Cache/Clear/{CacheKey}/{cacheKey}`, code `Cache/Clear/{CacheKey}`");
        }

        /// <summary>
        /// C0 M22 of the 2026-10-20 adversarial test: the same route, read from a constant. The recipe's default stood
        /// in, so the identical route was reported as contradicted ("code (none)"), which invited a wrong model fix.
        /// </summary>
        [Fact]
        public void A_route_read_from_a_constant_can_not_be_verified_rather_than_contradicting_the_model()
        {
            const string controller = """
            namespace Basislink.API.Controllers.Ssb
            {
                internal static class BasisRoutes
                {
                    public const string SiloMillDistances = "Get/SiloMillDistances";
                }

                [Authorization]
                [Produces("application/json")]
                [ApiController]
                [Route("v1/[controller]")]
                public class BasisController : ControllerBase
                {
                    /// <summary>Get all silo to-from mill distances.</summary>
                    [Route(BasisRoutes.SiloMillDistances)]
                    [HttpGet]
                    public IActionResult GetSiloMillDistances()
                    {
                        return Ok(siloMillDistanceBO.Get());
                    }

                    /// <summary>Get specific silo to-from mill distance.</summary>
                    [Route("Get/SiloMillDistance/{silo}/{mill}")]
                    [HttpGet]
                    public IActionResult GetSiloMillDistance(string silo, string mill)
                    {
                        return Ok(siloMillDistanceBO.GetDistance(silo, mill));
                    }
                }
            }
            """;
            var model = new List<ModelFact>
            {
                Fact(FactKinds.ServiceRoute, "BasisController", "v1/Basis", ("Service", "BasisController")),
                Operation(FactKinds.OperationVerb, "BasisController", "GetSiloMillDistances", "GET"),
                Operation(FactKinds.OperationRoute, "BasisController", "GetSiloMillDistances", "Get/SiloMillDistances"),
                Operation(FactKinds.Security, "BasisController", "GetSiloMillDistances", "Secured"),
                Operation(FactKinds.OperationVerb, "BasisController", "GetSiloMillDistance", "GET", "silo", "mill"),
                Operation(FactKinds.OperationRoute, "BasisController", "GetSiloMillDistance", "Get/SiloMillDistance/{silo}/{mill}", "silo", "mill"),
                Operation(FactKinds.Security, "BasisController", "GetSiloMillDistance", "Secured", "silo", "mill"),
                Parameter("BasisController", "GetSiloMillDistance", "silo", "From Route"),
                Parameter("BasisController", "GetSiloMillDistance", "mill", "From Route")
            };

            var report = Check("Services", Fixture("Basislink/guardrails/services.drift.json"), model,
                new Dictionary<string, string> { ["Basislink.API/Controllers/Ssb/BasisController.cs"] = controller });

            report.WrongLines.ShouldBe(new[]
            {
                "`BasisController.GetSiloMillDistances` route: stated in a form the recipe can't read: `[Route(BasisRoutes.SiloMillDistances)]`. Location: `Basislink.API/Controllers/Ssb/BasisController.cs:15`"
            });
            report.Wrong(DriftSection.Unverifiable).Count.ShouldBe(1);
        }

        /// <summary>
        /// C0 M16: a new DTO beside the modelled ones in <c>Basislink.DTO/Ssb</c>. No recipe owned DTOs, so nothing was
        /// reported. Basislink's DTOs follow a convention (every <c>*DTO</c> class in the DTO project), which the recipe
        /// file now records.
        /// </summary>
        [Fact]
        public void A_new_DTO_that_follows_the_DTO_convention_is_not_modelled()
        {
            const string millsDto = """
            namespace Basislink.DTO.Ssb
            {
                public class MillsDTO
                {
                    public string Code { get; set; }
                    public string Description { get; set; }
                    public string Province { get; set; }
                    public decimal? CapacityTons { get; set; }
                }
            }
            """;
            const string millSummaryDto = """
            namespace Basislink.DTO.Ssb
            {
                public class MillSummaryDTO
                {
                    public string Code { get; set; }
                    public int AlertCount { get; set; }
                }
            }
            """;

            const string recipe = """
            { "own": [{ "kind": "DTO", "find": "class with \\bclass \\w+DTO\\b", "files": "Basislink.DTO/**/*.cs" }] }
            """;

            var report = Check("Services", recipe, new List<ModelFact>(),
                new Dictionary<string, string>
                {
                    ["Basislink.DTO/Ssb/MillsDTO.cs"] = millsDto,
                    ["Basislink.DTO/Ssb/MillSummaryDTO.cs"] = millSummaryDto
                },
                new[] { new GeneratedFile { RelativePath = "Basislink.DTO/Ssb/MillsDTO.cs", Text = millsDto } });

            report.WrongLines.ShouldBe(new[]
            {
                "DTO `MillSummaryDTO`: not modelled. Location: `Basislink.DTO/Ssb/MillSummaryDTO.cs:3`"
            });
        }

        /// <summary>
        /// C0 M17: a new business class beside the modelled ones. The recipe's base-type pattern names it; the modelled
        /// business classes it also finds are generated, so they count as described.
        /// </summary>
        [Fact]
        public void A_new_business_class_is_found_by_its_base_type_and_the_modelled_ones_are_described()
        {
            const string mills = """
            namespace Basislink.Business.BusinessObject.Ssb
            {
                public class Mills : BaseBusinessObject
                {
                    public Mills(ITokenService tokenService) : base(tokenService)
                    {
                    }

                    public int CountEnabled() => Repo.Find<mills>(x => x.enabled).Count();
                }
            }
            """;
            const string generatedMills = """
            namespace Basislink.Business.BusinessObject.Ssb
            {
                public class Mills : BaseBusinessObject
                {
                    public Mills(ITokenService tokenService) : base(tokenService)
                    {
                    }

                    public int CountEnabled()
                    {
                    }
                }
            }
            """;
            const string millReport = """
            using System.Linq;
            using Basislink.Business.Helper;
            using Basislink.Data.Model.Entities;

            namespace Basislink.Business.BusinessObject.Ssb
            {
                public class MillReport : BaseBusinessObject
                {
                    public MillReport(ITokenService tokenService) : base(tokenService)
                    {
                    }

                    public int CountProvinces() => Repo.Find<mills>(x => x.enabled).Select(s => s.province).Distinct().Count();
                }
            }
            """;
            const string recipe = """
            {
              "files": "Basislink.Business/**/*.cs",
              "own": [{ "kind": "business class", "find": "class with :BaseBusinessObject\\b" }]
            }
            """;

            var report = Check("Services", recipe, new List<ModelFact>(),
                new Dictionary<string, string>
                {
                    ["Basislink.Business/BusinessObject/Ssb/Mills.cs"] = mills,
                    ["Basislink.Business/BusinessObject/Ssb/MillReport.cs"] = millReport
                },
                new[] { new GeneratedFile { RelativePath = "Basislink.Business/BusinessObject/Ssb/Mills.cs", Text = generatedMills } });

            report.WrongLines.ShouldBe(new[]
            {
                "business class `MillReport`: not modelled. Location: `Basislink.Business/BusinessObject/Ssb/MillReport.cs:7`"
            });
        }

        /// <summary>C0 M20: an index renamed and made unique. The model's <c>Index</c> stereotypes had no facts, so nothing was reported.</summary>
        [Fact]
        public void An_index_renamed_and_made_unique_contradicts_the_model_and_the_new_one_is_not_modelled()
        {
            const string context = """
            public partial class BasislinkContext : DbContext
            {
                protected override void OnModelCreating(ModelBuilder modelBuilder)
                {
                    modelBuilder.Entity<basislinkLinkedMembers>(entity =>
                    {
                        entity.HasIndex(e => new { e.Username, e.ClientNumber }, "IX_basislinkLinkedMembers_UsernameClientNumber");
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
                Index(FactKinds.IndexColumns, "basislinkLinkedMembers", "IX_basislinkLinkedMembers_UsernameClientNumber", "Username,ClientNumber"),
                Index(FactKinds.IndexUnique, "basislinkLinkedMembers", "IX_basislinkLinkedMembers_UsernameClientNumber", "false"),
                Index(FactKinds.IndexColumns, "transaction", "IX_Transactions_FillID", "fillID"),
                Index(FactKinds.IndexUnique, "transaction", "IX_Transactions_FillID", "false"),
                Table(FactKinds.TableName, "transaction", "transactions")
            };
            var recipe = Fixture("Basislink/guardrails/domain.drift.json");
            static Dictionary<string, string> Code(string text) => new() { ["Basislink.Data/Model/BasislinkContext.cs"] = text };

            Check("Domain", recipe, model, Code(context)).WrongLines.ShouldBeEmpty();

            var report = Check("Domain", recipe, model, Code(context.Replace(
                "entity.HasIndex(e => e.fillID, \"IX_Transactions_FillID\");",
                "entity.HasIndex(e => e.fillID, \"IX_Transactions_FillID_U\").IsUnique();")));

            report.WrongLines.ShouldBe(new[]
            {
                "`transaction.IX_Transactions_FillID` columns: model `fillID`, code (none) (the code doesn't state it). Near: `Basislink.Data/Model/BasislinkContext.cs:10`",
                "`transaction.IX_Transactions_FillID` unique: model `false`, code (none) (the code doesn't state it). Near: `Basislink.Data/Model/BasislinkContext.cs:10`",
                "index `IX_Transactions_FillID_U`: not modelled. Location: `Basislink.Data/Model/BasislinkContext.cs:16`"
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
            public partial class BasislinkContext : DbContext
            {
                protected override void OnModelCreating(ModelBuilder modelBuilder)
                {
                    modelBuilder.Entity<siloMillDistance>(entity =>
                    {
                        entity.HasKey(e => new { e.siloCode, e.millCode });

                        entity.ToTable("siloMillDistance", "ssb");

                        entity.Property(e => e.millCode)
                            .HasMaxLength(10)
                            .IsUnicode(false);
                    });
                }
            }
            """;
            var model = new List<ModelFact>
            {
                Table(FactKinds.Key, "siloMillDistance", "siloCode"),
                Column(FactKinds.ColumnRequired, "siloMillDistance", "millCode", "true"),
                Column(FactKinds.ColumnRequired, "siloMillDistance", "siloCode", "true")
            };

            var report = Check("Domain", Fixture("Basislink/guardrails/domain.drift.json"), model,
                new Dictionary<string, string> { ["Basislink.Data/Model/BasislinkContext.cs"] = context });

            report.WrongLines.ShouldBe(new[]
            {
                "`siloMillDistance` primary key: model `siloCode`, code `siloCode,millCode`. Location: `Basislink.Data/Model/BasislinkContext.cs:7`"
            });
        }

        private static Dictionary<string, string> Code() => CodeFiles.ToDictionary(x => x, x => Fixture(Path.Combine("Basislink", x)));

        /// <summary>The audit's dump of the Domain model, as the facts the model states.</summary>
        private static List<ModelFact> DatabaseFacts()
        {
            var facts = new List<ModelFact>();
            var lines = Fixture("Basislink/model-db.txt").Split('\n').Select(x => x.TrimEnd('\r')).Where(x => x.Length > 0 && !x.StartsWith("#")).Select(x => x.Split('|')).ToList();
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
            var lines = Fixture("Basislink/model-http.txt").Split('\n').Select(x => x.TrimEnd('\r')).Where(x => x.Length > 0 && !x.StartsWith("#")).Select(x => x.Split('|')).ToList();
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
