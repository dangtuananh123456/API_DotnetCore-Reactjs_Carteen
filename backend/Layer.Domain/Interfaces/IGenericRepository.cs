using Layer.Domain.Specifications;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Layer.Domain.Interfaces
{
	public interface IGenericRepository<T>
	{
		Task<List<T>> GetAllAsync();
		Task<T?> GetByIdAsync(int id);
		Task<T?> GetEntitySpecAsync(ISpecification<T> specification);
		Task<List<T>> GetListAsync(ISpecification<T> specification);
		//Task<IReadOnlyList<T>> ListAsync(ISpecification<T> specification);
		//Task<int> CountAsync(ISpecifications<T> specifications);
		void DeleteAsync(T entity);
		void UpdateAsync(T entity);
		void AddAsync(T entity);

		//void Add(T entity);
		//void Find(T entity);
		//void Update(T entity);
		//void Delete(T entity);
	}
}
