using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace AlgoProject.Models
{
    public static class Utilities
    {
        private static Point maxCoordinates;

        public static Point MaxCoordinates
        {
            get { return maxCoordinates; }
            set { maxCoordinates = value; }
        }

        internal static void DrawFloor(Point point)
        {
            
        }
    }
}
