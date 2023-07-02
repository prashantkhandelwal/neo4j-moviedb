using Migrator.Models.GraphData;
using MongoDB.Bson.IO;
using Neo4j.Driver;
using System.Text.RegularExpressions;

namespace Migrator
{
    public class Neo : IDisposable
    {
        private readonly IDriver _driver;

        public Neo()
        {
            _driver = GraphDatabase.Driver("bolt://localhost:7687", AuthTokens.Basic("neo4j", "Pass#w0rd1"));
        }

        #region NEOCode
        public async Task DumpData()
        {
            await using var session = _driver.AsyncSession(WithDatabase);

            for (int i = 0; i < 100; i++)
            {
                try
                {
                    string movie_person_create = File.ReadAllText($"queries\\{i}-movie-person-create.cypher");
                    string movie_person_attach = File.ReadAllText($"queries\\{i}-movie-person-attach.cypher");
                    string movie_genre_attach = File.ReadAllText($"queries\\{i}-movie-genre-attach.cypher");

                    await session.RunAsync(movie_person_create).ConfigureAwait(false);
                    await session.RunAsync(movie_person_attach).ConfigureAwait(false);
                    await session.RunAsync(movie_genre_attach).ConfigureAwait(false);
                }
                catch (Exception) { }
            }

        }
        public async Task CreateMovies(MovieWithCast? movieWithCast)
        {
            await using var session = _driver.AsyncSession(WithDatabase);
            if (movieWithCast != null)
            {
                string[]? dateformat = movieWithCast.movie.ReleaseDate?.ToShortDateString().Split('-');
                string final_date = $"{dateformat?[2]}-{dateformat?[1]}-{dateformat?[0]}";

                string query = $"MERGE(m_{movieWithCast.movie.TMDBId}" +
                    $":Movie{{" +
                    $"title:'{movieWithCast.movie.Title?.Replace("'", @"\'").Replace("\"", @"\""")}'," +
                    $"originaltitle:'{movieWithCast.movie.OriginalTitle?.Replace("'", @"\'").Replace("\"", @"\""")}'," +
                    $"tmdbid:{movieWithCast.movie.TMDBId}," +
                    $"imdbid:'{movieWithCast.movie.IMDBId}'," +
                    $"isadult:{movieWithCast.movie.IsAdult.ToString().ToLower()}," +
                    $"runtime:{movieWithCast.movie.Runtime}," +
                    $"budget:{movieWithCast.movie.Budget}," +
                    $"revenue:{movieWithCast.movie.Revenue}," +
                    $"votecount:{movieWithCast.movie.VoteCount}," +
                    $"voteaerage:{movieWithCast.movie.VoteAverage}," +
                    $"originallanguage:'{movieWithCast.movie.OriginalLanguage}'," +
                    $"releasedate:date('{final_date}')" +
                    $"}})\n";

                File.WriteAllText($"queries\\{movieWithCast.movie.TMDBId}-movie-create.cypher", query);

                string genre_merge = string.Empty;
                string genre_attachment = string.Empty;
                for (int i = 0; i < movieWithCast.movie.Genres?.Length; i++)
                {
                    genre_merge = genre_merge + $"MATCH({movieWithCast.movie.Genres[i].Name}_{movieWithCast.movie.Genres[i].Id}:Genre{{id:{movieWithCast.movie.Genres[i].Id}}})," +
                       $"(m_{movieWithCast.movie.TMDBId}:Movie{{tmdbid:{movieWithCast.movie.TMDBId}}})\n";

                    genre_attachment = genre_attachment + $"MERGE(m_{movieWithCast.movie.TMDBId})-[:GENRE]->({movieWithCast.movie.Genres[i].Name}_{movieWithCast.movie.Genres[i].Id})\n";
                }

                if (!string.IsNullOrEmpty(genre_merge))
                {
                    string final_genre_attach_query = genre_merge + genre_attachment;
                    File.WriteAllText($"queries\\{movieWithCast.movie.TMDBId}-movie-genre-attach.cypher", final_genre_attach_query);
                }

                string cast_attach_query = string.Empty;
                string cast_merge_query = string.Empty;
                string person_create_query = string.Empty;

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

                    person_create_query = person_create_query + $"MERGE({actorname}_{cast.Id}:Person{{" +
                        $"id:{cast.Id}," +
                        $"name:'{cast.Name?.Replace("'", @"\'").Replace("\"", @"\""")}'," +
                        $"originalname:'{cast.OriginalName?.Replace("'", @"\'").Replace("\"", @"\""")}'," +
                        $"character:'{cast.Character}'," +
                        $"isadult:{cast.IsAdult.ToString().ToLower()}," +
                        $"gender:'{GetGender(cast.Gender)}'," +
                        $"popularity:{cast.Popularity}," +
                        $"knownfordepartment:'{cast.KnownForDepartment?.Replace("'", @"\'").Replace("\"", @"\""")}'" +
                        $"}})\n";

                    cast_attach_query = cast_attach_query + $"MATCH({actorname}_{cast.Id}:Person{{id:{cast.Id}}})," +
                        $"(m_{movieWithCast.movie.TMDBId}:Movie{{tmdbid:{movieWithCast.movie.TMDBId}}})\n";
                    cast_merge_query = cast_merge_query + $"MERGE({actorname}_{cast.Id})-[:ACTED_IN]->(m_{movieWithCast.movie.TMDBId})\n";
                }

                //await session.RunAsync(query).ConfigureAwait(false);

                string final_attach_query = cast_attach_query + cast_merge_query;
                if (!string.IsNullOrEmpty(final_attach_query))
                {
                    //await session.RunAsync(final_attach_query).ConfigureAwait(false);
                    File.WriteAllText($"queries\\{movieWithCast.movie.TMDBId}-person-create.cypher", person_create_query);
                    File.WriteAllText($"queries\\{movieWithCast.movie.TMDBId}-movie-person-attach.cypher", final_attach_query);
                }

                ///Crew
                string crew_attach_query = string.Empty;
                string crew_merge_query = string.Empty;
                string crew_create_query = string.Empty;

                foreach (var crew in movieWithCast.crew)
                {
                    string crewname = ReplaceWhitespace(crew.Name, "")
                       .Replace("\"", "")
                       .Replace("-", "")
                       .Replace(".", "")
                       .Replace(",", "")
                       .Replace("!", "")
                       .Replace(":", "")
                       .Replace("'", "");

                    string ROLE = ReplaceWhitespace(crew.Job.ToUpperInvariant(), "")
                       .Replace("\"", "")
                       .Replace("-", "")
                       .Replace(".", "")
                       .Replace(",", "")
                       .Replace("!", "")
                       .Replace(":", "")
                       .Replace("'", "");

                    crew_create_query = crew_create_query + $"MERGE({crewname}_{crew.Id}_{ROLE}:Person{{" +
                        $"id:{crew.Id}," +
                        $"name:'{crew.Name?.Replace("'", @"\'").Replace("\"", @"\""")}'," +
                        $"originalname:'{crew.OriginalName?.Replace("'", @"\'").Replace("\"", @"\""")}'," +
                        $"job:'{crew.Job}'," +
                        $"isadult:{crew.IsAdult.ToString().ToLower()}," +
                        $"gender:'{GetGender(crew.Gender)}'," +
                        $"popularity:{crew.Popularity}," +
                        $"department:'{crew.Department?.Replace("'", @"\'").Replace("\"", @"\""")}'," +
                        $"knownfordepartment:'{crew.KnownForDepartment?.Replace("'", @"\'").Replace("\"", @"\""")}'" +
                        $"}})\n";

                    crew_attach_query = crew_attach_query + $"MATCH({crewname}_{crew.Id}_{ROLE}:Person{{id:{crew.Id}}})," +
                        $"(m_{movieWithCast.movie.TMDBId}:Movie{{tmdbid:{movieWithCast.movie.TMDBId}}})\n";

                    crew_merge_query = crew_merge_query + $"MERGE({crewname}_{crew.Id}_{ROLE})-[:{ROLE}]->(m_{movieWithCast.movie.TMDBId})\n";
                }

                string final_crew_attach_query = crew_attach_query + crew_merge_query;
                if (!string.IsNullOrEmpty(final_crew_attach_query))
                {
                    File.WriteAllText($"queries\\{movieWithCast.movie.TMDBId}-movie-person-crew-create.cypher", crew_create_query);
                    File.WriteAllText($"queries\\{movieWithCast.movie.TMDBId}-movie-person-crew-attach.cypher", final_crew_attach_query);
                }

            }
        }
        #endregion

