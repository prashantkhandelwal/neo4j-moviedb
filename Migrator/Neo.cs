using Migrator.Models.GraphData;
using Neo4j.Driver;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Migrator
{
    public class Neo : IDisposable
    {
        private readonly IDriver _driver;

        public Neo()
        {
            _driver = GraphDatabase.Driver("bolt://localhost:7687", AuthTokens.Basic("neo4j", "Pass#w0rd1"));
        }

        public async Task CreateMovie(MovieWithCast? movieWithCast)
        {
            //var serverInfo = await _driver.GetServerInfoAsync();

            await using var session = _driver.AsyncSession(WithDatabase);
            if (movieWithCast != null)
            {
                string[] dateformat = movieWithCast.release_date.ToShortDateString().Split('-');
                string final_date = $"{dateformat[2]}-{dateformat[1]}-{dateformat[0]}";

                //string query = $"CREATE ({movieWithCast.moviename.Trim()}:Movie {{title:'{movieWithCast.moviename}', released:date('{final_date}')}}) RETURN {movieWithCast.moviename.Trim()}";

                string query = $"CREATE ({movieWithCast.moviename.Trim()}:Movie {{title:'{movieWithCast.moviename}', released:date('{final_date}')}})";

                //$"CREATE (TuroPajala:Person {name: 'Turo Pajala' })

                //CREATE (TuroPajala)-[:ACTED_IN {character:['Taisto Kasurinen']}]->(Ariel)"

                //foreach (var cast in movieWithCast.cast)
                //{
                //    query = query + ""
                //}


                var result = await session.RunAsync(query).ConfigureAwait(false);
                var record = await result.SingleAsync();
                Console.WriteLine(record[0].As<INode>().ElementId);
            }
        }

        public async Task AssociateCastToMovies(MovieWithCast? movieWithCast)
        {
            await using var session = _driver.AsyncSession(WithDatabase);
            if (movieWithCast != null && movieWithCast.cast != null && movieWithCast.cast.Count > 0)
            {
                //string query = 
            }
        }

        public void Dispose()
        {
            _driver.Dispose();
        }

        private static void WithDatabase(SessionConfigBuilder sessionConfigBuilder)
        {
            sessionConfigBuilder.WithDatabase("moviedb");
        }
    }
}
