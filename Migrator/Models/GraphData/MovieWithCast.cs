using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Migrator.Models.GraphData
{
    public class MovieWithCast
    {
        public string moviename { get; set; }

        public DateTime release_date { get; set; }
        
        public List<string> cast { get; set; }
    }
}
