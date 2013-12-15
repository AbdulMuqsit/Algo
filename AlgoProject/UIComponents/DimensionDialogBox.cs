using AlgoProject.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

namespace AlgoProject.UIComponents
{
    class DimensionDialogBox : TransparentWindow
    {
        //Making dialog box to get size of the floor on application load
        StackPanel verticalStackPanel;
        StackPanel topHorizontalStackPanel;
        StackPanel bottomHorizontalStackPanel;
        Border brdSurround = new Border();
        TextBox txtX = new TextBox();
        TextBox txtY = new TextBox();
        Label lblTitle = new Label();
        Label lblX = new Label();
        Label lblException = new Label();
        Button btnOk = new Button();
        Button btnCancel = new Button();
        public DimensionDialogBox(bool cancelButtonEnabled, Window owner)
        {
            //setting dialog window's properties
            Owner = owner;
            SizeToContent = System.Windows.SizeToContent.Manual;
            WindowStyle = WindowStyle.None;
            AllowsTransparency = true;
            ResizeMode = System.Windows.ResizeMode.NoResize;
            Background = new SolidColorBrush(Color.FromArgb(0xB4, 0xFF, 0xFF, 0xFF));
            WindowStartupLocation = WindowStartupLocation.CenterOwner;
            
            
            this.MaxHeight = 200;
            this.MaxWidth = 300;
            //setting vertical Stack's properties 
            verticalStackPanel = new StackPanel() { Orientation = Orientation.Vertical, VerticalAlignment = VerticalAlignment.Center, HorizontalAlignment = HorizontalAlignment.Center };

            //setting horizontal stack's properties
            topHorizontalStackPanel = new StackPanel() { Orientation = Orientation.Horizontal, VerticalAlignment = VerticalAlignment.Center, HorizontalAlignment = HorizontalAlignment.Center };

            bottomHorizontalStackPanel = new StackPanel() { Orientation = Orientation.Horizontal, VerticalAlignment = VerticalAlignment.Center, HorizontalAlignment = HorizontalAlignment.Center };

            //setting border's properties
            brdSurround = new Border() { VerticalAlignment = VerticalAlignment.Stretch, HorizontalAlignment = HorizontalAlignment = HorizontalAlignment.Stretch, BorderBrush = Brushes.Black, BorderThickness = new Thickness(5) };

            //setting text box's properties
            txtX = new TextBox() { MinHeight = 20, MinWidth = 100, HorizontalContentAlignment = HorizontalAlignment.Center, VerticalContentAlignment = VerticalAlignment.Center, Margin = new Thickness(5), FontSize = 12 };
            this.Loaded += ((s, e) => { txtX.Focus(); });

            txtY = new TextBox() { MinHeight = 20, MinWidth = 100, HorizontalContentAlignment = HorizontalAlignment.Center, VerticalContentAlignment = VerticalAlignment.Center, Margin = new Thickness(5), FontSize = 12 };

            //setting label's properties
            lblX = new Label() { VerticalContentAlignment = VerticalAlignment.Center, HorizontalContentAlignment = HorizontalAlignment.Center, FontSize = 12, Content = "X", Margin = new Thickness(5) };

            //Setting title labels properties
            lblTitle = new Label() { VerticalContentAlignment = VerticalAlignment.Center, HorizontalAlignment = HorizontalAlignment.Center, FontSize = 14, Content = "Enter Floor Dimensions" };

            //exception label's settings
            lblException = new Label() { VerticalContentAlignment = VerticalAlignment.Center, HorizontalAlignment = HorizontalAlignment.Center, FontSize = 12, Foreground = Brushes.Red, Visibility = Visibility.Collapsed };

            //setting button's properties
            btnOk = new Button() { VerticalContentAlignment = VerticalAlignment.Center, HorizontalAlignment = HorizontalAlignment.Center, Content = "OK", FontSize = 12, Background = Brushes.Black, Foreground = Brushes.White, Margin = new Thickness(5), MinHeight = 20, MinWidth = 100, IsEnabled = false };
            btnOk.IsDefault = true;

            //cancel button
            btnCancel = new Button() { Foreground = Brushes.White, Margin = new Thickness(5), VerticalContentAlignment = VerticalAlignment.Center, HorizontalAlignment = HorizontalAlignment.Center, Content = "Cancel", FontSize = 12, Background = Brushes.Black, IsEnabled = cancelButtonEnabled, MinHeight = 20, MinWidth = 100 };

            //Attaching event handlers
            attachEventHandlers();

            //building dialog window's visual tree

            topHorizontalStackPanel.Children.Add(txtX);
            topHorizontalStackPanel.Children.Add(lblX);
            topHorizontalStackPanel.Children.Add(txtY);

            bottomHorizontalStackPanel.Children.Add(btnOk);
            bottomHorizontalStackPanel.Children.Add(btnCancel);

            verticalStackPanel.Children.Add(lblTitle);
            verticalStackPanel.Children.Add(lblException);
            verticalStackPanel.Children.Add(topHorizontalStackPanel);
            verticalStackPanel.Children.Add(bottomHorizontalStackPanel);

            brdSurround.Child = verticalStackPanel;

            Content = brdSurround;
        }

