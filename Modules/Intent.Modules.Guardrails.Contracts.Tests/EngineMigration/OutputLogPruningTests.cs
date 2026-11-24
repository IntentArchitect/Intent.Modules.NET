using System;
using System.IO;
using System.Text;
using Intent.Modules.Common.Drift.EngineMigration;
using Shouldly;
using Xunit;

namespace Intent.Modules.Guardrails.Contracts.Tests.EngineMigration
{
    /// <summary>
    /// The Software Factory forgets the contract files: their entries leave the three records it decides deletes from, in the
    /// formats of Intent's <c>OutputLogPersistable</c>, <c>LegacyOutputLogPersistable</c> and <c>OutputConfigPersistable</c>.
    /// </summary>
    public class OutputLogPruningTests : IDisposable
    {
        private const string OutputLog = """
            <?xml version="1.0" encoding="utf-8"?>
            <outputLog>
              <files>
                <file relativePath=".agents/skills/sync-model/SKILL.md">
                  <projectId>4f1c</projectId>
                  <correlationId>Intent.Common.Drift.SyncModelSkill#9a2e</correlationId>
                </file>
                <file relativePath="src/Domain/Order.cs">
                  <projectId>7b3d</projectId>
                  <correlationId>Intent.Domain.Guardrails.Contracts.EntityContract#1c5f</correlationId>
                  <overwriteBehaviour>OnceOff</overwriteBehaviour>
                </file>
                <file relativePath="src/Program.cs">
                  <projectId>7b3d</projectId>
                </file>
                <file relativePath="src/Api/OrdersController.cs">
                  <projectId>8c4e</projectId>
                  <correlationId>Intent.Services.Guardrails.Contracts.ServiceContract#2d6a</correlationId>
                  <models>
                    <model id="2d6a" description="Orders" />
                  </models>
                </file>
              </files>
            </outputLog>

            """;

        private const string LegacyLog = """
            <?xml version="1.0" encoding="utf-8"?>
            <outputLog xmlns:xsi="http://www.w3.org/2001/XMLSchema-instance" xmlns:xsd="http://www.w3.org/2001/XMLSchema">
              <ApplicationId>a1b2</ApplicationId>
              <FileLogs>
                <FileLog>
                  <ProjectId>7b3d</ProjectId>
                  <CorrelationId>Intent.Domain.Guardrails.Contracts.EntityContract#1c5f</CorrelationId>
                  <OverwriteBehaviour>Always</OverwriteBehaviour>
                  <ApplicationRelativeFilePath>../../src/Domain/Order.cs</ApplicationRelativeFilePath>
                  <IsIgnored>false</IsIgnored>
                </FileLog>
                <FileLog>
                  <ProjectId>7b3d</ProjectId>
                  <CorrelationId>Intent.Services.Guardrails.Contracts.DtoContract#3e7b</CorrelationId>
                  <OverwriteBehaviour>Always</OverwriteBehaviour>
                  <ApplicationRelativeFilePath>../../src/Api/OrderDto.cs</ApplicationRelativeFilePath>
                  <IsIgnored>false</IsIgnored>
                </FileLog>
                <FileLog>
                  <ProjectId>7b3d</ProjectId>
                  <OverwriteBehaviour>Always</OverwriteBehaviour>
                  <ApplicationRelativeFilePath>../../src/Startup.cs</ApplicationRelativeFilePath>
                  <IsIgnored>false</IsIgnored>
                </FileLog>
              </FileLogs>
            </outputLog>
            """;

        private const string OutputConfig = """
            <?xml version="1.0" encoding="utf-8"?>
            <outputFiles>
              <files>
                <file relativePath="src/Api/OrderDto.cs" state="ignored">
                  <isIgnoredNotes><![CDATA[Hand-written now]]></isIgnoredNotes>
                </file>
                <file relativePath="src/Program.cs" state="ignored;once-off-generated" />
              </files>
            </outputFiles>
            """;

        private readonly string _root = Path.Combine(Path.GetTempPath(), "output-log-pruning-" + Guid.NewGuid().ToString("N"));
        private readonly string _application;

