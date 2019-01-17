using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Shapes;

using Konstruktor.DataHelpers;
using Konstruktor.ViewModels;

namespace Konstruktor.Controls
{
    /// <summary>
    /// Interaction logic for CuboidControl.xaml
    /// </summary>
    public partial class CuboidControl : Border, INotifyPropertyChanged
    {
        #region Dependency Properties
        public static readonly DependencyProperty CuboidProperty = DependencyProperty.Register(
            "Cuboid", typeof(Cuboid), typeof(CuboidControl), new PropertyMetadata(new Cuboid())
        );

        public static readonly DependencyProperty ViewDirectionProperty = DependencyProperty.Register(
            "ViewDirection", typeof(ViewDirection), typeof(CuboidControl), new PropertyMetadata(ViewDirection.TopView)
        );

        public static readonly DependencyProperty FullSizeProperty = DependencyProperty.Register(
            "FullSize", typeof(bool), typeof(CuboidControl), new PropertyMetadata(false)
        );

        public static readonly DependencyProperty IsGridVisibleProperty = DependencyProperty.Register(
            "IsGridVisible", typeof(bool), typeof(CuboidControl), new PropertyMetadata(false)
        );
        #endregion

        #region Public Properties
        public Cuboid Cuboid
        {
            get { return (Cuboid)GetValue(CuboidProperty); }
            set { SetValue(CuboidProperty, value); }
        }

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

        public bool IsGridVisible
        {
            get { return (bool)GetValue(IsGridVisibleProperty); }
            set { SetValue(IsGridVisibleProperty, value); }
        }
        #endregion

        #region Constructor
        public CuboidControl()
        {
            InitializeComponent();
        }
        #endregion

        #region Event Handlers
        private object movingObject;
        private int startX, startY, startZ;
        private double firstXPos, firstYPos;

        private void CuboidControl_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (!FullSize || DataContext == null)
                return;

            // In this event, we get the current mouse position on the control to use it in the MouseMove event.
            CuboidControl cc = sender as CuboidControl;
            Canvas canvas = cc.Parent as Canvas;

            if (cc.Cuboid.FromSelection)
            {
                MainWindowViewModel vm = DataContext as MainWindowViewModel;
                vm.SelectedCuboidId = vm.SelectedCuboidId == cc.Cuboid.SelectId ? 0 : cc.Cuboid.SelectId;
            }

            if (IsGridVisible)
            {
                MainWindowViewModel vm = DataContext as MainWindowViewModel;
                // if none is selected remove
                if (vm.SelectedCuboidId == 0)
                {
                    vm.RemoveCuboid(cc.Cuboid);
                    return;
                }
            }

            if (!cc.Cuboid.IsOnTop)
                return;

            firstXPos = e.GetPosition(cc).X;
            firstYPos = e.GetPosition(cc).Y;

            movingObject = sender;
            startX = cc.Cuboid.X;
            startY = cc.Cuboid.Y;
            startZ = cc.Cuboid.Z;

            // Put the image currently being dragged on top of the others
            int start = Canvas.GetZIndex(cc);
            int top = start;
            foreach (UIElement child in canvas.Children)
                if (child is CuboidControl && top < Canvas.GetZIndex(child))
                    top = Canvas.GetZIndex(child);
            if (top > start)
                Canvas.SetZIndex(cc, top + 1);

            foreach (UIElement child in canvas.Children)
                if (child is Line)
                    Canvas.SetZIndex(child, top + 1);

            Mouse.Capture(cc);
        }

