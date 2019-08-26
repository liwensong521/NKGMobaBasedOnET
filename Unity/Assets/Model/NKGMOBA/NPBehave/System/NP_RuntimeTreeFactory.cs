//------------------------------------------------------------
// Author: 烟雨迷离半世殇
// Mail: 1778139321@qq.com
// Data: 2019年8月23日 15:06:15
//------------------------------------------------------------

using System.Collections.Generic;
using NPBehave;

namespace ETModel
{
    public class NP_RuntimeTreeFactory
    {
        /// <summary>
        /// 创建一个行为树实例
        /// </summary>
        /// <param name="unit">行为树所归属unit</param>
        /// <param name="NPDataId">行为树数据id</param>
        /// <returns></returns>
        public static NP_RuntimeTree CreateNpRuntimeTree(Unit unit, long NPDataId)
        {
            NP_DataSupportor npDataSupportor = Game.Scene.GetComponent<NP_RuntimeTreeRepository>().GetNPRuntimeTree(NPDataId);

            long theTreeID = IdGenerater.GenerateId();
            
            //然后开始处理非叶子结点
            foreach (var VARIABLE in npDataSupportor.mNP_DataSupportorDic)
            {
                switch (VARIABLE.Value.NodeType)
                {
                    case NodeType.Task:
                        VARIABLE.Value.CreateTask(unit.Id, theTreeID);
                        break;  
                    case NodeType.Decorator:
                        VARIABLE.Value.CreateDecoratorNode(unit.Id, theTreeID,
                            npDataSupportor.mNP_DataSupportorDic[VARIABLE.Value.linkedID[0]].NP_GetNode());
                        break;
                    case NodeType.Composite:
                        List<Node> temp = new List<Node>();
                        foreach (var VARIABLE1 in VARIABLE.Value.linkedID)
                        {
                            temp.Add(npDataSupportor.mNP_DataSupportorDic[VARIABLE1].NP_GetNode());
                        }

                        VARIABLE.Value.CreateComposite(temp.ToArray());
                        break;
                }
            }

            NP_RuntimeTree tempTree = ComponentFactory.CreateWithId<NP_RuntimeTree, Root>(theTreeID,
                (Root) npDataSupportor.mNP_DataSupportorDic[npDataSupportor.RootId].NP_GetNode());
            
            unit.GetComponent<NP_RuntimeTreeManager>().AddTree(tempTree.Id,tempTree);
            
            return tempTree;
        }
    }
}