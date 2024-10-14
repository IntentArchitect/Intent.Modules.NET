// ------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version: 17.0.0.0
//  
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
// ------------------------------------------------------------------------------
namespace Intent.Modules.Entities.Templates.CollectionExtensions
{
    using System.Collections.Generic;
    using System.Linq;
    using Intent.Modules.Common;
    using Intent.Modules.Common.Templates;
    using Intent.Modules.Common.CSharp.Templates;
    using Intent.Templates;
    using Intent.Metadata.Models;
    using System;
    
    /// <summary>
    /// Class to produce the template output
    /// </summary>
    
    #line 1 "C:\Dev\Intent.Modules.NET\Modules\Intent.Modules.Entities\Templates\CollectionExtensions\CollectionExtensionsTemplate.tt"
    [global::System.CodeDom.Compiler.GeneratedCodeAttribute("Microsoft.VisualStudio.TextTemplating", "17.0.0.0")]
    public partial class CollectionExtensionsTemplate : CSharpTemplateBase<object>
    {
#line hidden
        /// <summary>
        /// Create the template output
        /// </summary>
        public override string TransformText()
        {
            this.Write("using System;\r\nusing System.Collections.Generic;\r\nusing System.Linq;\r\n\r\n[assembly" +
                    ": DefaultIntentManaged(Mode.Fully)]\r\n\r\nnamespace ");
            
            #line 16 "C:\Dev\Intent.Modules.NET\Modules\Intent.Modules.Entities\Templates\CollectionExtensions\CollectionExtensionsTemplate.tt"
            this.Write(this.ToStringHelper.ToStringWithCulture(Namespace));
            
            #line default
            #line hidden
            this.Write("\r\n{\r\n    /// <summary>\r\n    /// Provides extension methods for collection objects" +
                    ".\r\n    /// </summary>\r\n    public static class ");
            
            #line 21 "C:\Dev\Intent.Modules.NET\Modules\Intent.Modules.Entities\Templates\CollectionExtensions\CollectionExtensionsTemplate.tt"
            this.Write(this.ToStringHelper.ToStringWithCulture(ClassName));
            
            #line default
            #line hidden
            this.Write("\r\n    {\r\n        /// <summary>\r\n        /// Compares two collections and returns " +
                    "a result indicating the differences between them.\r\n        /// </summary>\r\n     " +
                    "   /// <typeparam name=\"TChanged\">The type of elements in the changed collection" +
                    ".</typeparam>\r\n        /// <typeparam name=\"TOriginal\">The type of elements in t" +
                    "he base collection.</typeparam>\r\n        /// <param name=\"baseCollection\">The ba" +
                    "se collection to compare.</param>\r\n        /// <param name=\"changedCollection\">T" +
                    "he changed collection to compare against the base collection.</param>\r\n        /" +
                    "// <param name=\"equalityCheck\">A predicate to determine if two elements are equa" +
                    "l.</param>\r\n        /// <returns>A <see cref=\"ComparisonResult{TChanged, TOrigin" +
                    "al}\"/> object that describes the differences between the two collections.</retur" +
                    "ns>\r\n        public static ComparisonResult<TChanged, TOriginal> CompareCollecti" +
                    "ons<TChanged, TOriginal>(\r\n            this ICollection<TOriginal> baseCollectio" +
                    "n,\r\n            IEnumerable<TChanged> changedCollection, \r\n            Func<TOri" +
                    "ginal, TChanged, bool> equalityCheck)\r\n        {\r\n            changedCollection " +
                    "??= new List<TChanged>();\r\n\r\n            var toRemove = baseCollection.Where(bas" +
                    "eElement => changedCollection.All(changedElement => !equalityCheck(baseElement, " +
                    "changedElement))).ToList();\r\n            var toAdd = changedCollection.Where(cha" +
                    "ngedElement => baseCollection.All(baseElement => !equalityCheck(baseElement, cha" +
                    "ngedElement))).ToList();\r\n\r\n            var possibleEdits = new List<Match<TChan" +
                    "ged, TOriginal>>();\r\n            foreach (var changedElement in changedCollectio" +
                    "n)\r\n            {\r\n                var match = baseCollection.FirstOrDefault(bas" +
                    "eElement => equalityCheck(baseElement, changedElement));\r\n                if (ma" +
                    "tch is not null)\r\n                {\r\n                    possibleEdits.Add(new M" +
                    "atch<TChanged, TOriginal>(changedElement, match));\r\n                }\r\n         " +
                    "   }\r\n\r\n            return new ComparisonResult<TChanged, TOriginal>(toAdd, toRe" +
                    "move, possibleEdits);\r\n        }\r\n        \r\n        /// <summary>\r\n        /// R" +
                    "epresents the result of comparing two collections.\r\n        /// </summary>\r\n    " +
                    "    /// <typeparam name=\"TChanged\">The type of elements that have changed.</type" +
                    "param>\r\n        /// <typeparam name=\"TOriginal\">The type of original elements.</" +
                    "typeparam>\r\n        public class ComparisonResult<TChanged, TOriginal>\r\n        " +
                    "{\r\n            /// <summary>\r\n            /// Initializes a new instance of the " +
                    "<see cref=\"ComparisonResult{TChanged, TOriginal}\"/> class.\r\n            /// </su" +
                    "mmary>\r\n            /// <param name=\"toAdd\">A collection of elements to be added" +
                    ".</param>\r\n            /// <param name=\"toRemove\">A collection of elements to be" +
                    " removed.</param>\r\n            /// <param name=\"possibleEdits\">A collection of m" +
                    "atched elements that might have edits.</param>\r\n            public ComparisonRes" +
                    "ult(ICollection<TChanged> toAdd, ICollection<TOriginal> toRemove, ICollection<Ma" +
                    "tch<TChanged, TOriginal>> possibleEdits)\r\n            {\r\n                ToAdd =" +
                    " toAdd;\r\n                ToRemove = toRemove;\r\n                PossibleEdits = p" +
                    "ossibleEdits;\r\n            }\r\n\r\n            /// <summary>\r\n            /// Gets " +
                    "the collection of elements to be added.\r\n            /// </summary>\r\n           " +
                    " public ICollection<TChanged> ToAdd { get; }\r\n\r\n            /// <summary>\r\n     " +
                    "       /// Gets the collection of elements to be removed.\r\n            /// </sum" +
                    "mary>\r\n            public ICollection<TOriginal> ToRemove { get; }\r\n\r\n          " +
                    "  /// <summary>\r\n            /// Gets the collection of matched elements that mi" +
                    "ght have edits.\r\n            /// </summary>\r\n            public ICollection<Matc" +
                    "h<TChanged, TOriginal>> PossibleEdits { get; }\r\n\r\n            /// <summary>\r\n   " +
                    "         /// Determines whether there are any changes between the two collection" +
                    "s.\r\n            /// </summary>\r\n            /// <returns><see langword=\"true\" />" +
                    " if there are changes; otherwise, <see langword=\"false\" />.</returns>\r\n         " +
                    "   public bool HasChanges()\r\n            {\r\n                return ToAdd.Any() |" +
                    "| ToRemove.Any() || PossibleEdits.Any();\r\n            }\r\n        }\r\n\r\n        //" +
                    "/ <summary>\r\n        /// Represents a matched pair of changed and original eleme" +
                    "nts.\r\n        /// </summary>\r\n        /// <typeparam name=\"TChanged\">The type of" +
                    " the changed element.</typeparam>\r\n        /// <typeparam name=\"TOriginal\">The t" +
                    "ype of the original element.</typeparam>\r\n        public class Match<TChanged, T" +
                    "Original>\r\n        {\r\n            /// <summary>\r\n            /// Initializes a n" +
                    "ew instance of the <see cref=\"Match{TChanged, TOriginal}\"/> class.\r\n            " +
                    "/// </summary>\r\n            /// <param name=\"changed\">The changed element.</para" +
                    "m>\r\n            /// <param name=\"original\">The original element.</param>\r\n      " +
                    "      public Match(TChanged changed, TOriginal original)\r\n            {\r\n       " +
                    "         Changed = changed;\r\n                Original = original;\r\n            }" +
                    "\r\n\r\n            /// <summary>\r\n            /// Gets the changed element.\r\n      " +
                    "      /// </summary>\r\n            public TChanged Changed { get; private set; }\r" +
                    "\n\r\n            /// <summary>\r\n            /// Gets the original element.\r\n      " +
                    "      /// </summary>\r\n            public TOriginal Original { get; private set; " +
                    "}\r\n        }\r\n    }\r\n}");
            return this.GenerationEnvironment.ToString();
        }
    }
    
    #line default
    #line hidden
}
