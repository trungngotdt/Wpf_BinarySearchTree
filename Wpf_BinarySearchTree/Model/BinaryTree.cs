using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Wpf_BinarySearchTree.Model
{
    public class Node<T> : IComparable
        where T : IComparable
    {
        private T data;
        private Node<T> left;
        private Node<T> right;

        private double x;
        private double y;
        public T Data { get => data; set => data = value; }
        public Node<T> Left { get => left; set => left = value; }
        public Node<T> Right { get => right; set => right = value; }
        public double X { get => x; set => x = value; }
        public double Y { get => y; set => y = value; }


        public Node()
        {
            object dbNull = null;
            Data = Data is DBNull ? (T)dbNull : default(T);
            Left = null;// Letf is DBNull ? (T)dbNull : default(T);
            Right = null;
        }


        public Node(T data, Node<T> nodeChild, Node<T> nodeChild2)
        {
            this.Data = data;
            if (nodeChild.CompareTo(this) == 0)
            {
                this.Left = nodeChild;
                this.Right = null;
            }
            else if (nodeChild < this)
            {
                this.Left = nodeChild;
                this.Right = nodeChild2;
            }
            else if (nodeChild > this)
            {
                this.Left = nodeChild2;
                this.Right = nodeChild;
            }
        }
        public Node(T data,double x,double y)
        {
            this.X = x;
            this.Y = y;
            this.Data = data;
            this.Left = null;// Letf is DBNull ? (T)dbNull : default(T);
            this.Right = null;
        }

        public Node(T data)
        { 
            this.Data = data;
            this.Left = null;// Letf is DBNull ? (T)dbNull : default(T);
            this.Right = null;
        }

        public Node(Node<T> node)
        {
            this.Data = node.Data;
            this.Left = node.Left;
            this.Right = node.Right;
        }

        /// <summary>
        /// Find height of node
        /// </summary>
        /// <param name="node"></param>
        /// <returns></returns>
        public int Height(Node<T> node)
        {
            if (node == null) return -1;
            var leftH = Height(node.Left);
            var rightH = Height(node.Right);
            return Math.Max(leftH, rightH) + 1;
        }

        /// <summary>
        /// Find height of node root
        /// </summary>
        /// <returns></returns>
        public int Height()
        {
            return Height(this);
        }


        /// <summary>
        /// Adds the elements of the specified collection to the BST
        /// </summary>
        /// <param name="node"></param>
        public void AddRange(Node<T>[] node)
        {
            foreach (var item in node)
            {
                Insert(item);
            }
        }


        /// <summary>
        /// Find inorder predecessor of a node
        /// </summary>
        /// <returns></returns>
        public object Predecessor()
        {
            return this.Left.GetMax();
            //throw new NotImplementedException();
        }

        /// <summary>
        /// Find inorder successor of a node
        /// </summary>
        /// <returns></returns>
        public object Successor()
        {
            return this.Right.GetMin();
            //throw new NotImplementedException();
        }

        /// <summary>
        ///Return a minimum value in BST 
        /// </summary>
        /// <returns></returns>
        public T GetMin()
        {
            var temp = this;
            if (this.Data == null)
            {
                return this.Data;
            }
            while (true)
            {
                if (temp.Left == null)
                {
                    return temp.Data;
                }
                else if (temp.Left != null)
                {
                    temp = temp.Left;
                }
            }
        }

        /// <summary>
        /// Return a maximum value in BST
        /// </summary>
        /// <returns></returns>
        public T GetMax()
        {
            var temp = this;
            if (this.Data == null)
            {
                return this.Data;
            }
            while (true)
            {
                if (temp.Right == null)
                {
                    return temp.Data;
                }
                else if (temp.Right != null)
                {
                    temp = temp.Right;
                }
            }
        }

        /// <summary>
        /// Remove a element in BST
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        public bool Remove(Node<T> item)
        {
            Node<T> node = this;
            if (item == null)
            {
                return false;
            }
            while (node != null)
            {
                if (node.CompareTo(item) == 0)
                {
                    if (node.Right != null && node.Left == null || node.Right == null && node.Left!= null)//one child
                    {
                        var parent = FindParent(node);
                        if (node.Right != null)
                        {
                            parent.Item1.Left = parent.Item2 == -1 ? parent.Item1.Left.Right : parent.Item1.Left;
                            parent.Item1.Right = parent.Item2 == 1 ? parent.Item1.Right.Right : parent.Item1.Right;
                        }
                        else if (node.Left != null)
                        {
                            parent.Item1.Left = parent.Item2 == -1 ? parent.Item1.Left.Left : parent.Item1.Left;
                            parent.Item1.Right = parent.Item2 == 1 ? parent.Item1.Right.Left : parent.Item1.Right;
                        }
                        return true;
                    }
                    else if (node.Right == null && node.Left == null)//no child
                    {
                        var parent = FindParent(node);
                        parent.Item1.Left = parent.Item2 == -1 ? null : parent.Item1.Left;
                        parent.Item1.Right = parent.Item2 == 1 ? null : parent.Item1.Right;
                        return true;
                    }
                    else//two child
                    {
                        var suc = (T)node.Successor();
                        var nodeFind = node.FindNode(new Node<T>(suc));

                        var parent = FindParent(new Node<T>(suc));

                        if (parent.Item1.Data.Equals(item.Data))
                        {
                            parent.Item1.Right = nodeFind.Right;
                            node.Data = suc;
                            return true;
                        }
                        if (nodeFind.Right != null)
                        {
                            parent.Item1.Right = nodeFind.Right;
                        }
                        else
                        {
                            parent.Item1.Left = null;
                        }
                        node.Data = suc;
                        return true;
                        /*
                        var suc = (T)node.Successor();
                        var nodeFind = node.FindNode(new Node<T>(suc));

                        var parent = FindParent(new Node<T>(suc));
                        node.Data = suc;
                        if (nodeFind.Right != null)
                        {
                            parent.Item1.Right = nodeFind.Right;
                        }
                        else
                        {
                            parent.Item1.Letf = null;
                        }
                        return true;*/
                    }
                }
                if (node < item)
                {
                    node = node.Right;
                }
                else
                {
                    node = node.Left;
                }
            }
            return false;
            //throw new NotImplementedException();
        }

        /// <summary>
        /// Searches for an parent of element that matches the conditions defined by the specified
        /// item1 is RightLeaf (1) or LeftLeaf (-1) 
        /// </summary>
        /// <param name="node"></param>
        /// <returns ></returns>
        public Tuple<Node<T>, int> FindParent(Node<T> node)
        {
            int check = 0;
            if (node == null)
            {
                return null;
            }
            Node<T> temp = this;
            Node<T> parent = null;
            while (temp != null)
            {
                if (temp.CompareTo(node) == 0)
                {
                    return new Tuple<Node<T>, int>(parent, check);// temp;
                }
                if (temp > node)
                {
                    parent = temp;
                    check = -1;
                    temp = temp.Left;
                }
                else
                {
                    parent = temp;
                    check = 1;
                    temp = temp.Right;

                }
            }
            return null;
        }

        
        /// <summary>
        /// Searches for an element that matches the conditions defined by the specified
        /// </summary>
        /// <param name="node"></param>
        /// <returns></returns>
        public Node<T> FindNode(Node<T> node)
        {
            if (node == null)
            {
                return null;
            }
            Node<T> temp = this;
            while (temp != null)
            {
                if (temp.CompareTo(node) == 0)
                {
                    return temp;
                }
                if (temp > node)
                {
                    temp = temp.Left;
                }
                else
                {
                    temp = temp.Right;
                }
            }
            return null;
            //throw new NotImplementedException();
        }
        
        /// <summary>
        /// Adds an object to the BST
        /// </summary>
        /// <param name="item"></param>
        public void Insert(Node<T> item)
        {
            Node<T> temp = this;
            if (item == null)
            {
                return;
            }
            if (this.Data == null)
            {
                this.Data = item.Data;
                this.Left = item.Left;
                this.Right = item.Right;
            }
            //Node<T> temp = root;
            while (item != null)
            {
                if (temp.Data.Equals(item.Data))
                {
                    break;
                }
                if (item < temp)
                {
                    if (temp.Left != null)
                    {
                        temp = temp.Left;
                    }
                    else
                    {
                        temp.Left = item;
                        break;
                    }
                }
                else if (item > temp)
                {
                    if (temp.Right != null)
                    {
                        temp = temp.Right;
                    }
                    else
                    {
                        temp.Right = item;
                        break;
                    }
                }
            }
        }

        /// <summary>
        /// Determines whether an element is in the BST
        /// </summary>
        /// <param name="node"></param>
        /// <returns></returns>
        public bool Contains(Node<T> node)
        {
            Node<T> temp = this;
            if (node == null)
            {
                return false;
            }
            while (temp != null)
            {
                if (temp.CompareTo(node) == 0)
                {
                    return true;
                }
                if (temp > node)
                {
                    temp = temp.Left;
                }
                else
                {
                    temp = temp.Right;
                }
            }
            return false;
        }

        public int CompareTo(object obj)
        {
            try
            {
                Node<T> node = obj as Node<T>;
                return this.Data.CompareTo(node.Data);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public static bool operator <(Node<T> node, Node<T> node2)
        {
            return node.Data.CompareTo(node2.Data) < 0;
        }
        public static bool operator >(Node<T> node, Node<T> node2)
        {
            return node.Data.CompareTo(node2.Data) > 0;
        }
    }
}