        public async Task GenerateCSV(MovieWithCast? movieWithCast, string fileprefix)
        {
            await using var session = _driver.AsyncSession(WithDatabase);
            if (movieWithCast != null)
            {

                // Movies
                string movie_filename = $"movies{fileprefix}.csv";
                string movie_header = "title,originaltitle,tmdbid,imdbid,isadult,runtime,budget,revenue,votecount,voteaverage,originallanguage,releasedate\n";

                if (!File.Exists(movie_filename))
                {
                    File.AppendAllText(movie_filename, movie_header);

                }

                string data = string.Empty;

                string[]? dateformat = movieWithCast.movie.ReleaseDate?.ToShortDateString().Split('-');
                string final_date = $"{dateformat?[2]}-{dateformat?[1]}-{dateformat?[0]}";
                string? movie_title = string.Empty;
                string? movie_originaltitle = string.Empty;

                if (movieWithCast?.movie?.Title?.Replace("'", @"\'")?.Replace("\"", @"\'")?.Contains(",") == true)
                {
                    movie_title = "\"" + movieWithCast?.movie?.Title?.Replace("'", @"\'")?.Replace("\"", @"\'") + "\"";
                }
                else
                {
                    movie_title = movieWithCast?.movie?.Title?.Replace("'", @"\'")?.Replace("\"", @"\'");
                }
                if (movieWithCast?.movie?.OriginalTitle?.Replace("'", @"\'")?.Replace("\"", @"\'")?.Contains(",") == true)
                {
                    movie_originaltitle = "\"" + movieWithCast?.movie?.OriginalTitle?.Replace("'", @"\'")?.Replace("\"", @"\'") + "\"";
                }
                else
                {
                    movie_originaltitle = movieWithCast?.movie?.OriginalTitle?.Replace("'", @"\'")?.Replace("\"", @"\'");
                }

                data = data + movie_title + "," +
                    movie_originaltitle + "," +
                    movieWithCast?.movie.TMDBId + "," +
                    movieWithCast?.movie.IMDBId + "," +
                    movieWithCast?.movie.IsAdult.ToString().ToLower() + "," +
                    movieWithCast?.movie.Runtime + "," +
                    movieWithCast?.movie.Budget + "," +
                    movieWithCast?.movie.Revenue + "," +
                    movieWithCast?.movie.VoteCount + "," +
                    movieWithCast?.movie.VoteAverage + "," +
                    movieWithCast?.movie.OriginalLanguage + "," +
                    final_date + "\n";

                File.AppendAllText(movie_filename, data);
                data = string.Empty;

                // Cast
                string cast_filename = $"cast{fileprefix}.csv";
                string cast_header = "movieid,castid,name,originalname,character,isadult,gender,popularity,knownfordepartment\n";
                string? cast_character = string.Empty;

                if (!File.Exists(cast_filename))
                {
                    File.AppendAllText(cast_filename, cast_header);
                }

                foreach (var cast in movieWithCast.cast)
                {
                    if (cast.Character?.Replace("'", @"\'")?.Replace("\"", @"\'")?.Contains(",") == true)
                    {
                        cast_character = "\"" + cast.Character?.Replace("'", @"\'")?.Replace("\"", @"\'") + "\"";
                    }
                    else
                    {
                        cast_character = cast.Character?.Replace("'", @"\'")?.Replace("\"", @"\'");
                    }

                    data = data + movieWithCast.movie.TMDBId + "," +
                        cast.Id + "," +
                        cast.Name?.Replace("'", @"\'").Replace("\"", @"\'") + "," +
                        cast.OriginalName?.Replace("'", @"\'").Replace("\"", @"\'") + "," +
                        cast_character + "," +
                        cast.IsAdult.ToString().ToLower() + "," +
                        GetGender(cast.Gender) + "," +
                        cast.Popularity + "," +
                        cast.KnownForDepartment?.Replace("'", @"\'").Replace("\"", @"\'") +
                        "\n";
                }

                File.AppendAllText(cast_filename, data);
                data = string.Empty;

                // Crew
                string crew_filename = $"crew{fileprefix}.csv";
                string crew_header = "movieid,crewid,name,originalname,job,role,isadult,gender,popularity,department,knownfordepartment\n";
                string? crew_character = string.Empty;

                if (!File.Exists(crew_filename))
                {
                    File.AppendAllText(crew_filename, crew_header);
                }
                
                foreach (var crew in movieWithCast.crew)
                {
                    string ROLE = ReplaceWhitespace(crew.Job.ToUpperInvariant(), "")
                       .Replace("\"", "")
                       .Replace("-", "")
                       .Replace(".", "")
                       .Replace(",", "")
                       .Replace("!", "")
                       .Replace(":", "")
                       .Replace("'", "");

                    data = data + movieWithCast.movie.TMDBId + "," +
                        crew.Id + "," +
                        crew.Name?.Replace("'", @"\'").Replace("\"", @"\'") + "," +
                        crew.OriginalName?.Replace("'", @"\'").Replace("\"", @"\'") + "," +
                        crew.Job + "," +
                        ROLE + "," +
                        crew.IsAdult.ToString().ToLower() + "," +
                        GetGender(crew.Gender) + "," +
                        crew.Popularity + "," +
                        crew.Department + "," +
                        crew.KnownForDepartment?.Replace("'", @"\'").Replace("\"", @"\'") +
                        "\n";
                }

                File.AppendAllText(crew_filename, data);
                data = string.Empty;
            }
        }

        

        private string GetGender(int g)
        {
            string gender = string.Empty;

            switch (g)
            {
                case 0:
                    gender = "Not set/not specified";
                    break;
                case 1:
                    gender = "Female";
                    break;
                case 2:
                    gender = "Male";
                    break;
                case 3:
                    gender = "Non-binary";
                    break;
                default:
                    break;
            }

            return gender;
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
