using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CADEngine.DrawingObject;
using System.Drawing;
using System.Drawing.Drawing2D;

namespace ContourProgramming {

    public class CCuttingTree {
        public CuttingTreeNode _rootNode;
        public CCuttingTree() {
            CDrawingObjectCircleR rootCurve = new CDrawingObjectCircleR(new PointF(0, 0), 9999999, null);
            _rootNode = new CuttingTreeNode();
            _rootNode.ID = 0;
            _rootNode.Data = rootCurve;

        }
        public void TranversCuttingTree(CuttingTreeNode root, ref string strSequence) {
            foreach (CuttingTreeNode child in root.ChildrenList) {
                TranversCuttingTree(child, ref strSequence);
            }
            strSequence += "->" + root.ID.ToString();

        }
        /// <summary>
        /// 采用非递归的方法生成树
        /// </summary>
        /// <param name="parentNode"></param>
        /// <param name="nodesToAdd"></param>
        public void CreateCuttingTree(CuttingTreeNode rootNode, List<CuttingTreeNode> nodesToAdd) {
            //两两比较，把不包含于任何轮廓的节点标志出来.
            //只要一个节点被别人包含，就把这个节点添加到别人的孩子里。
            for (int i = 0; i < nodesToAdd.Count; ++i) {
                for (int j = i + 1; j < nodesToAdd.Count; ++j) {
                    //检查两个节点是否有包含关系
                    if (nodesToAdd[i].Contains(nodesToAdd[j])) {
                        nodesToAdd[i].AllContainedNodes.Add(nodesToAdd[j]);
                        nodesToAdd[j].IsContainedbyOtherNode = true;
                        nodesToAdd[j].Level++;
                    }
                    else if (nodesToAdd[j].Contains(nodesToAdd[i])) {
                        nodesToAdd[i].Level++;
                        nodesToAdd[j].AllContainedNodes.Add(nodesToAdd[i]);
                        nodesToAdd[i].IsContainedbyOtherNode = true;
                    }
                }
            }

            for (int i = 0; i < nodesToAdd.Count; ++i) {
                if (nodesToAdd[i].Level == 0)  //首先添加第一层节点
                {
                    rootNode.ChildrenList.Add(nodesToAdd[i]);
                    //递归添加下面层的所有的子节点
                    AddFirstLevelChidren(nodesToAdd[i]);
                }
            }
        }
        public void AddFirstLevelChidren(CuttingTreeNode parentNode) {
            foreach (CuttingTreeNode node in parentNode.AllContainedNodes) {
                if (parentNode.Level == node.Level - 1) {
                    parentNode.ChildrenList.Add(node);
                    AddFirstLevelChidren(node);
                }
            }
        }
        /// <summary>
        /// 采用递归的方法生成树，感觉效率不是很高，特别施层数较多的情况下
        /// 这个函数不再使用，只是放在这里而已。
        /// </summary>
        /// <param name="parentNode"></param>
        /// <param name="nodesToAdd"></param>
        private void InsertNode(CuttingTreeNode parentNode, List<CuttingTreeNode> nodesToAdd) {
            //清空所有的要添加的节点的是否包含到其他轮廓的标志位
            //并且清空该节点的所有的孩子
            foreach (CuttingTreeNode node in nodesToAdd) {
                node.IsContainedbyOtherNode = false;
                node.ChildrenList.Clear();
            }
            //两两比较，把不包含于任何轮廓的节点标志出来.
            //只要一个节点被别人包含，就把这个节点添加到别人的孩子里。
            for (int i = 0; i < nodesToAdd.Count; ++i) {
                for (int j = i + 1; j < nodesToAdd.Count; ++j) {
                    //检查两个节点是否有包含关系
                    if (nodesToAdd[i].Contains(nodesToAdd[j])) {
                        nodesToAdd[i].ChildrenList.Add(nodesToAdd[j]);
                        nodesToAdd[j].IsContainedbyOtherNode = true;
                    }
                    else if (nodesToAdd[j].Contains(nodesToAdd[i])) {
                        nodesToAdd[j].ChildrenList.Add(nodesToAdd[i]);
                        nodesToAdd[i].IsContainedbyOtherNode = true;
                    }
                }
            }
            //清空当前节点的所有的子节点
            parentNode.ChildrenList.Clear();
            //把不包含于任何轮廓的节点，加到当前节点的孩子中
            foreach (CuttingTreeNode node in nodesToAdd) {
                if (!node.IsContainedbyOtherNode) {
                    parentNode.ChildrenList.Add(node);
                }
            }
            foreach (CuttingTreeNode node in parentNode.ChildrenList) {
                //只有有叶子的节点才需要跟进一步的分析，
                if (node.ChildrenList.Count != 0) {
                    List<CuttingTreeNode> nodesList = CopyNewList(node.ChildrenList);
                    InsertNode(node, nodesList);
                }
            }
        }
        private List<CuttingTreeNode> CopyNewList(List<CuttingTreeNode> node2Copy) {
            List<CuttingTreeNode> nodelist = new List<CuttingTreeNode>();
            foreach (CuttingTreeNode nodetemp in node2Copy) {
                nodelist.Add(nodetemp);
            }

            return nodelist;
        }
    }

    /// <summary>
    /// 轮廓的存储结构
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class CuttingTreeNode {
        public int ID;
        public CDrawingObjectBase Data { get; set; } //数据域
        public bool IsContainedbyOtherNode;
        public int Level;
        public List<CuttingTreeNode> ChildrenList = new List<CuttingTreeNode>();
        public List<CuttingTreeNode> AllContainedNodes = new List<CuttingTreeNode>();
        public bool Contains(CuttingTreeNode node) {
            return node.Data.IsContainedBy(this.Data);
        }
    }

}
