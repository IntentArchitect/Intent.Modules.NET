using AutoMapper;
using Intent.RoslynWeaver.Attributes;
using JsonPatchRfc7396.Swashbuckle.Application.ConfigurationStores;
using JsonPatchRfc7396.Swashbuckle.Application.Interfaces;
using JsonPatchRfc7396.Swashbuckle.Domain.Common;
using JsonPatchRfc7396.Swashbuckle.Domain.Common.Exceptions;
using JsonPatchRfc7396.Swashbuckle.Domain.Configuration;
using JsonPatchRfc7396.Swashbuckle.Domain.Entities.Configuration;
using JsonPatchRfc7396.Swashbuckle.Domain.Repositories.Configuration;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.ServiceImplementations.ServiceImplementation", Version = "1.0")]

namespace JsonPatchRfc7396.Swashbuckle.Application.Implementation
{
    [IntentManaged(Mode.Merge)]
    public class ConfigurationStoresService : IConfigurationStoresService
    {
        private readonly IConfigurationStoreRepository _configurationStoreRepository;
        private readonly IMapper _mapper;

        [IntentManaged(Mode.Merge)]
        public ConfigurationStoresService(IConfigurationStoreRepository configurationStoreRepository, IMapper mapper)
        {
            _configurationStoreRepository = configurationStoreRepository;
            _mapper = mapper;
        }

        [IntentManaged(Mode.Fully, Body = Mode.Fully)]
        public async Task<Guid> CreateConfigurationStore(
            CreateConfigurationStoreDto dto,
            CancellationToken cancellationToken = default)
        {
            var configurationStore = new ConfigurationStore
            {
                Name = dto.Name,
                Items = dto.Items?
                    .Select(i => new ConfigurationItem
                    {
                        Key = new ConfigurationKey(
                            value: i.Key.Value),
                        ScopeKey = new ConfigurationScopeKey(
                            scope: i.ScopeKey.Scope,
                            scopeId: i?.ScopeKey.ScopeId),
                        ValueType = i.ValueType,
                        Value = i.Value,
                        IsActive = i.IsActive,
                        Version = i.Version,
                        UpdatedAtUtc = i.UpdatedAtUtc,
                        LatestChangeId = i?.LatestChangeId
                    })
                    .ToList(),
                Changes = dto.Changes?
                    .Select(c => new ConfigurationChange
                    {
                        Key = new ConfigurationKey(
                            value: c.Key.Value),
                        ScopeKey = new ConfigurationScopeKey(
                            scope: c.ScopeKey.Scope,
                            scopeId: c?.ScopeKey.ScopeId),
                        OldValue = c?.OldValue,
                        NewValue = c?.NewValue,
                        ChangedAtUtc = c.ChangedAtUtc,
                        ChangedBy = c.ChangedBy
                    })
                    .ToList()
            };

            _configurationStoreRepository.Add(configurationStore);
            await _configurationStoreRepository.UnitOfWork.SaveChangesAsync(cancellationToken);
            return configurationStore.Id;
        }

        [IntentManaged(Mode.Fully, Body = Mode.Fully)]
        public async Task UpdateConfigurationStore(
            Guid id,
            UpdateConfigurationStoreDto dto,
            CancellationToken cancellationToken = default)
        {
            var configurationStore = await _configurationStoreRepository.FindByIdAsync(id, cancellationToken);
            if (configurationStore is null)
            {
                throw new NotFoundException($"Could not find ConfigurationStore '{id}'");
            }

            configurationStore.Name = dto.Name;
            configurationStore.Items = UpdateHelper.CreateOrUpdateCollection(configurationStore.Items, dto.Items, (e, d) => e.Id == d.Id, CreateOrUpdateConfigurationItem);
            configurationStore.Changes = UpdateHelper.CreateOrUpdateCollection(configurationStore.Changes, dto.Changes, (e, d) => e.Id == d.Id, CreateOrUpdateConfigurationChange);
        }

        [IntentManaged(Mode.Fully, Body = Mode.Fully)]
        public async Task<ConfigurationStoreDto> PatchConfigurationStore(
            Guid id,
            PatchConfigurationStoreDto dto,
            CancellationToken cancellationToken = default)
        {
            var configurationStore = await _configurationStoreRepository.FindByIdAsync(id, cancellationToken);
            if (configurationStore is null)
            {
                throw new NotFoundException($"Could not find ConfigurationStore '{id}'");
            }

            LoadOriginalState(configurationStore, dto);
            dto.PatchExecutor.ApplyTo(dto);
            ApplyChangesTo(dto, configurationStore);
            return configurationStore.MapToConfigurationStoreDto(_mapper);
        }

