using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using UI.Utilities.Interfaces;

namespace UI.Utilities.Controls
{
    public delegate bool SearchDelegate( object item, string text);

    public class TreeView: System.Windows.Controls.TreeView
    {
        public static readonly char DefaultPathDelimiter = '/';
        char _pathDelimiter = DefaultPathDelimiter;

        class ItemLocator
        {
            readonly ItemsControl _container;
            

            public ItemLocator( ItemsControl container, object obj )
            {
                _container = container;
                
                if (_container.ItemContainerGenerator.Status != GeneratorStatus.ContainersGenerated)
                {

                    IItemContainerGenerator generator = _container.ItemContainerGenerator as IItemContainerGenerator;
                    var position = generator.GeneratorPositionFromIndex(0);
                    using (generator.StartAt(position, GeneratorDirection.Forward, true))
                    {
                        
                        foreach (var item in _container.Items)
                        {
                            var dp = generator.GenerateNext();
                            generator.PrepareItemContainer(dp);    
                        }
                    }
                }
                ChildIndex = 0;
                if( obj != null )
                {
                    ChildIndex = container.Items.IndexOf(obj);
                }
            }
            
            public ItemCollection Items
            {
                get
                {
                    return _container.Items;
                }
            }

            public ItemContainerGenerator ItemContainerGenerator
            {
                get 
                {
                    return _container.ItemContainerGenerator;
                }
            }

            public ItemsControl GetCurrentChildContainer()
            {
                var obj = _container.Items[ChildIndex];
                return _container.ItemContainerGenerator.ContainerFromItem(obj) as ItemsControl;
            }

            public object GetCurrentItem()
            {
                if (ChildIndex >= 0 && ChildIndex < _container.Items.Count)
                {
                    return _container.Items[ChildIndex];
                }
                return null;
            }

            public int ChildIndex
            {
                get;
                set;
            }

            public ItemsControl Container
            {
                get
                {
                    return _container;
                }
            }
        }

        public static readonly DependencyProperty InitialsSearchProperty =
            DependencyProperty.Register("InitialsSearch", typeof(SearchDelegate), 
            typeof(UI.Utilities.Controls.TreeView),
            new PropertyMetadata(OnInitialsSearchChanged));

        public static readonly DependencyProperty SearchDepth =
            DependencyProperty.Register("SearchDepth", typeof(int),
            typeof(UI.Utilities.Controls.TreeView));

        public static readonly DependencyProperty SelectedPathProperty =
            DependencyProperty.Register("SelectedPath", typeof(string),
            typeof(UI.Utilities.Controls.TreeView),
            new PropertyMetadata(OnSelectedPathChanged));

        
        List<UI.Utilities.Interfaces.IHierarchicalNode> _nodePath;

        /// <summary>
        /// Guard to avoid triggering a stack-overflow via "OnSelectedItemChanged" while
        /// expandind the tree to the selected path (from a binded property) or the OnSelectedItemChanged event.
        /// </summary>
        bool _selectedPathIsChanging = false; 

        /// <summary>
        /// Guard to avoid that a mouse-click on the tree causes a Expand action on it, having as result a movement of the scroll location
        /// </summary>
        bool _selectedItemIsChanging = false;

        public TreeView() : base()         
        {
            _nodePath = new List<Interfaces.IHierarchicalNode>();
            MaxSearchDepth = 2;
            //_selectionIsChanging = false;
            //AddHandler(TreeViewItem.SelectedEvent, new RoutedEventHandler(TreeView.OnTreeViewItemSelected), true);
            
        }

        protected override void OnSelectedItemChanged(RoutedPropertyChangedEventArgs<object> e)
        {
            //var node = e.NewValue as IHierarchicalNode;
            //var oldNode = e.OldValue as IHierarchicalNode;
            base.OnSelectedItemChanged(e);
            if (e.NewValue is IHierarchicalNode node)
            {
                _nodePath.Clear();
                _nodePath = new List<Interfaces.IHierarchicalNode>();
                do
                {
                    _nodePath.Add(node);
                    node = node.Parent;
                } while (node != null);
                _nodePath.Reverse();

                try
                {
                    _selectedItemIsChanging = true;
                    SelectedPath = string.Join(new string(PathDelimiter, 1), _nodePath.Select<Interfaces.IHierarchicalNode, string>((x) => x.Label).ToArray<string>());
                }
                finally
                {
                    _selectedItemIsChanging = false;
                }
            }
            
        }

        protected override void OnTextInput(System.Windows.Input.TextCompositionEventArgs e)
        {
            base.OnTextInput(e);

            var searchDelegate = InitialsSearch;
            if (searchDelegate != null)
            {
                ExpandAndSelectItem((x) => searchDelegate(x, e.Text));
            }

        }

        protected override void OnKeyDown(System.Windows.Input.KeyEventArgs e)
        {
            if( e.Key == System.Windows.Input.Key.F2)
            {
                var node = _nodePath.Last();
                if( node != null )
                {
                    node.AllowEdit = true;
                }
            }
            base.OnKeyDown(e);
        }