        private void CuboidControl_PreviewMouseMove(object sender, MouseEventArgs e)
        {
            if (!FullSize || IsGridVisible)
                return;

            if (e.LeftButton == MouseButtonState.Pressed && sender == movingObject)
            {
                CuboidControl cc = sender as CuboidControl;
                Canvas canvas = cc.Parent as Canvas;
                
                if (!cc.Cuboid.IsOnTop)
                    return;

                if (cc.Cuboid.FromSelection)
                {
                    MainWindowViewModel vm = DataContext as MainWindowViewModel;
                    vm.SelectedCuboidId = cc.Cuboid.SelectId;
                    cc.Cuboid.IsDragged = true;
                }

                int largeFactor = Settings.LargeFactor;
                int selectionWidth = Settings.LargeFactor * Settings.SelectionWidth;

                double newLeft = e.GetPosition(canvas).X - firstXPos - canvas.Margin.Left;
                double newTop = e.GetPosition(canvas).Y - firstYPos - canvas.Margin.Top;

                if (!cc.Cuboid.FromSelection)
                {
                    // newLeft inside canvas right-border?
                    if (newLeft > canvas.Margin.Left + canvas.ActualWidth - cc.ActualWidth - selectionWidth)
                        newLeft = canvas.Margin.Left + canvas.ActualWidth - cc.ActualWidth - selectionWidth;
                }
                else
                {
                    // newLeft inside canvas right-border?
                    if (newLeft > canvas.Margin.Left + canvas.ActualWidth - cc.ActualWidth - (Settings.SelectionWidth - 2) * Settings.LargeFactor)
                        newLeft = canvas.Margin.Left + canvas.ActualWidth - cc.ActualWidth - (Settings.SelectionWidth - 2) * Settings.LargeFactor;
                }

                // newLeft inside canvas left-border?
                if (newLeft < canvas.Margin.Left)
                    newLeft = canvas.Margin.Left;
                newLeft = Math.Floor(newLeft / largeFactor);

                // newTop inside canvas bottom-border?
                if (newTop > canvas.Margin.Top + canvas.ActualHeight - cc.ActualHeight)
                    newTop = canvas.Margin.Top + canvas.ActualHeight - cc.ActualHeight;
                // newTop inside canvas top-border?
                else if (newTop < canvas.Margin.Top)
                    newTop = canvas.Margin.Top;
                newTop = Math.Ceiling(newTop / largeFactor);

                int max = Settings.MaxSize;

                switch (cc.ViewDirection)
                {
                    case ViewDirection.TopView:
                        cc.Cuboid.X = (int)newLeft;
                        cc.Cuboid.Y = max - cc.Cuboid.Depth - (int)newTop;
                        break;
                    case ViewDirection.FrontView:
                        cc.Cuboid.X = (int)newLeft;
                        cc.Cuboid.Z = max - cc.Cuboid.Height - (int)newTop;
                        break;
                    case ViewDirection.BackView:
                        cc.Cuboid.X = max - cc.Cuboid.Width - (int)newLeft;
                        cc.Cuboid.Z = max - cc.Cuboid.Height - (int)newTop;
                        break;
                    case ViewDirection.LeftView:
                        cc.Cuboid.Y = max - cc.Cuboid.Depth - (int)newLeft;
                        cc.Cuboid.Z = max - cc.Cuboid.Height - (int)newTop;
                        break;
                    case ViewDirection.RightView:
                        cc.Cuboid.Y = (int)newLeft;
                        cc.Cuboid.Z = max - cc.Cuboid.Height - (int)newTop;
                        break;
                }
            }
        }

        private void CuboidControl_PreviewMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            if (!FullSize || IsGridVisible)
                return;

            CuboidControl cc = sender as CuboidControl;
            Canvas canvas = cc.Parent as Canvas;

            if (!cc.Cuboid.IsOnTop)
                return;

            MainWindowViewModel vm = DataContext as MainWindowViewModel;

            if (cc.Cuboid.FromSelection)
            {
                switch (ViewDirection)
                {
                    case ViewDirection.TopView:
                        cc.Cuboid.Z = CuboidsHelper.FindLastCoordinateForTopView(vm.Cuboids, cc.Cuboid.X, cc.Cuboid.Y);
                        break;
                    case ViewDirection.FrontView:
                        cc.Cuboid.Y = CuboidsHelper.FindLastCoordinateForFrontView(vm.Cuboids, cc.Cuboid.X, cc.Cuboid.Z) - cc.Cuboid.Depth;
                        break;
                    case ViewDirection.BackView:
                        cc.Cuboid.Y = CuboidsHelper.FindLastCoordinateForBackView(vm.Cuboids, cc.Cuboid.X, cc.Cuboid.Z);
                        break;
                    case ViewDirection.LeftView:
                        cc.Cuboid.X = CuboidsHelper.FindLastCoordinateForLeftView(vm.Cuboids, cc.Cuboid.Y, cc.Cuboid.Z) - cc.Cuboid.Width;
                        break;
                    case ViewDirection.RightView:
                        cc.Cuboid.X = CuboidsHelper.FindLastCoordinateForRightView(vm.Cuboids, cc.Cuboid.Y, cc.Cuboid.Z);
                        break;
                }

                Cuboid newCuboid = new Cuboid(cc.Cuboid);
                if (cc.Cuboid.IsDragged && CuboidsHelper.CheckIfCanBeAdded(vm.Cuboids, newCuboid))
                {
                    ObservableCollection<Cuboid> tmp = new ObservableCollection<Cuboid>(vm.Cuboids);
                    tmp.Add(newCuboid);
                    vm.Cuboids = new ObservableCollection<Cuboid>(tmp);
                    vm.Actions.Add(new List<ActionObject>() { new ActionObject(newCuboid, ActionType.Add) });
                }
                cc.Cuboid.IsDragged = false;
            }
            else
            {
                if (ViewDirection == ViewDirection.TopView)
                {
                    cc.Cuboid.Z = CuboidsHelper.FindLastCoordinateForTopView(vm.Cuboids, cc.Cuboid);
                }
                if (CuboidsHelper.CheckIfCanBeAdded(vm.Cuboids, cc.Cuboid))
                {
                    Cuboid startCuboid = new Cuboid(cc.Cuboid);
                    startCuboid.X = startX;
                    startCuboid.Y = startY;
                    startCuboid.Z = startZ;
                    List<ActionObject> actions = new List<ActionObject>()
                    {
                        new ActionObject(startCuboid, ActionType.Remove),
                        new ActionObject(new Cuboid(cc.Cuboid), ActionType.Add)
                    };
                    vm.Actions.Add(actions);
                }
                else
                {
                    cc.Cuboid.X = startX;
                    cc.Cuboid.Y = startY;
                    cc.Cuboid.Z = startZ;
                }
            }

            CuboidsHelper.ResetIsOnTop(vm.Cuboids);
            CuboidsHelper.ResetZIndexes(canvas, ViewDirection);
            movingObject = null;
            Mouse.Capture(null);
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
