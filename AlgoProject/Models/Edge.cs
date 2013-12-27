using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AlgoProject.Models
{
    public class Edge : Shape
    {
        public override Coordinate Location { get; set; }
        public override int Position { get; set; }
        public override double Top
        {
            get
            {
                if (U.Top < V.Top)
                {
                    return U.Top;
                }
                else
                {
                    return V.Top;
                }
            }
        }
        public override double Left
        {
            get
            {
                if (U.Left < V.Left)
                {
                    return U.Left;
                }
                else
                {
                    return V.Left;
                }
            }
        }
        public new int Width { get { return (int)Math.Abs(U.Left - V.Left) + Tile.Width; } }
        public new int Height { get { return (int)Math.Abs(U.Top - V.Top) + Tile.Height; } }
        public Tile U { get; private set; }
        public Tile V { get; private set; }

        public override Coordinate StartLocation
        {
            get
            {
                
                if (U.Top < V.Top && U.Left > V.Left)
                {
                    return new Coordinate() { X = (int)Width - (Tile.Width / 2), Y = (int)Tile.Height / 2 };
                }
                else if (U.Top > V.Top && U.Left > V.Left)
                {
                    return new Coordinate() { X = (int)Width - (Tile.Width / 2), Y = (int)Height - (Tile.Height / 2) };
                }
                else if (U.Top > V.Top && U.Left < V.Left)
                {
                    return new Coordinate() { X = (int)Tile.Width / 2, Y = (int)Height - (Tile.Height / 2) };
                }

                else
                {
                    return new Coordinate() { X = (int)Tile.Width / 2, Y = (int)Tile.Height / 2 };
                }

            }
        }
        public override Coordinate EndLocation
        {
            get
            {
                if (U.Top < V.Top && U.Left > V.Left)
                {
                    return new Coordinate() { X = (int)Tile.Width / 2, Y = (int)Height - (Tile.Height / 2) };
                }
                else if (U.Top > V.Top && U.Left > V.Left)
                {
                    return new Coordinate() { X = (int)Tile.Width / 2, Y = (int)Tile.Height / 2 };
                }
                else if (U.Top > V.Top && U.Left < V.Left)
                {
                    return new Coordinate() { X = (int)Width - (Tile.Width / 2), Y = (int)Tile.Height / 2 };
                }
                else
                {
                    return new Coordinate() { X = (int)Width - (Tile.Width / 2), Y = (int)Height - (Tile.Height / 2) };
                }
            }
        }
        public int Cost
        {
            get
            {
                return (int)Math.Sqrt((V.Location.X - U.Location.X) * (V.Location.X - U.Location.X) + (V.Location.Y - U.Location.Y) * (V.Location.Y - U.Location.Y));
            }
        }
        ShapeType type;
        public override ShapeType Type { get { return ShapeType.Edge; } set { type = value; } }
        public Edge(Tile u, Tile v)
        {
            U = u;
            V = v;
        }




    }
}
