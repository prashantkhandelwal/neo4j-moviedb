using MongoDB.Bson.Serialization.Attributes;
using Newtonsoft.Json;


[BsonIgnoreExtraElements]
[BsonDiscriminator("credits")]
public class Credits
{
    [BsonId]

    public object _id { get; set; }

    [JsonProperty("id")]
    public int Id { get; set; }

    [JsonProperty("cast")]
    public List<Cast> Cast { get; set; }
}

[BsonIgnoreExtraElements]
public class Cast
{
    [BsonId]
    public object _id { get; set; }

    [JsonProperty("id")]
    public int Id { get; set; }

    [JsonProperty("character")]
    public string? Character { get; set; }

    [JsonProperty("name")]
    public string Name { get; set; }

    [JsonProperty("original_name")]
    public string OriginalName { get; set; }

    [JsonProperty("adult")]
    public bool IsAdult { get; set; }

    [JsonProperty("gender")]
    public int Gender { get; set; }

    [JsonProperty("known_for_department")]
    public string KnownForDepartment { get; set; }

}

