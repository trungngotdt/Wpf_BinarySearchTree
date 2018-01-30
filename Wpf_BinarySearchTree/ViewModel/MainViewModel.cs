using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Shapes;
using System.Linq;
using System;
using System.Collections.Generic;
using System.Collections;
using System.Windows.Data;
using System.Windows.Resources;
using Wpf_BinarySearchTree.Model;
using System.Threading.Tasks;
using System.Threading;
using System.Windows.Media.Animation;

namespace Wpf_BinarySearchTree.ViewModel
{
    /// <summary>
    /// This class contains properties that the main View can data bind to.
    /// <para>
    /// See http://www.mvvmlight.net
    /// </para>
    /// </summary>
    public class MainViewModel : ViewModelBase
    {
        private readonly IDataService _dataService;

        private double widthGridBST;
        private int num;
        private int numbeFind;
        private int nodeBeDelete;
        private Node<int> nodeRoot;

        private readonly int VerticalMarging = 100;
        private readonly int HorizontalMarging = 50;
        private double heightGridBST;
        private ICommand btnAddNodeClickCommand;
        private ICommand bSTGridSizeChanged;
        private ICommand btnFindNodeClickCommand;
        private ICommand btnDeleteNodeClickCommand;

        public int Num { get => num; set => num = value; }
        public int NumbeFind { get => numbeFind; set => numbeFind = value; }
        public int NodeBeDelete { get => nodeBeDelete; set => nodeBeDelete = value; }
        public Node<int> NodeRoot { get => nodeRoot; set => nodeRoot = value; }
        public double WidthGridBST { get => widthGridBST; set => widthGridBST = value; }
        public double HeightGridBST { get => heightGridBST; set => heightGridBST = value; }


        public ICommand BtnAddNodeClickCommand
        {
            get
            {
                return btnAddNodeClickCommand = new RelayCommand<object[]>((p) =>
                {
                    AddNodeGridAsync(p[1] as Grid);
                });
            }
        }

        public ICommand BSTGridSizeChanged { get { return bSTGridSizeChanged; } }

        public ICommand BtnFindNodeClickCommand { get { return btnFindNodeClickCommand = new RelayCommand<UIElement>((p) => { FindNodeInGrid(new Node<int>(NumbeFind), p); }); } }

        public ICommand BtnDeleteNodeClickCommand { get { return btnDeleteNodeClickCommand = new RelayCommand<Grid>((p) => { DeleteNodeInGrid(p); }); } }


        /// <summary>
        /// Initializes a new instance of the MainViewModel class.
        /// </summary>
        public MainViewModel(IDataService dataService)
        {

            _dataService = dataService;
            HeightGridBST = 705;
            WidthGridBST = 1138.3333333333335;
        }

        /*
        public Tuple<int, int> GetSize(Node<int> tree)
        {
            var height = tree.Height() + 1;
            var width = Math.Pow(2, height - 1);

            var heightPx = (height - 1) * VerticalMarging + 2 * 50;
            var widthPx = (width - 1) * HorizontalMarging + 2 * 50;

            return new Tuple<int, int>((int)widthPx, heightPx);
        }*/


        #region Add a node to grid

        #region Draw line

        /// <summary>
        /// Draw a line from the button to the other button
        /// </summary>
        /// <param name="grid"></param>
        /// <param name="x1"></param>
        /// <param name="x2"></param>
        /// <param name="y1"></param>
        /// <param name="y2"></param>
        /// <param name="isRightLeaf"></param>
        /// <param name="name"></param>
        private async void DrawLine(Grid grid, double x1, double x2, double y1, double y2, bool isRightLeaf, string name)
        {
            await Task.Factory.StartNew(() =>
             {
                 while (true)
                 {
                     //Point btn1Point = button1.TransformToAncestor(this).Transform(new Point(0, 0));
                     var result = (Num != 0 ? Num.ToString() : null);
                     if (result != null)
                     {
                         //Thread.Sleep(5000);   
                         Application.Current.Dispatcher.Invoke(() =>
                         {
                             Line l = new Line
                             {
                                 Stroke = new SolidColorBrush(Colors.Black),
                                 StrokeThickness = 2.0,
                                 Name = name,
                                 X1 = x1 + (isRightLeaf ? 50 : 0),
                                 X2 = x1 + (isRightLeaf ? 50 : 0), //x2 + (isRightLeaf ? 0 : 50),
                                 Y1 = y1 + 25, //btn1Point.Y + a.ActualHeight / 2;
                                 Y2 = y1 + 25
                             };
                             AnimationGrowLine(x2 + (isRightLeaf ? 0 : 50), y2 + 25, TimeSpan.FromSeconds(1), l);
                             grid.Children.Add(l);
                         });// btn2Point.Y + b.ActualHeight / 2;  
                         //Application.Current.Dispatcher.Invoke(() => { (uI as Button).Content = "AAAAAAAAAAAAAAAA";});
                         return;
                     }
                 }
             });
        }



