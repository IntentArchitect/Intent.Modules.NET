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
                              # Visual Studio 17
                              VisualStudioVersion = 17.8.34322.80
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


        public static void AddSolutionItem(
            this SlnFile slnFile,
            SlnProject parentProject,
            string solutionItemAbsolutePath,
            string relativeOutputPathPrefix,
            Func<string> idProvider = null)
        {
            idProvider ??= () => Guid.NewGuid().ToString();

            var relativePath = slnFile.GetRelativePath(solutionItemAbsolutePath);
            var relativePathDirectory = Path.GetDirectoryName(relativePath.Replace('\\', Path.DirectorySeparatorChar));
            var relativeSolutionFolderPath = relativePathDirectory;

            if (!string.IsNullOrWhiteSpace(relativePathDirectory))
            {
                if (!string.IsNullOrWhiteSpace(relativeOutputPathPrefix))
                {
                    relativeSolutionFolderPath = Path.GetRelativePath(relativeOutputPathPrefix, relativePathDirectory!);
                }

                parentProject = relativeSolutionFolderPath
                    .Split(Path.DirectorySeparatorChar)
                    .Where(x => x is not "." and not "..")
                    .Aggregate(
                        seed: parentProject,
                        func: (current, path) => current?.GetOrCreateFolder(idProvider(), path) ??
                                                 slnFile.GetOrCreateFolder(idProvider(), path));
            }

            var targetSectionsCollection = parentProject?.Sections ??
                           slnFile.Sections;

            var targetSection = targetSectionsCollection.GetOrCreateSection(SectionId.SolutionItems, SlnSectionType.PreProcess);
            targetSection.Properties[relativePath] = relativePath;

            var toInsertInOrder = targetSection.Properties
                .OrderBy(x => x.Key)
                .ThenBy(x => x.Value)
                .ToArray();
            targetSection.Properties.Clear();

            foreach (var item in toInsertInOrder)
            {
                targetSection.Properties.TryAdd(item.Key, item.Value);
            }

            // Clean up others
            var otherSections = Enumerable.Empty<SlnSectionCollection>()
                .Append(slnFile.Sections)
                .Union(slnFile.Projects.Select(x => x.Sections))
                .Where(x => x != targetSectionsCollection)
                .ToArray();

            foreach (var otherSection in otherSections)
            {
                var solutionItems = otherSection.GetSection(SectionId.SolutionItems);
                if (solutionItems?.Properties.ContainsKey(relativePath) != true)
                {
                    continue;
                }

                solutionItems.Properties.Remove(relativePath);

                if (solutionItems.Properties.Count == 0)
                {
                    otherSection.RemoveSection(SectionId.SolutionItems);
                }
            }
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

            var project = slnProjects.FirstOrDefault(s => string.Equals(s.Id, id, StringComparison.OrdinalIgnoreCase)) ??
                          slnProjects.FirstOrDefault(s => string.Equals(s.Name, name, StringComparison.OrdinalIgnoreCase));

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
