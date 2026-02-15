namespace ChatDashboard.Api.Models
{
    public class CouchDbQueryResult<T>
    {
        public List<T> Docs { get; set; }
    }
}
