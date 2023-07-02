using Migrator;
using Migrator.Models.GraphData;

DB _db = new DB();

//await _db.GetAllMovieIDs().ConfigureAwait(false);
//118

for (int i = 0; i <= 50; i++)
{
    Console.WriteLine("" + i);
    MovieWithCast? movieWithCast = await _db.GetMovieData(i).ConfigureAwait(false);
    if (movieWithCast != null)
    {
        Neo neo = new Neo();
        //await neo.CreateMovies(movieWithCast);
        await neo.GenerateCSV(movieWithCast, "-100");

        neo.Dispose();
    }
}

//Neo n = new Neo();
//await n.DumpData().ConfigureAwait(false);


