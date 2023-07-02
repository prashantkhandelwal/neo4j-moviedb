using MongoDB.Bson.Serialization.Attributes;
using Newtonsoft.Json;


[BsonIgnoreExtraElements]
[BsonDiscriminator("credits")]
public class Credits
{
    [BsonId]
    public object _id { get; set; }

    [BsonElement("id")]
    [JsonProperty("id")]
    public int Id { get; set; }

    [BsonElement("cast")]
    [JsonProperty("cast")]
    public List<Cast> Cast { get; set; }

    [BsonElement("crew")]
    [JsonProperty("crew")]
    public List<Crew> Crew { get; set; }
}

[BsonIgnoreExtraElements]
public class Cast
{
    [BsonId]
    public object _id { get; set; }

    [BsonElement("id")]
    [JsonProperty("id")]
    public int Id { get; set; }

    [BsonElement("character")]
    [JsonProperty("character")]
    public string? Character { get; set; }

    [BsonElement("name")]
    [JsonProperty("name")]
    public string Name { get; set; }

    [BsonElement("original_name")]
    [JsonProperty("original_name")]
    public string OriginalName { get; set; }

    [BsonElement("adult")]
    [JsonProperty("adult")]
    public bool IsAdult { get; set; }

    [BsonElement("gender")]
    [JsonProperty("gender")]
    public int Gender { get; set; }

    [BsonElement("known_for_department")]
    [JsonProperty("known_for_department")]
    public string KnownForDepartment { get; set; }

    [BsonElement("popularity")]
    [JsonProperty("popularity")]
    public double Popularity { get; set; }

}

[BsonIgnoreExtraElements]
public class Crew
{
    [BsonId]
    public object _id { get; set; }

    [BsonElement("id")]
    [JsonProperty("id")]
    public int Id { get; set; }

    [BsonElement("credit_id")]
    [JsonProperty("creditid")]
    public string CreditId { get; set; }

    [BsonElement("department")]
    [JsonProperty("department")]
    public string Department { get; set; }

    [BsonElement("job")]
    [JsonProperty("job")]
    public string Job { get; set; }

    [BsonElement("adult")]
    [JsonProperty("adult")]
    public bool IsAdult { get; set; }

    [BsonElement("gender")]
    [JsonProperty("gender")]
    public int Gender { get; set; }

    [BsonElement("known_for_department")]
    [JsonProperty("known_for_department")]
    public string KnownForDepartment { get; set; }

    [BsonElement("name")]
    [JsonProperty("name")]
    public string Name { get; set; }

    [BsonElement("original_name")]
    [JsonProperty("original_name")]
    public string OriginalName { get; set; }

    [BsonElement("profile_path")]
    [JsonProperty("profile_path")]
    public string? ProfilePath { get; set; }

    [BsonElement("popularity")]
    [JsonProperty("popularity")]
    public double Popularity { get; set; }
}

