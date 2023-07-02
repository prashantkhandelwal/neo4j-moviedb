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

        public async Task GetAllMovieIDs()
        {
            List<int> ids = new List<int>();
            try
            {
                //var collection = database.GetCollection<BsonDocument>("movies");
                IMongoCollection<BsonDocument> _collection = _database.GetCollection<BsonDocument>("movies");
                var filter = Builders<BsonDocument>.Filter.Empty;
                var projection = Builders<BsonDocument>.Projection.Include("id").Exclude("_id");

                var options = new FindOptions<BsonDocument, BsonDocument> { Projection = projection };

                var result = await _collection.FindAsync<BsonDocument>(filter, options); //.Project(projection).ToList();
                while(await result.MoveNextAsync())
                {
                    var id = result.Current;
                }
                    
            }
            catch (Exception ex)
            {
                Console.WriteLine($"ERROR: Unable to execute query.\n StackTrace - {ex.StackTrace}");
            }
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
                    //new BsonDocument("$match", new BsonDocument("result.cast.known_for_department", "Acting")),
                    //new BsonDocument("$project", new BsonDocument
                    //{
                    //    { "movies", "$fromItems" },
                    //    { "moviename", "$title" },
                    //    { "releasedate", "$release_date" },
                    //    { "cast", "$result.cast" }
                    //})
                };

                var result = await _collection.AggregateAsync<BsonDocument>(pipeline).ConfigureAwait(false);
                var d = result.FirstOrDefault();

                if (d == null) return null;
                
                d.RemoveAt(0);

                var cast_rawdata = d.GetElement("result").Value.AsBsonArray.AsBsonArray.AsBsonArray[0];
                Credits c = new Credits();
                
                Movie m = new Movie();
                
                c.Cast = JsonConvert.DeserializeObject<List<Cast>>(cast_rawdata[2].ToJson());
                c.Crew = JsonConvert.DeserializeObject<List<Crew>>(cast_rawdata[3].ToJson());

                object val = BsonTypeMapper.MapToDotNetValue(d.AsBsonValue);
                string jsonString = JsonConvert.SerializeObject(val);

                m = JsonConvert.DeserializeObject<Movie>(jsonString);

                MovieWithCast data_movie_with_cast = new MovieWithCast
                {
                    movie = m,
                    cast = c.Cast,
                    crew = c.Crew,
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
