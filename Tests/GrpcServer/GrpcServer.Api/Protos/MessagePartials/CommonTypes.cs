using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using Google.Protobuf;
using Google.Protobuf.WellKnownTypes;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.AspNetCore.Grpc.CommonTypesPartial", Version = "1.0")]

namespace GrpcServer.Api.Protos.Messages
{
    public partial class DecimalValue
    {
        private const decimal NanoFactor = 1_000_000_000;

        public DecimalValue(long units, int nanos)
        {
            Units = units;
            Nanos = nanos;
        }

        public static implicit operator decimal(DecimalValue grpcDecimal)
        {
            return grpcDecimal.Units + grpcDecimal.Nanos / NanoFactor;
        }

        public static implicit operator DecimalValue(decimal value)
        {
            var units = decimal.ToInt64(value);
            var nanos = decimal.ToInt32((value - units) * NanoFactor);
            return new DecimalValue(units, nanos);
        }
    }

    public partial class ListOfAny
    {
        public List<object> ToContract()
        {
            return Items.Cast<object>().ToList();
        }

        [return: NotNullIfNotNull(nameof(contract))]
        public static ListOfAny? Create(IEnumerable<object>? contract)
        {
            if (contract == null)
            {
                return null;
            }

            var message = new ListOfAny();
            message.Items.AddRange(contract.Select(x => Any.Pack((IMessage)x)));
            return message;
        }
    }

    public partial class ListOfBool
    {
        public List<bool> ToContract()
        {
            return Items.ToList();
        }

        [return: NotNullIfNotNull(nameof(contract))]
        public static ListOfBool? Create(IEnumerable<bool>? contract)
        {
            if (contract == null)
            {
                return null;
            }

            var message = new ListOfBool();
            message.Items.AddRange(contract);
            return message;
        }
    }

    public partial class ListOfBytes
    {
        public List<byte[]> ToContract()
        {
            return Items.Select(x => x.ToByteArray()).ToList();
        }

        [return: NotNullIfNotNull(nameof(contract))]
        public static ListOfBytes? Create(IEnumerable<byte[]>? contract)
        {
            if (contract == null)
            {
                return null;
            }

            var message = new ListOfBytes();
            message.Items.AddRange(contract.Select(ByteString.CopyFrom));
            return message;
        }
    }

    public partial class ListOfDecimalValue
    {
        public List<decimal> ToContract()
        {
            return Items.Select(x => (decimal)x).ToList();
        }

        [return: NotNullIfNotNull(nameof(contract))]
        public static ListOfDecimalValue? Create(IEnumerable<decimal>? contract)
        {
            if (contract == null)
            {
                return null;
            }

            var message = new ListOfDecimalValue();
            message.Items.AddRange(contract.Select(x => (DecimalValue)x));
            return message;
        }
    }

    public partial class ListOfDouble
    {
        public List<double> ToContract()
        {
            return Items.ToList();
        }

        [return: NotNullIfNotNull(nameof(contract))]
        public static ListOfDouble? Create(IEnumerable<double>? contract)
        {
            if (contract == null)
            {
                return null;
            }

            var message = new ListOfDouble();
            message.Items.AddRange(contract);
            return message;
        }
    }

    public partial class ListOfDuration
    {
        public List<TimeSpan> ToContract()
        {
            return Items.Select(x => x.ToTimeSpan()).ToList();
        }

        [return: NotNullIfNotNull(nameof(contract))]
        public static ListOfDuration? Create(IEnumerable<TimeSpan>? contract)
        {
            if (contract == null)
            {
                return null;
            }

            var message = new ListOfDuration();
            message.Items.AddRange(contract.Select(Duration.FromTimeSpan));
            return message;
        }
    }

    public partial class ListOfFloat
    {
        public List<float> ToContract()
        {
            return Items.ToList();
        }

        [return: NotNullIfNotNull(nameof(contract))]
        public static ListOfFloat? Create(IEnumerable<float>? contract)
        {
            if (contract == null)
            {
                return null;
            }

            var message = new ListOfFloat();
            message.Items.AddRange(contract);
            return message;
        }
    }

    public partial class ListOfInt32
    {
        public List<int> ToContract()
        {
            return Items.ToList();
        }

        [return: NotNullIfNotNull(nameof(contract))]
        public static ListOfInt32? Create(IEnumerable<int>? contract)
        {
            if (contract == null)
            {
                return null;
            }

            var message = new ListOfInt32();
            message.Items.AddRange(contract);
            return message;
        }
    }

    public partial class ListOfInt64
    {
        public List<long> ToContract()
        {
            return Items.ToList();
        }

        [return: NotNullIfNotNull(nameof(contract))]
        public static ListOfInt64? Create(IEnumerable<long>? contract)
        {
            if (contract == null)
            {
                return null;
            }

            var message = new ListOfInt64();
            message.Items.AddRange(contract);
            return message;
        }
    }

    public partial class ListOfMapOfStringAndString
    {
        public List<Dictionary<string, string>> ToContract()
        {
            return Items.Select(x => x.Items.ToDictionary()).ToList();
        }

        [return: NotNullIfNotNull(nameof(contract))]
        public static ListOfMapOfStringAndString? Create(IEnumerable<Dictionary<string, string>>? contract)
        {
            if (contract == null)
            {
                return null;
            }

            var message = new ListOfMapOfStringAndString();
            message.Items.AddRange(contract.Select(MapOfStringAndString.Create));
            return message;
        }
    }

    public partial class ListOfString
    {
        public List<string> ToContract()
        {
            return Items.ToList();
        }

        [return: NotNullIfNotNull(nameof(contract))]
        public static ListOfString? Create(IEnumerable<string>? contract)
        {
            if (contract == null)
            {
                return null;
            }

            var message = new ListOfString();
            message.Items.AddRange(contract);
            return message;
        }
    }

    public partial class ListOfTimestamp
    {
        public List<Timestamp> ToContract()
        {
            return Items.ToList();
        }

        [return: NotNullIfNotNull(nameof(contract))]
        public static ListOfTimestamp? Create(IEnumerable<Timestamp>? contract)
        {
            if (contract == null)
            {
                return null;
            }

            var message = new ListOfTimestamp();
            message.Items.AddRange(contract);
            return message;
        }
    }

    public partial class ListOfUInt32
    {
        public List<uint> ToContract()
        {
            return Items.ToList();
        }

        [return: NotNullIfNotNull(nameof(contract))]
        public static ListOfUInt32? Create(IEnumerable<uint>? contract)
        {
            if (contract == null)
            {
                return null;
            }

            var message = new ListOfUInt32();
            message.Items.AddRange(contract);
            return message;
        }
    }

    public partial class MapOfStringAndString
    {
        public Dictionary<string, string> ToContract()
        {
            return Items.ToDictionary(x => x.Key, x => x.Value);
        }

        [return: NotNullIfNotNull(nameof(contract))]
        public static MapOfStringAndString? Create(Dictionary<string, string>? contract)
        {
            if (contract == null)
            {
                return null;
            }

            var message = new MapOfStringAndString();
            message.Items.Add(contract);
            return message;
        }
    }
}