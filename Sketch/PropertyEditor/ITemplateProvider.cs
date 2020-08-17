﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace Sketch.PropertyEditor
{
    public interface ITemplateProvider
    {
        DataTemplateSelector CellTemplateSelector
        {
            get;
        }

        DataTemplateSelector CellEditingTemplateSelector
        {
            get;
        }
    }
}