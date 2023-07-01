using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Migrator.Models.GraphData
{
    public class MovieWithCast
    {
        public Movie movie { get; set; }
        public List<Cast> cast { get; set; }
    }
}
