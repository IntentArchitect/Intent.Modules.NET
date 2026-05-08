using System;
using System.Collections.Generic;
using System.Linq;
using Intent.Engine;
using Intent.Modules.Application.ServiceImplementations.Api;
using Intent.Modules.Common.Templates.AIStaticContent;
using Intent.Modules.Common.Templates.StaticContent;
using Intent.RoslynWeaver.Attributes;
using static Intent.Modules.Constants.TemplateRoles.Application;

[assembly: DefaultIntentManaged(Mode.Merge)]
[assembly: IntentTemplate("Intent.ModuleBuilder.Templates.StaticContentTemplateRegistration", Version = "1.0")]

namespace Intent.Modules.Application.ServiceImplementations.Templates.StaticContentTemplateRegistrations
{
    public class AISkillsStaticContentTemplateRegistration : AIStaticContentTemplateRegistration
    {
        public new const string TemplateId = "Intent.Modules.Application.ServiceImplementations.Templates.StaticContentTemplateRegistrations.AISkillsStaticContentTemplateRegistration";

        public AISkillsStaticContentTemplateRegistration(IApplicationConfigurationProvider applicationConfigurationProvider) : base(TemplateId, applicationConfigurationProvider)
        {
        }

        public override string ContentSubFolder => "AISkills";


        public override string[] BinaryFileGlobbingPatterns => new string[] { "*.jpg", "*.png", "*.xlsx", "*.ico", "*.pdf" };


        [IntentManaged(Mode.Fully, Body = Mode.Ignore)]
        public override IReadOnlyDictionary<string, string> Replacements(IOutputTarget outputTarget)
        {
            var automaticallyPersistUnitOfWork = outputTarget.ExecutionContext.GetSettings().GetUnitOfWorkSettings()?.AutomaticallyPersistUnitOfWork() ?? true;

            var installedModuleIds = outputTarget.ExecutionContext.InstalledModules
                .Select(m => m.ModuleId)
                .ToHashSet();

            var usingEFRepos = installedModuleIds.Contains("Intent.EntityFrameworkCore.Repositories");
            var usingAutoMapper = installedModuleIds.Contains("Intent.Application.AutoMapper");

            var additionalServiceRules = new List<string>();

            if (automaticallyPersistUnitOfWork)
            {
                additionalServiceRules.Add("""
## Unit of Work guidance (Only applicable to service operations which change persistent state)

- SaveChanges rule (STRICT): Do not call UnitOfWork.SaveChangesAsync(...) / SaveChangesAsync(...) in a handler/service method unless the operation returns a payload that requires DB-generated values, such as a generated Id, surrogate key, RowVersion/concurrency token, DB-generated timestamp, or computed column.
- If the operation returns Unit, void, Task, or IRequest with no result: do not call SaveChangesAsync.
- If the operation returns an identifier or DTO that needs generated fields: call SaveChangesAsync before returning.
- If unsure, omit SaveChangesAsync and assume an outer unit-of-work/pipeline commit.
- When reviewing code, remove SaveChangesAsync unless there is a clear generated-value or immediate-commit requirement.
""");
            }


            if (usingEFRepos)
            {
                additionalServiceRules.Add("""
## Entity Framework repository guidance

- Repository update rule (STRICT): Do not call repository.Update(...) / repo.Update(...) when using EF repositories.
- EF tracks loaded entities automatically. Modify the entity properties directly and let the Unit of Work persist the tracked changes.
- Only call Add/Create/Delete operations when inserting or removing entities.
- When reviewing code, remove unnecessary Update calls for entities loaded from an EF repository.
""");
            }

            if (usingAutoMapper)
            {
                var autoMapperProjectionGuidance = """"
## AutoMapper guidance

- Any read/query method (including application services) that returns Application-layer DTOs (*Dto) derived from Domain entities must use AutoMapper.
  -Do not manually construct DTOs (new XxxDto { ... }) on read/query paths.
- If the required mapping does not exist, create it:
  - Add an AutoMapper Profile.
  - Include mapping extension methods in the same file, matching existing conventions:
- Before using repository `ProjectTo` operations, verify that the required AutoMapper mappings exist.
- Allowed exception (rare):
  - Manual DTO construction is allowed only when the DTO is a non-entity-shaped view model/aggregation and AutoMapper is not reasonable.
  - This must include an inline code comment explaining why AutoMapper is not reasonable.
  - “Mapping doesn’t exist yet” is not a valid exception.

Example:
```csharp
public class CustomerDtoProfile : Profile
{
    public CustomerDtoProfile()
    {
        CreateMap<Customer, CustomerDto>();
    }
}

public static class CustomerDtoMappingExtensions
{
    public static CustomerDto MapToCustomerDto(this Customer projectFrom, IMapper mapper) => mapper.Map<CustomerDto>(projectFrom);

    public static List<CustomerDto> MapToCustomerDtoList(this IEnumerable<Customer> projectFrom, IMapper mapper) => projectFrom.Select(x => x.MapToCustomerDto(mapper)).ToList();
}
```
"""";

                additionalServiceRules.Add(autoMapperProjectionGuidance);
            }

            var result = new Dictionary<string, string>();
            result["Additional Service Rules"] = string.Join(Environment.NewLine + Environment.NewLine, additionalServiceRules);
            return result;


        }
    }
}