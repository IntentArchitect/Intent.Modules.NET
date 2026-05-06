namespace Intent.Modules.VisualStudio.Projects.Templates.SqlServerDatabase.SqlProject.Content;

internal static class SqlProjectSdkContent
{
    public static string Generate(string name, string id, string version) =>
        $$"""
          <?xml version="1.0" encoding="utf-8"?>
          <Project DefaultTargets="Build">
            <Sdk Name="Microsoft.Build.Sql" Version="{{version}}" />
            <PropertyGroup>
              <Name>{{name}}</Name>
              <ProjectGuid>{{{id}}}</ProjectGuid>
              <DSP>Microsoft.Data.Tools.Schema.Sql.Sql130DatabaseSchemaProvider</DSP>
              <ModelCollation>1033, CI</ModelCollation>
              <EnableDefaultSqlItems>False</EnableDefaultSqlItems>
              <TargetDatabaseSet>True</TargetDatabaseSet>
            </PropertyGroup>
          </Project>
          """;
}