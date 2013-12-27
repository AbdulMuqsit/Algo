using System;
using System.IO;
using System.Linq;
using System.Windows;
using AlgoProject.Models;
using System.Windows.Media;
using System.Windows.Input;
using System.Windows.Shapes;
using System.Windows.Markup;
using System.Windows.Controls;
using System.Collections.Generic;
using System.Windows.Threading;

namespace AlgoProject.UIComponents
{
    public partial class MainWindow : TransparentWindow
    {
        //Grid's related fields
        Grid grdMain = new Grid();
        RowDefinition rowDefinition;
        ColumnDefinition columnDefiniton;

        ShapeType? selectedType;

        //Border around the window
        Border brdSurround;

        //Separators
        GridSplitter splitterLeft;
        GridSplitter splitterRight;

        //titleBar
        Rectangle rctTitleBar;
        StackPanel pnlTitleBar;
        Label lblTitle;
        Label lblExitX;
        Border brdExit;
        DrawingVisual vslMaximize;
        Label lblminimize;

        //Status Bar
        Rectangle rctStatusBar;

        //Floor's related fields
        //floorBacklog will be the in memory floor
        FloorBacklog floorBacklog;

        Tile firstPoint = null;

        ZoomableCanvas cvsFloor;
        //Left Panel//Right Panel//toolbar
        Button btnFinalTree;
        Button btnDimensions;
        StackPanel stkLeft, stkRight, stkToolbar;
        Border brdFloorSurround;
        ListBox lstFloor;
        ComboBox cmbDown;
        ComboBox cmbDimensions;
        CheckBox chb1, chb2;
        ListBox lstRight;
        TextBox txt1;
        TextBox txt2;
        Canvas cvsGraph;
        TextBox txtDebug;
        TextBox txtleft;
        StackPanel stkLeftInternal;
        Rectangle rctClear, rctWayPoint, rctHurdle, rctInitialPoint, rctDestination;
        Line linEdge;


        #region properties
        public Coordinate Dimensions { get; set; }
        public FloorBacklog Backlog { get { return floorBacklog; } }
        #endregion
        public MainWindow()
        {
            InitializeComponent();
            //Adding definitions
            addMainGridDefinitions();

            //Setting Main Grid's Properties      
            this.AllowsTransparency = true;
            this.WindowStyle = WindowStyle.None;
            //building the main window's visual tree
            buildUI();

            this.Loaded += Window_Loaded;

        }

        //capturing the canvas on the back of lstFloor for later use
        void loaded(object sender, RoutedEventArgs e)
        {
            cvsFloor = sender as ZoomableCanvas;
        }

