using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;
using Pizzeria.Domain.Core;
using Pizzeria.Domain.Repository;
using Pizzeria.Infrastructure.Context;

namespace Pizzeria.Infrastructure.Core
{
    public abstract class BaseRepository<TEntity> : IBaseRepository<TEntity> where TEntity : BaseEntity
    {
        protected readonly PizzeriaContext Context;
        protected readonly DbSet<TEntity> Entities;

        protected BaseRepository(PizzeriaContext context)
        {
            Context = context;
            Entities = context.Set<TEntity>();
        }

        public virtual Task<List<TEntity>> GetAllAsync()
        {
            return Entities.AsNoTracking().Where(e => !e.IsDeleted).ToListAsync();
        }

        public virtual Task<TEntity?> GetByIdAsync(int id)
        {
            return Entities.FirstOrDefaultAsync(e => e.Id == id && !e.IsDeleted);
        }

        public virtual Task<List<TEntity>> FindAsync(Expression<Func<TEntity, bool>> filter)
        {
            return Entities.AsNoTracking().Where(e => !e.IsDeleted).Where(filter).ToListAsync();
        }

        public virtual Task<bool> ExistsAsync(Expression<Func<TEntity, bool>> filter)
        {
            return Entities.Where(e => !e.IsDeleted).AnyAsync(filter);
        }

        public virtual async Task SaveAsync(TEntity entity)
        {
            entity.CreatedDate = DateTime.Now;
            await Entities.AddAsync(entity);
            await Context.SaveChangesAsync();
        }

        public virtual async Task UpdateAsync(TEntity entity)
        {
            entity.ModifiedDate = DateTime.Now;
            Entities.Update(entity);
            await Context.SaveChangesAsync();
        }

        public virtual async Task RemoveAsync(TEntity entity)
        {
            entity.IsDeleted = true;
            entity.DeletedDate = DateTime.Now;
            Entities.Update(entity);
            await Context.SaveChangesAsync();
        }
    }
}
