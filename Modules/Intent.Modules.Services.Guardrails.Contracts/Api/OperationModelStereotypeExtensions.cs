using System;
using System.Collections.Generic;
using System.Linq;
using Intent.Metadata.Models;
using Intent.Modelers.Services.Api;
using Intent.Modules.Common;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.ModuleBuilder.Templates.Api.ApiElementModelExtensions", Version = "1.0")]

namespace Intent.Services.Guardrails.Contracts.Api
{
    public static class OperationModelStereotypeExtensions
    {
        public static AsyncConfiguration GetAsyncConfiguration(this OperationModel model)
        {
            var stereotype = model.GetStereotype(AsyncConfiguration.DefinitionId);
            return stereotype != null ? new AsyncConfiguration(stereotype) : null;
        }


        public static bool HasAsyncConfiguration(this OperationModel model)
        {
            return model.HasStereotype(AsyncConfiguration.DefinitionId);
        }

        public static bool TryGetAsyncConfiguration(this OperationModel model, out AsyncConfiguration stereotype)
        {
            if (!HasAsyncConfiguration(model))
            {
                stereotype = null;
                return false;
            }

            stereotype = new AsyncConfiguration(model.GetStereotype(AsyncConfiguration.DefinitionId));
            return true;
        }

        public static CollectionType GetCollectionType(this OperationModel model)
        {
            var stereotype = model.GetStereotype(CollectionType.DefinitionId);
            return stereotype != null ? new CollectionType(stereotype) : null;
        }


        public static bool HasCollectionType(this OperationModel model)
        {
            return model.HasStereotype(CollectionType.DefinitionId);
        }

        public static bool TryGetCollectionType(this OperationModel model, out CollectionType stereotype)
        {
            if (!HasCollectionType(model))
            {
                stereotype = null;
                return false;
            }

            stereotype = new CollectionType(model.GetStereotype(CollectionType.DefinitionId));
            return true;
        }

        public static OperationConfiguration GetOperationConfiguration(this OperationModel model)
        {
            var stereotype = model.GetStereotype(OperationConfiguration.DefinitionId);
            return stereotype != null ? new OperationConfiguration(stereotype) : null;
        }


        public static bool HasOperationConfiguration(this OperationModel model)
        {
            return model.HasStereotype(OperationConfiguration.DefinitionId);
        }

        public static bool TryGetOperationConfiguration(this OperationModel model, out OperationConfiguration stereotype)
        {
            if (!HasOperationConfiguration(model))
            {
                stereotype = null;
                return false;
            }

            stereotype = new OperationConfiguration(model.GetStereotype(OperationConfiguration.DefinitionId));
            return true;
        }

        public class AsyncConfiguration
        {
            private IStereotype _stereotype;
            public const string DefinitionId = "582f45ce-db44-4e70-bf08-4d69d2b3a760";

            public AsyncConfiguration(IStereotype stereotype)
            {
                _stereotype = stereotype;
            }

            public string Name => _stereotype.Name;

            public bool IncludeCancellationToken()
            {
                return _stereotype.GetProperty<bool>("Include Cancellation Token");
            }

            public AsyncModeOptions AsyncMode()
            {
                return new AsyncModeOptions(_stereotype.GetProperty<string>("Async Mode"));
            }

            public TaskTypeOptions TaskType()
            {
                return new TaskTypeOptions(_stereotype.GetProperty<string>("Task Type"));
            }

            public string CancellationTokenName()
            {
                return _stereotype.GetProperty<string>("Cancellation Token Name");
            }

            public bool CancellationTokenDefault()
            {
                return _stereotype.GetProperty<bool>("Cancellation Token Default");
            }

            public class AsyncModeOptions
            {
                public readonly string Value;

                public AsyncModeOptions(string value)
                {
                    Value = value;
                }

                public AsyncModeOptionsEnum AsEnum()
                {
                    switch (Value)
                    {
                        case "async":
                            return AsyncModeOptionsEnum.Async;
                        case "no async keyword":
                            return AsyncModeOptionsEnum.NoAsyncKeyword;
                        case "sync":
                            return AsyncModeOptionsEnum.Sync;
                        default:
                            throw new ArgumentOutOfRangeException();
                    }
                }

                public bool IsAsync()
                {
                    return Value == "async";
                }
                public bool IsNoAsyncKeyword()
                {
                    return Value == "no async keyword";
                }
                public bool IsSync()
                {
                    return Value == "sync";
                }
            }

            public enum AsyncModeOptionsEnum
            {
                Async,
                NoAsyncKeyword,
                Sync
            }
            public class TaskTypeOptions
            {
                public readonly string Value;

                public TaskTypeOptions(string value)
                {
                    Value = value;
                }

                public TaskTypeOptionsEnum AsEnum()
                {
                    switch (Value)
                    {
                        case "Task":
                            return TaskTypeOptionsEnum.Task;
                        case "ValueTask":
                            return TaskTypeOptionsEnum.ValueTask;
                        default:
                            throw new ArgumentOutOfRangeException();
                    }
                }

                public bool IsTask()
                {
                    return Value == "Task";
                }
                public bool IsValueTask()
                {
                    return Value == "ValueTask";
                }
            }

