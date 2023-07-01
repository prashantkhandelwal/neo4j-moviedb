using Migrator.Models.GraphData;
using Neo4j.Driver;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace Migrator
{
    public class Neo : IDisposable
    {
        private readonly IDriver _driver;

        public Neo()
        {
            _driver = GraphDatabase.Driver("bolt://localhost:7687", AuthTokens.Basic("neo4j", "Pass#w0rd1"));
        }

        public async Task DumpData()
        {
            await using var session = _driver.AsyncSession(WithDatabase);

            for (int i = 0; i < 100; i++)
            {
                try
                {
                    string create = File.ReadAllText($"queries\\{i}-create.cypher");
                    string attach = File.ReadAllText($"queries\\{i}-attach.cypher");

                    await session.RunAsync(create).ConfigureAwait(false);
                    await session.RunAsync(attach).ConfigureAwait(false);
                }
                catch (Exception) { }
            }
            
        }


        //public async Task CreateMovie(MovieWithCast? movieWithCast)
        //{
        //    //var serverInfo = await _driver.GetServerInfoAsync();

        //    await using var session = _driver.AsyncSession(WithDatabase);
        //    if (movieWithCast != null)
        //    {
        //        string[] dateformat = movieWithCast.release_date.ToShortDateString().Split('-');
        //        string final_date = $"{dateformat[2]}-{dateformat[1]}-{dateformat[0]}";
        //        string movie_name = ReplaceWhitespace("m_" + movieWithCast.moviename, "")
        //            .Replace("\"", "")
        //            .Replace("-", "")
        //            .Replace(".", "")
        //            .Replace("'", "")
        //            .Replace("!", "")
        //            .Replace(",", "")
        //            .Replace("`", "")
        //            .Replace("~", "")
        //            .Replace("?", "")
        //            .Replace("+", "")
        //            .Replace("$", "")
        //            .Replace("@", "")
        //            .Replace(":", "");

        //        string query = $"CREATE({movie_name}:Movie{{title:'{movieWithCast.moviename?.Replace("'", @"\'").Replace("\"", @"\""")}',released:date('{final_date}')}})\n";

        //        foreach (var cast in movieWithCast.cast)
        //        {
        //            string actorname = ReplaceWhitespace(cast.OriginalName, "")
        //                .Replace("\"", "")
        //                .Replace("-", "")
        //                .Replace(".", "")
        //                .Replace(",", "")
        //                .Replace("!", "")
        //                .Replace(":", "")
        //                .Replace("'", ""); //string.Join("", cast.OriginalName.Split(" "));

        //            query = query + $"CREATE({actorname}:Person{{name:'{cast.OriginalName?.Replace("'", @"\'").Replace("\"", @"\""")}'}})\n" +
        //                $"CREATE({actorname})-[:ACTED_IN{{character:['{cast.Character?.Replace("'", @"\'").Replace("\"", @"\""")}']}}]->({movie_name})\n";
        //        }
        //        await session.RunAsync(query).ConfigureAwait(false);
        //        //if (result.Current.Keys.Count > 0)
        //        //{
        //        //    var record = await result.SingleAsync();
        //        //    Console.WriteLine(record[0].As<INode>().ElementId);
        //        //}
        //    }
        //}

        public async Task CreateMovies(MovieWithCast? movieWithCast)
        {
            await using var session = _driver.AsyncSession(WithDatabase);
            if (movieWithCast != null)
            {
                //string movie_title = (string.IsNullOrEmpty(movieWithCast.movie.Title) ? movieWithCast.movie.Title
                //string movie_name = ReplaceWhitespace($"{movieWithCast.movie.Title}_{movieWithCast.movie.TMDBId}", "")
                //    .Replace("\"", "")
                //    .Replace("-", "")
                //    .Replace(".", "")
                //    .Replace("'", "")
                //    .Replace("!", "")
                //    .Replace(",", "")
                //    .Replace("`", "")
                //    .Replace("~", "")
                //    .Replace("?", "")
                //    .Replace("+", "")
                //    .Replace("$", "")
                //    .Replace("@", "")
                //    .Replace(":", "");

                string query = $"MERGE(m_{movieWithCast.movie.TMDBId}:Movie{{title:'{movieWithCast.movie.Title?.Replace("'", @"\'").Replace("\"", @"\""")}'}})\n";
                string attach_query = string.Empty;
                string merge_query = string.Empty;

                foreach (var cast in movieWithCast.cast)
                {
                    string actorname = ReplaceWhitespace(cast.Name, "")
                        .Replace("\"", "")
                        .Replace("-", "")
                        .Replace(".", "")
                        .Replace(",", "")
                        .Replace("!", "")
                        .Replace(":", "")
                        .Replace("'", "");

                    query = query + $"MERGE({actorname}_{cast.Id}:Person{{name:'{cast.Name?.Replace("'", @"\'").Replace("\"", @"\""")}'}})\n";

                    attach_query = attach_query + $"MATCH({actorname}_{cast.Id}:Person{{name:'{cast.Name?.Replace("'", @"\'").Replace("\"", @"\""")}'}})," +
                        $"(m_{movieWithCast.movie.TMDBId}:Movie{{title:'{movieWithCast.movie.Title?.Replace("'", @"\'").Replace("\"", @"\""")}'}})\n";
                    merge_query = merge_query + $"MERGE({actorname}_{cast.Id})-[:ACTED_IN]->(m_{movieWithCast.movie.TMDBId})\n";
                }

                //await session.RunAsync(query).ConfigureAwait(false);

                string final_attach_query = attach_query + merge_query;
                if (string.IsNullOrEmpty(final_attach_query)) return;
                //await session.RunAsync(final_attach_query).ConfigureAwait(false);

                File.WriteAllText($"queries\\{movieWithCast.movie.TMDBId}-create.cypher", query);
                File.WriteAllText($"queries\\{movieWithCast.movie.TMDBId}-attach.cypher", final_attach_query);
            }
        }

        public void Dispose()
        {
            _driver.Dispose();
        }

        private static readonly Regex sWhitespace = new Regex(@"\s+");
        public static string ReplaceWhitespace(string input, string replacement)
        {
            return sWhitespace.Replace(input, replacement);
        }

        private static void WithDatabase(SessionConfigBuilder sessionConfigBuilder)
        {
            sessionConfigBuilder.WithDatabase("moviedb");
        }
    }
}