        #region Methods
        //constructor will call this function to build visual tree of main window 
        private void buildUI()
        {
            //border's settings
            brdSurround = new Border() { BorderBrush = Brushes.Black, BorderThickness = new Thickness(2) };

            //Tittle bar's settings 
            //rectangle
            rctTitleBar = new Rectangle() { Fill = Brushes.Black, MinHeight = 40 };
            Grid.SetColumnSpan(rctTitleBar, 6);
            rctTitleBar.MouseLeftButtonDown += titleBar_MouseLeftButtonDown;

            lblExitX = new Label() { Content = "X", FontSize = 15, VerticalAlignment = VerticalAlignment.Center, HorizontalAlignment = HorizontalAlignment.Center, Foreground=Brushes.White, Margin= new Thickness(5) };
            brdExit = new Border() { HorizontalAlignment = HorizontalAlignment.Right, VerticalAlignment = VerticalAlignment.Center };
            brdExit.MouseLeftButtonDown += ((s, e) => this.Close());
            Grid.SetColumn(brdExit, 5);
            //label
            lblTitle = new Label() { Content = "Floor Covering Simulator", VerticalContentAlignment = VerticalAlignment.Center, HorizontalAlignment = HorizontalAlignment.Left, Foreground = Brushes.White, FontSize = 14 };
            Grid.SetColumnSpan(lblTitle, 6);
            lblTitle.MouseLeftButtonDown += titleBar_MouseLeftButtonDown;

            //setting left separator's properties
            splitterLeft = new GridSplitter() { VerticalAlignment = VerticalAlignment.Stretch, HorizontalAlignment = HorizontalAlignment.Center, Width = 2, Foreground = Brushes.Black, Background = Brushes.Black };
            Grid.SetRow(splitterLeft, 1);
            Grid.SetRowSpan(splitterLeft, 2);
            Grid.SetColumn(splitterLeft, 2);

            //setting right separator's properties
            splitterRight = new GridSplitter() { VerticalAlignment = VerticalAlignment.Stretch, HorizontalAlignment = HorizontalAlignment.Center, Width = 2, Foreground = Brushes.Black, Background = Brushes.Black };
            Grid.SetRow(splitterRight, 1);
            Grid.SetColumn(splitterRight, 4);
            Grid.SetRowSpan(splitterRight, 2);

            //setting status bar's properties
            rctStatusBar = new Rectangle() { Fill = Brushes.Black, MinHeight = 20 };
            Grid.SetRow(rctStatusBar, 3);
            Grid.SetColumnSpan(rctStatusBar, 6);

            //toolbar
            stkToolbar = new StackPanel() { HorizontalAlignment = HorizontalAlignment.Center, VerticalAlignment = VerticalAlignment.Top };
            Grid.SetRow(stkToolbar, 1);

            rctClear = new Rectangle() { MinHeight = 20, MinWidth = 20, HorizontalAlignment = HorizontalAlignment.Center, VerticalAlignment = VerticalAlignment.Center, Fill = Brushes.Gray, Stroke = Brushes.White, Margin = new Thickness(5), ToolTip = "Clear" };
            rctClear.MouseLeftButtonDown += rctClear_MouseLeftButtonDown;

            rctWayPoint = new Rectangle() { MinHeight = 20, MinWidth = 20, HorizontalAlignment = HorizontalAlignment.Center, VerticalAlignment = VerticalAlignment.Center, Fill = Brushes.LightGreen, Stroke = Brushes.White, Margin = new Thickness(5), ToolTip = "Way Point" };
            rctWayPoint.MouseLeftButtonDown += rctWayPoint_MouseLeftButtonDown;

            rctHurdle = new Rectangle() { MinHeight = 20, MinWidth = 20, HorizontalAlignment = HorizontalAlignment.Center, VerticalAlignment = VerticalAlignment.Center, Fill = Brushes.Black, Stroke = Brushes.White, Margin = new Thickness(5), ToolTip = "Obstacle" };
            rctHurdle.MouseLeftButtonDown += rctHurdle_MouseLeftButtonDown;

            rctDestination = new Rectangle() { MinHeight = 20, MinWidth = 20, HorizontalAlignment = HorizontalAlignment.Center, VerticalAlignment = VerticalAlignment.Center, Fill = Brushes.Orange, Stroke = Brushes.White, Margin = new Thickness(5), ToolTip = "Destination" };
            rctDestination.MouseLeftButtonDown += rctDestination_MouseLeftButtonDown;

            rctInitialPoint = new Rectangle() { MinHeight = 20, MinWidth = 20, HorizontalAlignment = HorizontalAlignment.Center, VerticalAlignment = VerticalAlignment.Center, Fill = Brushes.Purple, Stroke = Brushes.White, Margin = new Thickness(5), ToolTip = "Source" };
            rctInitialPoint.MouseLeftButtonDown += rctInitialPoint_MouseLeftButtonDown;

            linEdge = new Line() { X1 = 0, Y1 = 0, X2 = 15, Y2 = 10, StrokeThickness = 2, Stroke = Brushes.Red, Margin = new Thickness(5), HorizontalAlignment = HorizontalAlignment.Center, VerticalAlignment = VerticalAlignment.Center };
            linEdge.MouseLeftButtonDown += linEdge_MouseLeftButtonDown;

            //side panels
            txtleft = new TextBox() { MinWidth = 150, HorizontalAlignment = HorizontalAlignment.Stretch, VerticalAlignment = VerticalAlignment.Stretch, Margin = new Thickness(5) };

            chb1 = new CheckBox();
            chb1.Margin = new Thickness(5);

            chb2 = new CheckBox();
            chb2.Margin = new Thickness(5);

            lstRight = new ListBox() { MinWidth = 150, MinHeight = 200, HorizontalAlignment = HorizontalAlignment.Stretch, VerticalAlignment = VerticalAlignment.Stretch, Margin = new Thickness(5) };

            txt1 = new TextBox() { MinWidth = 150, MinHeight = 100, HorizontalAlignment = HorizontalAlignment.Stretch, VerticalAlignment = VerticalAlignment.Stretch, Margin = new Thickness(5), AcceptsReturn = true };
            txt2 = new TextBox() { MinWidth = 150, MinHeight = 100, HorizontalAlignment = HorizontalAlignment.Stretch, VerticalAlignment = VerticalAlignment.Stretch, Margin = new Thickness(5), AcceptsReturn = true };

            stkLeft = new StackPanel() { VerticalAlignment = VerticalAlignment.Stretch, HorizontalAlignment = HorizontalAlignment.Stretch };
            Grid.SetColumn(stkLeft, 1);
            Grid.SetRow(stkLeft, 1);

            stkLeftInternal = new StackPanel() { Orientation = Orientation.Horizontal, HorizontalAlignment = HorizontalAlignment.Left };

            //right side panel
            stkRight = new StackPanel() { HorizontalAlignment = HorizontalAlignment.Stretch, VerticalAlignment = VerticalAlignment.Stretch };
            Grid.SetRow(stkRight, 1);
            Grid.SetColumn(stkRight, 5);

            btnDimensions = new Button() { HorizontalAlignment = HorizontalAlignment.Center, VerticalAlignment = VerticalAlignment.Center, MinHeight = 30, MinWidth = 100, Background = Brushes.Black, Content = "Get Floor Dimensions", Padding = new Thickness(5), Foreground = Brushes.White };
            btnDimensions.Click += ((s, e) => { initiateNewFloor(); });

            cmbDimensions = new ComboBox() { HorizontalAlignment = HorizontalAlignment.Stretch, MinWidth = 100, Margin = new Thickness(5) };

            cvsGraph = new Canvas() { MinHeight = 200, MinWidth = 150, HorizontalAlignment = HorizontalAlignment.Stretch, VerticalAlignment = VerticalAlignment.Stretch, Margin = new Thickness(5), Background = Brushes.White };

            txtDebug = new TextBox() { MinHeight = 200, MinWidth = 150, HorizontalAlignment = HorizontalAlignment.Stretch, VerticalAlignment = VerticalAlignment.Stretch, Margin = new Thickness(5) };

            cmbDown = new ComboBox() { MinWidth = 150, HorizontalAlignment = HorizontalAlignment.Stretch, Margin = new Thickness(5) };

            btnFinalTree = new Button() { HorizontalAlignment = HorizontalAlignment.Center, VerticalAlignment = VerticalAlignment.Center, MinHeight = 50, MinWidth = 100, Background = Brushes.Black, Content = "Calculate Path", Padding = new Thickness(5), Foreground = Brushes.White };
            btnFinalTree.Click += btnFinalTree_Click;

            //listBox hosting tiles of floor
            lstFloor = new ListBox() { HorizontalAlignment = HorizontalAlignment.Stretch, VerticalAlignment = VerticalAlignment.Stretch };
            lstFloor.AddHandler(MouseLeftButtonDownEvent, new MouseButtonEventHandler(lstBoxMouseDown), true);

            lstFloor.MouseLeftButtonDown += lstBoxMouseDown;
            lstFloor.MouseMove += lstFloor_MouseMove;
            //setting style of floor

            lstFloor.ItemContainerStyle = (Style)this.FindResource("lstBoxStyle");

            lstFloor.ItemsPanel = (ItemsPanelTemplate)this.FindResource("lstBoxTemplate"); ;

            lstFloor.ItemsSource = floorBacklog;
            lstFloor.IsSynchronizedWithCurrentItem = true;

            brdFloorSurround = new Border() { BorderBrush = Brushes.Black, Margin = new Thickness(5), Padding = new Thickness(5) };
            Grid.SetRow(brdFloorSurround, 1);
            Grid.SetColumn(brdFloorSurround, 3);

            //building visual tree of the window
            brdExit.Child = lblExitX;

            stkToolbar.Children.Add(rctWayPoint);
            stkToolbar.Children.Add(rctHurdle);
            stkToolbar.Children.Add(rctClear);
            stkToolbar.Children.Add(rctInitialPoint);
            stkToolbar.Children.Add(rctDestination);
            stkToolbar.Children.Add(linEdge);

            stkLeftInternal.Children.Add(chb1);
            stkLeftInternal.Children.Add(chb2);
            stkLeft.Children.Add(txtleft);
            stkLeft.Children.Add(stkLeftInternal);
            stkLeft.Children.Add(lstRight);
            stkLeft.Children.Add(txt1);
            stkLeft.Children.Add(txt2);

            stkRight.Children.Add(cmbDimensions);
            stkRight.Children.Add(btnDimensions);
            stkRight.Children.Add(cvsGraph);
            stkRight.Children.Add(txtDebug);
            stkRight.Children.Add(btnFinalTree);
            stkRight.Children.Add(cmbDown);


            grdMain.Children.Add(rctTitleBar);
            grdMain.Children.Add(lblTitle);
            grdMain.Children.Add(brdExit);
            grdMain.Children.Add(splitterLeft);
            grdMain.Children.Add(splitterRight);
            grdMain.Children.Add(rctStatusBar);
            brdFloorSurround.Child = lstFloor;
            grdMain.Children.Add(brdFloorSurround);
            grdMain.Children.Add(stkLeft);
            grdMain.Children.Add(stkRight);
            brdSurround.Child = grdMain;
            grdMain.Children.Add(stkToolbar);

            //add everything to MainWindow
            this.Content = brdSurround;
        }

