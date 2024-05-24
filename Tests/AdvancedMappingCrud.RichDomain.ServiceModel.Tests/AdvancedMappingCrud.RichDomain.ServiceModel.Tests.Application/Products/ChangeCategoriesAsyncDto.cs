using System;
using System.Collections.Generic;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.Dtos.DtoModel", Version = "1.0")]

namespace AdvancedMappingCrud.RichDomain.ServiceModel.Tests.Application.Products
{
    public class ChangeCategoriesAsyncDto
    {
        public ChangeCategoriesAsyncDto()
        {
            CategoryNames = null!;
        }

        public Guid Id { get; set; }
        public List<string> CategoryNames { get; set; }

        public static ChangeCategoriesAsyncDto Create(Guid id, List<string> categoryNames)
        {
            return new ChangeCategoriesAsyncDto
            {
                Id = id,
                CategoryNames = categoryNames
            };
        }
    }
}