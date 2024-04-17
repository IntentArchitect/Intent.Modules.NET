using System;
using System.Collections.Generic;
using System.Linq;
using AutoMapper;
using Intent.RoslynWeaver.Attributes;
using MongoDb.TestApplication.Application.Common.Mappings;
using MongoDb.TestApplication.Domain.Common.Exceptions;
using MongoDb.TestApplication.Domain.Entities;
using MongoDb.TestApplication.Domain.Entities.Mappings;
using MongoDb.TestApplication.Domain.Repositories;
using MongoDb.TestApplication.Domain.Repositories.Mappings;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.Dtos.DtoModel", Version = "1.0")]

namespace MongoDb.TestApplication.Application.MapperRoots
{
    public class MapperRootDto : IMapFrom<MapperRoot>
    {
        public MapperRootDto()
        {
            Id = null!;
            No = null!;
            MapAggChildrenIds = null!;
            MapAggPeerId = null!;
            CompChildAtt = null!;
            CompChildAggAtt = null!;
            PeerAtt = null!;
            MapAggPeerAggId = null!;
            PeerCompChildAtt = null!;
            MapPeerCompChildAggAtt = null!;
            MapAggPeerAggAtt = null!;
            MapAggPeerAggMoreAtt = null!;
            MapAggChildren = null!;
            MapMapMe = null!;
            MapImplyOptionalDescription = null!;
            MapperM2MS = null!;
        }

        public string Id { get; set; }
        public string No { get; set; }
        public List<string> MapAggChildrenIds { get; set; }
        public string MapAggPeerId { get; set; }
        public string CompChildAtt { get; set; }
        public string CompChildAggAtt { get; set; }
        public string PeerAtt { get; set; }
        public string MapAggPeerAggId { get; set; }
        public string PeerCompChildAtt { get; set; }
        public string MapPeerCompChildAggAtt { get; set; }
        public string MapAggPeerAggAtt { get; set; }
        public string MapAggPeerAggMoreAtt { get; set; }
        public List<MapAggChildDto> MapAggChildren { get; set; }
        public MapMapMeDto MapMapMe { get; set; }
        public string MapImplyOptionalDescription { get; set; }
        public List<MapperM2MDto> MapperM2MS { get; set; }

        public static MapperRootDto Create(
            string id,
            string no,
            List<string> mapAggChildrenIds,
            string mapAggPeerId,
            string compChildAtt,
            string compChildAggAtt,
            string peerAtt,
            string mapAggPeerAggId,
            string peerCompChildAtt,
            string mapPeerCompChildAggAtt,
            string mapAggPeerAggAtt,
            string mapAggPeerAggMoreAtt,
            List<MapAggChildDto> mapAggChildren,
            MapMapMeDto mapMapMe,
            string mapImplyOptionalDescription,
            List<MapperM2MDto> mapperM2MS)
        {
            return new MapperRootDto
            {
                Id = id,
                No = no,
                MapAggChildrenIds = mapAggChildrenIds,
                MapAggPeerId = mapAggPeerId,
                CompChildAtt = compChildAtt,
                CompChildAggAtt = compChildAggAtt,
                PeerAtt = peerAtt,
                MapAggPeerAggId = mapAggPeerAggId,
                PeerCompChildAtt = peerCompChildAtt,
                MapPeerCompChildAggAtt = mapPeerCompChildAggAtt,
                MapAggPeerAggAtt = mapAggPeerAggAtt,
                MapAggPeerAggMoreAtt = mapAggPeerAggMoreAtt,
                MapAggChildren = mapAggChildren,
                MapMapMe = mapMapMe,
                MapImplyOptionalDescription = mapImplyOptionalDescription,
                MapperM2MS = mapperM2MS
            };
        }

        public void Mapping(Profile profile)
        {
            profile.CreateMap<MapperRoot, MapperRootDto>()
                .ForMember(d => d.CompChildAtt, opt => opt.MapFrom(src => src.MapCompChild.CompChildAtt))
                .AfterMap<MappingAction>();
        }

        internal class MappingAction : IMappingAction<MapperRoot, MapperRootDto>
        {
            private readonly IMapCompChildAggRepository _mapCompChildAggRepository;
            private readonly IMapAggPeerRepository _mapAggPeerRepository;
            private readonly IMapPeerCompChildAggRepository _mapPeerCompChildAggRepository;
            private readonly IMapAggPeerAggRepository _mapAggPeerAggRepository;
            private readonly IMapAggPeerAggMoreRepository _mapAggPeerAggMoreRepository;
            private readonly IMapAggChildRepository _mapAggChildRepository;
            private readonly IMapMapMeRepository _mapMapMeRepository;
            private readonly IMapImplyOptionalRepository _mapImplyOptionalRepository;
            private readonly IMapperM2MRepository _mapperM2MRepository;
            private readonly IMapper _mapper;

