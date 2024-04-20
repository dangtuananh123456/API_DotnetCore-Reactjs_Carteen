using Infrastructure.Data;
using Layer.Domain.Interfaces;
using Layer.Domain.Specifications;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Repositories
{
    public class GenericRepository<T> : IGenericRepository<T> where T : class
    {
        private readonly AppDbContext dbContext;

        public GenericRepository(AppDbContext _dbContext)
        {
            this.dbContext = _dbContext;
        }

        public async Task<List<T>> GetAllAsync()
        {
            return await dbContext.Set<T>().ToListAsync();
        }

        public async Task<T?> GetByIdAsync(int id)
        {
            return await dbContext.Set<T>().FindAsync(id);
        }

        public async Task<List<T>> GetListAsync(ISpecification<T> specification)
        {
			return await ApplySpecification(specification).ToListAsync();
		}

        public async Task<T?> GetEntitySpecAsync(ISpecification<T> specification)
        {
            return await ApplySpecification(specification).FirstOrDefaultAsync();
        }

		private IQueryable<T> ApplySpecification(ISpecification<T> specifications)
		{
			return SpecificationEvaluator<T>.GetQuery(dbContext.Set<T>().AsQueryable(), specifications);
		}

		public async void DeleteAsync(T entity)
        {
            dbContext.Set<T>().Remove(entity);
            await dbContext.SaveChangesAsync();
        }

        public async void UpdateAsync(T entity)
        {
            dbContext.Attach<T>(entity);
            dbContext.Entry(entity).State = EntityState.Modified;
            await dbContext.SaveChangesAsync();
        }

        public async void AddAsync(T entity)
        {
            await dbContext.AddAsync<T>(entity);
            await dbContext.SaveChangesAsync();
        }
    }
}
