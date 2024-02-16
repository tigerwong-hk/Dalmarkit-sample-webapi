using Dalmarkit.Common.Api.Responses;
using Dalmarkit.EntityFrameworkCore.Services.DataServices;
using Dalmarkit.Sample.Core.Dtos.Inputs;
using Dalmarkit.Sample.EntityFrameworkCore.Contexts;
using Dalmarkit.Sample.EntityFrameworkCore.Entities;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace Dalmarkit.Sample.Application.Services.DataServices;

public class EntityDataService(DalmarkitSampleDbContext dbContext)
    : ReadWriteDataServiceBase<DalmarkitSampleDbContext, Entity>(dbContext), IEntityDataService
{
    public async Task<Entity?> GetEntityDetailAsync(Guid entityId, CancellationToken cancellationToken = default)
    {
        IQueryable<Entity> queryable = DbContext.Entities
            .Where(x => !x.IsDeleted && x.EntityId == entityId)
            .Include(x => x.EntityImages!.Where(y => !y.IsDeleted));

        return await queryable.SingleOrDefaultAsync(cancellationToken);
    }

    public async Task<ResponsePagination<Entity>> GetEntityListAsync(GetEntityListInputDto inputDto, CancellationToken cancellationToken = default)
    {
        IQueryable<Entity> queryable = DbContext.Entities
            .Where(GetEntityListFilter(inputDto))
            .OrderByDescending(x => x.ModifiedOn);

        int filteredCount = await queryable.CountAsync(cancellationToken);

        List<Entity> data = await queryable
            .Skip(inputDto.GetSkipCount())
            .Take(inputDto.PageSize)
            .ToListAsync(cancellationToken);

        return new ResponsePagination<Entity>(data, filteredCount, inputDto.PageNumber, inputDto.PageSize);
    }

    protected static Expression<Func<Entity, bool>> GetEntityListFilter(GetEntityListInputDto inputDto)
    {
        return inputDto switch
        {
            GetEntitiesInputDto input when string.IsNullOrWhiteSpace(inputDto.EntityName) =>
                x => !x.IsDeleted,

            GetEntitiesInputDto input when !string.IsNullOrWhiteSpace(inputDto.EntityName) =>
                x => !x.IsDeleted && x.EntityName.Contains(inputDto.EntityName.Trim()),

            _ => throw new InvalidOperationException()
        };
    }
}
