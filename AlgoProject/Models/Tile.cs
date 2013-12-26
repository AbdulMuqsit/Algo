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
    public class Tile : INotifyPropertyChanged
    {

        #region Fields
        private ShapeType type;
        #endregion
        static MainWindow mainWindow = ((MainWindow)Application.Current.MainWindow);
        #region properties
        public Coordinate Location { get; set; }
        public int Position { get { return (int)((Location.Y * ((MainWindow)Application.Current.MainWindow).Dimensions.X) + Location.X); } set { } }
        public ShapeType Type
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
        public double Top { get { return Location.Y * Height; } }
        public double Left { get { return Location.X * Width; } }
        public static int Width { get; private set; }
        public static int Height { get; private set; }

        public event PropertyChangedEventHandler PropertyChanged;
        public bool IsTraversed { get; set; }
        public int CostSoFar { get; set; }
        public int TotalLinksSoFar { get; set; }
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
