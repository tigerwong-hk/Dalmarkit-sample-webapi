using Dalmarkit.Blockchain.Evm.Services;
using Dalmarkit.Sample.Core.Dtos.Inputs;
using Dalmarkit.Sample.EntityFrameworkCore.Entities;
using Riok.Mapperly.Abstractions;

namespace Dalmarkit.Sample.Application.Mapping.InputDtosToEntities;

[Mapper(RequiredMappingStrategy = RequiredMappingStrategy.Source)]
public static partial class EvmEventInputDtoToEntityMapper
{
    [MapperIgnoreSource(nameof(PutEvmEventByNameInputDto.ContractName))]
    public static partial EvmEvent ToTarget(PutEvmEventByNameInputDto source, string contractAddress, string eventDetail);

    [MapperIgnoreSource(nameof(EvmEventDto.BlockHash))]
    [MapperIgnoreSource(nameof(EvmEventDto.BlockNumber))]
    [MapperIgnoreSource(nameof(EvmEventDto.EventData))]
    [MapperIgnoreSource(nameof(EvmEventDto.EventJson))]
    [MapperIgnoreSource(nameof(EvmEventDto.LogIndex))]
    [MapperIgnoreSource(nameof(EvmEventDto.Topics))]
    [MapperIgnoreSource(nameof(EvmEventDto.TransactionIndex))]
    public static partial EvmEvent ToTarget(EvmEventDto source, string createRequestId);

    [MapperIgnoreSource(nameof(UpdateEntityInputDto.EntityId))]
    public static partial void UpdateTarget(UpdateEntityInputDto source, Entity target);
}