            public MappingAction(IMapCompChildAggRepository mapCompChildAggRepository,
                IMapAggPeerRepository mapAggPeerRepository,
                IMapPeerCompChildAggRepository mapPeerCompChildAggRepository,
                IMapAggPeerAggRepository mapAggPeerAggRepository,
                IMapAggPeerAggMoreRepository mapAggPeerAggMoreRepository,
                IMapAggChildRepository mapAggChildRepository,
                IMapMapMeRepository mapMapMeRepository,
                IMapImplyOptionalRepository mapImplyOptionalRepository,
                IMapperM2MRepository mapperM2MRepository,
                IMapper mapper)
            {
                _mapCompChildAggRepository = mapCompChildAggRepository;
                _mapAggPeerRepository = mapAggPeerRepository;
                _mapPeerCompChildAggRepository = mapPeerCompChildAggRepository;
                _mapAggPeerAggRepository = mapAggPeerAggRepository;
                _mapAggPeerAggMoreRepository = mapAggPeerAggMoreRepository;
                _mapAggChildRepository = mapAggChildRepository;
                _mapMapMeRepository = mapMapMeRepository;
                _mapImplyOptionalRepository = mapImplyOptionalRepository;
                _mapperM2MRepository = mapperM2MRepository;
                _mapper = mapper;
            }

            public void Process(MapperRoot source, MapperRootDto destination, ResolutionContext context)
            {
                var mapAggPeer = _mapAggPeerRepository.FindByIdAsync(source.MapAggPeerId).Result;
                if (mapAggPeer == null)
                {
                    throw new NotFoundException($"Unable to load required relationship for Id({source.MapAggPeerId}). (MapperRoot)->(MapAggPeer)");
                }
                var mapperM2MS = _mapperM2MRepository.FindByIdsAsync(source.MapperM2MSIds.ToArray()).Result;
                var mapAggChildren = _mapAggChildRepository.FindByIdsAsync(source.MapAggChildrenIds.ToArray()).Result;
                var mapAggPeerMapMapMe = _mapMapMeRepository.FindByIdAsync(mapAggPeer.MapMapMeId).Result;

                if (mapAggPeerMapMapMe == null)
                {
                    throw new NotFoundException($"Unable to load required relationship for Id({mapAggPeer.MapMapMeId}). (MapAggPeer)->(MapMapMe)");
                }
                var mapAggPeerMapAggPeerAgg = _mapAggPeerAggRepository.FindByIdAsync(mapAggPeer.MapAggPeerAggId).Result;

                if (mapAggPeerMapAggPeerAgg == null)
                {
                    throw new NotFoundException($"Unable to load required relationship for Id({mapAggPeer.MapAggPeerAggId}). (MapAggPeer)->(MapAggPeerAgg)");
                }
                var mapCompChildMapCompChildAgg = _mapCompChildAggRepository.FindByIdAsync(source.MapCompChild.MapCompChildAggId).Result;

                if (mapCompChildMapCompChildAgg == null)
                {
                    throw new NotFoundException($"Unable to load required relationship for Id({source.MapCompChild.MapCompChildAggId}). (MapCompChild)->(MapCompChildAgg)");
                }
                var mapCompOptionalMapImplyOptional = source.MapCompOptional?.MapImplyOptionalId != null ? _mapImplyOptionalRepository.FindByIdAsync(source.MapCompOptional.MapImplyOptionalId).Result : null;
                var mapAggPeerMapAggPeerAggMapAggPeerAggMore = _mapAggPeerAggMoreRepository.FindByIdAsync(mapAggPeerMapAggPeerAgg.MapAggPeerAggMoreId).Result;

                if (mapAggPeerMapAggPeerAggMapAggPeerAggMore == null)
                {
                    throw new NotFoundException($"Unable to load required relationship for Id({mapAggPeerMapAggPeerAgg.MapAggPeerAggMoreId}). (MapAggPeerAgg)->(MapAggPeerAggMore)");
                }
                var mapAggPeerMapPeerCompChildMapPeerCompChildAgg = _mapPeerCompChildAggRepository.FindByIdAsync(mapAggPeer.MapPeerCompChild.MapPeerCompChildAggId).Result;

                if (mapAggPeerMapPeerCompChildMapPeerCompChildAgg == null)
                {
                    throw new NotFoundException($"Unable to load required relationship for Id({mapAggPeer.MapPeerCompChild.MapPeerCompChildAggId}). (MapPeerCompChild)->(MapPeerCompChildAgg)");
                }
                destination.CompChildAggAtt = mapCompChildMapCompChildAgg.CompChildAggAtt;
                destination.PeerAtt = mapAggPeer.PeerAtt;
                destination.MapAggPeerAggId = mapAggPeer.MapAggPeerAggId;
                destination.PeerCompChildAtt = mapAggPeer.MapPeerCompChild.PeerCompChildAtt;
                destination.MapPeerCompChildAggAtt = mapAggPeerMapPeerCompChildMapPeerCompChildAgg.MapPeerCompChildAggAtt;
                destination.MapAggPeerAggAtt = mapAggPeerMapAggPeerAgg.MapAggPeerAggAtt;
                destination.MapAggPeerAggMoreAtt = mapAggPeerMapAggPeerAggMapAggPeerAggMore.MapAggPeerAggMoreAtt;
                destination.MapAggChildren = mapAggChildren.MapToMapAggChildDtoList(_mapper);
                destination.MapMapMe = mapAggPeerMapMapMe.MapToMapMapMeDto(_mapper);
                destination.MapImplyOptionalDescription = mapCompOptionalMapImplyOptional != null ? mapCompOptionalMapImplyOptional.Description : null;
                destination.MapperM2MS = mapperM2MS.MapToMapperM2MDtoList(_mapper);
            }
        }
    }
}