        /// <summary>
        /// Grow a line to x2,y2
        /// </summary>
        /// <param name="x2"></param>
        /// <param name="y2"></param>
        /// <param name="time">The time the line will be grow</param>
        /// <param name="line"></param>
        public void AnimationGrowLine(double x2, double y2, TimeSpan time, Line line)
        {
            Storyboard sb = new Storyboard();
            DoubleAnimation animationX2 = new DoubleAnimation(x2, time);
            DoubleAnimation animationY2 = new DoubleAnimation(y2, time);
            Storyboard.SetTarget(animationX2, line);
            Storyboard.SetTargetProperty(animationX2, new PropertyPath("X2"));
            Storyboard.SetTarget(animationY2, line);
            Storyboard.SetTargetProperty(animationY2, new PropertyPath("Y2"));
            sb.Children.Add(animationX2);
            sb.Children.Add(animationY2);
            sb.Begin();
            /*line.BeginAnimation(Line.X2Property, animationX2);
            line.BeginAnimation(Line.Y2Property, animationY2);*/
        }
        #endregion


        /// <summary>
        /// Add a node to Grid
        /// </summary>
        /// <param name="p">this is a grid which will be add a button</param>
        private async void AddNodeGridAsync(UIElement p)
        {
            double x = 0;
            double y = 0;
            if ((p as Grid).Children.OfType<Button>().Count<Button>() == 0)
            {
                nodeRoot = new Node<int>(Num, (p as Grid).ActualWidth / 2, VerticalMarging);
                AddNode(p as Grid, Num, new Thickness((p as Grid).ActualWidth / 2, VerticalMarging, 0, 0));
                return;
            }
            if (NodeRoot.Contains(new Node<int>(Num)))
            {
                return;
            }
            var node = new Node<int>(Num, x, y);
            NodeRoot.Insert(node);
            var checkExitsParent = nodeRoot.FindParent(new Node<int>(Num));
            if (checkExitsParent.Item1 != null)
            {
                var oldX = checkExitsParent.Item1.X;
                var oldY = checkExitsParent.Item1.Y;
                if (checkExitsParent.Item2 == -1)
                {
                    x = oldX - (p as Grid).ActualWidth / Math.Pow(2, ((oldY + VerticalMarging) / VerticalMarging));
                    y = oldY + VerticalMarging;
                    AddNode(p as Grid, Num, new Thickness(x, y, 0, 0));
                }
                else if (checkExitsParent.Item2 == 1)
                {
                    x = oldX + (p as Grid).ActualWidth / Math.Pow(2, ((oldY + VerticalMarging) / VerticalMarging));
                    y = oldY + VerticalMarging;
                    AddNode(p as Grid, Num, new Thickness(x, y, 0, 0));
                }
                node.X = x;
                node.Y = y;
                if (x + 50 >= WidthGridBST || x - 50 <= 0)
                {
                    ResizeGrid();
                    Task taskReLayoutBtn = Task.Factory.StartNew(() => { ReLayoutAllButton(p as Grid); });
                    await Task.WhenAll(taskReLayoutBtn);
                }
                Task taskDrawLine = Task.Factory.StartNew(() =>
                DrawLine(p as Grid, checkExitsParent.Item1.X, node.X, checkExitsParent.Item1.Y, node.Y, checkExitsParent.Item2 > 0, $"{"Btn" + checkExitsParent.Item1.Data.ToString() + "Btn" + node.Data.ToString() }")
                );
                await Task.WhenAll(taskDrawLine);
            }

        }