        public OutputLogPruningTests()
        {
            _application = Path.Combine(_root, "intent", "Acme", "Acme.application.config");
            Directory.CreateDirectory(Path.Combine(_root, "intent", "Acme", ".intent"));
            File.WriteAllText(_application, "<application />");
        }

        public void Dispose() => Directory.Delete(_root, recursive: true);

        [Fact]
        public void The_contract_files_are_those_either_log_lists_relative_to_the_output_folder()
        {
            Write(OutputLogPruning.OutputLogPath(_application), OutputLog);
            Write(OutputLogPruning.LegacyLogPath(_application), LegacyLog);

            OutputLogPruning.ContractFiles(_application, _root).ShouldBe(new[] { "src/Domain/Order.cs", "src/Api/OrdersController.cs", "src/Api/OrderDto.cs" }, ignoreOrder: true);
        }

        [Fact]
        public void Each_contract_file_comes_with_its_correlation_id_and_output_target_the_output_log_first()
        {
            Write(OutputLogPruning.OutputLogPath(_application), OutputLog.Replace(
                "<correlationId>Intent.Domain.Guardrails.Contracts.EntityContract#1c5f",
                "<correlationId>Intent.Domain.Guardrails.Contracts.EntityContract#4a8b"));
            Write(OutputLogPruning.LegacyLogPath(_application), LegacyLog);

            OutputLogPruning.ContractEntries(_application, _root).ShouldBe(new[]
            {
                new OutputLogPruning.ContractFile("src/Domain/Order.cs", "Intent.Domain.Guardrails.Contracts.EntityContract#4a8b", "7b3d"),
                new OutputLogPruning.ContractFile("src/Api/OrdersController.cs", "Intent.Services.Guardrails.Contracts.ServiceContract#2d6a", "8c4e"),
                new OutputLogPruning.ContractFile("src/Api/OrderDto.cs", "Intent.Services.Guardrails.Contracts.DtoContract#3e7b", "7b3d"),
            });
        }

        [Fact]
        public void Only_the_contract_entries_leave_the_output_log()
        {
            Write(OutputLogPruning.OutputLogPath(_application), OutputLog);

            var removed = OutputLogPruning.Prune(_application, _root);

            removed[".intent/output-log.xml"].ShouldBe(2);
            Read(OutputLogPruning.OutputLogPath(_application)).ShouldBe(Lf("""
                <?xml version="1.0" encoding="utf-8"?>
                <outputLog>
                  <files>
                    <file relativePath=".agents/skills/sync-model/SKILL.md">
                      <projectId>4f1c</projectId>
                      <correlationId>Intent.Common.Drift.SyncModelSkill#9a2e</correlationId>
                    </file>
                    <file relativePath="src/Program.cs">
                      <projectId>7b3d</projectId>
                    </file>
                  </files>
                </outputLog>

                """));
        }

        [Fact]
        public void Only_the_contract_entries_leave_the_legacy_log()
        {
            Write(OutputLogPruning.LegacyLogPath(_application), LegacyLog);

            OutputLogPruning.Prune(_application, _root)["Acme.application.output.log"].ShouldBe(2);

            Read(OutputLogPruning.LegacyLogPath(_application)).ShouldBe(Lf("""
                <?xml version="1.0" encoding="utf-8"?>
                <outputLog xmlns:xsi="http://www.w3.org/2001/XMLSchema-instance" xmlns:xsd="http://www.w3.org/2001/XMLSchema">
                  <ApplicationId>a1b2</ApplicationId>
                  <FileLogs>
                    <FileLog>
                      <ProjectId>7b3d</ProjectId>
                      <OverwriteBehaviour>Always</OverwriteBehaviour>
                      <ApplicationRelativeFilePath>../../src/Startup.cs</ApplicationRelativeFilePath>
                      <IsIgnored>false</IsIgnored>
                    </FileLog>
                  </FileLogs>
                </outputLog>
                """));
        }

