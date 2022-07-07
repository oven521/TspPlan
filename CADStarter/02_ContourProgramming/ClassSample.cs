using System;
using System.Collections.Generic;
using System.Collections;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataStructure {
    /// <summary>
    /// 描述:二叉树操作类
    /// 作者:鲁宁
    /// 时间:2013/9/5 11:36:43
    /// </summary>
    public class BinTreeBLL {
        //按层遍历的存储空间长度
        public static int Length { get; set; }

        /// <summary>
        /// 生成根节点
        /// </summary>
        /// <returns></returns>
        public static BinTree<string> CreateRoot() {
            BinTree<string> root = new BinTree<string>();
            Console.WriteLine("请输入根节点，以便生成树");
            root.Data = Console.ReadLine();
            Console.WriteLine("根节点生成成功");
            return root;
        }

        /// <summary>
        /// 插入节点
        /// </summary>
        /// <param name="tree">待操作的二叉树</param>
        /// <returns>插入节点后的二叉树</returns>
        public static BinTree<string> Insert(BinTree<string> tree) {
            while (true) {
                //创建要插入的节点
                BinTree<string> node = new BinTree<string>();
                Console.WriteLine("请输入待插入节点的数据");
                node.Data = Console.ReadLine();

                //获取父节点数据
                Console.WriteLine("请输入待查找的父节点数据");
                var parentNodeData = Console.ReadLine();

                //确定插入方向
                Console.WriteLine("请确定要插入到父节点的：1 左侧, 2 右侧");
                Direction direction = (Direction)Enum.Parse(typeof(Direction), Console.ReadLine());

                //插入节点
                tree = InsertNode(tree, node, parentNodeData, direction);

                //todo:没有找到父节点没有提示???
                if (tree == null) {
                    Console.WriteLine("未找到父节点，请重新输入!");
                    continue;
                }
                Console.WriteLine("插入成功，是否继续? 1 继续，2 退出");

                if (int.Parse(Console.ReadLine()) == 1) continue;
                else break; //退出循环
            }
            return tree;
        }

        public static BinTree<T> InsertNode<T>(BinTree<T> tree, BinTree<T> node, T parentNodeData, Direction direction) {
            if (tree == null) return null;

            //找到父节点
            if (tree.Data.Equals(parentNodeData)) {
                switch (direction) {
                    case Direction.Left:
                        if (tree.Left != null) throw new Exception("左节点已存在，不能插入!");
                        else tree.Left = node;
                        break;
                    case Direction.Right:
                        if (tree.Right != null) throw new Exception("右节点已存在，不能插入!");
                        else tree.Right = node;
                        break;
                }
            }

            //向左子树查找父节点(递归)
            InsertNode(tree.Left, node, parentNodeData, direction);

            //向右子树查找父节点(递归)
            InsertNode(tree.Right, node, parentNodeData, direction);

            return tree;
        }

        /// <summary>
        /// 查找节点
        /// </summary>
        /// <typeparam name="T">节点数据类型</typeparam>
        /// <param name="tree">二叉树</param>
        /// <param name="data">要查找的节点数据</param>
        /// <returns>要查找的节点</returns>
        public static bool GetNode<T>(BinTree<T> tree, T data) {
            if (tree == null) return false;

            //查找成功
            if (tree.Data.Equals(data)) return true;

            //递归查找
            return GetNode(tree.Left, data); //这里有问题？？？
        }

        /// <summary>
        /// 获取二叉树的深度
        /// 思路：分别递归左子树和右子树，然后得出较大的一个即为二叉树的深度
        /// </summary>
        /// <typeparam name="T">二叉树的数据类型</typeparam>
        /// <param name="tree">待操作的二叉树</param>
        /// <returns>二叉树的深度</returns>
        public static int GetLength<T>(BinTree<T> tree) {
            if (tree == null) return 0;
            int leftLength, rightLength;

            //递归左子树的深度
            leftLength = GetLength(tree.Left);

            //递归右子树的深度
            rightLength = GetLength(tree.Right);

            if (leftLength > rightLength) return leftLength + 1;
            else return rightLength + 1;
        }

        /// <summary>
        /// 先序遍历
        /// 思路：输出根节点--->遍历左子树--->遍历右子树
        /// </summary>
        /// <typeparam name="T">二叉树的数据类型</typeparam>
        /// <param name="tree">待操作的二叉树</param>
        public static void Traversal_DLR<T>(BinTree<T> tree) {
            if (tree == null) return;

            //输出节点的值
            Console.Write(tree.Data + "\t");

            //递归遍历左子树
            Traversal_DLR(tree.Left);

            //递归遍历右子树
            Traversal_DLR(tree.Right);
        }

        /// <summary>
        /// 中序遍历
        /// 思路：遍历左子树--->输出根节点--->遍历右子树
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="tree"></param>
        public static void Traversal_LDR<T>(BinTree<T> tree) {
            if (tree == null) return;

            //遍历左子树
            Traversal_LDR(tree.Left);

            //输出节点的值
            Console.Write(tree.Data + "\t");

            //遍历右子树
            Traversal_LDR(tree.Right);
        }

        /// <summary>
        /// 后序遍历
        /// 思路：遍历左子树--->遍历右子树--->输出根节点
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="tree"></param>
        public static void Traversal_LRD<T>(BinTree<T> tree) {
            if (tree == null) return;

            //遍历左子树
            Traversal_LRD(tree.Left);

            //遍历右子树
            Traversal_LRD(tree.Right);

            //输出节点的值
            Console.Write(tree.Data + "\t");
        }

        /// <summary>
        /// 按层遍历
        /// 思路：从上到下，从左到右遍历节点
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="tree"></param>
        public static void Traversal_Level<T>(BinTree<T> tree) {
            if (tree == null) return;
            int head = 0;
            int tail = 0;

            //申请保存空间
            BinTree<T>[] treeList = new BinTree<T>[Length];

            //将当前二叉树保存到数组中
            treeList[tail] = tree;

            //计算tail的位置
            tail = (tail + 1) % Length; //除留余数法

            while (head != tail) {
                var tempNode = treeList[head];

                //计算head的位置
                head = (head + 1) % Length;

                //输出节点的值
                Console.Write(tempNode.Data + "\t");

                //如果左子树不为空，则将左子树保存到数组的tail位置
                if (tempNode.Left != null) {
                    treeList[tail] = tempNode.Left;

                    //重新计算tail的位置
                    tail = (tail + 1) % Length;
                }

                //如果右子树不为空，则将右子树保存到数组的tail位置
                if (tempNode.Right != null) {
                    treeList[tail] = tempNode.Right;

                    //重新计算tail的位置
                    tail = (tail + 1) % Length;
                }
            }
        }

        /// <summary>
        /// 清空
        /// 思路：使用递归释放当前节点的数据值
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="tree"></param>
        public static void Clear<T>(BinTree<T> tree) {
            if (tree == null) return;

            //递归左子树
            Clear(tree.Left);

            //递归右子树
            Clear(tree.Right);

            //释放节点数据
            tree = null;
        }

    }

    /// <summary>
    /// 二叉树(二叉链表)存储结构
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class BinTree<T> {
        public T Data { get; set; } //数据域

        public BinTree<T> Left { get; set; } //左孩子

        public BinTree<T> Right { get; set; } //右孩子
    }

    /// <summary>
    /// 插入方向(左节点还是右节点)
    /// </summary>
    public enum Direction {
        Left = 1,
        Right = 2
    }
}




