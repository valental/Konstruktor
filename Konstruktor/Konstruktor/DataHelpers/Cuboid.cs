using System;

namespace Konstruktor.DataHelpers
{
    public class Cuboid : DataObjectBase
    {
        #region Public Properties
        #region Dimensions
        private int width;
        public int Width
        {
            get => width;
            set { width = value; OnPropertyChanged("Width"); }
        }

        private int depth;
        public int Depth
        {
            get => depth;
            set { depth = value; OnPropertyChanged("Depth"); }
        }

        private int height;
        public int Height
        {
            get => height;
            set { height = value; OnPropertyChanged("Height"); }
        }
        #endregion

        #region Position
        private int x;
        public int X
        {
            get => x;
            set { x = value; OnPropertyChanged("X"); }
        }

        private int y;
        public int Y
        {
            get => y;
            set { y = value; OnPropertyChanged("Y"); }
        }

        private int z;
        public int Z
        {
            get => z;
            set { z = value; OnPropertyChanged("Z"); }
        }
        #endregion

        private bool isOnTop = true;
        public bool IsOnTop
        {
            get => isOnTop;
            set { isOnTop = value; }
        }

        #region Selection
        private bool fromSelection = false;
        public bool FromSelection
        {
            get => fromSelection;
            set { fromSelection = value; OnPropertyChanged("FromSelection"); }
        }

        private bool isSelected = false;
        public bool IsSelected
        {
            get => isSelected;
            set { isSelected = value; OnPropertyChanged("IsSelected"); }
        }

        private bool isDragged = false;
        public bool IsDragged
        {
            get => isDragged;
            set { isDragged = value; OnPropertyChanged("IsDragged"); }
        }

        private int selectId;
        public int SelectId
        {
            get => selectId;
            set { selectId = value; OnPropertyChanged("SelectId"); }
        }
        #endregion
        #endregion

        #region Constructor
        public Cuboid(int width = 1, int depth = 1, int height = 1, int x = 0, int y = 0, int z = 0)
        {
            Width = width;
            Depth = depth;
            Height = height;
            X = x;
            Y = y;
            Z = z;
        }

        public Cuboid(Cuboid c) : this(c.width, c.depth, c.height, c.x, c.y, c.z) { }
        #endregion

        #region Methods
        public bool SitsOn(Cuboid cuboid)
        {
            if (cuboid.X + cuboid.Width <= X || X + Width <= cuboid.X)
                return false;
            if (cuboid.Y + cuboid.Depth <= Y || Y + Depth <= cuboid.Y)
                return false;
            if (cuboid.Z + cuboid.Height != Z)
                return false;
            return true;
        }

        public bool OverlapsWith(Cuboid cuboid)
        {
            if (cuboid.X + cuboid.Width <= X || X + Width <= cuboid.X)
                return false;
            if (cuboid.Y + cuboid.Depth <= Y || Y + Depth <= cuboid.Y)
                return false;
            if (cuboid.Z + cuboid.Height <= Z || Z + Height <= cuboid.Z)
                return false;
            return true;
        }

        public bool SameValues(Cuboid cuboid)
        {
            return width == cuboid.width && depth == cuboid.depth && height == cuboid.height &&
                x == cuboid.x && y == cuboid.y && z == cuboid.z;
        }

        public override string ToString()
        {
            return width + "," + depth + "," + height + "," + x + "," + y + "," + z;
        }

        public static Cuboid GetCuboid(string s)
        {
            string[] values = s.Split(',');
            if (values.Length == 6 &&
                Int32.TryParse(values[0], out int width) && Int32.TryParse(values[1], out int depth) && Int32.TryParse(values[2], out int height) &&
                Int32.TryParse(values[3], out int x) && Int32.TryParse(values[4], out int y) && Int32.TryParse(values[5], out int z))
            {
                return new Cuboid(width, depth, height, x, y, z);
            }
            else
            {
                return null;
            }
        }
        #endregion
    }
}
