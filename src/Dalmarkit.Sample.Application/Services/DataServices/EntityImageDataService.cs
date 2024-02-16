using Dalmarkit.EntityFrameworkCore.Services.DataServices;
using Dalmarkit.Sample.EntityFrameworkCore.Contexts;
using Dalmarkit.Sample.EntityFrameworkCore.Entities;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace Dalmarkit.Sample.Application.Services.DataServices;

public class EntityImageDataService(DalmarkitSampleDbContext dbContext)
            : ReadWriteDataServiceBase<DalmarkitSampleDbContext, EntityImage>(dbContext), IEntityImageDataService
{
    public Task<EntityImage?> GetByEntityImageIdAsync(Guid entityImageId)
    {
        return DbContext.EntityImages
            .SingleOrDefaultAsync(x => x.EntityImageId == entityImageId && !x.IsDeleted && !x.Entity.IsDeleted);
    }

    public Task<EntityImage?> GetEntityByExpressionAsync(Expression<Func<EntityImage, bool>> expression, CancellationToken cancellationToken = default)
    {
        return DbContext.EntityImages.Where(expression).SingleOrDefaultAsync(cancellationToken);
    }
}
