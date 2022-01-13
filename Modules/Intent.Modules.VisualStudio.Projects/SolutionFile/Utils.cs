using System.Collections.Generic;

namespace Intent.Modules.VisualStudio.Projects.SolutionFile
{
    internal static class Utils
    {
        public static int GetIndentation(this string line)
        {
            var indentation = 0;
            foreach (var @char in line)
            {
                if (@char != '\t')
                {
                    break;
                }

                indentation++;
            }

            return indentation;
        }

        public static bool IsHierarchicalStart(this string line, Queue<string> queue, out string name)
        {
            name = line.Trim().Split("(")[0];

            if (!queue.TryPeek(out var nextLine) ||
                !nextLine.Trim().StartsWith($"End{name}") &&
                nextLine.GetIndentation() <= line.GetIndentation())
            {
                return false;
            }

            name = line.Trim().Split("(")[0];
            return true;
        }
    }
}