        /// <summary>
        /// Add a node to Grid with location
        /// </summary>
        /// <param name="gridPanel"></param>
        /// <param name="data"></param>
        /// <param name="thickness"></param>
        /// <returns></returns>
        private bool AddNode(UIElement gridPanel, int data, Thickness thickness)
        {
            try
            {
                var grid = gridPanel as Grid;
                Button button = new Button() { Name = "Btn" + data.ToString(), Content = data.ToString() };

                button.SetValue(Grid.HorizontalAlignmentProperty, HorizontalAlignment.Left);
                button.SetValue(Grid.VerticalAlignmentProperty, VerticalAlignment.Top);
                button.Margin = thickness;// new Thickness(1138.3 / 2, 20, 0, 0);
                button.Width = 50;
                button.Height = 50;
                RoundButton(button);
                grid.Children.Add(button);
                return true;
            }
            catch (System.Exception ex)
            {
                MessageBox.Show(ex.Message.ToString());
                return false;
            }
        }

        #region Relayout for button

        /// <summary>
        /// Re-layout all buttons from grid
        /// </summary>
        /// <param name="grid"></param>
        private void ReLayoutAllButton(Grid grid)
        {
            Application.Current.Dispatcher.Invoke(() =>
            {
                grid.Children.OfType<Button>().ToList().ForEach(p =>
               {
                   Task.Factory.StartNew(() =>
                   {
                       Application.Current.Dispatcher.Invoke(() =>
                        {
                            var Left = p.Margin.Left * 2;
                            NodeRoot.FindNode(new Node<int>(int.Parse(p.Content.ToString()))).X = Left;
                            p.Margin = new Thickness(Left, p.Margin.Top, p.Margin.Right, p.Margin.Bottom);
                        });
                   });
               });
                grid.Children.OfType<Line>().ToList().ForEach(p =>
                {
                    Task.Factory.StartNew(() =>
                    {
                        Application.Current.Dispatcher.Invoke(() =>
                        {
                            var X2 = p.X2;
                            var Y2 = p.Y2;
                            p.BeginAnimation(Line.X2Property, null);//Animation be removed
                            if (p.X1 < X2)
                            {
                                p.X1 = p.X1 * 2 - 50;
                                p.X2 = X2 * 2;
                            }
                            else
                            {
                                p.X2 = X2 * 2 - 50;
                                p.X1 = p.X1 * 2;
                            }
                            p.BeginAnimation(Line.Y2Property, null);//Animation be removed
                            p.Y2 = Y2;
                        });
                    });
                });
            });
        }
        /// <summary>
        /// Resize the grid (<seealso cref="HeightGridBST"/>+100; <seealso cref="WidthGridBST"/>*2)
        /// </summary>
        private void ResizeGrid()
        {
            WidthGridBST = WidthGridBST * 2;
            HeightGridBST = HeightGridBST + 100;
            RaisePropertyChanged("WidthGridBST");
            RaisePropertyChanged("HeightGridBST");
        }

        #endregion

        /// <summary>
        /// Draw the round button
        /// </summary>
        /// <param name="button"></param>
        public void RoundButton(Button button)
        {
            ControlTemplate circleButtonTemplate = new ControlTemplate(typeof(Button));

            // Create the circle
            FrameworkElementFactory circle = new FrameworkElementFactory(typeof(Ellipse));
            circle.SetValue(Ellipse.FillProperty, Brushes.LightGreen);
            circle.SetValue(Ellipse.StrokeProperty, Brushes.Black);
            circle.SetValue(Ellipse.StrokeThicknessProperty, 1.0);
            circle.SetValue(Ellipse.FillProperty, Brushes.WhiteSmoke);

            // Create the ContentPresenter to show the Button.Content
            FrameworkElementFactory presenter = new FrameworkElementFactory(typeof(ContentPresenter));
            presenter.SetValue(ContentPresenter.ContentProperty, new TemplateBindingExtension(Button.ContentProperty));
            presenter.SetValue(ContentPresenter.HorizontalAlignmentProperty, HorizontalAlignment.Center);
            presenter.SetValue(ContentPresenter.VerticalAlignmentProperty, VerticalAlignment.Center);

            // Create the Grid to hold both of the elements
            FrameworkElementFactory grid = new FrameworkElementFactory(typeof(Grid));
            grid.AppendChild(circle);
            grid.AppendChild(presenter);

            // Set the Grid as the ControlTemplate.VisualTree
            circleButtonTemplate.VisualTree = grid;

            // Set the ControlTemplate as the Button.Template
            button.Template = circleButtonTemplate;
        }
        #endregion

