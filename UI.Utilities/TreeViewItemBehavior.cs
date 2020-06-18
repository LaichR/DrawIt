using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace Bluebottle.Base.Behaviors
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
          TreeViewItem item = depObj as TreeViewItem;
          if (item == null)
            return;

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
