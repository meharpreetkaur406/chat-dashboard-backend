using System.Text.Json.Serialization;

public class CouchDbFindResponse<T>
{
    [JsonPropertyName("docs")]
    public List<T> Docs { get; set; } = new();
}