        private void attachEventHandlers()
        {
            //dialog window's event loading and closing
            Loaded += ((s, eve) => { this.Owner.IsEnabled = false; });
            Closing += ((s, eve) => { this.Owner.IsEnabled = true; });

            //event handler disabling the alt+f4 key on dialog
            PreviewKeyDown += ((s, eve) =>
            {
                if (eve.Key == Key.System && eve.SystemKey == Key.F4)
                {
                    eve.Handled = true;
                }
            });

            //key down event handlers on text boxes to validate input.
            txtX.KeyUp += ((s, e) =>
            {
                validateInput(txtX, txtY, lblException, btnOk, s, e);
            });

            txtY.KeyUp += ((s, e) =>
            {

                validateInput(txtX, txtY, lblException, btnOk, s, e);
            });

            //OK and cancel button click
            btnOk.Click += ((s, eve) =>
            {
                Utilities.DrawFloor(Utilities.MaxCoordinates);
                ((MainWindow)Owner).Dimensions = new Coordinate() { X = Int32.Parse(txtX.Text), Y = Int32.Parse(txtY.Text) };
                this.Owner.IsEnabled = true;
                this.DialogResult = true;
                this.Close();
            });
            btnCancel.Click += ((s, eve) => { this.DialogResult = false; this.Close(); });
        }

        //validation helper function.
        private static void validateInput(TextBox txtX, TextBox txtY, Label lblException, Button btnOk, object sender, KeyEventArgs e)
        {
            int x;
            Point? point = vailidateInput(txtX, txtY);
            TextBox theSender = sender as TextBox == txtX ? txtX : txtY;
            TextBox theOther = sender as TextBox == txtX ? txtY : txtX;


            if ((e.Key == Key.Tab && Int32.TryParse(theOther.Text, out x)) || (Int32.TryParse(theSender.Text, out x) && theOther.Text == "" && e.Key != Key.Tab))
            {
                setValidationViewer(lblException, btnOk, null, Visibility.Collapsed, false);
            }

            else if ((e.Key == Key.Back && theSender.Text == "") || (e.Key == Key.Tab && theOther.Text == ""))
            {
                setValidationViewer(lblException, btnOk, "Floor dimensions must be specified.", Visibility.Visible, false);
            }

            else if (point.HasValue && point.Value.X * point.Value.Y <= 1000000)
            {
                setValidationViewer(lblException, btnOk, null, Visibility.Collapsed, true);
            }
            else if (point.HasValue && point.Value.X * point.Value.Y >= 1000000)
            {
                setValidationViewer(lblException, btnOk, "More than 100000 boxes will have a huge impect on performance.", Visibility.Visible, true);
            }
            else
            {
                setValidationViewer(lblException, btnOk, "Specify the input in correct format.", Visibility.Visible, false);
            }

        }


        //input validation helper function
        private static Point? vailidateInput(TextBox txtX, TextBox txtY)
        {
            int x, y;
            if (Int32.TryParse(txtX.Text, out x) && Int32.TryParse(txtY.Text, out y))
            {
                return new Point(x, y);
            }
            else
            {
                return null;
            }

        }
        private static void setValidationViewer(Label lblException, Button btnOk, string lblExceptionContent, Visibility lblExceptionVisibility, bool btnOkEnabled)
        {
            lblException.Content = lblExceptionContent;
            lblException.Visibility = lblExceptionVisibility;
            btnOk.IsEnabled = btnOkEnabled;
        }
    }
}
