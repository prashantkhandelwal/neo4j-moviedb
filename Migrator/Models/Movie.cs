using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using Newtonsoft.Json;

[BsonIgnoreExtraElements]
[BsonDiscriminator("movies")]
public class Movie
{
    private DateTime? _releaseDate;

    [BsonId]
    public object _id { get; set; }

    [BsonElement("id")]
    [JsonProperty("id")]
    public int TMDBId { get; set; }

    [BsonElement("imdb_id")]
    [JsonProperty("imdb_id")]
    public string IMDBId { get; set; }

    [BsonElement("title")]
    [JsonProperty("title")]
    public string Title { get; set; }

    [BsonElement("belongs_to_collection")]
    [JsonProperty("belongs_to_collection")]
    public BelongsToCollection? BelongsToCollection { get; set; }

    [BsonElement("original_title")]
    [JsonProperty("original_title")]
    public string OriginalTitle { get; set; }

    [BsonElement("original_language")]
    [JsonProperty("original_language")]
    public string OriginalLanguage { get; set; }

    [BsonElement("tagline")]
    [JsonProperty("tagline")]
    public string? TagLine { get; set; }

    [BsonElement("overview")]
    [JsonProperty("overview")]
    public string Overview { get; set; }

    [BsonElement("popularity")]
    [JsonProperty("popularity")]
    public double Popularity { get; set; }

    [BsonElement("production_companies")]
    [JsonProperty("production_companies")]
    public ProductionCompanies[]? ProductionCompanies { get; set; }

    [BsonElement("production_countries")]
    [JsonProperty("production_countries")]
    public ProductionCountries[]? ProductionCountries { get; set; }

    [BsonElement("spoken_languages")]
    [JsonProperty("spoken_languages")]
    public SpokenLanguages[] SpokenLanguages { get; set; }

    [BsonElement("adult")]
    [JsonProperty("adult")]
    public bool IsAdult { get; set; }

    [BsonElement("homepage")]
    [JsonProperty("homepage")]
    public string Homepage { get; set; }

    [BsonElement("backdrop_path")]
    [JsonProperty("backdrop_path")]
    public string? Backdrop { get; set; }

    [BsonElement("poster_path")]
    [JsonProperty("poster_path")]
    public string? Poster { get; set; }

    [BsonElement("genres")]
    [JsonProperty("genres")]
    public Genres[]? Genres { get; set; }

    [BsonElement("runtime")]
    [JsonProperty("runtime")]
    public int Runtime { get; set; }

    [BsonElement("release_date")]
    [JsonProperty("release_date")]
    [BsonDateTimeOptions(Kind = DateTimeKind.Utc)]
    public DateTime? ReleaseDate
    {
        get
        {
            return _releaseDate;
        }
        set
        {
            DateTime dt;
            if (DateTime.TryParse(value.ToString(), out dt))
            {
                if (dt.ToString() == "01-01-1970 00:00:00")
                {
                    _releaseDate = null;
                }
                else
                {
                    _releaseDate = Convert.ToDateTime(value);
                }
            }
            else
            {
                _releaseDate = null;
            }
        }
    }

    [BsonElement("budget")]
    [JsonProperty("budget")]
    public int Budget { get; set; }

    [BsonElement("revenue")]
    [JsonProperty("revenue")]
    public long Revenue { get; set; }

    [BsonElement("status")]
    [JsonProperty("status")]
    public string Status { get; set; }

    [BsonElement("video")]
    [JsonProperty("video")]
    public bool IsVideo { get; set; }

    [BsonElement("vote_average")]
    [JsonProperty("vote_average")]
    public double VoteAverage { get; set; }

    [BsonElement("vote_count")]
    [JsonProperty("vote_count")]
    public int VoteCount { get; set; }

}

[BsonIgnoreExtraElements]
public class Genres
{
    [BsonElement("id")]
    [JsonProperty("id")]
    public int Id { get; set; }

    [BsonElement("name")]
    [JsonProperty("name")]
    public string Name { get; set; }
}

[BsonIgnoreExtraElements]
public class ProductionCompanies
{
    [BsonElement("logo_path")]
    [JsonProperty("logo_path")]
    public string? LogoPath { get; set; }

    [BsonElement("name")]
    [JsonProperty("name")]
    public string Name { get; set; }

    [BsonElement("origin_country")]
    [JsonProperty("origin_country")]
    public string? OriginCountry { get; set; }
}

[BsonIgnoreExtraElements]
public class ProductionCountries
{
    [BsonElement("iso_3166_1")]
    [JsonProperty("iso_3166_1")]
    public string CountryCode { get; set; }

    [BsonElement("name")]
    [JsonProperty("name")]
    public string Name { get; set; }
}

[BsonIgnoreExtraElements]
public class SpokenLanguages
{
    [BsonElement("english_name")]
    [JsonProperty("english_name")]
    public string EnglishName { get; set; }

    [BsonElement("iso_639_1")]
    [JsonProperty("iso_639_1")]
    public string LanguageCode { get; set; }

    [BsonElement("name")]
    [JsonProperty("name")]
    public string Name { get; set; }
}

[BsonIgnoreExtraElements]
public class BelongsToCollection
{
    [BsonElement("id")]
    [JsonProperty("id")]
    public int Id { get; set; }

    [BsonElement("name")]
    [JsonProperty("name")]
    public string Name { get; set; }

    [BsonElement("backdrop_path")]
    [JsonProperty("backdrop_path")]
    public string? Backdrop { get; set; }

    [BsonElement("poster_path")]
    [JsonProperty("poster_path")]
    public string? Poster { get; set; }
}

