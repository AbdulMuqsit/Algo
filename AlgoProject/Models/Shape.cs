using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AlgoProject.Models
{
    public abstract class Shape
    {
        public abstract ShapeType Type { get; set; }
        public abstract Coordinate Location { get; set; }
        public abstract Coordinate StartLocation { get;  }
        public abstract Coordinate EndLocation { get;  }
        public abstract int Position { get; set; }
        public abstract double Top { get; }
        public abstract double Left { get;  }
        public  static  int Width { get; set; }
        public  static int Height { get; set; }
    }
}
