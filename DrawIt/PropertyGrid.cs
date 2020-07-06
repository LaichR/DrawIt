using System;
using System.Activities.Presentation;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace DrawIt
{
    public class PropertyGrid: Grid
    {
        WorkflowDesigner _workflow;
        UIElement _grid;

        public PropertyGrid()
        {
            _workflow = new WorkflowDesigner();
            _grid = _workflow.PropertyInspectorView;
        }

        public UIElement Grid => _grid;
    }
}
