using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Shapes;

using Konstruktor.Converters;
using Konstruktor.DataHelpers;
using Konstruktor.ViewModels;

namespace Konstruktor.Controls
{
    public enum ViewDirection
    {
        TopView,
        FrontView,
        LeftView,
        RightView,
        BackView
    }

    /// <summary>
    /// Interaction logic for ConstructionView.xaml
    /// </summary>
    public partial class ConstructionView : UserControl, INotifyPropertyChanged
    {
        #region Dependency Properties
        public static readonly DependencyProperty ViewDirectionProperty = DependencyProperty.Register(
            "ViewDirection", typeof(ViewDirection), typeof(ConstructionView), new PropertyMetadata(ViewDirection.TopView)
        );

        public static readonly DependencyProperty FullSizeProperty = DependencyProperty.Register(
            "FullSize", typeof(bool), typeof(ConstructionView), new PropertyMetadata(false)
        );

        public static readonly DependencyProperty CuboidsProperty = DependencyProperty.Register(
            "Cuboids",
            typeof(ObservableCollection<Cuboid>),
            typeof(ConstructionView),
            new PropertyMetadata(new ObservableCollection<Cuboid>(), OnCuboidsChanged)
        );

        public static readonly DependencyProperty CuboidsSelectionProperty = DependencyProperty.Register(
            "CuboidsSelection",
            typeof(ObservableCollection<Cuboid>),
            typeof(ConstructionView),
            new PropertyMetadata(new ObservableCollection<Cuboid>(), OnCuboidsChanged)
        );

        public static readonly DependencyProperty IsGridVisibleProperty = DependencyProperty.Register(
            "IsGridVisible", typeof(bool), typeof(ConstructionView), new PropertyMetadata(false)
        );
        #endregion

        #region Public Properties
        public ViewDirection ViewDirection
        {
            get { return (ViewDirection)GetValue(ViewDirectionProperty); }
            set { SetValue(ViewDirectionProperty, value); }
        }

        public bool FullSize
        {
            get { return (bool)GetValue(FullSizeProperty); }
            set { SetValue(FullSizeProperty, value); }
        }

        public ObservableCollection<Cuboid> Cuboids
        {
            get { return (ObservableCollection<Cuboid>)GetValue(CuboidsProperty); }
            set { SetValue(CuboidsProperty, value); }
        }

        public ObservableCollection<Cuboid> CuboidsSelection
        {
            get { return (ObservableCollection<Cuboid>)GetValue(CuboidsSelectionProperty); }
            set { SetValue(CuboidsSelectionProperty, value); }
        }

        public bool IsGridVisible
        {
            get { return (bool)GetValue(IsGridVisibleProperty); }
            set { SetValue(IsGridVisibleProperty, value); }
        }
        #endregion

        #region Constructor
        public ConstructionView()
        {
            InitializeComponent();
        }
        #endregion

        #region Methods
        private void DisplayInControl()
        {
            int factor = FullSize ? Settings.LargeFactor : Settings.SmallFactor;
            int max = Settings.MaxSize;
            int size = factor * max;

            MainCanvas.Children.Clear();

            Border border = new Border
            {
                Width = max * factor + 2,
                Height = size,
                BorderThickness = new Thickness(0, 0, 2, 0),
                BorderBrush = Brushes.Black
            };
            MainCanvas.Children.Add(border);

            DisplayCuboids(Cuboids);
            DisplayCuboids(CuboidsSelection);

            DisplaySelectionCuboidsDimensions();
            DisplayTheGrid(factor, max, size);

            CuboidsHelper.ResetIsOnTop(Cuboids);
            CuboidsHelper.ResetZIndexes(MainCanvas, ViewDirection);
        }

        private void DisplayCuboids(ObservableCollection<Cuboid> cuboids)
        {
            foreach (Cuboid cuboid in cuboids)
            {
                CuboidControl cuboidControl = new CuboidControl
                {
                    Cuboid = cuboid,
                    ViewDirection = ViewDirection,
                    FullSize = FullSize
                };
                Binding binding = new Binding("IsGridVisible")
                {
                    ElementName = "konstruktor",
                    Mode = BindingMode.OneWay
                };
                cuboidControl.SetBinding(CuboidControl.IsGridVisibleProperty, binding);
                MainCanvas.Children.Add(cuboidControl);
            }
        }

        private void DisplayTheGrid(int factor, int max, int size)
        {
            MultiBinding multiBinding = new MultiBinding()
            {
                Converter = new MultipleBoolToVisibilityConverter()
            };
            multiBinding.Bindings.Add(new Binding("IsGridVisible") { ElementName = "konstruktor", Mode = BindingMode.OneWay });
            multiBinding.Bindings.Add(new Binding("FullSize") { ElementName = "konstruktor", Mode = BindingMode.OneWay });

            for (int i = 0; i <= max; i++)
            {
                #region Vertical Line
                Line verticalLine = new Line
                {
                    Stroke = Brushes.Gray,
                    X1 = i * factor,
                    X2 = i * factor,
                    Y1 = 0,
                    Y2 = size,
                    StrokeThickness = 1
                };
                verticalLine.SetBinding(Line.VisibilityProperty, multiBinding);
                MainCanvas.Children.Add(verticalLine);
                #endregion

                #region Horizontal Line
                Line horizontalLine = new Line
                {
                    Stroke = Brushes.Gray,
                    X1 = 0,
                    X2 = size,
                    Y1 = i * factor,
                    Y2 = i * factor,
                    StrokeThickness = 1
                };
                horizontalLine.SetBinding(Line.VisibilityProperty, multiBinding);
                MainCanvas.Children.Add(horizontalLine);
                #endregion
            }
        }