        public SearchDelegate InitialsSearch
        {
            get
            {
                return GetValue(InitialsSearchProperty) as SearchDelegate;
            }
            set
            {
                SetValue(InitialsSearchProperty, value);
            }
        }

        public int MaxSearchDepth
        {
            get
            {
                return (int)GetValue(SearchDepth);
            }
            set
            {
                SetValue(SearchDepth, value);
            }
        }

        public string SelectedPath
        {
            get
            {
                return (string)GetValue(SelectedPathProperty);
            }

            set
            {
                SetValue(SelectedPathProperty, value);
            }
        }
        
        public char PathDelimiter
        {
            get { return _pathDelimiter; }
            set
            {
                _pathDelimiter = value;
            }
        }
        
        public void ExpandAll()
        {
            ExpandChildren(this);
        }

        static void ExpandChildren(System.Windows.Controls.ItemsControl container)
        {
            foreach (Object item in container.Items)
            {
                
                if(container.ItemContainerGenerator.ContainerFromItem(item) is TreeViewItem currentContainer &&
                   currentContainer.Items.Count > 0)
                {
                    //expand the item
                    currentContainer.IsExpanded = true;

                    //if the item's children are not generated, they must be expanded
                    if (currentContainer.ItemContainerGenerator.Status != GeneratorStatus.ContainersGenerated)
                    {
                        //store the event handler in a variable so we can remove it (in the handler itself)
                        EventHandler eh = null;
                        eh = new EventHandler(delegate
                        {
                            //once the children have been generated, expand those children's children then remove the event handler
                            if (currentContainer.ItemContainerGenerator.Status == GeneratorStatus.ContainersGenerated)
                            {
                                ExpandChildren(currentContainer);
                                currentContainer.ItemContainerGenerator.StatusChanged -= eh;
                            }
                        });

                        currentContainer.ItemContainerGenerator.StatusChanged += eh;
                    }
                    else //otherwise the children have already been generated, so we can now expand those children
                    {
                        ExpandChildren(currentContainer);
                    }
                }
            }
        }

        public void SelectPath(string path)
        {
            this.UpdateLayout();
            var pathList = path.Split(PathDelimiter);
            ItemsControl container = this;
            object item = null;
            TreeViewItem selected;
            _selectedPathIsChanging = true;

            try
            {
                foreach (var p in pathList)
                {
                    if (container == null) return;
                    var itemLocator = new ItemLocator(container, item);

                    for (int i = 0; i < itemLocator.Items.Count; ++i)
                    {
                        itemLocator.ChildIndex = i;
                        if (itemLocator is Interfaces.IHierarchicalNode node)
                        {
                            if (node.Label == p)
                            {
                                container = itemLocator.GetCurrentChildContainer();

                                selected = container as TreeViewItem;
                                if (selected != null)
                                {

                                    if (selected.Items.Count > 0)
                                    {
                                        item = selected.Items[0];
                                    }
                                    selected.IsExpanded = true;
                                    selected.IsSelected = true;
                                    selected.Focus();
                                    selected.BringIntoView();
                                    this.UpdateLayout();
                                }
                                break;
                            }
                            else
                            {
                                if (container is TreeViewItem notSelected)
                                {
                                    notSelected.IsExpanded = false;
                                }
                            }
                        }
                    }
                }
            }
            finally
            {
                _selectedPathIsChanging = false;
            }            
        }

