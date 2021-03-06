﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace AlgoProject.Models
{
    public class FloorBacklog : IList, ZoomableCanvas.ISpatialItemsSource
    {
        private static Coordinate floorDimensions;
        private Tile source = null;
        private Tile destination = null;
        #region fields
        //Collection storing tiles drawn until now
        private ObservableCollection<Shape> shapesCollection;

        public Tile Destination
        {
            get
            {
                return destination;
            }
            set
            {
                destination = value;
            }
        }
        public Tile Source
        {
            get
            {
                return source;
            }
            set
            {
                source = value;
            }
        }
        public ObservableCollection<Shape> ShapesCollection
        {
            get { return shapesCollection; }
        }
        List<Edge> edges = new List<Edge>();

        Dictionary<int, List<int>> edgeMapping = new Dictionary<int, List<int>>();

        public Dictionary<int, List<int>> EdgeMapping
        {
            get { return edgeMapping; }
           
        }
        public List<Edge> Edges
        {
            get { return edges; }

        }
        private List<Tile> wayPoints = new List<Tile>();

        public List<Tile> WayPoints
        {
            get { return wayPoints; }

        }
        #endregion

        #region properties
        public Coordinate FloorDimensions
        {
            get
            {
                return floorDimensions;
            }
            set
            {

                floorDimensions = value;
                // initializeTileCollection(value);
                if (this.ExtentChanged != null && this.QueryInvalidated != null)
                {
                    this.ExtentChanged(this, EventArgs.Empty);
                    this.QueryInvalidated(this, EventArgs.Empty);
                }

            }
        }



        //Canvas automatically checks this property to get the height and width of whole surface to be drawn
        public Rect Extent
        {
            get { return new Rect(0, 0, FloorDimensions.X * Tile.Width, FloorDimensions.Y * Tile.Height); }
        }
        #endregion

        #region constructors



        public FloorBacklog(Coordinate dimension)
        {

            initializeTileCollection(dimension);
            //attaching event handlers


        }

        #endregion

        #region methods


        //for the given dimensions initialize the tiles collection with null
        //so that later, tiles can be inserted at appropriate indexes
        private void initializeTileCollection(Coordinate dimensions)
        {
            shapesCollection = new ObservableCollection<Shape>();
            this.FloorDimensions = dimensions;
            for (int i = 0; i < dimensions.X * dimensions.Y; i++)
            {
                shapesCollection.Add(null);
            }
            shapesCollection.CollectionChanged += ((s, e) => { this.QueryInvalidated(this, EventArgs.Empty); });


        }

        //canvas calls this method to get the indexes of items to be drawn on screen
        public IEnumerable<int> Query(Rect VisibleArea)
        {
            //local variables
            IEnumerable<int> indexes = new List<int>();
            Tile tile;

            //get the intersection of canvas's visible area and the whole virtual area
            VisibleArea.Intersect(this.Extent);

            //calculating the coordinates of the rectangular area to be drawn  
            int initialVerticalPosition = (int)Math.Floor(VisibleArea.Top / Tile.Height);
            int initialHorizontalPosition = (int)Math.Floor(VisibleArea.Left / Tile.Width);
            int maxVerticalPosition = (int)Math.Ceiling(VisibleArea.Bottom / Tile.Height);
            int maxHorizontalPosition = (int)Math.Ceiling(VisibleArea.Right / Tile.Width);

           /// //calculating the indexes of items within the visible rectangle//////
            //from starting point(xy) to closing point of visible rectangle, calculate the position of the tiles to be drawn
            //if a tile does not exist on the calculated index in the tiles collection, create a tile 
            //give it coordinates and insert it to the collection of tiles at calculated index.
            //log the calculated index into list of integers.
            //also by the mapping dictionary check either there is any edge originating from a visible tile, if so include it's index too in the list of indexes
            //after both loops end, return the list of calculated indexes
            for (int i = initialVerticalPosition; i < maxVerticalPosition; i++)
            {
                for (int j = initialHorizontalPosition; j < maxHorizontalPosition; j++)
                {

                    int currentPosition = (int)(i * FloorDimensions.X) + j;

                    if (shapesCollection[currentPosition] == null)
                    {

                        tile = new Tile() { Type = ShapeType.Clear, Location = new Coordinate() { X = j, Y = i } };
                        shapesCollection[currentPosition] = tile;

                    }
                    ((List<int>)indexes).Add(currentPosition);
                }
            }
            List<int> listIndex = indexes as List<int>;
            for (int i = 0; i < listIndex.Count; i++)
            {
                if (EdgeMapping.ContainsKey(listIndex[i]))
                {
                    foreach (int mapping in edgeMapping[listIndex[i]])
                    {
                        ((List<int>)indexes).Add(mapping);
                    }
                }
            }
           
            return indexes;
        }
        #endregion

        #region events

        public event EventHandler ExtentChanged; //supposed to be fired when the whole virtual area changes
        public event EventHandler QueryInvalidated; //supposed to be fired when a change occurs in the visible area of canvas
        #endregion

        #region indexer

        //whenever an item is asked from floorBacklog through array syntax this indexer is used

        //Note: after calling the query method of this class and getting the indexes of shapes to be drawn, the list box gets each and every item by this indexer
        public object this[int i]
        {
            get
            {

                return  shapesCollection[i];
            }
            set
            {
                shapesCollection[i] = (Tile)value;

            }
        }

        #endregion

        #region ilistImplementation

        int IList.Add(object value)
        {
            return 0;
        }

        void IList.Clear()
        {
        }

        bool IList.Contains(object value)
        {
            return false;
        }

        int IList.IndexOf(object value)
        {
            return 0;
        }

        void IList.Insert(int index, object value)
        {
        }

        void IList.Remove(object value)
        {
        }

        void IList.RemoveAt(int index)
        {
        }

        void ICollection.CopyTo(Array array, int index)
        {
        }

        bool IList.IsFixedSize
        {
            get { return false; }
        }

        bool IList.IsReadOnly
        {
            get { return true; }
        }

        bool ICollection.IsSynchronized
        {
            get { return false; }
        }

        object ICollection.SyncRoot
        {
            get { return null; }
        }

        int ICollection.Count
        {
            get { return Int32.MaxValue; }
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            yield break;
        }


        #endregion
    }
}
