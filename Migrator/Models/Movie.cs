using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

[BsonIgnoreExtraElements]
[BsonDiscriminator("movies")]
public class Movie
{
    private DateTime? _releaseDate;

    [BsonId]
    public object _id { get; set; }

    [BsonElement("id")]
    public int TMDBId { get; set; }

    [BsonElement("imdb_id")]
    public string IMDBId { get; set; }

    [BsonElement("title")]
    public string Title { get; set; }

    [BsonElement("belongs_to_collection")]
    public BelongsToCollection? BelongsToCollection { get; set; }

    [BsonElement("original_title")]
    public string OriginalTitle { get; set; }

    [BsonElement("original_language")]
    public string OriginalLanguage { get; set; }

    [BsonElement("tagline")]
    public string? TagLine { get; set; }

    [BsonElement("overview")]
    public string Overview { get; set; }

    [BsonElement("popularity")]
    public double Popularity { get; set; }

    [BsonElement("production_companies")]
    public ProductionCompanies[]? ProductionCompanies { get; set; }

    [BsonElement("production_countries")]
    public ProductionCountries[]? ProductionCountries { get; set; }

    [BsonElement("spoken_languages")]
    public SpokenLanguages[] SpokenLanguages { get; set; }

    [BsonElement("adult")]
    public bool IsAdult { get; set; }

    [BsonElement("homepage")]
    public string Homepage { get; set; }

    [BsonElement("backdrop_path")]
    public string? Backdrop { get; set; }

    [BsonElement("poster_path")]
    public string? Poster { get; set; }

    [BsonElement("genres")]
    public Genres[]? Genres { get; set; }

    [BsonElement("runtime")]
    public int Runtime { get; set; }

    [BsonElement("release_date")]
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
    public int Budget { get; set; }

    [BsonElement("revenue")]
    public long Revenue { get; set; }

    [BsonElement("status")]
    public string Status { get; set; }

    [BsonElement("video")]
    public bool IsVideo { get; set; }

    [BsonElement("vote_average")]
    public double VoteAverage { get; set; }

    [BsonElement("vote_count")]
    public int VoteCount { get; set; }

}

[BsonIgnoreExtraElements]
public class Genres
{
    [BsonElement("name")]
    public string Name { get; set; }
}

[BsonIgnoreExtraElements]
public class ProductionCompanies
{
    [BsonElement("logo_path")]
    public string? LogoPath { get; set; }

    [BsonElement("name")]
    public string Name { get; set; }

    [BsonElement("origin_country")]
    public string? OriginCountry { get; set; }
}

[BsonIgnoreExtraElements]
public class ProductionCountries
{
    [BsonElement("iso_3166_1")]
    public string CountryCode { get; set; }

    [BsonElement("name")]
    public string Name { get; set; }
}

[BsonIgnoreExtraElements]
public class SpokenLanguages
{
    [BsonElement("english_name")]
    public string EnglishName { get; set; }

    [BsonElement("iso_639_1")]
    public string LanguageCode { get; set; }

    [BsonElement("name")]
    public string Name { get; set; }
}

[BsonIgnoreExtraElements]
public class BelongsToCollection
{
    [BsonElement("id")]
    public int Id { get; set; }

    [BsonElement("name")]
    public string Name { get; set; }

    [BsonElement("backdrop_path")]
    public string? Backdrop { get; set; }

    [BsonElement("poster_path")]
    public string? Poster { get; set; }
}