        #region Find a Node (button) in grid

        /// <summary>
        /// Function for btnFindNodeClickCommand
        /// It will find a node-button in grid and draw a circle around button 
        /// </summary>
        /// <param name="node"></param>
        /// <param name="grid"></param>
        private void FindNodeInGrid(Node<int> node, UIElement grid)
        {
            var nodeBeFind = NodeRoot.FindNode(node);
            if (nodeBeFind == null)
            {
                return;
            }
            var button = (grid as Grid).Children.OfType<Button>().ToList()
                .Where(p => p.Content.Equals(nodeBeFind.Data.ToString()))
                .FirstOrDefault();
            if (button == null)
            {
                return;
            }
            var point = button.TranslatePoint(new Point(0, 0), grid as Grid);
            CreateCircleAsync(point, grid);

        }

        /// <summary>
        /// Create a circle in 10 seconds
        /// </summary>
        /// <param name="point"></param>
        private async void CreateCircleAsync(Point point, UIElement grid)
        {
            Ellipse ellipse;
            await Task.Factory.StartNew(() =>
            {
                Application.Current.Dispatcher.Invoke(() =>
                {
                    ellipse = new Ellipse()
                    {
                        Stroke = new SolidColorBrush(Colors.Red),
                        Width = 55,
                        Height = 55,
                        StrokeThickness = 1.0
                    };
                    (grid as Grid).Children.Add(ellipse);

                });

                //.OfType<Ellipse>()
            });
            await Task.Factory.StartNew(() =>
            {
                Application.Current.Dispatcher.Invoke(async () =>
                {
                    await Task.Delay(3000);
                    (grid as Grid).Children.Remove((grid as Grid).Children.OfType<Ellipse>().FirstOrDefault());
                });
            });
        }
        #endregion

        #region Delete a node (button)

        private void DeleteNodeInGrid(Grid grid)
        {
            Button button = null;
            List<Task> listTask = new List<Task>();
            for (int i = 0; i <= grid.Children.OfType<Button>().ToList().Count; i++)
            {
                int j = i;
                var task = Task.Factory.StartNew(() =>
                 {
                     Application.Current.Dispatcher.Invoke(() =>
                     {
                         if (grid.Children[j] is Button)
                         {
                             if ((grid.Children[j] as Button).Content.Equals(NodeBeDelete.ToString()))
                             {
                                 button = grid.Children[j] as Button;
                                 return;
                             }
                         }
                     });
                 });
                listTask.Add(task);
            }

            Task.Factory.ContinueWhenAll(listTask.ToArray(), (p) =>
            {
                Application.Current.Dispatcher.Invoke(() =>
                { AnimationButtonMovetTo(20, 20, button); });
            });
            //var button = grid.Children.OfType<Button>().ToList().AsParallel().Where(p =>p.Content.Equals(NodeBeDelete.ToString())).FirstOrDefault();

        }

        /// <summary>
        /// Move a button from here to (x,y) with animation
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <param name="button"></param>
        private void AnimationButtonMovetTo(double x, double y, Button button)
        {
            if (button == null)
            {
                return;
            }
            Storyboard sb = new Storyboard();
            ThicknessAnimation animation = new ThicknessAnimation(new Thickness(x, y, 0, 0), TimeSpan.FromSeconds(1));
            Storyboard.SetTarget(animation, button);
            Storyboard.SetTargetProperty(animation, new PropertyPath("Margin"));
            sb.Children.Add(animation);
            sb.Completed += (o, s) =>
            {
                var margin = button.Margin;
                button.BeginAnimation(Button.MarginProperty, null);
                button.Margin = margin;
            };
            sb.Begin();
        }

        #endregion



        ////public override void Cleanup()
        ////{
        ////    // Clean up if needed

        ////    base.Cleanup();
        ////}
    }
}