        [IntentManaged(Mode.Fully, Body = Mode.Fully)]
        public async Task<ConfigurationStoreDto> FindConfigurationStoreById(
            Guid id,
            CancellationToken cancellationToken = default)
        {
            var configurationStore = await _configurationStoreRepository.FindByIdAsync(id, cancellationToken);
            if (configurationStore is null)
            {
                throw new NotFoundException($"Could not find ConfigurationStore '{id}'");
            }
            return configurationStore.MapToConfigurationStoreDto(_mapper);
        }

        [IntentManaged(Mode.Fully, Body = Mode.Fully)]
        public async Task<List<ConfigurationStoreDto>> FindConfigurationStores(CancellationToken cancellationToken = default)
        {
            var configurationStores = await _configurationStoreRepository.FindAllAsync(cancellationToken);
            return configurationStores.MapToConfigurationStoreDtoList(_mapper);
        }

        [IntentManaged(Mode.Fully, Body = Mode.Fully)]
        public async Task DeleteConfigurationStore(Guid id, CancellationToken cancellationToken = default)
        {
            var configurationStore = await _configurationStoreRepository.FindByIdAsync(id, cancellationToken);
            if (configurationStore is null)
            {
                throw new NotFoundException($"Could not find ConfigurationStore '{id}'");
            }


            _configurationStoreRepository.Remove(configurationStore);
        }

        [IntentManaged(Mode.Fully, Body = Mode.Fully)]
        public async Task<ConfigurationConfigurationItemDto> PatchConfigurationItem(
            Guid configurationStoreId,
            Guid id,
            PatchConfigurationItemDto dto,
            CancellationToken cancellationToken = default)
        {
            var configurationStore = await _configurationStoreRepository.FindByIdAsync(configurationStoreId, cancellationToken);
            if (configurationStore is null)
            {
                throw new NotFoundException($"Could not find ConfigurationStore '{configurationStoreId}'");
            }

            var configurationItem = configurationStore.Items.FirstOrDefault(x => x.Id == id);
            if (configurationItem is null)
            {
                throw new NotFoundException($"Could not find ConfigurationItem '{id}'");
            }
            LoadOriginalState(configurationItem, dto);
            dto.PatchExecutor.ApplyTo(dto);
            ApplyChangesTo(dto, configurationItem);
            return configurationItem.MapToConfigurationConfigurationItemDto(_mapper);
        }

        private static PatchConfigurationStoreDto LoadOriginalState(
            ConfigurationStore entity,
            PatchConfigurationStoreDto dto)
        {
            ArgumentNullException.ThrowIfNull(entity);
            ArgumentNullException.ThrowIfNull(dto);
            dto.Name = entity.Name;
            dto.Changes = entity.Changes?
                .Select(c => new PatchConfigurationStoreChangesDto
                {
                    Id = c.Id,
                    Key = new PatchConfigurationStoreKeyDto
                    {
                        Value = c.Key.Value
                    },
                    ScopeKey = new PatchConfigurationStoreScopeKeyDto
                    {
                        Scope = c.ScopeKey.Scope,
                        ScopeId = c?.ScopeKey.ScopeId
                    },
                    OldValue = c?.OldValue,
                    NewValue = c?.NewValue,
                    ChangedAtUtc = c.ChangedAtUtc,
                    ChangedBy = c.ChangedBy
                })
                .ToList();
            dto.Items = entity.Items?
                .Select(i => new PatchConfigurationStoreItemsDto
                {
                    Id = i.Id,
                    Key = new PatchConfigurationStoreKeyDto
                    {
                        Value = i.Key.Value
                    },
                    ScopeKey = new PatchConfigurationStoreScopeKeyDto
                    {
                        Scope = i.ScopeKey.Scope,
                        ScopeId = i?.ScopeKey.ScopeId
                    },
                    ValueType = i.ValueType,
                    Value = i.Value,
                    IsActive = i.IsActive,
                    Version = i.Version,
                    UpdatedAtUtc = i.UpdatedAtUtc,
                    LatestChangeId = i?.LatestChangeId
                })
                .ToList();
            return dto;
        }

        private static PatchConfigurationItemDto LoadOriginalState(ConfigurationItem entity, PatchConfigurationItemDto dto)
        {
            ArgumentNullException.ThrowIfNull(entity);
            ArgumentNullException.ThrowIfNull(dto);
            dto.Key ??= new PatchConfigurationItemKeyDto1();
            dto.Key.Value = entity.Key.Value;
            dto.ScopeKey ??= new PatchConfigurationItemScopeKeyDto1();
            dto.ScopeKey.Scope = entity.ScopeKey.Scope;
            dto.ScopeKey.ScopeId = entity.ScopeKey.ScopeId;
            dto.ValueType = entity.ValueType;
            dto.Value = entity.Value;
            dto.IsActive = entity.IsActive;
            dto.Version = entity.Version;
            dto.UpdatedAtUtc = entity.UpdatedAtUtc;
            dto.LatestChangeId = entity.LatestChangeId;
            return dto;
        }

