using Microsoft.EntityFrameworkCore;
using WMS.SharedKernel.Entities;
using WMS.SharedKernel.Interfaces;

namespace WMS.Inventory.Infrastructure.Data
{
    public class SpecificationEvaluator<T> where T : Entity
    {
        public static IQueryable<T> GetQuery(IQueryable<T> inputQuery, ISpecification<T> specification)
        {
            var query = inputQuery;
            
            // Apply where clause
            if (specification.Criteria != null)
            {
                query = query.Where(specification.Criteria);
            }
            
            // Include related entities
            query = specification.Includes.Aggregate(query, (current, include) => current.Include(include));
            
            // Include string-based includes
            query = specification.IncludeStrings.Aggregate(query, (current, include) => current.Include(include));
            
            // Apply ordering
            if (specification.OrderBy != null)
            {
                query = query.OrderBy(specification.OrderBy);
            }
            
            if (specification.OrderByDescending != null)
            {
                query = query.OrderByDescending(specification.OrderByDescending);
            }
            
            // Apply paging
            if (specification.IsPagingEnabled)
            {
                query = query.Skip(specification.Skip).Take(specification.Take);
            }
            
            return query;
        }
    }
}