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
    public class MainWindow : TransparentWindow
    {

        //Grid's related fields
        Grid grdMain = new Grid();
        RowDefinition rowDefinition;
        ColumnDefinition columnDefiniton;

        TileType? selectedType;

        //Border around the window
        Border brdSurround;

        //Separators
        Separator separatorLeft;
        Separator separatorRight;

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
        ZoomableCanvas cvsFloor;
        //Left Panel
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
        Rectangle rctClear, rctWayPoint, rctHurdle;

        //Right Panel


        #region properties
        public Coordinate Dimensions { get; set; }
        public FloorBacklog Backlog { get { return floorBacklog; } }
        #endregion

        public MainWindow()
        {

            //Adding definitions
            addMainGridDefinitions();

            //Setting Main Grid's Properties      
            this.AllowsTransparency = true;
            this.WindowStyle = WindowStyle.None;
            //building the main window's visual tree
            buildUI();

            this.Loaded += Window_Loaded;

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
            separatorLeft = new Separator() { VerticalAlignment = VerticalAlignment.Stretch, HorizontalAlignment = HorizontalAlignment.Center, Width = 2, Foreground = Brushes.Black };
            Grid.SetRow(separatorLeft, 1);
            Grid.SetRowSpan(separatorLeft, 2);
            Grid.SetColumn(separatorLeft, 2);

            //setting right separator's properties
            separatorRight = new Separator() { VerticalAlignment = VerticalAlignment.Stretch, HorizontalAlignment = HorizontalAlignment.Center, Width = 2, Foreground = Brushes.Black };
            Grid.SetRow(separatorRight, 1);
            Grid.SetColumn(separatorRight, 4);
            Grid.SetRowSpan(separatorRight, 2);

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

            //listBox hosting tiles of floor
            lstFloor = new ListBox() { HorizontalAlignment = HorizontalAlignment.Stretch, VerticalAlignment = VerticalAlignment.Stretch };
            lstFloor.AddHandler(MouseLeftButtonDownEvent, new MouseButtonEventHandler(lstBoxMouseDown), true);

            lstFloor.MouseLeftButtonDown += lstBoxMouseDown;
            lstFloor.MouseMove += lstFloor_MouseMove;
            //setting style of floor
            using (FileStream fileStream = new FileStream("TileStyle.xaml", FileMode.Open, FileAccess.Read))
            {
                lstFloor.ItemContainerStyle = (Style)XamlReader.Load(fileStream);
            }
            ItemsPanelTemplate template = new ItemsPanelTemplate();
            using (FileStream fileStream = new FileStream("FloorPanelTemplate.xaml", FileMode.Open, FileAccess.Read))
            {
                template = (ItemsPanelTemplate)XamlReader.Load(fileStream);

            }
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
            stkRight.Children.Add(cmbDown);

            grdMain.Children.Add(rctTitleBar);
            grdMain.Children.Add(lblTitle);
            grdMain.Children.Add(separatorLeft);
            grdMain.Children.Add(separatorRight);
            grdMain.Children.Add(rctStatusBar);
            brdFloorSurround.Child = lstFloor;
            grdMain.Children.Add(brdFloorSurround);
            grdMain.Children.Add(stkLeft);
            grdMain.Children.Add(stkRight);
            brdSurround.Child = grdMain;
            grdMain.Children.Add(stkToolbar);
            this.Content = brdSurround;
        }

        void lstFloor_MouseMove(object sender, MouseEventArgs e)
        {
            Tile selectedItem = ((Tile)((ListBox)sender).SelectedItem);
            if (e.LeftButton == MouseButtonState.Pressed && selectedItem != null && selectedType == TileType.Obstacle)
            {
                selectedItem.Type = TileType.Obstacle;
            }
        }

        void rctHurdle_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            selectedType = TileType.Obstacle;
            rctClear.StrokeThickness = 0;
            rctHurdle.StrokeThickness = 2;
            rctWayPoint.StrokeThickness = 0;
        }

        void rctWayPoint_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            selectedType = TileType.WayPoint;
            rctClear.StrokeThickness = 0;
            rctHurdle.StrokeThickness = 0;
            rctWayPoint.StrokeThickness = 2;
        }

        void rctClear_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            selectedType = TileType.Clear;
            rctClear.StrokeThickness = 2;
            rctHurdle.StrokeThickness = 0;
            rctWayPoint.StrokeThickness = 0;
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
            Tile selectedItem = ((Tile)((ListBox)sender).SelectedItem);
            if (selectedItem != null && selectedType != null && selectedItem.Type != selectedType)
            {
                selectedItem.Type = (TileType)selectedType;

            }
        }
        protected override void OnPreviewMouseWheel(MouseWheelEventArgs e)
        {
           // var x = Math.Pow(2, e.Delta / 3.0 / Mouse.MouseWheelDeltaForOneLine);
           // cvsFloor.Scale *= x;

            // Adjust the offset to make the point under the mouse stay still.

        }

        private void ZoomableCanvas_Loaded(object sender, RoutedEventArgs e)
        {
            // Store the canvas in a local variable since x:Name doesn't work.
            cvsFloor = (ZoomableCanvas)sender;
        }

        #endregion

        private childItem FindVisualChild<childItem>(DependencyObject obj)
    where childItem : DependencyObject
        {
            for (int i = 0; i < VisualTreeHelper.GetChildrenCount(obj); i++)
            {
                DependencyObject child = VisualTreeHelper.GetChild(obj, i);
                if (child != null && child is childItem)
                    return (childItem)child;
                else
                {
                    childItem childOfChild = FindVisualChild<childItem>(child);
                    if (childOfChild != null)
                        return childOfChild;
                }
            }
            return null;
            
        }


    }
}
