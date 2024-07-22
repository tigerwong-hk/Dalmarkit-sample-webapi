using Dalmarkit.Common.Api.Responses;
using Dalmarkit.EntityFrameworkCore.Services.DataServices;
using Dalmarkit.Sample.Core.Dtos.Inputs;
using Dalmarkit.Sample.EntityFrameworkCore.Contexts;
using Dalmarkit.Sample.EntityFrameworkCore.Entities;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace Dalmarkit.Sample.Application.Services.DataServices;

public class EvmEventDataService(DalmarkitSampleDbContext dbContext)
    : ReadOnlyDataServiceBase<DalmarkitSampleDbContext, EvmEvent>(dbContext), IEvmEventDataService
{
    public async Task<EvmEvent?> GetEvmEventInfoAsync(Guid evmEventId, CancellationToken cancellationToken = default)
    {
        IQueryable<EvmEvent> queryable = DbContext.EvmEvents
            .Where(x => x.EvmEventId == evmEventId);

        return await queryable.SingleOrDefaultAsync(cancellationToken);
    }

    public async Task<ResponsePagination<EvmEvent>> GetEvmEventsAsync(GetEvmEventsInputDto inputDto, CancellationToken cancellationToken = default)
    {
        IQueryable<EvmEvent> queryable = DbContext.EvmEvents
            .Where(GetEvmEventsFilter(inputDto))
            .OrderByDescending(x => x.CreatedOn);

        int filteredCount = await queryable.CountAsync(cancellationToken);

        List<EvmEvent> data = await queryable
            .Skip(inputDto.GetSkipCount())
            .Take(inputDto.PageSize)
            .ToListAsync(cancellationToken);

        return new ResponsePagination<EvmEvent>(data, filteredCount, inputDto.PageNumber, inputDto.PageSize);
    }

    protected static Expression<Func<EvmEvent, bool>> GetEvmEventsFilter(GetEvmEventsInputDto inputDto)
    {
        return inputDto switch
        {
            GetEvmEventsInputDto input when string.IsNullOrWhiteSpace(inputDto.EventName) =>
                _ => true,

            GetEvmEventsInputDto input when !string.IsNullOrWhiteSpace(inputDto.EventName) =>
                x => x.EventName.Contains(inputDto.EventName.Trim()),

            _ => throw new InvalidOperationException()
        };
    }
}
