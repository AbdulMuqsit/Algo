using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Configuration;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using AlgoProject.UIComponents;
namespace AlgoProject.Models
{
    public class Tile :Shape, INotifyPropertyChanged
    {

        #region Fields
        ShapeType type;
        #endregion
        static MainWindow mainWindow = ((MainWindow)Application.Current.MainWindow);
        #region properties
        public override Coordinate Location { get; set; }
        public override int  Position { get { return (int)((Location.Y * ((MainWindow)Application.Current.MainWindow).Dimensions.X) + Location.X); } set { } }
        public override ShapeType Type
        {
            get
            {
                return type;
            }
            set
            {
                type = value;
                NotifyPropertyChanged();
            }
        }

        public override Coordinate StartLocation { get { return null; } }
        public override Coordinate EndLocation { get { return null; } }
        static Tile()
        {
            Width = Int32.Parse(ConfigurationManager.AppSettings["TileWidth"]);
            Height = Int32.Parse(ConfigurationManager.AppSettings["TileHeight"]);
        }
        private void NotifyPropertyChanged([CallerMemberName] String propertyName = "")
        {
            if (this.PropertyChanged != null)
            {
                this.PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }

        }
        public override double Top { get { return Location.Y * Height; } }
        public override double Left { get { return Location.X * Width; } }
        public new static int Width { get; private set; }
        public new static int Height { get; private set; }

        public event PropertyChangedEventHandler PropertyChanged;
        public bool IsTraversed { get; set; }
        public double CostSoFar { get; set; }
        public int TotalLinksSoFar { get; set; }

        public Tile Parent { get; set; }

        public Tile UpperTile { get { return (Tile)mainWindow.Backlog[((Location.Y - 1) * mainWindow.Dimensions.X) + Location.X]; } }
        public Tile UpperLeftTile { get { return (Tile)mainWindow.Backlog[((Location.Y - 1) * mainWindow.Dimensions.X) + Location.X - 1]; } }
        public Tile UpperRightTile { get { return (Tile)mainWindow.Backlog[((Location.Y - 1) * mainWindow.Dimensions.X) + Location.X + 1]; } }
        public Tile RightTile { get { return (Tile)mainWindow.Backlog[((Location.Y) * mainWindow.Dimensions.X) + Location.X + 1]; } }
        public Tile LowerRightTile { get { return (Tile)mainWindow.Backlog[((Location.Y + 1) * mainWindow.Dimensions.X) + Location.X + 1]; } }
        public Tile LowerTile { get { return (Tile)mainWindow.Backlog[((Location.Y + 1) * mainWindow.Dimensions.X) + Location.X]; } }
        public Tile LowerLeftTile { get { return (Tile)mainWindow.Backlog[((Location.Y + 1) * mainWindow.Dimensions.X) + Location.X - 1]; } }
        public Tile LeftTile { get { return (Tile)mainWindow.Backlog[((Location.Y) * mainWindow.Dimensions.X) + Location.X - 1]; } }
        #endregion




    }
}