        [Fact]
        public void An_ignored_contract_file_leaves_the_output_config_by_its_path()
        {
            Write(OutputLogPruning.LegacyLogPath(_application), LegacyLog);
            Write(OutputLogPruning.OutputConfigPath(_application), OutputConfig);

            OutputLogPruning.Prune(_application, _root)["Acme.application.output.config.xml"].ShouldBe(1);

            Read(OutputLogPruning.OutputConfigPath(_application)).ShouldBe(Lf("""
                <?xml version="1.0" encoding="utf-8"?>
                <outputFiles>
                  <files>
                    <file relativePath="src/Program.cs" state="ignored;once-off-generated" />
                  </files>
                </outputFiles>
                """));
        }

        [Fact]
        public void Only_the_contract_files_leave_the_managed_files_inventory_by_their_template_id()
        {
            var path = OutputLogPruning.ManagedFilesPath(_application);
            Write(path, """
                <?xml version="1.0" encoding="utf-8"?>
                <managedFiles version="1" applicationId="a1b2">
                  <file path=".agents/skills/sync-model/SKILL.md" templateId="Intent.Common.Drift.SyncModelSkill" />
                  <file path="src/Api/OrdersController.cs" templateId="Intent.Services.Guardrails.Contracts.ServiceContract" />
                  <file path="src/Domain/Order.cs" templateId="Intent.Domain.Guardrails.Contracts.EntityContract" deviationsHash="9f8e" />
                  <file path="src/Program.cs" />
                </managedFiles>
                """);

            OutputLogPruning.Prune(_application, _root)["Acme.application.managed-files.xml"].ShouldBe(2);

            Read(path).ShouldBe(Lf("""
                <?xml version="1.0" encoding="utf-8"?>
                <managedFiles version="1" applicationId="a1b2">
                  <file path=".agents/skills/sync-model/SKILL.md" templateId="Intent.Common.Drift.SyncModelSkill" />
                  <file path="src/Program.cs" />
                </managedFiles>
                """));
        }

        [Fact]
        public void Records_that_arent_there_are_fine()
        {
            var removed = OutputLogPruning.Prune(_application, _root);

            removed.Values.ShouldAllBe(x => x == 0);
            File.Exists(OutputLogPruning.OutputLogPath(_application)).ShouldBeFalse();
            File.Exists(OutputLogPruning.LegacyLogPath(_application)).ShouldBeFalse();
            File.Exists(OutputLogPruning.OutputConfigPath(_application)).ShouldBeFalse();
            File.Exists(OutputLogPruning.ManagedFilesPath(_application)).ShouldBeFalse();
        }

        [Fact]
        public void Pruning_again_changes_nothing()
        {
            Write(OutputLogPruning.OutputLogPath(_application), OutputLog);
            Write(OutputLogPruning.LegacyLogPath(_application), LegacyLog);
            Write(OutputLogPruning.OutputConfigPath(_application), OutputConfig);
            OutputLogPruning.Prune(_application, _root);
            var written = File.GetLastWriteTimeUtc(OutputLogPruning.LegacyLogPath(_application));

            OutputLogPruning.Prune(_application, _root).Values.ShouldAllBe(x => x == 0);
            File.GetLastWriteTimeUtc(OutputLogPruning.LegacyLogPath(_application)).ShouldBe(written);
        }

        [Fact]
        public void A_record_with_windows_line_endings_and_a_byte_order_mark_keeps_both()
        {
            var path = OutputLogPruning.OutputLogPath(_application);
            File.WriteAllText(path, OutputLog.Replace("\r\n", "\n").Replace("\n", "\r\n"), new UTF8Encoding(true));

            OutputLogPruning.Prune(_application, _root);

            var bytes = File.ReadAllBytes(path);
            bytes[..3].ShouldBe(new byte[] { 0xEF, 0xBB, 0xBF });
            var text = Encoding.UTF8.GetString(bytes, 3, bytes.Length - 3);
            text.ShouldNotContain("EntityContract");
            text.Replace("\r\n", string.Empty).ShouldNotContain("\n");
            text.ShouldEndWith("</outputLog>\r\n");
        }

        private static void Write(string path, string text) => File.WriteAllText(path, text.Replace("\r\n", "\n"), new UTF8Encoding(false));

        private static string Read(string path) => File.ReadAllText(path);

        // Raw string literals take the source file's line endings.
        private static string Lf(string text) => text.Replace("\r\n", "\n");
    }
}
