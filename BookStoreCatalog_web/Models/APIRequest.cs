using static BookStoreCatalogUtils.ClassDefinitions;

namespace BookStoreCatalog_web.Models
{
    public class APIRequest
    {
        public APIType APIType { get; set; } = APIType.GET;

        public string URL { get; set; }

        public object Data { get; set; }

        public string Token { get; set; }

        public Parameters Parameters { get; set; }
    }

    public class Parameters
    {
        public int PageNumber { get; set; }
        public int PageSize { get; set; }
    }
}
