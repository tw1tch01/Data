using System.Collections.Generic;

namespace Data.Common
{
    public class PagedCollection<TType> where TType : class
    {
        public PagedCollection(int page, int pageSize, int totalRecords, ICollection<TType> items)
        {
            Page = page;
            PageSize = pageSize;
            PageCount = (items.Count + totalRecords - 1) / pageSize;
            Items = items;
        }

        public int Page { get; }
        public int PageSize { get; }
        public int PageCount { get; }
        public int TotalRecords { get; }
        public virtual ICollection<TType> Items { get; }
    }
}