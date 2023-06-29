using Migrator.Models.GraphData;
using MongoDB.Bson;
using MongoDB.Driver;
using MongoDB.Driver.Core.Events;
using Newtonsoft.Json;
using System.Linq;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Migrator
{
    internal class DB
    {
        private IMongoClient _client;
        private IMongoDatabase _database;

        public DB()
        {
            _client = new MongoClient("mongodb://127.0.0.1");
            _database = _client.GetDatabase("moviedb1");
        }

        public async Task<MovieWithCast?> GetMovieData(int id)
        {
            try
            {
                IMongoCollection<Movie> _collection = _database.GetCollection<Movie>("movies");

                var pipeline = new BsonDocument[]
                {
                    new BsonDocument("$match", new BsonDocument("id", id)),
                    new BsonDocument("$lookup", new BsonDocument
                    {
                        { "from", "credits" },
                        { "localField", "id" },
                        { "foreignField", "id" },
                        { "as", "result" }
                    }),
                    new BsonDocument("$match", new BsonDocument("result.cast.known_for_department", "Acting")),
                    new BsonDocument("$project", new BsonDocument
                    {
                        { "moviename", "$title" },
                        { "releasedate", "$release_date" },
                        { "cast", "$result.cast.name" }
                    })
                };

                var result = await _collection.AggregateAsync<BsonDocument>(pipeline).ConfigureAwait(false);
                var d = result.FirstOrDefault();
                d.RemoveAt(0);

                var cast_rawdata = d.GetElement(2).Value.AsBsonArray.AsBsonArray[0];
                List<string> allcast = JsonConvert.DeserializeObject<List<string>>(cast_rawdata.ToJson());

                MovieWithCast data_movie_with_cast = new MovieWithCast
                {
                    moviename = Convert.ToString(d.GetValue("moviename")),
                    cast = allcast,
                    release_date = Convert.ToDateTime(d.GetValue("releasedate")),
                };

                return data_movie_with_cast;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"ERROR: Unable to execute query.\n StackTrace - {ex.StackTrace}");
            }

            return null;
        }

    }
}
