using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Layer.Domain.Specifications
{
	public class BaseSpecification<T> : ISpecification<T>
	{
		public Expression<Func<T, bool>> Criteria { get; }

		public BaseSpecification()
		{

		}

		//filter theo tiêu chí.
		public BaseSpecification(Expression<Func<T, bool>> Criteria)
		{
			this.Criteria = Criteria;
		}

		public List<Expression<Func<T, object>>> Includes { get; } //load dữ liệu
		= new List<Expression<Func<T, object>>>();

		public Expression<Func<T, object>> OrderBy { get; private set; }

		public Expression<Func<T, object>> OrderByDescending { get; private set; }

		public int Take { get; private set; }

		public int Skip { get; private set; }

		public bool isPagingEnabled { get; private set; }

		//load data
		protected void AddInclude(Expression<Func<T, object>> includeExpression)
		{
			Includes.Add(includeExpression);
		}

		public void AddOrderBy(Expression<Func<T, object>> OrderByexpression)
		{
			OrderBy = OrderByexpression;
		}
		public void AddOrderByDecending(Expression<Func<T, object>> OrderByDecending)
		{
			OrderByDescending = OrderByDecending;
		}
		public void ApplyPagging(int take, int skip)
		{
			Take = take;
			Skip = skip;
			isPagingEnabled = true;
		}
	}
}
