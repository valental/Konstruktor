using Konstruktor.Controls;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Controls;

namespace Konstruktor.DataHelpers
{
    public static class CuboidsHelper
    {
        public static void ResetIsOnTop(ObservableCollection<Cuboid> cuboids)
        {
            foreach (Cuboid cuboid in cuboids)
                cuboid.IsOnTop = CheckIfTop(cuboids, cuboid);
        }

        public static bool CheckIfTop(ObservableCollection<Cuboid> cuboids, Cuboid cuboid)
        {
            foreach (Cuboid c in cuboids)
            {
                if (c != cuboid && c.SitsOn(cuboid))
                    return false;
            }
            return true;
        }

        public static bool CheckIfCanBeAdded(ObservableCollection<Cuboid> cuboids, Cuboid cuboid)
        {
            if (cuboid.X < 0 || cuboid.Y < 0 || cuboid.Z < 0)
                return false;

            if ((cuboid.X + cuboid.Width > Settings.MaxSize) ||
                (cuboid.Y + cuboid.Depth > Settings.MaxSize) ||
                (cuboid.Z + cuboid.Height > Settings.MaxSize))
                return false;

            foreach (Cuboid c in cuboids)
            {
                if (c != cuboid && c.OverlapsWith(cuboid))
                    return false;
            }

            if (cuboid.Z == 0)
                return true;

            bool[,] checks = new bool[cuboid.Width, cuboid.Depth];
            for (int i = 0; i < cuboid.Width; i++)
                for (int j = 0; j < cuboid.Depth; j++)
                    checks[i, j] = false;

            foreach (Cuboid c in cuboids)
            {
                if (cuboid.SitsOn(c))
                {
                    int xStart = cuboid.X > c.X ? cuboid.X : c.X;
                    xStart -= cuboid.X;
                    int yStart = cuboid.Y > c.Y ? cuboid.Y : c.Y;
                    yStart -= cuboid.Y;

                    int xEnd = (cuboid.X + cuboid.Width) < (c.X + c.Width) ? (cuboid.X + cuboid.Width) : (c.X + c.Width);
                    xEnd -= cuboid.X;
                    int yEnd = (cuboid.Y + cuboid.Depth) < (c.Y + c.Depth) ? (cuboid.Y + cuboid.Depth) : (c.Y + c.Depth);
                    yEnd -= cuboid.Y;

                    for (int i = xStart; i < xEnd; i++)
                    {
                        for (int j = yStart; j < yEnd; j++)
                        {
                            checks[i, j] = true;
                        }
                    }
                }
            }

            for (int i = 0; i < cuboid.Width; i++)
                for (int j = 0; j < cuboid.Depth; j++)
                    if (!checks[i, j])
                        return false;

            return true;
        }
        
        public static void ResetZIndexes(Canvas canvas, ViewDirection viewDirection)
        {
            foreach (UIElement child in canvas.Children)
            {
                int zIndex = 0;
                if (child is CuboidControl)
                {
                    CuboidControl cuboidControl = child as CuboidControl;
                    switch (viewDirection)
                    {
                        case ViewDirection.TopView:
                            zIndex = cuboidControl.Cuboid.Z;
                            break;
                        case ViewDirection.FrontView:
                            zIndex = Settings.MaxSize - cuboidControl.Cuboid.Y;
                            break;
                        case ViewDirection.BackView:
                            zIndex = cuboidControl.Cuboid.Y;
                            break;
                        case ViewDirection.LeftView:
                            zIndex = Settings.MaxSize - cuboidControl.Cuboid.X;
                            break;
                        case ViewDirection.RightView:
                            zIndex = cuboidControl.Cuboid.X;
                            break;
                    }
                }
                else
                {
                    zIndex = Settings.MaxSize + 1;
                }
                Canvas.SetZIndex(child, zIndex);
            }
        }

        #region Find the third coordinate for insert
        public static int FindLastCoordinateForTopView(ObservableCollection<Cuboid> cuboids, Cuboid cuboid)
        {
            int max = 0;
            foreach (Cuboid c in cuboids)
            {
                if ((c.X <= cuboid.X) && (cuboid.X < c.X + c.Width) &&
                    (c.Y <= cuboid.Y) && (cuboid.Y < c.Y + c.Depth) &&
                    (c.Z + c.Height > max) && cuboid != c)
                {
                    max = c.Z + c.Height;
                }
            }
            return max;
        }

        public static int FindLastCoordinateForTopView(ObservableCollection<Cuboid> cuboids, int x, int y)
        {
            int max = 0;
            foreach (Cuboid cuboid in cuboids)
            {
                if ((cuboid.X <= x) && (x < cuboid.X + cuboid.Width) &&
                    (cuboid.Y <= y) && (y < cuboid.Y + cuboid.Depth) &&
                    (cuboid.Z + cuboid.Height > max))
                {
                    max = cuboid.Z + cuboid.Height;
                }
            }
            return max;
        }

        public static int FindLastCoordinateForRightView(ObservableCollection<Cuboid> cuboids, int y, int z)
        {
            int max = 0;
            foreach (Cuboid cuboid in cuboids)
            {
                if ((cuboid.Y <= y) && (y < cuboid.Y + cuboid.Depth) &&
                    (cuboid.Z <= z) && (z < cuboid.Z + cuboid.Height) &&
                    (cuboid.X + cuboid.Width > max))
                {
                    max = cuboid.X + cuboid.Width;
                }
            }
            return max;
        }

        public static int FindLastCoordinateForLeftView(ObservableCollection<Cuboid> cuboids, int y, int z)
        {
            int min = Settings.MaxSize;
            foreach (Cuboid cuboid in cuboids)
            {
                if ((cuboid.Y <= y) && (y < cuboid.Y + cuboid.Depth) &&
                    (cuboid.Z <= z) && (z < cuboid.Z + cuboid.Height) &&
                    (cuboid.X < min))
                {
                    min = cuboid.X;
                }
            }
            return min;
        }

        public static int FindLastCoordinateForBackView(ObservableCollection<Cuboid> cuboids, int x, int z)
        {
            int max = 0;
            foreach (Cuboid cuboid in cuboids)
            {
                if ((cuboid.X <= x) && (x < cuboid.X + cuboid.Width) &&
                    (cuboid.Z <= z) && (z < cuboid.Z + cuboid.Height) &&
                    (cuboid.Y + cuboid.Depth > max))
                {
                    max = cuboid.Y + cuboid.Depth;
                }
            }
            return max;
        }

        public static int FindLastCoordinateForFrontView(ObservableCollection<Cuboid> cuboids, int x, int z)
        {
            int min = Settings.MaxSize;
            foreach (Cuboid cuboid in cuboids)
            {
                if ((cuboid.X <= x) && (x < cuboid.X + cuboid.Width) &&
                    (cuboid.Z <= z) && (z < cuboid.Z + cuboid.Height) &&
                    (cuboid.Y < min))
                {
                    min = cuboid.Y;
                }
            }
            return min;
        }
        #endregion
    }
}