        private void DisplaySelectionCuboidsDimensions()
        {
            foreach (Cuboid cuboid in CuboidsSelection)
            {
                TextBlock textBlock = new TextBlock
                {
                    VerticalAlignment = VerticalAlignment.Center,
                    FontWeight = FontWeights.DemiBold
                };
                MultiBinding multiBinding = new MultiBinding()
                {
                    Converter = new CuboidDimensionsToStringConverter()
                };
                multiBinding.Bindings.Add(new Binding("Width") { Source = cuboid, Mode = BindingMode.OneWay });
                multiBinding.Bindings.Add(new Binding("Depth") { Source = cuboid, Mode = BindingMode.OneWay });
                multiBinding.Bindings.Add(new Binding("Height") { Source = cuboid, Mode = BindingMode.OneWay });
                textBlock.SetBinding(TextBlock.TextProperty, multiBinding);

                int left = Settings.LargeFactor * (Settings.MaxSize + 2);
                int top = 2 * Settings.LargeFactor * (cuboid.SelectId - 1);

                Border border = new Border
                {
                    Height = Settings.LargeFactor,
                    Margin = new Thickness(5, 0, 0, 0),
                    Child = textBlock
                };
                Canvas.SetLeft(border, left);
                Canvas.SetTop(border, top);
                MainCanvas.Children.Add(border);
            }
        }
        #endregion

        #region Event Handlers
        private static void OnCuboidsChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ConstructionView constructionView = d as ConstructionView;
            constructionView.DisplayInControl();
        }

        private void Konstruktor_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            if (!FullSize || !IsGridVisible)
                return;

            Canvas canvas = sender as Canvas;
            int largeFactor = Settings.LargeFactor;
            double firstCoordinate = e.GetPosition(canvas).X - canvas.Margin.Left;
            double secondCoordinate = e.GetPosition(canvas).Y - canvas.Margin.Top;
            firstCoordinate = Math.Floor(firstCoordinate / largeFactor);
            secondCoordinate = Math.Floor(secondCoordinate / largeFactor);

            int max = Settings.MaxSize;
            MainWindowViewModel vm = DataContext as MainWindowViewModel;

            if (vm.SelectedCuboidId <= 0)
                return;

            // Add
            Cuboid selectedCuboid = vm.CuboidsSelection[vm.SelectedCuboidId - 1];
            int x = 0;
            int y = 0;
            int z = 0;
            int width = selectedCuboid.Width;
            int depth = selectedCuboid.Depth;
            int height = selectedCuboid.Height;

            switch (ViewDirection)
            {
                case ViewDirection.TopView:
                    x = (int)firstCoordinate;
                    y = max - depth - (int)secondCoordinate;
                    z = CuboidsHelper.FindLastCoordinateForTopView(Cuboids, x, y);
                    break;
                case ViewDirection.FrontView:
                    x = (int)firstCoordinate;
                    z = max - height - (int)secondCoordinate;
                    y = CuboidsHelper.FindLastCoordinateForFrontView(Cuboids, x, z) - depth;
                    break;
                case ViewDirection.BackView:
                    x = max - width - (int)firstCoordinate;
                    z = max - height - (int)secondCoordinate;
                    y = CuboidsHelper.FindLastCoordinateForBackView(Cuboids, x, z);
                    break;
                case ViewDirection.LeftView:
                    y = max - depth - (int)firstCoordinate;
                    z = max - height - (int)secondCoordinate;
                    x = CuboidsHelper.FindLastCoordinateForLeftView(Cuboids, y, z) - width;
                    break;
                case ViewDirection.RightView:
                    y = (int)firstCoordinate;
                    z = max - height - (int)secondCoordinate;
                    x = CuboidsHelper.FindLastCoordinateForRightView(Cuboids, y, z);
                    break;
            }

            Cuboid newCuboid = new Cuboid(width, depth, height, x, y, z);
            if (CuboidsHelper.CheckIfCanBeAdded(Cuboids, newCuboid))
            {
                AddCuboid(newCuboid);
            }
        }

        private void AddCuboid(Cuboid newCuboid)
        {
            var tmp = new ObservableCollection<Cuboid>();
            foreach (Cuboid cuboid in Cuboids)
                tmp.Add(cuboid);
            tmp.Add(newCuboid);
            Cuboids = tmp;
            MainWindowViewModel vm = DataContext as MainWindowViewModel;
            vm.Actions.Add(new List<ActionObject>() { new ActionObject(newCuboid, ActionType.Add) });
        }
        #endregion

        #region INotifyPropertyChanged
        /// <summary>
        /// The event that is fired when any child property changes its value
        /// </summary>
        public event PropertyChangedEventHandler PropertyChanged = (sender, e) => { };

        /// <summary>
        /// Call this to fire a <see cref="PropertyChanged"/> event
        /// </summary>
        /// <param name="name"></param>
        public void OnPropertyChanged(string name)
        {
            PropertyChanged(this, new PropertyChangedEventArgs(name));
        }
        #endregion
    }
}
