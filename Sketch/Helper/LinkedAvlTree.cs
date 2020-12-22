using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Sketch.Helper
{
    public class LinkedAvlTree<T> where T: IComparable
    {
        readonly object _synchRoot = new object();
        int _count = 0;
        LinkedAvlTreeNode<T> _root = null;

        public void Add(T data)
        {
            lock (_synchRoot)
            {
                bool dataAdded = true;
                if (_count == 0)
                {
                    _root = new LinkedAvlTreeNode<T>(data);
                }
                else
                {
                    _root.AddData(data, out _root, out dataAdded);
                }
                if (dataAdded)
                {
                    _count++;
                }
            }
        }

        public void Delete(T data)
        {
            lock (_synchRoot)
            {
                int localCount = _count;
                if (_count > 0)
                {

                    bool heightChanged = false;
                    //LinkedAvlTreeNode<T> newRoot;
                    bool elementDeleted = _root.DeleteElement(data, out _root, out heightChanged);
                    if (elementDeleted)
                    {
                        _count--;
                    }
                }
            }
        }

        public LinkedAvlTreeNode<T> Find(T data)
        {
            LinkedAvlTreeNode<T> parent;
            int comparison = -1;
            LinkedAvlTreeNode<T> result = null;
            if (_root != null)
            {
                result = _root.Find(data, out parent, out comparison);
            }
            if (comparison != 0)
            {
                throw new KeyNotFoundException();
            }
            return result;
        }

        public LinkedAvlTreeNode<T> LowerBound(T data)
        {
            if (_root == null) return null;
            
            LinkedAvlTreeNode<T> parent;
            int comparison;
            LinkedAvlTreeNode<T> result = _root.Find(data, out parent, out comparison);
            if (comparison == 0)
            {
                return result;
            }
            else if (comparison > 0 && parent.Previous != null)
            {
                return parent.Previous;
            }
            else
            {
                return parent;
            }
        }

        public LinkedAvlTreeNode<T> UpperBound(T data)
        {
            if (_root == null) return null;
            LinkedAvlTreeNode<T> parent;
            int comparison;
            LinkedAvlTreeNode<T> result = _root.Find(data, out parent, out comparison);
            if (comparison == 0)
            {
                return result;
            }
            else if (comparison < 0 && parent.Next != null)
            {
                return parent.Next;
            }
            else
            {
                return parent;
            }
        }

        public bool Contains(T data)
        {
            if (_count > 0)
            {
                return _root.Contains(data);
            }
            return false;
        }

        public T[] ToArray()
        {
            List<T> elems = new List<T>();
            Traverse(elems, _root);
            return elems.ToArray<T>();
        }

        private void Traverse( List<T> list , LinkedAvlTreeNode<T> node )
        {
            if (node == null)
            {
                return;
            }
            Traverse(list, node.Left);
            list.Add(node.Data);
            Traverse(list, node.Right);
        }

        public void PrintTree()
        {
            printNode(0, _root);
        }

        private void printNode(int depth, LinkedAvlTreeNode<T> node)
        {
            if (node == null)
            {
                Console.WriteLine("null");
                return;
            }
            PrintIdent(depth); Console.WriteLine(string.Format("d: {0}", node.Data));
            PrintIdent(depth); Console.Write("l:"); printNode(depth + 1, node.Left);
            PrintIdent(depth); Console.Write("r:"); printNode(depth + 1, node.Right);
        }

        public int Count
        {
            get
            {
                return _count;
            }
        }

        private void PrintIdent(int count)
        {
            while (count > 0)
            {
                Console.Write("\t");
                count--;
            }
        }
    }
}