            public enum TaskTypeOptionsEnum
            {
                Task,
                ValueTask
            }

        }

        public class CollectionType
        {
            private IStereotype _stereotype;
            public const string DefinitionId = "2550e9f2-2d5c-4a98-b6a9-8348f86f06b2";

            public CollectionType(IStereotype stereotype)
            {
                _stereotype = stereotype;
            }

            public string Name => _stereotype.Name;

            public TypeOptions Type()
            {
                return new TypeOptions(_stereotype.GetProperty<string>("Type"));
            }

            public string CustomFormat()
            {
                return _stereotype.GetProperty<string>("Custom Format");
            }

            public class TypeOptions
            {
                public readonly string Value;

                public TypeOptions(string value)
                {
                    Value = value;
                }

                public TypeOptionsEnum AsEnum()
                {
                    switch (Value)
                    {
                        case "IEnumerable":
                            return TypeOptionsEnum.IEnumerable;
                        case "ICollection":
                            return TypeOptionsEnum.ICollection;
                        case "IList":
                            return TypeOptionsEnum.IList;
                        case "IReadOnlyCollection":
                            return TypeOptionsEnum.IReadOnlyCollection;
                        case "IReadOnlyList":
                            return TypeOptionsEnum.IReadOnlyList;
                        case "List":
                            return TypeOptionsEnum.List;
                        case "HashSet":
                            return TypeOptionsEnum.HashSet;
                        case "ISet":
                            return TypeOptionsEnum.ISet;
                        case "IQueryable":
                            return TypeOptionsEnum.IQueryable;
                        case "Array":
                            return TypeOptionsEnum.Array;
                        case "Custom":
                            return TypeOptionsEnum.Custom;
                        default:
                            throw new ArgumentOutOfRangeException();
                    }
                }

                public bool IsIEnumerable()
                {
                    return Value == "IEnumerable";
                }
                public bool IsICollection()
                {
                    return Value == "ICollection";
                }
                public bool IsIList()
                {
                    return Value == "IList";
                }
                public bool IsIReadOnlyCollection()
                {
                    return Value == "IReadOnlyCollection";
                }
                public bool IsIReadOnlyList()
                {
                    return Value == "IReadOnlyList";
                }
                public bool IsList()
                {
                    return Value == "List";
                }
                public bool IsHashSet()
                {
                    return Value == "HashSet";
                }
                public bool IsISet()
                {
                    return Value == "ISet";
                }
                public bool IsIQueryable()
                {
                    return Value == "IQueryable";
                }
                public bool IsArray()
                {
                    return Value == "Array";
                }
                public bool IsCustom()
                {
                    return Value == "Custom";
                }
            }

            public enum TypeOptionsEnum
            {
                IEnumerable,
                ICollection,
                IList,
                IReadOnlyCollection,
                IReadOnlyList,
                List,
                HashSet,
                ISet,
                IQueryable,
                Array,
                Custom
            }
        }

        public class OperationConfiguration
        {
            private IStereotype _stereotype;
            public const string DefinitionId = "f5ebf7a5-e3bf-4fbb-8abc-2b7dc60fa510";

            public OperationConfiguration(IStereotype stereotype)
            {
                _stereotype = stereotype;
            }

            public string Name => _stereotype.Name;

            public AccessModifierOptions AccessModifier()
            {
                return new AccessModifierOptions(_stereotype.GetProperty<string>("Access Modifier"));
            }

            public bool Static()
            {
                return _stereotype.GetProperty<bool>("Static");
            }

            public bool Virtual()
            {
                return _stereotype.GetProperty<bool>("Virtual");
            }

            public bool Override()
            {
                return _stereotype.GetProperty<bool>("Override");
            }

            public bool Sealed()
            {
                return _stereotype.GetProperty<bool>("Sealed");
            }

            public bool New()
            {
                return _stereotype.GetProperty<bool>("New");
            }

            public class AccessModifierOptions
            {
                public readonly string Value;

                public AccessModifierOptions(string value)
                {
                    Value = value;
                }

                public AccessModifierOptionsEnum AsEnum()
                {
                    switch (Value)
                    {
                        case "public":
                            return AccessModifierOptionsEnum.Public;
                        case "protected":
                            return AccessModifierOptionsEnum.Protected;
                        case "internal":
                            return AccessModifierOptionsEnum.Internal;
                        case "private":
                            return AccessModifierOptionsEnum.Private;
                        case "protected internal":
                            return AccessModifierOptionsEnum.ProtectedInternal;
                        case "none":
                            return AccessModifierOptionsEnum.None;
                        default:
                            throw new ArgumentOutOfRangeException();
                    }
                }

                public bool IsPublic()
                {
                    return Value == "public";
                }
                public bool IsProtected()
                {
                    return Value == "protected";
                }
                public bool IsInternal()
                {
                    return Value == "internal";
                }
                public bool IsPrivate()
                {
                    return Value == "private";
                }
                public bool IsProtectedInternal()
                {
                    return Value == "protected internal";
                }
                public bool IsNone()
                {
                    return Value == "none";
                }
            }

            public enum AccessModifierOptionsEnum
            {
                Public,
                Protected,
                Internal,
                Private,
                ProtectedInternal,
                None
            }
        }

    }
}