        bool ExpandAndSelectItem( Func<object, bool> objectFind )
        {
            //check all items at the current level
            System.Windows.Controls.ItemsControl parentControl = this;
            Stack<ItemLocator> searchStack = new Stack<ItemLocator>();
            
            int i = 0;

            SortedSet<int> searched = new SortedSet<int>();
            // if no item was selected start with the first node
            bool skipSearch = true;
            if (_nodePath.Count == 0)
            {
                var locator = new ItemLocator(parentControl, null);
                searchStack.Push(locator);
                skipSearch = false;
            }
            else
            {
                for (; i < _nodePath.Count; ++i)
                {
                    var n = _nodePath[i];
                    var locator = new ItemLocator(parentControl, n);

                    searchStack.Push(locator);
                    parentControl = locator.GetCurrentChildContainer();

                    if (parentControl == null) break;
                }
            }
            
            int initialDepth = searchStack.Count();
            if (initialDepth == 0) return false;

            // skip first search
            
            do
            {

                var locator = searchStack.Peek();    

                var itemCollection = locator.Items;
                var nrOfItems = itemCollection.Count;
                i = locator.ChildIndex;
                if (i >= nrOfItems && searchStack.Count == 1)
                {
                    i = 0;
                }
                while( i < nrOfItems)
                { 
                    
                    locator.ChildIndex = i;
                    
                    if (locator.GetCurrentChildContainer() is TreeViewItem currentContainer)
                    {
                        currentContainer.BringIntoView();

                        //if the data item matches the item we want to select, set the corresponding
                        //TreeViewItem IsSelected to true
                        
                        if (!skipSearch && objectFind(locator.GetCurrentItem()))
                        {
                                                     
                            currentContainer.Focus();
                            currentContainer.IsSelected = true;   
                            
                            // set the new start index, where the search has to continue

                            //the item was found
                            return true;
                        }
                        skipSearch = false;
                        var hash = currentContainer.GetHashCode();
                        // visit the children!
                        if (currentContainer.Items.Count > 0 && (MaxSearchDepth > (searchStack.Count()-initialDepth)) &&
                            (!searched.Contains(hash)))
                        {
                            searched.Add(hash);
                            currentContainer.IsExpanded = true;
                            locator = new ItemLocator(currentContainer, null);
                            
                            
                            searchStack.Push(locator);   
                            itemCollection = locator.Items;
                            nrOfItems = itemCollection.Count;
                            i = 0;
                        }
                        else
                        {
                            locator.ChildIndex += 1;
                            i = locator.ChildIndex;
                        }
                    }
                }

                // do not remove last item
                if ( searchStack.Count > 1)
                {
                    var oldLocator = searchStack.Pop();
                    
                    if (oldLocator.Container is TreeViewItem oldContainer)
                    {
                        oldContainer.IsExpanded = false;
                    }
                    locator = searchStack.Peek();
                    locator.ChildIndex += 1;
                }
                else // searchStackCount == 1;
                {
                    searchStack.Pop();
                }


            } while (searchStack.Count > 0);
            return false;

            ////if we get to this point, the selected item was not found at the current level, so we must check the children
            //for (int i = 0; i < itemCollection.Count; ++i)
            //{
            //    var item = itemCollection[i];
            //    System.Windows.Controls.TreeViewItem currentContainer = container.ItemContainerGenerator.ContainerFromItem(item) as System.Windows.Controls.TreeViewItem;

            //    //if children exist
            //    if (currentContainer != null && currentContainer.Items.Count > 0)
            //    {
            //        //keep track of if the TreeViewItem was expanded or not
            //        bool wasExpanded = currentContainer.IsExpanded;

            //        //expand the current TreeViewItem so we can check its child TreeViewItems
            //        currentContainer.IsExpanded = true;

            //        //if the TreeViewItem child containers have not been generated, we must listen to
            //        //the StatusChanged event until they are
            //        if (currentContainer.ItemContainerGenerator.Status != GeneratorStatus.ContainersGenerated)
            //        {
            //            //store the event handler in a variable so we can remove it (in the handler itself)
            //            EventHandler eh = null;
            //            eh = new EventHandler(delegate
            //            {
            //                if (currentContainer.ItemContainerGenerator.Status == GeneratorStatus.ContainersGenerated)
            //                {
            //                    _containerStack = ExpandAndSelectItem(currentContainer, objectFind);
            //                    if ( _containerStack == null)
            //                    {
            //                        //The assumption is that code executing in this EventHandler is the result of the parent not
            //                        //being expanded since the containers were not generated.
            //                        //since the itemToSelect was not found in the children, collapse the parent since it was previously collapsed
            //                        currentContainer.IsExpanded = false;
            //                    }
                                
            //                    //remove the StatusChanged event handler since we just handled it (we only needed it once)
            //                    currentContainer.ItemContainerGenerator.StatusChanged -= eh;
            //                }
            //            });
            //            currentContainer.ItemContainerGenerator.StatusChanged += eh;
            //        }
            //        else //otherwise the containers have been generated, so look for item to select in the children
            //        {
            //            _containerStack = ExpandAndSelectItem(currentContainer, objectFind);
            //            if( _containerStack == null)
            //            {
            //                //restore the current TreeViewItem's expanded state
            //                currentContainer.IsExpanded = wasExpanded;
            //            }
            //            else //otherwise the node was found and selected, so return true
            //            {
            //                _startIndex = i;
            //                return _containerStack;
            //            }
            //        }
            //    }
            //}

            ////no item was found
            //_startIndex = 0;
            //_containerStack = null;
            //return null;
        }

        //void treeViewItem_Selected(object sender, RoutedEventArgs e)
        //{
        //    Console.WriteLine("tree view items is selected");
        //}
       

        private static void OnInitialsSearchChanged(DependencyObject source,
        DependencyPropertyChangedEventArgs e)
        {}

        private static void OnSelectedPathChanged(DependencyObject source,
        DependencyPropertyChangedEventArgs e)
        {
            if (e.NewValue != null && e.NewValue != e.OldValue )
            {
                if( source is TreeView target)
                {
                    if(!target._selectedPathIsChanging &&
                       !target._selectedItemIsChanging)
                    {
                        target.SelectPath(e.NewValue.ToString());
                    }
                }
            }
        }

        //private static void OnTreeViewItemSelected(object sender, RoutedEventArgs args)
        //{
        //    if (Keyboard.Modifiers.HasFlag(ModifierKeys.Control))
        //    {
        //        args.Handled = true;
        //    }
        //    else
        //    {
        //        args.Handled = false;
        //    }
        //}
    }
}
