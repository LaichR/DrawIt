﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Collections.ObjectModel;


namespace Sketch.Interface
{
    public interface ISketchItemContainer : ISketchItemLabelProvider
    {


        ObservableCollection<ISketchItemModel> SketchItems
        {
            get;
        }

    }
}
