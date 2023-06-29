using MongoDB.Bson.Serialization.Attributes;

[BsonIgnoreExtraElements]
[BsonDiscriminator("credits")]
public class Credits
{
    [BsonId]
    public object _id { get; set; }

    [BsonElement("id")]
    public int Id { get; set; }

    [BsonElement("cast")]
    public Cast[] Cast { get; set; }
}

[BsonIgnoreExtraElements]
public class Cast
{
    [BsonId]
    public object _id { get; set; }

    [BsonElement("id")]
    public int Id { get; set; }

    [BsonElement("character")]
    public string? Character { get; set; }

    [BsonElement("name")]
    public string Name { get; set; }

    [BsonElement("original_name")]
    public string OriginalName { get; set; }

    [BsonElement("adult")]
    public bool IsAdult { get; set; }

    [BsonElement("gender")]
    public int Gender { get; set; }

    [BsonElement("known_for_department")]
    public string KnownForDepartment { get; set; }

}

