using Dalmarkit.EntityFrameworkCore.Services.DataServices;
using Dalmarkit.Sample.EntityFrameworkCore.Contexts;
using Dalmarkit.Sample.EntityFrameworkCore.Entities;
using Microsoft.EntityFrameworkCore;

namespace Dalmarkit.Sample.Application.Services.DataServices;

public class DependentEntityDataService(DalmarkitSampleDbContext dbContext)
    : MultipleReadWriteDataServiceBase<DalmarkitSampleDbContext, DependentEntity>(dbContext), IDependentEntityDataService
{
    public Task<DependentEntity?> GetDependentEntityDetailAsync(Guid dependentEntityId)
    {
        return DbContext.DependentEntities
            .SingleOrDefaultAsync(x => x.DependentEntityId == dependentEntityId && !x.IsDeleted && !x.Entity.IsDeleted);
    }

    public async Task<IEnumerable<DependentEntity>> GetDependentEntitiesAsync(Guid entityId, CancellationToken cancellationToken = default)
    {
        IQueryable<DependentEntity> queryable = DbContext.DependentEntities
            .Where(x => x.EntityId == entityId && !x.IsDeleted && !x.Entity.IsDeleted)
            .OrderBy(x => x.CreatedOn);

        int filteredCount = await queryable.CountAsync(cancellationToken);

        return await queryable.ToListAsync(cancellationToken);
    }
}
