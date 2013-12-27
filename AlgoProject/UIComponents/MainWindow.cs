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
    public partial class MainWindow : Window
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
        DrawingVisual vslMaximize;
        Label lblminimize;

        //Status Bar
        Rectangle rctStatusBar;

        //Floor's related fields
        //floorBacklog will be the in memory floor
        FloorBacklog floorBacklog;

        Tile firstPoint = null;

        ZoomableCanvas cvsFloor;
        //Left Panel
        Button btnFinalTree;
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

        //Right Panel


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

            cmbDimensions = new ComboBox() { HorizontalAlignment = HorizontalAlignment.Stretch, MinWidth = 100, Margin = new Thickness(5) };

            cvsGraph = new Canvas() { MinHeight = 200, MinWidth = 150, HorizontalAlignment = HorizontalAlignment.Stretch, VerticalAlignment = VerticalAlignment.Stretch, Margin = new Thickness(5), Background = Brushes.White };

            txtDebug = new TextBox() { MinHeight = 200, MinWidth = 150, HorizontalAlignment = HorizontalAlignment.Stretch, VerticalAlignment = VerticalAlignment.Stretch, Margin = new Thickness(5) };

            cmbDown = new ComboBox() { MinWidth = 150, HorizontalAlignment = HorizontalAlignment.Stretch, Margin = new Thickness(5) };

            btnFinalTree = new Button() { HorizontalAlignment = HorizontalAlignment.Center, VerticalAlignment = VerticalAlignment.Center, MinHeight = 50, MinWidth = 100, Background = Brushes.Black };
            btnFinalTree.Click += btnFinalTree_Click;

            //listBox hosting tiles of floor
            lstFloor = new ListBox() { HorizontalAlignment = HorizontalAlignment.Stretch, VerticalAlignment = VerticalAlignment.Stretch };
            lstFloor.AddHandler(MouseLeftButtonDownEvent, new MouseButtonEventHandler(lstBoxMouseDown), true);

            lstFloor.MouseLeftButtonDown += lstBoxMouseDown;
            lstFloor.MouseMove += lstFloor_MouseMove;
            //setting style of floor

            lstFloor.ItemContainerStyle = (Style)this.FindResource("lstBoxStyle");

            ItemsPanelTemplate template = new ItemsPanelTemplate();

            template = (ItemsPanelTemplate)this.FindResource("lstBoxTemplate");


            lstFloor.ItemsPanel = template;

            lstFloor.ItemsSource = floorBacklog;
            lstFloor.IsSynchronizedWithCurrentItem = true;

            brdFloorSurround = new Border() { BorderBrush = Brushes.Black, Margin = new Thickness(5), Padding = new Thickness(5) };
            Grid.SetRow(brdFloorSurround, 1);
            Grid.SetColumn(brdFloorSurround, 3);

            //building visual tree of the window
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
            stkRight.Children.Add(cvsGraph);
            stkRight.Children.Add(txtDebug);
            stkRight.Children.Add(btnFinalTree);
            stkRight.Children.Add(cmbDown);

            grdMain.Children.Add(rctTitleBar);
            grdMain.Children.Add(lblTitle);
            grdMain.Children.Add(splitterLeft);
            grdMain.Children.Add(splitterRight);
            grdMain.Children.Add(rctStatusBar);
            brdFloorSurround.Child = lstFloor;
            grdMain.Children.Add(brdFloorSurround);
            grdMain.Children.Add(stkLeft);
            grdMain.Children.Add(stkRight);
            brdSurround.Child = grdMain;
            grdMain.Children.Add(stkToolbar);
            this.Content = brdSurround;
        }

        void btnFinalTree_Click(object sender, RoutedEventArgs e)
        {
            BellmanFord.MakeBellmanFordTree(floorBacklog.Edges, floorBacklog.Source);
            Tile vertex = floorBacklog.Destination;
            while (vertex.Parent != null)
            {
                vertex.Type = ShapeType.Destination;
                vertex = vertex.Parent;
            }
        }

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
        void lstFloor_MouseMove(object sender, MouseEventArgs e)
        {
            Tile selectedItem = ((ListBox)sender).SelectedItem as Tile;
            if (e.LeftButton == MouseButtonState.Pressed && selectedItem != null && selectedType == ShapeType.Obstacle)
            {
                selectedItem.Type = ShapeType.Obstacle;
            }
        }


        //function to add row and column definitions to main grid
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
            DimensionDialogBox dimensionDialogBox = new DimensionDialogBox(false, this);
            dimensionDialogBox.ShowDialog();
            floorBacklog = new FloorBacklog(this.Dimensions);
            this.lstFloor.ItemsSource = floorBacklog;
        }
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
        private void lstBoxMouseDown(object sender, MouseButtonEventArgs e)
        {
            AlgoProject.Models.Shape selectedItem = (((ListBox)sender).SelectedItem) as AlgoProject.Models.Shape;

            if (selectedItem != null && selectedType != null && selectedType != ShapeType.Edge && selectedItem.Type != selectedType)
            {
                if (selectedItem.Type != ShapeType.Edge)
                {
                    if (selectedItem.Type == ShapeType.Destination)
                    {
                        floorBacklog.Destination = null;

                    }
                    else if (selectedItem.Type == ShapeType.InitialPoint)
                    {
                        floorBacklog.Source = null;
                    }

                    if (selectedType == ShapeType.InitialPoint && floorBacklog.Source == null)
                    {
                        selectedItem.Type = (ShapeType)selectedType;
                        floorBacklog.Source = (Tile)selectedItem;
                    }
                    else if (selectedType == ShapeType.Destination && floorBacklog.Destination == null)
                    {
                        selectedItem.Type = (ShapeType)selectedType;
                        floorBacklog.Destination = (Tile)selectedItem;
                    }
                    else if (selectedType == ShapeType.WayPoint )
                    {
                        selectedItem.Type = (ShapeType)selectedType;

                        floorBacklog.WayPoints.Add((Tile)selectedItem);


                    }
                }
                else
                {
                    Point position = e.GetPosition(cvsFloor);
                    int top = (int)position.Y / Tile.Width;
                    int left = (int)position.X / Tile.Height;

                    int location = ((int)(position.Y / Tile.Width)) * floorBacklog.FloorDimensions.X + ((int)(position.X / Tile.Width));
                    floorBacklog.TilesCollection[location].Type = (ShapeType)selectedType;

                }

            }
            else if (selectedType == ShapeType.Edge && selectedItem.Type != ShapeType.Clear && selectedItem.Type != ShapeType.Obstacle)
            {
                if (firstPoint == null)
                {
                    firstPoint = (Tile)selectedItem;
                }

                else
                {
                    Edge edge = new Edge((Tile)firstPoint, (Tile)selectedItem);

                    if (floorBacklog.EdgeMapping.ContainsKey(firstPoint.Position) && !isDuplicateEdge(edge) && edge.U != edge.V)
                    {
                        floorBacklog.TilesCollection.Add(edge);
                        floorBacklog.Edges.Add(edge);
                        floorBacklog.EdgeMapping[firstPoint.Position].Add((floorBacklog.TilesCollection.Count - 1));
                    }

                    else if (!floorBacklog.EdgeMapping.ContainsKey(firstPoint.Position) && edge.U != edge.V)
                    {
                        floorBacklog.TilesCollection.Add(edge);
                        floorBacklog.Edges.Add(edge);
                        List<int> newList = new List<int>();
                        newList.Add(floorBacklog.TilesCollection.Count - 1);
                        floorBacklog.EdgeMapping[firstPoint.Position] = newList;
                    }
                    firstPoint = null;
                }

            }



        }

        private bool isDuplicateEdge(Edge edge)
        {
            foreach (int index in floorBacklog.EdgeMapping[firstPoint.Position])
            {
                if (((Edge)floorBacklog.TilesCollection[index]).V == edge.V)
                { return true; }
            }
            return false;
        }


        #endregion
    }
}