        //event handler for calculate path button
        void btnFinalTree_Click(object sender, RoutedEventArgs e)
        {
            if (floorBacklog == null || floorBacklog.Edges == null || floorBacklog.Source== null)
            {
                return;
            }
            
            //call bellmanFord Function with the list of all edges and initial point 
            BellmanFord.MakeBellmanFordTree(floorBacklog.Edges, floorBacklog.Source);


            Tile vertex = floorBacklog.Destination;


            //start from destination and until source is not reached color all the vertices in the way orange (using converting tile into destination as shortcut to painting orange, as destination tiles already have a style, containing orange foreground
            while (vertex.Parent != null)
            {
                vertex.Type = ShapeType.Destination;
                vertex = vertex.Parent;
            }
        }

        #region ToolBar's buttons's event handlers

        //when a button is clicked on the tool bar it's style is changed to make a click effect, private member selectedType is set to appropriate related 
        //type of next shape to be drawn
        //also other buttons on tool bar  are reset
        //if user has changed the tool while drawing an edge then the firstPoint will be lost!
        void linEdge_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            selectedType = ShapeType.Edge;
            rctClear.StrokeThickness = 0;
            rctHurdle.StrokeThickness = 0;
            rctWayPoint.StrokeThickness = 0;
            rctDestination.StrokeThickness = 0;
            rctInitialPoint.StrokeThickness = 0;
            (sender as Line).Stroke = Brushes.Orange;

        }

