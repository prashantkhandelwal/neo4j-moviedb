using Migrator;
using Migrator.Models.GraphData;

DB _db = new DB();

MovieWithCast? movieWithCast = await _db.GetMovieData(2).ConfigureAwait(false);

Neo neo = new Neo();
await neo.CreateMovie(movieWithCast);

neo.Dispose();

