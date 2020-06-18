using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Sketch.Types
{
    public class LinkedAvlTreeNode<T> where T : IComparable
    {
        T _data;
        LinkedAvlTreeNode<T> _left;
        LinkedAvlTreeNode<T> _right;
        LinkedAvlTreeNode<T> _previous;
        LinkedAvlTreeNode<T> _next;
        int _balance;

        public LinkedAvlTreeNode(T data)
        {
            _data = data;
        }

        public bool Contains(T data)
        {
            int comparison = Data.CompareTo( data );
            if(  comparison == 0 )
            {
                return true;
            }
            else if (comparison > 0 && _left != null)
            {
                return _left.Contains(data);
            }
            else if (comparison < 0 && _right != null)
            {
                return _right.Contains(data);
            }
            else
            {
                return false;
            }
        }

        public T Data
        {
            get
            {
                return _data;
            }
        }

        public LinkedAvlTreeNode<T> Next
        {
            get
            {
                return _next;
            }
        }

        public LinkedAvlTreeNode<T> Previous
        {
            get
            {
                return _previous;
            }
        }

        internal LinkedAvlTreeNode<T> Left
        {
            get { return _left; }
            set { _left = value; }
        }
        
        internal LinkedAvlTreeNode<T> Right
        {
            get { return _right; }
            set { _right = value; }
        }

        internal bool AddData( T data, out LinkedAvlTreeNode<T> node, out bool added)
        {
            int comparison = this.Data.CompareTo( data );
            if (comparison < 0)
            {
                return AddChildRight(data, out node, out added);
            }
            else if (comparison > 0)
            {
                return AddChildLeft(data, out node, out added);
            }
            else // same
            {
                node = this;
                added = false;
                return false;
            }
        }

        internal bool DeleteElement(T data, out LinkedAvlTreeNode<T> node, out bool heightDecreased)
        {
            bool deletedLeft = true;
            bool elementDeleted = false;
            node = this;
            int comparison = this.Data.CompareTo( data );
            if (comparison > 0 && _left != null)
            {
                elementDeleted = _left.DeleteElement(data, out _left, out heightDecreased);
            }
            else if (comparison < 0 && _right != null)
            {
                elementDeleted = _right.DeleteElement(data, out _right, out heightDecreased);
                deletedLeft = false;
            }
            else if (comparison == 0)
            {

                if (_left == null)
                {
                    UnlinkThis();
                    node = _right; heightDecreased = true;
                    return true;
                }
                else if (_right == null)
                {
                    UnlinkThis();
                    node = _left; heightDecreased = true;
                    return true;
                }
                else
                {
                    SwapDataWithNextSmaller(this, this._left);
                    elementDeleted = _left.DeleteElement(data, out _left, out heightDecreased);
                }
            }
            else
            {
                heightDecreased = false;
                return false;
            }

            if (heightDecreased && deletedLeft)
            {
                if (_balance == 0) // if an element is deleted on the left side, and the node was balanced the node is now taller on the right side
                {                  // but the overall height did not change
                    _balance =  1;
                    node = this;
                    heightDecreased = false;
                }
                else if (_balance > 0) // the right side was already taller; therefore we need to rearrange
                {
                    if (_right._balance == 0 ) // the left subtree will be higher by one element
                    {
                        _right._balance = -1;
                        _balance = 1;
                        RotateRight(out node);
                        heightDecreased = false;
                    
                    }
                    else if (_right._balance > 0) // the left subtree grows; therefore the overall size will shrink
                    {
                        _balance = 0;
                        _right._balance = 0;
                        RotateRight(out node);
                        heightDecreased = true;
                    }
                    else //(_right._balance < 0) // a double rotation is required
                    {

                        DoubleRotateRight(out node);
                        AdjustBalance(node);
                        heightDecreased = true;
                    }
                }
                else // the left subtree was higher; now left and right have the same hight
                {
                    _balance = 0;
                    node = this;
                    heightDecreased = true;
                }
            }
            else if (heightDecreased) // deleted right
            {
                if (_balance == 0) // if an element is deleted on the right side and the node was balanced, the node left subtree is now taller
                {
                    _balance = -1;
                    node = this;
                    heightDecreased = false;
                }
                else if (_balance < 0) //the left subtree was taller; we need to rearrange
                {
                    if (_left._balance == 0)
                    {
                        _left._balance = 1;
                        _balance = -1;
                        RotateLeft(out node);
                        heightDecreased = false;
                    }
                    else if (_left._balance < 0)
                    {
                        _balance = 0;
                        _left._balance = 0;
                        RotateLeft(out node);
                        heightDecreased  = true;
                    }
                    else // left._balance > 0 
                    {
                        DoubleRotateLeft(out node);
                        AdjustBalance(node);
                        heightDecreased = true;
                    }
                }
                else
                {
                    _balance = 0;
                    node = this;
                    heightDecreased = true;
                }
            }
            else // heigth of subtree did not change!
            {
                node = this;
                heightDecreased = false;
            }
            return elementDeleted;
        }

        internal LinkedAvlTreeNode<T> Find(T data, out LinkedAvlTreeNode<T> parent, out int comparison)
        {
            comparison = this.Data.CompareTo(data);
            parent = this;
            if (comparison < 0 && _right != null)
            {
                return _right.Find(data, out parent, out comparison);
            }
            if (comparison > 0 && _left != null)
            {
                return _left.Find(data, out parent, out comparison);
            }
            if (comparison == 0)
            {
                return this;
            }
            return null;
        }

        /// <summary>
        /// add a child whose value is smaller than the current data item
        /// </summary>
        /// <param name="child"></param>
        private bool AddChildLeft( T data, out LinkedAvlTreeNode<T> node, out bool added)
        { 
            bool subTreeHeightIncreased = false;
            if( _left == null ) // there is no smaller subtree!
            {
                LinkedAvlTreeNode<T> child = new LinkedAvlTreeNode<T>(data);
                _left = child;
                child._next = this;
                child._previous = this._previous;
                if (this._previous != null)
                {
                    this._previous._next = child;
                }
                this._previous = child;
                node = this;
                added = true;
                if( _balance == 0 ) // if the tree was balanced before, it is now longer on the left side
                {
                    _balance = -1;
                }
                else // if it was longer on the right side!
                {
                    System.Diagnostics.Debug.Assert(_balance == 1);
                    _balance = 0;
                }

                return (_balance != 0);
            }
            else
            {
                subTreeHeightIncreased = _left.AddData(data, out _left, out added);
            }
            if (!subTreeHeightIncreased)
            {
                node = this;
                return false;
            }
            // the subtree height has changed!
            if( _balance > 0 ) // the tree is balanced in a way that it has more children that are bigger
            {
                _balance = 0; // now the tree is balanced
                node = this;
                return false;
            }
            else if (_balance == 0)
            {
                _balance = -1; // the tree is now unbalanced but still ok
                node = this;
                return true;
            }
            else //we have to reestablish the order criterion of the tree
            {
                if (_left._balance == -1)  // single rotation
                {

                    _left._balance = 0;
                    _balance = 0;
                    RotateLeft(out node);
                    
                }
                else                        // double rotation
                {
                    DoubleRotateLeft(out node);
                    AdjustBalance(node);
                }
                return false;
            }
        }

        private bool AddChildRight( T data, out LinkedAvlTreeNode<T> node, out bool added)
        {
        
            bool subTreeHeightIncreased = false;
            if( _right == null )
            {
                LinkedAvlTreeNode<T> child = new LinkedAvlTreeNode<T>(data);
                _right = child;
                child._previous = this;
                child._next = this._next;
                if (this._next != null)
                {
                    this._next._previous = child;
                }
                this._next = child;
                added = true;
                if (_balance == 0)
                {
                    _balance = 1;
                }
                else
                {
                    System.Diagnostics.Debug.Assert(_balance == -1);
                    _balance = 0;
                }
                node = this;
                return (_balance != 0);
            }
            else
            {
                subTreeHeightIncreased = _right.AddData(data, out _right, out added);
            }
            if (!subTreeHeightIncreased)
            {
                node = this;
                return false;
            }
            // the subtree height has changed!
            if( _balance < 0 ) // the tree is balanced in a way that it has more children that are smaller
            {
                _balance = 0; // now the tree is balanced
                node = this;
                return false;
            }
            else if (_balance == 0)
            {
                _balance = 1; // the tree is now unbalanced but still ok
                node = this;
                return true;
            }
            else //we have to reestablish the order criterion of the tree
            {
                if (_right._balance == 1)  // single rotation
                {

                    RotateRight(out node);
                    node._balance = 0;
                    _balance = 0;
                }
                else                        // double rotation
                {
                    DoubleRotateRight(out node);
                    AdjustBalance(node);

                }
                return false;
            }
        }

        private void RotateLeft(out LinkedAvlTreeNode<T> node)
        {
            //System.Diagnostics.Trace.WriteLine("RotateLeft");
            LinkedAvlTreeNode<T> tmp = _left;
            _left = tmp.Right;
            tmp.Right = this;
            node = tmp;
            System.Diagnostics.Debug.Assert(node != null);
        }

        private void DoubleRotateLeft(out LinkedAvlTreeNode<T> node)
        {
            //System.Diagnostics.Trace.WriteLine("DoubleRotateLeft");
            LinkedAvlTreeNode<T> tmp = Left.Right;
            Left.Right = tmp.Left;
            tmp.Left = Left;
            Left = tmp.Right;
            tmp.Right = this;
            node = tmp;
            System.Diagnostics.Debug.Assert(node != null);
        }

        private void RotateRight(out LinkedAvlTreeNode<T> node)
        {
            //System.Diagnostics.Trace.WriteLine("RotateRight");
            LinkedAvlTreeNode<T> tmp = _right;
            _right = tmp.Left;
            tmp.Left = this;
            node = tmp;
            System.Diagnostics.Debug.Assert(node != null);
        }

        private void DoubleRotateRight(out LinkedAvlTreeNode<T> node)
        {
            
            //System.Diagnostics.Trace.WriteLine("DoubleRotateRight");
            LinkedAvlTreeNode<T> tmp = Right.Left;
            Right.Left = tmp.Right;
            tmp.Right = Right;
            Right = tmp.Left;
            tmp.Left = this;
            node = tmp;
            System.Diagnostics.Debug.Assert(node != null);
        }

        private void SwapDataWithNextSmaller(LinkedAvlTreeNode<T> parent, LinkedAvlTreeNode<T> node)
        {
            LinkedAvlTreeNode<T> bigger = node._right;
            while (bigger != null)
            {
                node = bigger;
                bigger = node._right;
            }
            T tmp = node._data;
            node._data = parent._data;
            parent._data = tmp;

            node.UnlinkThis();

        }

        private void AdjustBalance(LinkedAvlTreeNode<T> node)
        {
            int oldBalance = node._balance;
            node._balance = 0;
            if (oldBalance == 0)
            {
                node.Left._balance = 0;
                node.Right._balance = 0;
            }
            else if (oldBalance == 1)
            {
                node.Right._balance = 0;
                node.Left._balance = -1;
            }
            else
            {
                node.Right._balance = 1;
                node.Left._balance = 0;
            }
        }

        private void UnlinkThis()
        {
            if (this._previous != null)
            {
                System.Diagnostics.Debug.Assert(Previous.Next == this);
                this._previous._next = this._next;
            }
            if (this._next != null)
            {
                System.Diagnostics.Debug.Assert(Next.Previous == this);
                this._next._previous = this._previous;
            }
            this._next = null;
            this._previous = null;
        }
    }
}
