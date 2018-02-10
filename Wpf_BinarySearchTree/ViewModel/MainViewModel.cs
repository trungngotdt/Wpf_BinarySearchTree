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
                    AddButtonGridAsync(p[1] as Grid);
                });
            }
        }

        public ICommand BSTGridSizeChanged { get { return bSTGridSizeChanged; } }

        public ICommand BtnFindNodeClickCommand { get { return btnFindNodeClickCommand = new RelayCommand<UIElement>((p) => { FindNodeInGrid(new Node<int>(NumbeFind), p); }); } }

        public ICommand BtnDeleteNodeClickCommand { get { return btnDeleteNodeClickCommand = new RelayCommand<Grid>((p) => { DeleteNodeInGrid(p, NodeBeDelete); }); } }


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
        /// To calculate new position and button will be added to Grid
        /// </summary>
        /// <param name="p">this is a grid which will be add a button</param>
        private async void AddButtonGridAsync(UIElement p)
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

        private void DeleteNodeInGrid(Grid grid, int nodeDelete)
        {
            if (nodeBeDelete == null)
            {
                return;
            }
            /*
            Button button = null;
            List<Task> listTask = new List<Task>();
            for (int i = 0; i <= grid.Children.OfType<Button>().ToList().Count; i++)//Find a button will be remove
            {
                int j = i;
                var task = Task.Factory.StartNew(() =>
                 {
                     Application.Current.Dispatcher.Invoke(() =>
                     {
                         if (grid.Children[j] is Button)
                         {
                             if ((grid.Children[j] as Button).Content.Equals(nodeDelete.ToString()))
                             {
                                 button = grid.Children[j] as Button;
                                 return;
                             }
                         }
                     });
                 });
                listTask.Add(task);
            }*/
            var tup = FindButtonInGrid(grid, nodeDelete);
            //Task.Factory.ContinueWhenAll(tup.Result.Item1.ToArray(), p => { });
            tup.ContinueWith(p =>
            {
                Application.Current.Dispatcher.Invoke(() =>
                {
                    AnimationButtonMovetTo(20, 20, p.Result.Item2);
                    UpdateButtonAfterDelete(grid, nodeDelete);
                });
            });
            /*
            Task.Factory.ContinueWhenAll(tup.Result.Item1.ToArray(), (p) =>
            {
                Application.Current.Dispatcher.Invoke(() =>
                { AnimationButtonMovetTo(20, 20, tup.Result.Item2); UpdateButtonAfterDelete(grid, nodeDelete); });
            });*/

            //var button = grid.Children.OfType<Button>().ToList().AsParallel().Where(p =>p.Content.Equals(NodeBeDelete.ToString())).FirstOrDefault();
        }

        /// <summary>
        /// The line be found in grid by name
        /// </summary>
        /// <param name="grid"></param>
        /// <param name="name"></param>
        /// <returns></returns>
        Line FindLineInGrid(Grid grid, string name)
        {
            var line = grid.Children.OfType<Line>().Where(p => p.Name.Equals(name)).FirstOrDefault();
            return line;
        }

        /// <summary>
        /// Find the Button in grid  - async
        /// </summary>
        /// <param name="grid"></param>
        /// <param name="match">name of the button</param>
        /// <returns></returns>
        async Task<Tuple<List<Task>, Button>> FindButtonInGrid(Grid grid, object match)
        {
            Button button = null;
            List<Task> listTask = new List<Task>();
            var allButtonInGrid = grid.Children.OfType<Button>().ToList();
            for (int i = 0; i < allButtonInGrid.Count; i++)//Find a button will be remove
            {
                int j = i;
                var task = Task.Factory.StartNew(() =>
                {
                    Application.Current.Dispatcher.Invoke(() =>
                    {
                        if (allButtonInGrid[j].Content.Equals(match.ToString()))
                        {
                            button = allButtonInGrid[j];
                        }
                    });
                });
                listTask.Add(task);
            }
            await Task.WhenAll(listTask);
            //var result = new Tuple<List<Task>, Button>(listTask, button);
            return new Tuple<List<Task>, Button>(listTask, button);
        }

        /// <summary>
        /// Update X,Y for button (node)
        /// </summary>
        /// <param name="grid"></param>
        /// <param name="nodeDelete"></param>
        void UpdateButtonAfterDelete(Grid grid, int nodeDelete)
        {
            var nodeDel = NodeRoot.FindNode(new Node<int>(nodeDelete));

            if (nodeDel.Right != null && nodeDel.Letf != null)//Delete Button in grid (the button have 2 child )
            {
                //We will move successor from there to new position (the real button will be deleted)
                var nodePreviousDel = nodeDel;
                nodeDel = nodeDel.FindNode(new Node<int>(int.Parse(nodeDel.Successor().ToString())));
                var tup = FindButtonInGrid(grid, nodeDel.Data);
                tup.ContinueWith(p =>
                {
                    Application.Current.Dispatcher.Invoke(() =>
                    {
                        AnimationButtonMovetTo(nodePreviousDel.X, nodePreviousDel.Y, p.Result.Item2);//to move a successor to new position (the button will be deleted)
                        UpdateButtonAfterDelete(grid, nodeDel.Data);
                    });
                });
            }
            else
            {
                //Delete button have one or none child
                var nodePa = NodeRoot.FindParent(new Node<int>(nodeDel.Data));
                var line = FindLineInGrid(grid, $"{"Btn" +nodePa.Item1.Data.ToString()+"Btn"+nodeDel.Data.ToString()}");
                var lineL = nodeDel.Letf == null ? null : FindLineInGrid(grid, $"{"Btn" +nodeDel.Data.ToString()+"Btn"+nodeDel.Letf.Data.ToString()}");                
                var lineR = nodeDel.Right == null ? null : FindLineInGrid(grid, $"{"Btn"+nodeDel.Data.ToString()+"Btn"+nodeDel.Right.Data.ToString()}");
                grid.Children.Remove(lineR);
                grid.Children.Remove(lineL);
                grid.Children.Remove(line);
                var taskL = Task.Factory.StartNew(() =>
                 {
                     Application.Current.Dispatcher.Invoke(() => { RelayoutButtonAfterDelete(grid, nodePa.Item1, false, nodeDel.Letf); });
                 });
                var taskR = Task.Factory.StartNew(() =>
                 {
                     Application.Current.Dispatcher.Invoke(() => { RelayoutButtonAfterDelete(grid, nodePa.Item1, true, nodeDel.Right); });
                 });
                Task.WhenAll(new Task[] { taskL, taskR });
            }
        }


        void RelayoutButtonAfterDelete(Grid grid, Node<int> nodeParent, bool isRight, Node<int> node = null)
        {
            if (node == null)
            {
                return;
            }
            if (nodeParent != null)
            {
                var nameLine = $"{"Btn"+ nodeParent.Data.ToString()+"Btn"+node.Data.ToString()}";
                var line = FindLineInGrid(grid,nameLine );
                grid.Children.Remove(line);
                var remainingSpace = grid.ActualWidth / Math.Pow(2, ((nodeParent.Y + VerticalMarging) / VerticalMarging));
                node.X = nodeParent.X + (isRight == true ? remainingSpace : -remainingSpace);
                node.Y = nodeParent.Y + VerticalMarging;
                DrawLine(grid, nodeParent.X, node.X, nodeParent.Y, node.Y, isRight, nameLine);
            }
            else
            {
                node.X = grid.ActualWidth / 2;
                node.Y = VerticalMarging;
            }
            /*
            if (isRight)
            {
                node.X = nodeParent.X + grid.ActualWidth / Math.Pow(2, ((nodeParent.Y + VerticalMarging) / VerticalMarging));
                node.Y = nodeParent.Y + VerticalMarging;
            }
            else
            {
                node.X = nodeParent.X - grid.ActualWidth / Math.Pow(2, ((nodeParent.Y + VerticalMarging) / VerticalMarging));
                node.Y = nodeParent.Y + VerticalMarging;
            }
            */
            var button = grid.Children.OfType<Button>().Where(p => p.Name.Equals("Btn" + node.Data.ToString())).Single();
            AnimationButtonMovetTo(node.X, node.Y, button);

            var taskR = Task.Factory.StartNew(() =>
            {
                Application.Current.Dispatcher.Invoke(() => { RelayoutButtonAfterDelete(grid, node, true, node.Right); });
            });
            var taskL = Task.Factory.StartNew(() =>
            {
                Application.Current.Dispatcher.Invoke(() => { RelayoutButtonAfterDelete(grid, node, false, node.Letf); });
            });
            Task.WhenAll(new Task[] { taskL, taskR });
        }

        void ReDrawLine(Grid grid, double x1, double x2, double y1, double y2, bool isButtonDelete, string name, Node<int> nodePa = null, Node<int> nodeChild = null)
        {
            if (isButtonDelete)
            {
                if (nodePa.Right == null && nodePa.Letf == null)
                {

                }
            }
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
                button.Margin = new Thickness(x, y, 0, 0);
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