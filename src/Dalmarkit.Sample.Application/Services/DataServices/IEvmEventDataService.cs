using Dalmarkit.Common.Api.Responses;
using Dalmarkit.EntityFrameworkCore.Services.DataServices;
using Dalmarkit.Sample.Core.Dtos.Inputs;
using Dalmarkit.Sample.EntityFrameworkCore.Entities;

namespace Dalmarkit.Sample.Application.Services.DataServices;

public interface IEvmEventDataService : IReadOnlyDataServiceBase<EvmEvent>
{
    Task<EvmEvent?> GetEvmEventInfoAsync(Guid evmEventId, CancellationToken cancellationToken = default);

    Task<ResponsePagination<EvmEvent>> GetEvmEventsAsync(GetEvmEventsInputDto inputDto, CancellationToken cancellationToken = default);
}
