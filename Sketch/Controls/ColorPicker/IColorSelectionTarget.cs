﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;

namespace Sketch.Controls.ColorPicker
{
    public interface IColorSelectionTarget
    {
        void SetColor( Color newFillColor );
    }
}
