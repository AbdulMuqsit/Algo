using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AlgoProject.Models
{
    public class Edge:Tile
    {
        public Tile U { get; private set; }
        public Tile V { get; private set; }

        public int Cost
        {
            get
            {
                return (int)Math.Sqrt((V.Location.X - U.Location.X) * (V.Location.X - U.Location.X) + (V.Location.Y - U.Location.Y) * (V.Location.Y - U.Location.Y));
            }
        }
        public string Type { get { return "Edge"; } }
        public Edge(Tile u, Tile v)
        {
            U = u;
            V = v;
        }
       
      
         
    }
}
