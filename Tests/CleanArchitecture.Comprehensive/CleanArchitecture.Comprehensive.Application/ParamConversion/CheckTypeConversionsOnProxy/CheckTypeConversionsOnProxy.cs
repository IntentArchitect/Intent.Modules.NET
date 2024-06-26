using System;
using CleanArchitecture.Comprehensive.Application.Common.Interfaces;
using Intent.RoslynWeaver.Attributes;
using MediatR;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.QueryModels", Version = "1.0")]

namespace CleanArchitecture.Comprehensive.Application.ParamConversion.CheckTypeConversionsOnProxy
{
    public class CheckTypeConversionsOnProxy : IRequest<bool>, IQuery
    {
        public CheckTypeConversionsOnProxy(DateTime from,
            DateTime? to,
            Guid id,
            decimal value,
            TimeSpan time,
            bool active,
            DateOnly justDate,
            DateTimeOffset otherDate)
        {
            From = from;
            To = to;
            Id = id;
            Value = value;
            Time = time;
            Active = active;
            JustDate = justDate;
            OtherDate = otherDate;
        }

        public DateTime From { get; set; }
        public DateTime? To { get; set; }
        public Guid Id { get; set; }
        public decimal Value { get; set; }
        public TimeSpan Time { get; set; }
        public bool Active { get; set; }
        public DateOnly JustDate { get; set; }
        public DateTimeOffset OtherDate { get; set; }
    }
}