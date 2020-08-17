using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace UI.Utilities.Behaviors
{
    public static class TreeViewItemBehavior
    {
        public static bool GetBringIntoViewWhenSelected(TreeViewItem treeViewItem)
        {
          return (bool)treeViewItem.GetValue(BringIntoViewWhenSelectedProperty);
        }

        public static void SetBringIntoViewWhenSelected(TreeViewItem treeViewItem, bool value)
        {
          treeViewItem.SetValue(BringIntoViewWhenSelectedProperty, value);
        }

        public static readonly DependencyProperty BringIntoViewWhenSelectedProperty =
            DependencyProperty.RegisterAttached("BringIntoViewWhenSelected", typeof(bool),
            typeof(TreeViewItemBehavior), new UIPropertyMetadata(false, OnBringIntoViewWhenSelectedChanged));

        static void OnBringIntoViewWhenSelectedChanged(DependencyObject depObj, DependencyPropertyChangedEventArgs e)
        {
            if (!(depObj is TreeViewItem item)) return;
     

          if (e.NewValue is bool == false)
            return;

          if ((bool)e.NewValue)
          {
              var parent = item.Parent as TreeViewItem;
              while( parent!= null)
              {
                  parent.IsExpanded = true;
                  parent = parent.Parent as TreeViewItem;
              }
              item.BringIntoView();
          }
        }
    }
}
