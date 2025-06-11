namespace BookStoreCatalog_API.Models.Settings
{
    public class PagedList<T> :List<T>
    {
        public MetaData MetaData { get; set; }

        public PagedList(List<T> items, int count, int pageNumber, int pageSize) 
        {
            MetaData = new MetaData();

            MetaData.TotalCount = count;
            MetaData.PageSize = pageSize;
            MetaData.TotalPages = (int)Math.Ceiling(count / (double)pageSize); //--- 1.5 rount to 2
            
            AddRange(items);

        }

        public static PagedList<T> ToPagedList(IEnumerable<T> entity, int pageNumber, int pageSize)
        {
            var count = entity.Count();
            var items = entity.Skip((pageNumber - 1) * pageSize).Take(pageSize).ToList();

            return new PagedList<T>(items, count, pageNumber, pageSize);
        }
        
    }
}
