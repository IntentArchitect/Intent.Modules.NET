using Intent.Modules.Common.CSharp.Builder;
using Intent.Modules.Common.Templates;
using Intent.Modules.Common.VisualStudio;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Intent.Modules.AspNetCore.IntegrationTesting.Templates.EFContainerFixture
{
    /// <summary>
    /// Describes how the EF database fixture backs the test database for a given database provider.
    /// Most providers spin up a Testcontainers container (<see cref="ForContainer"/>); providers which
    /// run in-process, such as SQLite, hold a live connection instead (<see cref="ForConnection"/>).
    /// </summary>
    internal class DbStrategy
    {
        public const string ContainerFieldName = "_dbContainer";
        public const string ConnectionFieldName = "_dbConnection";

        /// <summary>
        /// Usings common to every container-backed strategy. These come from the <c>Testcontainers</c>
        /// package, so they must only be emitted when a container strategy is in play — an in-process
        /// strategy such as SQLite has no Testcontainers reference to resolve them against.
        /// </summary>
        private static readonly string[] ContainerUsings =
        {
            "DotNet.Testcontainers.Builders",
            "DotNet.Testcontainers.Configurations",
        };

        private DbStrategy(string fieldType,
            string fieldName,
            List<string> usings,
            List<INugetPackageInfo> nuGetPackages,
            IEnumerable<CSharpStatement> fieldInitialization,
            string connectionExpression,
            IEnumerable<CSharpStatement> initializeStatements,
            IEnumerable<CSharpStatement> disposeStatements)
        {
            FieldType = fieldType;
            FieldName = fieldName;
            Usings = usings;
            NuGetPackages = nuGetPackages;
            FieldInitialization = fieldInitialization;
            ConnectionExpression = connectionExpression;
            InitializeStatements = initializeStatements;
            DisposeStatements = disposeStatements;
        }

        /// <summary>Type of the backing field — a Testcontainers container, or a live database connection.</summary>
        public string FieldType { get; }

        /// <summary>Name of the backing field on the generated fixture.</summary>
        public string FieldName { get; }

        public IEnumerable<string> Usings { get; }

        public IEnumerable<INugetPackageInfo> NuGetPackages { get; }

        /// <summary>Statements assigning the backing field, emitted into the fixture's constructor.</summary>
        public IEnumerable<CSharpStatement> FieldInitialization { get; }

        /// <summary>
        /// Expression substituted for the application's connection string when the fixture re-registers
        /// the <c>DbContext</c> against the test database.
        /// </summary>
        public string ConnectionExpression { get; }

        public IEnumerable<CSharpStatement> InitializeStatements { get; }

        public IEnumerable<CSharpStatement> DisposeStatements { get; }

        /// <summary>
        /// A container-backed database, started and stopped around the test run by Testcontainers.
        /// </summary>
        public static DbStrategy ForContainer(string containerType,
            List<string> usings,
            List<INugetPackageInfo> nuGetPackages,
            IEnumerable<CSharpStatement> containerInitialization)
        {
            return new DbStrategy(
                fieldType: containerType,
                fieldName: ContainerFieldName,
                usings: ContainerUsings.Concat(usings).ToList(),
                nuGetPackages: nuGetPackages,
                fieldInitialization: containerInitialization,
                connectionExpression: $"{ContainerFieldName}.GetConnectionString()",
                initializeStatements: $"await {ContainerFieldName}.StartAsync();".ConvertToStatements(),
                disposeStatements: $"await {ContainerFieldName}.StopAsync();".ConvertToStatements());
        }

        /// <summary>
        /// An in-process database held open for the lifetime of the fixture. The live connection — not a
        /// connection string — is handed to EF Core, because an in-memory SQLite database only exists for
        /// as long as a connection to it remains open.
        /// </summary>
        public static DbStrategy ForConnection(string connectionType,
            List<string> usings,
            List<INugetPackageInfo> nuGetPackages,
            IEnumerable<CSharpStatement> connectionInitialization)
        {
            return new DbStrategy(
                fieldType: connectionType,
                fieldName: ConnectionFieldName,
                usings: usings,
                nuGetPackages: nuGetPackages,
                fieldInitialization: connectionInitialization,
                connectionExpression: ConnectionFieldName,
                initializeStatements: $"await {ConnectionFieldName}.OpenAsync();".ConvertToStatements(),
                disposeStatements: $"await {ConnectionFieldName}.DisposeAsync();".ConvertToStatements());
        }
    }
}