        void rctInitialPoint_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            selectedType = ShapeType.InitialPoint;
            rctClear.StrokeThickness = 0;
            rctHurdle.StrokeThickness = 0;
            rctWayPoint.StrokeThickness = 0;
            rctDestination.StrokeThickness = 0;
            rctInitialPoint.StrokeThickness = 2;
            linEdge.Stroke = Brushes.Red;
            firstPoint = null;
        }

        void rctDestination_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            selectedType = ShapeType.Destination;
            rctClear.StrokeThickness = 0;
            rctHurdle.StrokeThickness = 0;
            rctWayPoint.StrokeThickness = 0;
            rctDestination.StrokeThickness = 2;
            rctInitialPoint.StrokeThickness = 0;
            linEdge.Stroke = Brushes.Red;

            firstPoint = null;

        }

        void rctHurdle_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            selectedType = ShapeType.Obstacle;
            rctClear.StrokeThickness = 0;
            rctHurdle.StrokeThickness = 2;
            rctWayPoint.StrokeThickness = 0;
            rctDestination.StrokeThickness = 0;
            rctInitialPoint.StrokeThickness = 0;
            linEdge.Stroke = Brushes.Red;

            firstPoint = null;

        }

        void rctWayPoint_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            selectedType = ShapeType.WayPoint;
            rctClear.StrokeThickness = 0;
            rctHurdle.StrokeThickness = 0;
            rctWayPoint.StrokeThickness = 2;
            rctDestination.StrokeThickness = 0;
            rctInitialPoint.StrokeThickness = 0;
            linEdge.Stroke = Brushes.Red;


            firstPoint = null;
        }

        void rctClear_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            selectedType = ShapeType.Clear;
            rctClear.StrokeThickness = 2;
            rctHurdle.StrokeThickness = 0;
            rctWayPoint.StrokeThickness = 0;
            rctDestination.StrokeThickness = 0;
            rctInitialPoint.StrokeThickness = 0;
            linEdge.Stroke = Brushes.Red;

            firstPoint = null;

        }
        #endregion

        //mouse move event on list
        //if left button is pressed while moving mouse on the floor the hit tile is converted into hurdle
        //giving a smooth experience
        void lstFloor_MouseMove(object sender, MouseEventArgs e)
        {
            Tile selectedItem = ((ListBox)sender).SelectedItem as Tile;
            if (e.LeftButton == MouseButtonState.Pressed && selectedItem != null && selectedType == ShapeType.Obstacle)
            {
                selectedItem.Type = ShapeType.Obstacle;
            }
        }


        //function to add row and column definitions to main grid
        //this grid and in turn MainWindow contains 6 columns and 4 rows for various control's placement
        private void addMainGridDefinitions()
        {
            //adding row definitions
            rowDefinition = new RowDefinition() { Height = GridLength.Auto };
            grdMain.RowDefinitions.Add(rowDefinition);

            rowDefinition = new RowDefinition() { Height = new GridLength(1, GridUnitType.Star) };
            grdMain.RowDefinitions.Add(rowDefinition);

            rowDefinition = new RowDefinition() { Height = GridLength.Auto };
            grdMain.RowDefinitions.Add(rowDefinition);

            rowDefinition = new RowDefinition() { Height = GridLength.Auto };
            grdMain.RowDefinitions.Add(rowDefinition);

            //adding column definitions
            columnDefiniton = new ColumnDefinition() { Width = GridLength.Auto };
            grdMain.ColumnDefinitions.Add(columnDefiniton);

            columnDefiniton = new ColumnDefinition() { Width = GridLength.Auto };
            grdMain.ColumnDefinitions.Add(columnDefiniton);

            columnDefiniton = new ColumnDefinition() { Width = GridLength.Auto };
            grdMain.ColumnDefinitions.Add(columnDefiniton);

            columnDefiniton = new ColumnDefinition() { Width = new GridLength(1, GridUnitType.Star) };
            grdMain.ColumnDefinitions.Add(columnDefiniton);

            columnDefiniton = new ColumnDefinition() { Width = GridLength.Auto };
            grdMain.ColumnDefinitions.Add(columnDefiniton);

            columnDefiniton = new ColumnDefinition() { Width = GridLength.Auto };
            grdMain.ColumnDefinitions.Add(columnDefiniton);
        }
        #endregion
        #region eventHandlers
        //Window load event handler
        void Window_Loaded(object sender, RoutedEventArgs e)
        {
            initiateNewFloor();
        }

        private void initiateNewFloor()
        {
            //prompt the user for floor's dimensions, if user gives some draw the create a new floor backlog passing it's constructor 
            //the given dimensions and set the listFloor's (the list box in which floor is intended to be drawn) ItemsSource property to this newly created 
            //floorBacklog
            DimensionDialogBox dimensionDialogBox = new DimensionDialogBox(true, this);
            if ((bool)dimensionDialogBox.ShowDialog())
            {
                floorBacklog = new FloorBacklog(this.Dimensions);
                this.lstFloor.ItemsSource = floorBacklog;
            }
        }

        //tittle bar's click event handler 
        //double click to maximize/ restore
        //click and drag
        private void titleBar_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ClickCount == 2 && Application.Current.MainWindow.WindowState == WindowState.Maximized)
            {
                Application.Current.MainWindow.WindowState = WindowState.Normal;
            }
            else if (e.ClickCount == 2 && Application.Current.MainWindow.WindowState == WindowState.Normal)
            {
                Application.Current.MainWindow.WindowState = WindowState.Maximized;
            }
            Application.Current.MainWindow.DragMove();

        }


        //event handler for a click inside the floor(sorry for so much if else statements, there was no time to apply proposition logic and shorten the statements)
        private void lstBoxMouseDown(object sender, MouseButtonEventArgs e)
        {
            //Cast clicked item into Shape
            AlgoProject.Models.Shape selectedItem = (((ListBox)sender).SelectedItem) as AlgoProject.Models.Shape;

            //should the type of shape be changed? if yes then change it to selected item in the tool bar except the case that selected item is edge
            if (selectedItem != null && selectedType != null && selectedType != ShapeType.Edge && selectedItem.Type != selectedType)
            {
                //if a destination tile is replaced then there is no more destination
                if (selectedItem.Type == ShapeType.Destination)
                {
                    floorBacklog.Destination = null;

                }
                    //if initial point is replaced then there is no more source
                else if (selectedItem.Type == ShapeType.InitialPoint)
                {
                    floorBacklog.Source = null;
                }

                //only create initial point if there is none before
                if (selectedType == ShapeType.InitialPoint && floorBacklog.Source == null)
                {
                    selectedItem.Type = (ShapeType)selectedType;
                    floorBacklog.Source = (Tile)selectedItem;
                }
                    //there can be only one destination point
                else if (selectedType == ShapeType.Destination && floorBacklog.Destination == null)
                {
                    selectedItem.Type = (ShapeType)selectedType;
                    floorBacklog.Destination = (Tile)selectedItem;
                }
                    //every else tile
                else if (selectedType == ShapeType.WayPoint || selectedType == ShapeType.Clear || selectedType == ShapeType.Obstacle)
                {
                    selectedItem.Type = (ShapeType)selectedType;

                    floorBacklog.WayPoints.Add((Tile)selectedItem);

                }

            }
                //if selected item is edge in the tool bar then  draw the edge, also edge can not be drawn on a clear tile and an obstacle
            else if (selectedType == ShapeType.Edge && selectedItem.Type != ShapeType.Clear && selectedItem.Type != ShapeType.Obstacle)
            {

                //firstPoint is a private member variable to remember the first click of the user on a tile, when user is trying to draw an edge, if it is null then it means user is starting a new edge
                //so assign currently selected item to this global variable
                if (firstPoint == null)
                {
                    firstPoint = (Tile)selectedItem;
                }
                //if first point is not null then this means that user has clicked the second tile, while making the edge, so make a new object of Edge class
                //  passing it's constructor the firstPoint and currently selected tile as argument (They will be U and V of the edge respectively.... see Edge class)

                    //Note: as the listBox's ItemsSource is set to an object of floorBacklog, also floorBacklog returns items from it's tilesCollection 
                    ///whenever it's indexer is used. Also we have to draw the edges on the Tiles, inside the listBox ,so we have to put the edges inside 
                    ////floorBackg's tilesCollection 
                    //But as this same collection contains the tiles to be drawn on the screen and also each tile can have many edges originating from it, so
                     //we have to map the indexes of elements inside tilesCollection, which contain these edges, to the related tile
                    //For this mapping a key having value, equal to index of a tile and against it, a list containing the indexes of the tilesCollection 
                    //containing the edges originating from this tile are added to a dictionary of key value paries(floorBackLog.EdgeMapping the type of 
                    //which is a dictionary having integer keys and against them list of integer values)
                else
                {
                    Edge edge = new Edge((Tile)firstPoint, (Tile)selectedItem);
                    //if a key value payer already exists then there is already a list of integers in the dictionary against current tile(position of current Tile which is an integer) so add the new index in already existing list
                    if (floorBacklog.EdgeMapping.ContainsKey(firstPoint.Position) && !isDuplicateEdge(edge) && edge.U != edge.V)
                    {
                        floorBacklog.ShapesCollection.Add(edge);
                        floorBacklog.Edges.Add(edge);
                        floorBacklog.EdgeMapping[firstPoint.Position].Add((floorBacklog.ShapesCollection.Count - 1));
                    }
                    //else there is no list against current tile(there is no edge originating from this tile before) so create a new list, add to it the index of newly created edge's location and put the key value paire in mapping dictionary
                    else if (!floorBacklog.EdgeMapping.ContainsKey(firstPoint.Position) && edge.U != edge.V)
                    {
                        floorBacklog.ShapesCollection.Add(edge);
                        floorBacklog.Edges.Add(edge);
                        List<int> newList = new List<int>();
                        newList.Add(floorBacklog.ShapesCollection.Count - 1);
                        floorBacklog.EdgeMapping[firstPoint.Position] = newList;
                    }
                    firstPoint = null;
                }
            }
        }


        //if a given edge exists in the collection of edges, this function returns true, else false
        private bool isDuplicateEdge(Edge edge)
        {
            foreach (int index in floorBacklog.EdgeMapping[firstPoint.Position])
            {
                if (((Edge)floorBacklog.ShapesCollection[index]).V == edge.V)
                { return true; }
            }
            return false;
        }


        #endregion
    }
}
