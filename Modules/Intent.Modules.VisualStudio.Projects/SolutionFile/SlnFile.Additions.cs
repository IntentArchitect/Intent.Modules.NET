using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Microsoft.DotNet.Cli.Sln.Internal.FileManipulation;
using static System.Collections.Specialized.BitVector32;

namespace Microsoft.DotNet.Cli.Sln.Internal
{
    partial class SlnFile
    {
        public static SlnFile CreateEmpty(string file)
        {
            return Read(file, """
                              Microsoft Visual Studio Solution File, Format Version 12.00
                              # Visual Studio 14
                              VisualStudioVersion = 14.0.25420.1
                              MinimumVisualStudioVersion = 10.0.40219.1
                              """);
        }

        public static SlnFile Read(string file, string contents)
        {
            SlnFile slnFile = new()
            {
                FullPath = Path.GetFullPath(file),
                _format = new TextFormatInfo()
            };

            using (var sr = new StringReader(contents))
            {
                slnFile.Read(sr);
            }

            return slnFile;
        }

        public string Generate()
        {
            var sw = new StringWriter();
            Write(sw);

            return sw.ToString();
        }
    }

    internal static class SlnFileExtensions
    {
        public static SlnPropertySet GetOrCreateSolutionPropertiesSection(this SlnFile slnFile, out bool alreadyExisted)
        {
            alreadyExisted = slnFile.Sections.Any(x => x.Id == SectionId.SolutionProperties);

            return slnFile.Sections.GetOrCreateSection(SectionId.SolutionProperties, SlnSectionType.PreProcess).Properties;
        }

        public static IEnumerable<SlnProject> GetChildProjects(this SlnFile slnFile)
        {
            var nestedProjects = slnFile.Sections.GetSection(SectionId.NestedProjects);

            return slnFile.Projects
                .Where(project => nestedProjects?.Properties.ContainsKey(project.Id) != true);
        }

        public static IEnumerable<SlnProject> GetChildProjects(this SlnProject slnProject)
        {
            var slnFile = slnProject.ParentFile;
            var nestedProjects = slnFile.Sections.GetSection(SectionId.NestedProjects);

            return slnFile.Projects
                .Where(project => nestedProjects?.Properties.TryGetValue(project.Id, out var value) == true &&
                                  value == slnProject.Id);
        }

        public static void SetParent(this SlnProject slnProject, SlnProject parentProject)
        {
            var slnFile = slnProject.ParentFile;

            if (parentProject == null)
            {
                slnFile.Sections.GetSection(SectionId.NestedProjects)?.Properties.Remove(slnProject.Id);
            }
            else
            {
                var nestedProjects = slnFile.Sections.GetOrCreateSection(SectionId.NestedProjects, SlnSectionType.PreProcess);
                nestedProjects.Properties[slnProject.Id] = parentProject.Id;
            }
        }

        public static SlnProject GetOrCreateFolder(this SlnFile slnFile, string id, string name)
        {
            id = $"{{{id}}}".ToUpperInvariant();

            var folderProject = slnFile.GetChildProjects().SingleOrDefault(x => x.Id == id) ??
                                slnFile.GetChildProjects().SingleOrDefault(x => x.Name == name);
            if (folderProject == null)
            {
                folderProject = slnFile.Projects.GetOrCreateProject(id);
                folderProject.TypeGuid = TypeGuid.Folder;
            }

            folderProject.Name = name;
            folderProject.FilePath = name;

            return folderProject;
        }

        public static SlnProject GetOrCreateFolder(this SlnProject project, string id, string name)
        {
            id = $"{{{id}}}".ToUpperInvariant();

            var childProjects = project.GetChildProjects().ToArray();
            var folderProject = childProjects.SingleOrDefault(x => x.Id == id) ??
                                childProjects.SingleOrDefault(x => x.Name == name);
            if (folderProject == null)
            {
                folderProject = project.ParentFile.Projects.GetOrCreateProject(id);
                folderProject.TypeGuid = TypeGuid.Folder;
            }

            folderProject.Name = name;
            folderProject.FilePath = name;
            folderProject.SetParent(project);

            return folderProject;
        }


        public static void AddSolutionItem(this SlnSectionCollection sections, string solutionItemPath)
        {
            var relativePath = sections.ParentFile.GetRelativePath(solutionItemPath);

            var solutionItems = sections.GetOrCreateSection(SectionId.SolutionItems, SlnSectionType.PreProcess);
            solutionItems.Properties[relativePath] = relativePath;
        }

        public static void RemoveSolutionItem(this SlnFile slnFile, string solutionItemPath)
        {
            var relativePath = slnFile.GetRelativePath(solutionItemPath);

            var section = slnFile.Sections.GetSection(SectionId.SolutionItems);
            section?.Properties.Remove(relativePath);

            foreach (var project in slnFile.Projects)
            {
                section = project.Sections.GetSection(SectionId.SolutionItems);
                section?.Properties.Remove(relativePath);
            }
        }

        public static SlnProject GetOrCreateProject(this SlnProjectCollection slnProjects,
            string id,
            string name,
            string typeGuid,
            string filePath,
            SlnProject parent,
            out bool alreadyExisted)
        {
            id = $"{{{id}}}".ToUpperInvariant();
            typeGuid = $"{{{typeGuid}}}".ToUpperInvariant();

            var project = slnProjects.FirstOrDefault(s => s.Id.Equals(id, StringComparison.OrdinalIgnoreCase)) ??
                          slnProjects.FirstOrDefault(s => s.Name.Equals(name, StringComparison.OrdinalIgnoreCase));

            alreadyExisted = project != null;

            if (project == null)
            {
                project = new SlnProject { Id = id };
                slnProjects.Add(project);
            }

            project.Name = name;
            project.FilePath = Normalize(filePath);
            project.TypeGuid = typeGuid;
            project.SetParent(parent);

            return project;
        }

        public static class TypeGuid
        {
            public const string Folder = "{2150E333-8FDC-42A3-9474-1A3956D46DE8}";
        }

        public static string GetRelativePath(this SlnFile slnFile, string path)
        {
            return Normalize(Path.GetRelativePath(Path.GetDirectoryName(slnFile.FullPath)!, path));
        }

        private static string Normalize(string path)
        {
            // .sln files always use back-slashes
            path = path.Replace("/", "\\");

            // Remove folders in path which are just "."
            path = path
                .Split("\\")
                .Where(x => !string.IsNullOrWhiteSpace(x) && x != ".")
                .Aggregate((x, y) => $"{x}\\{y}");

            return path;
        }

        public static class SectionId
        {
            public const string NestedProjects = "NestedProjects";
            public const string SolutionItems = "SolutionItems";
            public const string SolutionProperties = "SolutionProperties";
        }
    }
}