        private static ConfigurationStore ApplyChangesTo(PatchConfigurationStoreDto dto, ConfigurationStore entity)
        {
            ArgumentNullException.ThrowIfNull(dto);
            ArgumentNullException.ThrowIfNull(entity);
            entity.Name = dto.Name;
            entity.Items = UpdateHelper.CreateOrUpdateCollection(entity.Items, dto.Items, (e, d) => e.Id == d.Id, CreateOrUpdateConfigurationItem);
            entity.Changes = UpdateHelper.CreateOrUpdateCollection(entity.Changes, dto.Changes, (e, d) => e.Id == d.Id, CreateOrUpdateConfigurationChange);
            return entity;
        }

        private static ConfigurationItem ApplyChangesTo(PatchConfigurationItemDto dto, ConfigurationItem entity)
        {
            ArgumentNullException.ThrowIfNull(dto);
            ArgumentNullException.ThrowIfNull(entity);
            entity.Key = new ConfigurationKey(
                value: dto.Key.Value);
            entity.ScopeKey = new ConfigurationScopeKey(
                scope: dto.ScopeKey.Scope,
                scopeId: dto.ScopeKey.ScopeId);
            entity.ValueType = dto.ValueType;
            entity.Value = dto.Value;
            entity.IsActive = dto.IsActive;
            entity.Version = dto.Version;
            entity.UpdatedAtUtc = dto.UpdatedAtUtc;
            entity.LatestChangeId = dto.LatestChangeId;
            return entity;
        }

        [IntentManaged(Mode.Fully)]
        private static ConfigurationItem? CreateOrUpdateConfigurationItem(
            ConfigurationItem? entity,
            UpdateConfigurationStoreItemsDto dto)
        {
            if (dto == null)
            {
                return null;
            }
            entity ??= new ConfigurationItem();
            entity.Id = dto.Id;
            entity.Key = new ConfigurationKey(
                value: dto.Key.Value);
            entity.ScopeKey = new ConfigurationScopeKey(
                scope: dto.ScopeKey.Scope,
                scopeId: dto?.ScopeKey.ScopeId);
            entity.ValueType = dto.ValueType;
            entity.Value = dto.Value;
            entity.IsActive = dto.IsActive;
            entity.Version = dto.Version;
            entity.UpdatedAtUtc = dto.UpdatedAtUtc;
            entity.LatestChangeId = dto?.LatestChangeId;
            return entity;
        }

        [IntentManaged(Mode.Fully)]
        private static ConfigurationItem? CreateOrUpdateConfigurationItem(
            ConfigurationItem? entity,
            PatchConfigurationStoreItemsDto dto)
        {
            if (dto == null)
            {
                return null;
            }
            entity ??= new ConfigurationItem();
            entity.Id = dto.Id;
            entity.Key = new ConfigurationKey(
                value: dto.Key.Value);
            entity.ScopeKey = new ConfigurationScopeKey(
                scope: dto.ScopeKey.Scope,
                scopeId: dto?.ScopeKey.ScopeId);
            entity.ValueType = dto.ValueType;
            entity.Value = dto.Value;
            entity.IsActive = dto.IsActive;
            entity.Version = dto.Version;
            entity.UpdatedAtUtc = dto.UpdatedAtUtc;
            entity.LatestChangeId = dto?.LatestChangeId;
            return entity;
        }

        [IntentManaged(Mode.Fully)]
        private static ConfigurationChange? CreateOrUpdateConfigurationChange(
            ConfigurationChange? entity,
            UpdateConfigurationStoreChangesDto dto)
        {
            if (dto == null)
            {
                return null;
            }
            entity ??= new ConfigurationChange();
            entity.Id = dto.Id;
            entity.Key = new ConfigurationKey(
                value: dto.Key.Value);
            entity.ScopeKey = new ConfigurationScopeKey(
                scope: dto.ScopeKey.Scope,
                scopeId: dto?.ScopeKey.ScopeId);
            entity.OldValue = dto?.OldValue;
            entity.NewValue = dto?.NewValue;
            entity.ChangedAtUtc = dto.ChangedAtUtc;
            entity.ChangedBy = dto.ChangedBy;
            return entity;
        }

        [IntentManaged(Mode.Fully)]
        private static ConfigurationChange? CreateOrUpdateConfigurationChange(
            ConfigurationChange? entity,
            PatchConfigurationStoreChangesDto dto)
        {
            if (dto == null)
            {
                return null;
            }
            entity ??= new ConfigurationChange();
            entity.Id = dto.Id;
            entity.Key = new ConfigurationKey(
                value: dto.Key.Value);
            entity.ScopeKey = new ConfigurationScopeKey(
                scope: dto.ScopeKey.Scope,
                scopeId: dto?.ScopeKey.ScopeId);
            entity.OldValue = dto?.OldValue;
            entity.NewValue = dto?.NewValue;
            entity.ChangedAtUtc = dto.ChangedAtUtc;
            entity.ChangedBy = dto.ChangedBy;
            return entity;
        }
    }
}