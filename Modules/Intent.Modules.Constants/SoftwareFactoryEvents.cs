using System;

namespace Intent.Modules.Constants
{
    public static class SoftwareFactoryEvents
    {
        /// <summary>
        /// Obsolete, use <see cref="FileAddedEvent"/> instead.
        /// </summary>
        [Obsolete]
        public const string FileAdded = FileAddedEvent;
        public const string FileAddedEvent = "Intent.SoftwareFactory.AddProjectItemEvent";
        /// <summary>
        /// Obsolete, use <see cref="FileRemovedEvent"/> instead.
        /// </summary>
        [Obsolete]
        public const string FileRemoved = FileRemovedEvent;
        public const string FileRemovedEvent = "Intent.SoftwareFactory.RemoveProjectItemEvent";
        public const string AddTargetEvent = "Intent.SoftwareFactory.AddTargetEvent";
        public const string AddTaskEvent = "Intent.SoftwareFactory.AddTaskEvent";
        public const string ChangeProjectItemTypeEvent = "Intent.SoftwareFactory.ChangeProjectItemTypeEvent";
        public const string DeleteFileCommand = "Intent.SoftwareFactory.DeleteFileCommand";
        public const string OverwriteFileCommand = "Intent.SoftwareFactory.OverwriteFileCommand";
        public const string CreateFileCommand = "Intent.SoftwareFactory.CreateFileCommand";
        public const string CodeWeaveCodeLossEvent = "Intent.SoftwareFactory.CodeWeaveCodeLossEvent";
    }
}