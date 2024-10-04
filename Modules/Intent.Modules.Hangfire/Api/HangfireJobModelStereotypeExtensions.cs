using System;
using System.Collections.Generic;
using System.Linq;
using Intent.Metadata.Models;
using Intent.Modules.Common;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.ModuleBuilder.Templates.Api.ApiElementModelExtensions", Version = "1.0")]

namespace Intent.Modules.Hangfire.Api
{
    public static class HangfireJobModelStereotypeExtensions
    {
        public static JobOptions GetJobOptions(this HangfireJobModel model)
        {
            var stereotype = model.GetStereotype(JobOptions.DefinitionId);
            return stereotype != null ? new JobOptions(stereotype) : null;
        }


        public static bool HasJobOptions(this HangfireJobModel model)
        {
            return model.HasStereotype(JobOptions.DefinitionId);
        }

        public static bool TryGetJobOptions(this HangfireJobModel model, out JobOptions stereotype)
        {
            if (!HasJobOptions(model))
            {
                stereotype = null;
                return false;
            }

            stereotype = new JobOptions(model.GetStereotype(JobOptions.DefinitionId));
            return true;
        }

        public class JobOptions
        {
            private IStereotype _stereotype;
            public const string DefinitionId = "268b1f8f-71ac-4a27-ad87-bdaeffb06022";

            public JobOptions(IStereotype stereotype)
            {
                _stereotype = stereotype;
            }

            public string Name => _stereotype.Name;

            public bool Enabled()
            {
                return _stereotype.GetProperty<bool>("Enabled");
            }

            public JobTypeOptions JobType()
            {
                return new JobTypeOptions(_stereotype.GetProperty<string>("Job Type"));
            }

            public string CronSchedule()
            {
                return _stereotype.GetProperty<string>("Cron Schedule");
            }

            public DelayTimeFrameOptions DelayTimeFrame()
            {
                return new DelayTimeFrameOptions(_stereotype.GetProperty<string>("Delay Time Frame"));
            }

            public int? DelayValue()
            {
                return _stereotype.GetProperty<int?>("Delay Value");
            }

            public bool DisallowConcurrentExecution()
            {
                return _stereotype.GetProperty<bool>("Disallow Concurrent Execution");
            }

            public int? ConcurrentExecutionTimeout()
            {
                return _stereotype.GetProperty<int?>("Concurrent Execution Timeout");
            }

            public int? RetryAttempts()
            {
                return _stereotype.GetProperty<int?>("Retry Attempts");
            }

            public OnAttemptsExceededOptions OnAttemptsExceeded()
            {
                return new OnAttemptsExceededOptions(_stereotype.GetProperty<string>("On Attempts Exceeded"));
            }

            public IElement Queue()
            {
                return _stereotype.GetProperty<IElement>("Queue");
            }

            public class JobTypeOptions
            {
                public readonly string Value;

                public JobTypeOptions(string value)
                {
                    Value = value;
                }

                public JobTypeOptionsEnum AsEnum()
                {
                    switch (Value)
                    {
                        case "Recurring":
                            return JobTypeOptionsEnum.Recurring;
                        case "Delayed":
                            return JobTypeOptionsEnum.Delayed;
                        case "Fire and Forget":
                            return JobTypeOptionsEnum.FireAndForget;
                        default:
                            throw new ArgumentOutOfRangeException();
                    }
                }

                public bool IsRecurring()
                {
                    return Value == "Recurring";
                }
                public bool IsDelayed()
                {
                    return Value == "Delayed";
                }
                public bool IsFireAndForget()
                {
                    return Value == "Fire and Forget";
                }
            }

            public enum JobTypeOptionsEnum
            {
                Recurring,
                Delayed,
                FireAndForget
            }
            public class DelayTimeFrameOptions
            {
                public readonly string Value;

                public DelayTimeFrameOptions(string value)
                {
                    Value = value;
                }

                public DelayTimeFrameOptionsEnum AsEnum()
                {
                    switch (Value)
                    {
                        case "Milliseconds":
                            return DelayTimeFrameOptionsEnum.Milliseconds;
                        case "Seconds":
                            return DelayTimeFrameOptionsEnum.Seconds;
                        case "Minutes":
                            return DelayTimeFrameOptionsEnum.Minutes;
                        case "Hours":
                            return DelayTimeFrameOptionsEnum.Hours;
                        case "Days":
                            return DelayTimeFrameOptionsEnum.Days;
                        case "Ticks":
                            return DelayTimeFrameOptionsEnum.Ticks;
                        default:
                            throw new ArgumentOutOfRangeException();
                    }
                }

                public bool IsMilliseconds()
                {
                    return Value == "Milliseconds";
                }
                public bool IsSeconds()
                {
                    return Value == "Seconds";
                }
                public bool IsMinutes()
                {
                    return Value == "Minutes";
                }
                public bool IsHours()
                {
                    return Value == "Hours";
                }
                public bool IsDays()
                {
                    return Value == "Days";
                }
                public bool IsTicks()
                {
                    return Value == "Ticks";
                }
            }

            public enum DelayTimeFrameOptionsEnum
            {
                Milliseconds,
                Seconds,
                Minutes,
                Hours,
                Days,
                Ticks
            }
            public class OnAttemptsExceededOptions
            {
                public readonly string Value;

                public OnAttemptsExceededOptions(string value)
                {
                    Value = value;
                }

                public OnAttemptsExceededOptionsEnum AsEnum()
                {
                    switch (Value)
                    {
                        case "Fail":
                            return OnAttemptsExceededOptionsEnum.Fail;
                        case "Delete":
                            return OnAttemptsExceededOptionsEnum.Delete;
                        default:
                            throw new ArgumentOutOfRangeException();
                    }
                }

                public bool IsFail()
                {
                    return Value == "Fail";
                }
                public bool IsDelete()
                {
                    return Value == "Delete";
                }
            }

            public enum OnAttemptsExceededOptionsEnum
            {
                Fail,
                Delete
            }
        }

    }
}