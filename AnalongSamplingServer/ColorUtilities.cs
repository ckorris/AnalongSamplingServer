using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UI
{
    public static class ColorUtilities
    {
        private static List<Color> _colors = new List<Color>()
        {
            Color.Blue,
            Color.Red,
            Color.Green,
            Color.Magenta,
            Color.Orange,
            Color.Yellow,
            Color.Cyan,
            Color.DeepPink,
            Color.LightSeaGreen,
            Color.Purple,
            Color.Navy,
            Color.Aquamarine,
            Color.Chocolate,
            Color.Silver,
            Color.Crimson,
            Color.SlateGray,
            Color.MediumPurple,
            Color.SpringGreen,
            Color.SteelBlue,
            Color.OrangeRed
        };

        private static int _colorIncrementIndex = 0;

        public static Color GetColorByIndex(int colorIndex)
        {
            return _colors[colorIndex % _colors.Count];
        }

        public static Color GetSequentialColor()
        {
            Color returnColor = _colors[_colorIncrementIndex];

            _colorIncrementIndex++;
            if(_colorIncrementIndex >= _colors.Count)
            {
                _colorIncrementIndex = 0;
            }

            return returnColor;
        }
    }
}
