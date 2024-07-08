using System;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.Dtos.DtoModel", Version = "1.0")]

namespace EntityFrameworkCore.Repositories.TestApplication.Application.SqlOutParameter
{
    public class MultipleOutDto
    {
        public MultipleOutDto()
        {
            Param1 = null!;
        }

        public string Param1 { get; set; }
        public DateTime Param2 { get; set; }
        public bool Param3 { get; set; }

        public static MultipleOutDto Create(string param1, DateTime param2, bool param3)
        {
            return new MultipleOutDto
            {
                Param1 = param1,
                Param2 = param2,
                Param3 = param3
            };
        }
    }
}