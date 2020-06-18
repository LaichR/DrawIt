using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace Bluebottle.Base.Controls
{
    public class AutoscrollListView:ListView
    {
        protected override void OnItemsChanged(NotifyCollectionChangedEventArgs e)
        {
            if( e != null && e.NewItems != null && e.NewItems.Count > 0)
            
                this.ScrollIntoView(e.NewItems[e.NewItems.Count - 1]);

            base.OnItemsChanged(e);
        }



        protected override void OnInitialized(EventArgs e)
        {
            base.OnInitialized(e);
            //CorrectColumnWidths();
            this.SizeChanged += AutoscrollListView_SizeChanged;
        }

        private void AutoscrollListView_SizeChanged(object sender, System.Windows.SizeChangedEventArgs e)
        {
            CorrectColumnWidths();
        }

        void CorrectColumnWidths()
        {
            double remainingSpace = this.ActualWidth;
            int lastColIndex = (View as GridView).Columns.Count - 1;
            if (remainingSpace > 0)
            {
                for (int i = 0; i < lastColIndex; i++)
                        remainingSpace -= (View as GridView).Columns[i].ActualWidth;

                //Leave 15 px free for scrollbar
                remainingSpace -= 15;
                remainingSpace = Math.Max(350, remainingSpace);
                (View as GridView).Columns[lastColIndex].Width = remainingSpace;
            }
        }


    }
}
