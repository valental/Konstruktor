using System;
using System.Configuration;

namespace Konstruktor
{
    public static class Settings
    {
        public static int MaxSize
        {
            get
            {
                Int32.TryParse(ConfigurationManager.AppSettings["MaxSize"], out int maxSize);
                return maxSize;
            }
        }

        public static int LargeFactor
        {
            get
            {
                Int32.TryParse(ConfigurationManager.AppSettings["LargeFactor"], out int largeFactor);
                return largeFactor;
            }
        }

        public static int SmallFactor
        {
            get
            {
                Int32.TryParse(ConfigurationManager.AppSettings["SmallFactor"], out int smallFactor);
                return smallFactor;
            }
        }

        public static int SelectionWidth
        {
            get
            {
                Int32.TryParse(ConfigurationManager.AppSettings["SelectionWidth"], out int selectionWidth);
                return selectionWidth;
            }
        }
    }
}
