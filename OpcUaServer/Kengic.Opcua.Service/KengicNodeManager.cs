using Kengic.Opcua.Demo.Common;
using Kengic.Opcua.Demo.Model;
using Opc.Ua;
using Opc.Ua.Server;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using SharpNodeSettings.Core;
using System.Xml.Linq;
using SharpNodeSettings.Node.Regular;
using SharpNodeSettings.Node.Request;
using SharpNodeSettings.Node.Server;
using Range = Opc.Ua.Range;
using SharpNodeSettings.OpcUaServer;
using KengicCommunication_Core.Enthernet;
using KengicCommunication_Core;
using KengicCommunication_Core.LogNet;
using System.Drawing;

namespace Kengic.Opcua.Demo.Service
{
    /// <summary>
    /// 以下备注中  测点即代表最叶子级节点
    /// 目前设计是 只有测点有数据  其余节点都是目录
    /// </summary>
    public class KengicNodeManager : CustomNodeManager2
    {
        /// <summary>
        /// 配置修改次数  主要用来识别菜单树是否有变动  如果发生变动则修改菜单树对应节点  测点的实时数据变化不算在内
        /// </summary>
        private int cfgCount = -1;
        private IList<IReference> _references;
        /// <summary>
        /// 测点集合,实时数据刷新时,直接从字典中取出对应的测点,修改值即可
        /// </summary>
        private Dictionary<string, BaseDataVariableState> _nodeDic = new Dictionary<string, BaseDataVariableState>();
        /// <summary>
        /// 目录集合,修改菜单树时需要(我们需要知道哪些菜单需要修改,哪些需要新增,哪些需要删除)
        /// </summary>
        private Dictionary<string, FolderState> _folderDic = new Dictionary<string, FolderState>();

        public KengicNodeManager(IServerInternal server, ApplicationConfiguration configuration) : base(server, configuration, "http://opcfoundation.org/Quickstarts/ReferenceApplications")
        {
        }

        /// <summary>
        /// 重写NodeId生成方式(目前采用'_'分隔,如需更改,请修改此方法)
        /// </summary>
        /// <param name="context"></param>
        /// <param name="node"></param>
        /// <returns></returns>
        public override NodeId New(ISystemContext context, NodeState node)
        {
            BaseInstanceState instance = node as BaseInstanceState;

            if (instance != null && instance.Parent != null)
            {
                string id = instance.Parent.NodeId.Identifier as string;

                if (id != null)
                {
                    return new NodeId(id + "_" + instance.SymbolicName, instance.Parent.NodeId.NamespaceIndex);
                }
            }

            return node.NodeId;
        }

        /// <summary>
        /// 重写获取节点句柄的方法
        /// </summary>
        /// <param name="context"></param>
        /// <param name="nodeId"></param>
        /// <param name="cache"></param>
        /// <returns></returns>
        protected override NodeHandle GetManagerHandle(ServerSystemContext context, NodeId nodeId, IDictionary<NodeId, NodeState> cache)
        {
            lock (Lock)
            {
                // quickly exclude nodes that are not in the namespace. 
                if (!IsNodeIdInNamespace(nodeId))
                {
                    return null;
                }

                NodeState node = null;

                if (!PredefinedNodes.TryGetValue(nodeId, out node))
                {
                    return null;
                }

                NodeHandle handle = new NodeHandle();

                handle.NodeId = nodeId;
                handle.Node = node;
                handle.Validated = true;

                return handle;
            }
        }

        /// <summary>
        /// 重写节点的验证方式
        /// </summary>
        /// <param name="context"></param>
        /// <param name="handle"></param>
        /// <param name="cache"></param>
        /// <returns></returns>
        protected override NodeState ValidateNode(ServerSystemContext context, NodeHandle handle, IDictionary<NodeId, NodeState> cache)
        {
            // not valid if no root.
            if (handle == null)
            {
                return null;
            }

            // check if previously validated.
            if (handle.Validated)
            {
                return handle.Node;
            }
            // TBD
            return null;
        }

        /// <summary>
        /// 重写创建基础目录
        /// </summary>
        /// <param name="externalReferences"></param>
        public override void CreateAddressSpace(IDictionary<NodeId, IList<IReference>> externalReferences)
        {
            lock (Lock)
            {
                ILogNet logNet = new LogNetSingle("log.txt");
                logNet.WriteAnyString("创建基础目录");
                LoadPredefinedNodes(SystemContext, externalReferences);

                IList<IReference> references = null;

                if (!externalReferences.TryGetValue(ObjectIds.ObjectsFolder, out references))
                {
                    externalReferences[ObjectIds.ObjectsFolder] = references = new List<IReference>();
                }


                dict_BaseDataVariableState = new Dictionary<string, BaseDataVariableState>();
                try
                {
                    // =========================================================================================
                    // 
                    // 此处需要加载本地文件，并且创建对应的节点信息，
                    // 
                    // =========================================================================================
                    sharpNodeServer = new SharpNodeServer();
                    sharpNodeServer.WriteCustomerData = (SharpNodeSettings.Device.DeviceCore deviceCore, string name) =>
                    {
                        string opcNode = "ns=2;s=" + string.Join("/", deviceCore.DeviceNodes) + "/" + name;
                        lock (Lock)
                        {
                            if (dict_BaseDataVariableState.ContainsKey(opcNode))
                            {
                                dict_BaseDataVariableState[opcNode].Value = deviceCore.GetDynamicValueByName(name);
                                dict_BaseDataVariableState[opcNode].ClearChangeMasks(SystemContext, false);
                            }
                        }
                    };
                   

                    XElement element = XElement.Load("settings.xml");
                    dicRegularItemNode = SharpNodeSettings.Util.ParesRegular(element);
                    AddNodeClass(null, element, references);

                    // 加载配置文件之前设置写入方法

                    logNet.BeforeSaveToFile += LogNet_BeforeSaveToFile;
                    sharpNodeServer.LogNet = logNet;
                    sharpNodeServer.LoadByXmlFile("settings.xml");
                    // 最后再启动服务器信息
                    sharpNodeServer.ServerStart(port);
                    simplifyClient = new NetSimplifyClient(this.ipAddress, this.port);
                }
                catch (Exception e)
                {
                    logNet.WriteException("异常：", e);
                    Utils.Trace(e, "Error creating the address space.");
                }
            }
        }

        private void LogNet_BeforeSaveToFile(object sender, HslEventArgs e)
        {
            StringBuilder br = new StringBuilder("");
            br.AppendLine(e.HslMessage.ToString());
         

        }

        /// <summary>
        /// 生成根节点(由于根节点需要特殊处理,此处单独出来一个方法)
        /// </summary>
        /// <param name="nodes"></param>
        /// <param name="references"></param>
        private void GeneraterNodes(List<OpcuaNode> nodes, IList<IReference> references)
        {
            var list = nodes.Where(d => d.NodeType == NodeType.Scada);
            foreach (var item in list)
            {
                try
                {
                    FolderState root = CreateFolder(null, item.NodePath, item.NodeName);
                    root.AddReference(ReferenceTypes.Organizes, true, ObjectIds.ObjectsFolder);
                    references.Add(new NodeStateReference(ReferenceTypes.Organizes, false, root.NodeId));
                    root.EventNotifier = EventNotifiers.SubscribeToEvents;
                    AddRootNotifier(root);
                    CreateNodes(nodes, root, item.NodePath);
                    _folderDic.Add(item.NodePath, root);
                    //添加引用关系
                    AddPredefinedNode(SystemContext, root);
                }
                catch (Exception ex)
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine("创建OPC-UA根节点触发异常:" + ex.Message);
                    Console.ResetColor();
                }
            }
        }

        /// <summary>
        /// 递归创建子节点(包括创建目录和测点)
        /// </summary>
        /// <param name="nodes"></param>
        /// <param name="parent"></param>
        private void CreateNodes(List<OpcuaNode> nodes, FolderState parent, string parentPath)
        {
            var list = nodes.Where(d => d.ParentPath == parentPath);
            foreach (var node in list)
            {
                try
                {
                    if (!node.IsTerminal)
                    {
                        FolderState folder = CreateFolder(parent, node.NodePath, node.NodeName);
                        _folderDic.Add(node.NodePath, folder);
                        CreateNodes(nodes, folder, node.NodePath);
                    }
                    else
                    {
                        BaseDataVariableState variable = CreateVariable(parent, node.NodePath, node.NodeName, DataTypeIds.Double, ValueRanks.Scalar);
                        //此处需要注意  目录字典是以目录路径作为KEY 而 测点字典是以测点ID作为KEY  为了方便更新实时数据
                        _nodeDic.Add(node.NodeId.ToString(), variable);
                    }
                }
                catch (Exception ex)
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine("创建OPC-UA子节点触发异常:" + ex.Message);
                    Console.ResetColor();
                }
            }
        }

       

        /// <summary>
        /// 创建节点
        /// </summary>
        /// <param name="parent"></param>
        /// <param name="path"></param>
        /// <param name="name"></param>
        /// <param name="dataType"></param>
        /// <param name="valueRank"></param>
        /// <returns></returns>
        private BaseDataVariableState CreateVariable(NodeState parent, string path, string name, NodeId dataType, int valueRank)
        {
            BaseDataVariableState variable = new BaseDataVariableState(parent);

            variable.SymbolicName = name;
            variable.ReferenceTypeId = ReferenceTypes.Organizes;
            variable.TypeDefinitionId = VariableTypeIds.BaseDataVariableType;
            variable.NodeId = new NodeId(path, NamespaceIndex);
            variable.BrowseName = new QualifiedName(path, NamespaceIndex);
            variable.DisplayName = new LocalizedText("en", name);
            variable.WriteMask = AttributeWriteMask.DisplayName | AttributeWriteMask.Description;
            variable.UserWriteMask = AttributeWriteMask.DisplayName | AttributeWriteMask.Description;
            variable.DataType = dataType;
            variable.ValueRank = valueRank;
            variable.AccessLevel = AccessLevels.CurrentReadOrWrite;
            variable.UserAccessLevel = AccessLevels.CurrentReadOrWrite;
            variable.Historizing = false;
            //variable.Value = GetNewValue(variable);
            variable.StatusCode = StatusCodes.Good;
            variable.Timestamp = DateTime.Now;
            variable.OnWriteValue = OnWriteDataValue;

            if (valueRank == ValueRanks.OneDimension)
            {
                variable.ArrayDimensions = new ReadOnlyList<uint>(new List<uint> { 0 });
            }
            else if (valueRank == ValueRanks.TwoDimensions)
            {
                variable.ArrayDimensions = new ReadOnlyList<uint>(new List<uint> { 0, 0 });
            }

            if (parent != null)
            {
                parent.AddChild(variable);
            }

            return variable;
        }

        /// <summary>
        /// 创建父级目录(请确保对应的根目录已创建)
        /// </summary>
        /// <param name="nodes"></param>
        /// <param name="currentNode"></param>
        /// <returns></returns>
        public FolderState GetParentFolderState(IEnumerable<OpcuaNode> nodes, OpcuaNode currentNode)
        {
            FolderState folder = null;
            if (!_folderDic.TryGetValue(currentNode.ParentPath, out folder))
            {
                var parent = nodes.Where(d => d.NodePath == currentNode.ParentPath).FirstOrDefault();
                if (!string.IsNullOrEmpty(parent.ParentPath))
                {
                    var pFol = GetParentFolderState(nodes, parent);
                    folder = CreateFolder(pFol, parent.NodePath, parent.NodeName);
                    pFol.ClearChangeMasks(SystemContext, false);
                    AddPredefinedNode(SystemContext, folder);
                    _folderDic.Add(currentNode.ParentPath, folder);
                }
            }
            return folder;
        }

        /// <summary>
        /// 客户端写入值时触发(绑定到节点的写入事件上)
        /// </summary>
        /// <param name="context"></param>
        /// <param name="node"></param>
        /// <param name="indexRange"></param>
        /// <param name="dataEncoding"></param>
        /// <param name="value"></param>
        /// <param name="statusCode"></param>
        /// <param name="timestamp"></param>
        /// <returns></returns>
        private ServiceResult OnWriteDataValue(ISystemContext context, NodeState node, NumericRange indexRange, QualifiedName dataEncoding,
            ref object value, ref StatusCode statusCode, ref DateTime timestamp)
        {
            BaseDataVariableState variable = node as BaseDataVariableState;
            try
            {
                //验证数据类型
                TypeInfo typeInfo = TypeInfo.IsInstanceOfDataType(
                    value,
                    variable.DataType,
                    variable.ValueRank,
                    context.NamespaceUris,
                    context.TypeTable);

                if (typeInfo == null || typeInfo == TypeInfo.Unknown)
                {
                    return StatusCodes.BadTypeMismatch;
                }
                if (typeInfo.BuiltInType == BuiltInType.Double)
                {
                    double number = Convert.ToDouble(value);
                    value = TypeInfo.Cast(number, typeInfo.BuiltInType);
                }
                OperateResult<string> result = simplifyClient.ReadFromServer(2, node.NodeId.Identifier + "&" + Convert.ToString(value));
                return ServiceResult.Good;
            }
            catch (Exception)
            {
                return StatusCodes.BadTypeMismatch;
            }
        }

        /// <summary>
        /// 读取历史数据
        /// </summary>
        /// <param name="context"></param>
        /// <param name="details"></param>
        /// <param name="timestampsToReturn"></param>
        /// <param name="releaseContinuationPoints"></param>
        /// <param name="nodesToRead"></param>
        /// <param name="results"></param>
        /// <param name="errors"></param>
        public override void HistoryRead(OperationContext context, HistoryReadDetails details, TimestampsToReturn timestampsToReturn, bool releaseContinuationPoints,
            IList<HistoryReadValueId> nodesToRead, IList<HistoryReadResult> results, IList<ServiceResult> errors)
        {
            ReadProcessedDetails readDetail = details as ReadProcessedDetails;
            //假设查询历史数据  都是带上时间范围的
            if (readDetail == null || readDetail.StartTime == DateTime.MinValue || readDetail.EndTime == DateTime.MinValue)
            {
                errors[0] = StatusCodes.BadHistoryOperationUnsupported;
                return;
            }
            for (int ii = 0; ii < nodesToRead.Count; ii++)
            {
                int sss = readDetail.StartTime.Millisecond;
                double res = sss + DateTime.Now.Millisecond;
                //这里  返回的历史数据可以是多种数据类型  请根据实际的业务来选择
                Opc.Ua.KeyValuePair keyValue = new Opc.Ua.KeyValuePair()
                {
                    Key = new QualifiedName(nodesToRead[ii].NodeId.Identifier.ToString()),
                    Value = res
                };
                results[ii] = new HistoryReadResult()
                {
                    StatusCode = StatusCodes.Good,
                    HistoryData = new ExtensionObject(keyValue)
                };
                errors[ii] = StatusCodes.Good;
                //切记,如果你已处理完了读取历史数据的操作,请将Processed设为true,这样OPC-UA类库就知道你已经处理过了 不需要再进行检查了
                nodesToRead[ii].Processed = true;
            }
        }
        #region SharpNodeSettings Server

        private SharpNodeServer sharpNodeServer = null;
        private Dictionary<string, List<RegularItemNode>> dicRegularItemNode = null;
        private Dictionary<string, BaseDataVariableState> dict_BaseDataVariableState;    // 节点管理器
        private string ipAddress = "127.0.0.1";
        private int port = 506;                                                          // 服务器的端口号
        private NetSimplifyClient simplifyClient;
        #endregion

        #region Overrides
        #endregion

        #region Private Fields
        private ReferenceServerConfiguration m_configuration;
        private Opc.Ua.Test.DataGenerator m_generator;
        private Timer m_simulationTimer;
        private UInt16 m_simulationInterval = 1000;
        private bool m_simulationEnabled = true;
        private List<BaseDataVariableState> m_dynamicNodes;
        #endregion


        private void AddNodeClass(NodeState parent, XElement nodeClass, IList<IReference> references)
        {
            foreach (var xmlNode in nodeClass.Elements())
            {
                if (xmlNode.Name == "NodeClass")
                {
                    SharpNodeSettings.Node.NodeBase.NodeClass nClass = new SharpNodeSettings.Node.NodeBase.NodeClass();
                    nClass.LoadByXmlElement(xmlNode);

                    FolderState son;
                    if (parent == null)
                    {
                        son = CreateFolder(null, nClass.Name);
                        son.Description = nClass.Description;
                        son.AddReference(ReferenceTypes.Organizes, true, ObjectIds.ObjectsFolder);
                        references.Add(new NodeStateReference(ReferenceTypes.Organizes, false, son.NodeId));
                        son.EventNotifier = EventNotifiers.SubscribeToEvents;
                        AddRootNotifier(son);

                        AddNodeClass(son, xmlNode, references);
                        AddPredefinedNode(SystemContext, son);
                    }
                    else
                    {
                        son = CreateFolder(parent, nClass.Name, nClass.Description);
                        AddNodeClass(son, xmlNode, references);
                    }
                }
                else if (xmlNode.Name == "DeviceNode")
                {
                    AddDeviceCore(parent, xmlNode);
                }
                else if (xmlNode.Name == "Server")
                {
                    AddServer(parent, xmlNode, references);
                }
            }
        }

        private void AddDeviceCore(NodeState parent, XElement device)
        {
            if (device.Name == "DeviceNode")
            {
                // 提取名称和描述信息
                string name = device.Attribute("Name").Value;
                string description = device.Attribute("Description").Value;

                // 创建OPC节点
                FolderState deviceFolder = CreateFolder(parent, device.Attribute("Name").Value, device.Attribute("Description").Value);
                // 添加Request
                foreach (var requestXml in device.Elements("DeviceRequest"))
                {
                    DeviceRequest deviceRequest = new DeviceRequest();
                    deviceRequest.LoadByXmlElement(requestXml);

                    AddDeviceRequest(deviceFolder, deviceRequest);
                }
            }
        }

        private void AddServer(NodeState parent, XElement xmlNode, IList<IReference> references)
        {
            int serverType = int.Parse(xmlNode.Attribute("ServerType").Value);
            if (serverType == ServerNode.ModbusServer)
            {
                NodeModbusServer serverNode = new NodeModbusServer();
                serverNode.LoadByXmlElement(xmlNode);

                FolderState son = CreateFolder(parent, serverNode.Name, serverNode.Description);
                AddNodeClass(son, xmlNode, references);
            }
            else if (serverType == ServerNode.AlienServer)
            {
                AlienServerNode alienNode = new AlienServerNode();
                alienNode.LoadByXmlElement(xmlNode);

                FolderState son = CreateFolder(parent, alienNode.Name, alienNode.Description);
                AddNodeClass(son, xmlNode, references);
            }
        }
        private void AddDeviceRequest(NodeState parent, DeviceRequest deviceRequest)
        {
            // 提炼真正的数据节点
            if (!dicRegularItemNode.ContainsKey(deviceRequest.PraseRegularCode)) return;
            List<RegularItemNode> regularNodes = dicRegularItemNode[deviceRequest.PraseRegularCode];

            foreach (var regularNode in regularNodes)
            {
                if (regularNode.RegularCode == RegularNodeTypeItem.FBool().Code)
                {
                    if (regularNode.TypeLength == 1)
                    {
                        var dataVariableState = CreateBaseVariable(parent, regularNode.Name, regularNode.Description, DataTypeIds.Boolean, ValueRanks.Scalar, default(bool));
                        dict_BaseDataVariableState.Add(dataVariableState.NodeId.ToString(), dataVariableState);
                    }
                    else
                    {
                        var dataVariableState = CreateBaseVariable(parent, regularNode.Name, regularNode.Description, DataTypeIds.Boolean, ValueRanks.OneDimension, new bool[regularNode.TypeLength]);
                        dict_BaseDataVariableState.Add(dataVariableState.NodeId.ToString(), dataVariableState);
                    }
                }
                else if (regularNode.RegularCode == RegularNodeTypeItem.FByte().Code)
                {
                    if (regularNode.TypeLength == 1)
                    {
                        var dataVariableState = CreateBaseVariable(parent, regularNode.Name, regularNode.Description, DataTypeIds.Byte, ValueRanks.Scalar, default(byte));
                        dict_BaseDataVariableState.Add(dataVariableState.NodeId.ToString(), dataVariableState);
                    }
                    else
                    {
                        var dataVariableState = CreateBaseVariable(parent, regularNode.Name, regularNode.Description, DataTypeIds.Byte, ValueRanks.OneDimension, new byte[regularNode.TypeLength]);
                        dict_BaseDataVariableState.Add(dataVariableState.NodeId.ToString(), dataVariableState);
                    }
                }
                else if (regularNode.RegularCode == RegularNodeTypeItem.FInt16().Code)
                {
                    if (regularNode.TypeLength == 1)
                    {
                        var dataVariableState = CreateBaseVariable(parent, regularNode.Name, regularNode.Description, DataTypeIds.Int16, ValueRanks.Scalar, default(short));
                        dict_BaseDataVariableState.Add(dataVariableState.NodeId.ToString(), dataVariableState);
                    }
                    else
                    {
                        var dataVariableState = CreateBaseVariable(parent, regularNode.Name, regularNode.Description, DataTypeIds.Int16, ValueRanks.OneDimension, new short[regularNode.TypeLength]);
                        dict_BaseDataVariableState.Add(dataVariableState.NodeId.ToString(), dataVariableState);
                    }
                }
                else if (regularNode.RegularCode == RegularNodeTypeItem.FUInt16().Code)
                {
                    if (regularNode.TypeLength == 1)
                    {
                        var dataVariableState = CreateBaseVariable(parent, regularNode.Name, regularNode.Description, DataTypeIds.UInt16, ValueRanks.Scalar, default(ushort));
                        dict_BaseDataVariableState.Add(dataVariableState.NodeId.ToString(), dataVariableState);
                    }
                    else
                    {
                        var dataVariableState = CreateBaseVariable(parent, regularNode.Name, regularNode.Description, DataTypeIds.UInt16, ValueRanks.OneDimension, new ushort[regularNode.TypeLength]);
                        dict_BaseDataVariableState.Add(dataVariableState.NodeId.ToString(), dataVariableState);
                    }
                }
                else if (regularNode.RegularCode == RegularNodeTypeItem.FInt32().Code)
                {
                    if (regularNode.TypeLength == 1)
                    {
                        var dataVariableState = CreateBaseVariable(parent, regularNode.Name, regularNode.Description, DataTypeIds.Int32, ValueRanks.Scalar, default(int));
                        dict_BaseDataVariableState.Add(dataVariableState.NodeId.ToString(), dataVariableState);
                    }
                    else
                    {
                        var dataVariableState = CreateBaseVariable(parent, regularNode.Name, regularNode.Description, DataTypeIds.Int32, ValueRanks.OneDimension, new int[regularNode.TypeLength]);
                        dict_BaseDataVariableState.Add(dataVariableState.NodeId.ToString(), dataVariableState);
                    }
                }
                else if (regularNode.RegularCode == RegularNodeTypeItem.FUInt32().Code)
                {
                    if (regularNode.TypeLength == 1)
                    {
                        var dataVariableState = CreateBaseVariable(parent, regularNode.Name, regularNode.Description, DataTypeIds.UInt32, ValueRanks.Scalar, default(uint));
                        dict_BaseDataVariableState.Add(dataVariableState.NodeId.ToString(), dataVariableState);
                    }
                    else
                    {
                        var dataVariableState = CreateBaseVariable(parent, regularNode.Name, regularNode.Description, DataTypeIds.UInt32, ValueRanks.OneDimension, new uint[regularNode.TypeLength]);
                        dict_BaseDataVariableState.Add(dataVariableState.NodeId.ToString(), dataVariableState);
                    }
                }
                else if (regularNode.RegularCode == RegularNodeTypeItem.FFloat().Code)
                {
                    if (regularNode.TypeLength == 1)
                    {
                        var dataVariableState = CreateBaseVariable(parent, regularNode.Name, regularNode.Description, DataTypeIds.Float, ValueRanks.Scalar, default(float));
                        dict_BaseDataVariableState.Add(dataVariableState.NodeId.ToString(), dataVariableState);
                    }
                    else
                    {
                        var dataVariableState = CreateBaseVariable(parent, regularNode.Name, regularNode.Description, DataTypeIds.Float, ValueRanks.OneDimension, new float[regularNode.TypeLength]);
                        dict_BaseDataVariableState.Add(dataVariableState.NodeId.ToString(), dataVariableState);
                    }
                }
                else if (regularNode.RegularCode == RegularNodeTypeItem.FInt64().Code)
                {
                    if (regularNode.TypeLength == 1)
                    {
                        var dataVariableState = CreateBaseVariable(parent, regularNode.Name, regularNode.Description, DataTypeIds.Int64, ValueRanks.Scalar, default(long));
                        dict_BaseDataVariableState.Add(dataVariableState.NodeId.ToString(), dataVariableState);
                    }
                    else
                    {
                        var dataVariableState = CreateBaseVariable(parent, regularNode.Name, regularNode.Description, DataTypeIds.Int64, ValueRanks.OneDimension, new long[regularNode.TypeLength]);
                        dict_BaseDataVariableState.Add(dataVariableState.NodeId.ToString(), dataVariableState);
                    }
                }
                else if (regularNode.RegularCode == RegularNodeTypeItem.FUInt64().Code)
                {
                    if (regularNode.TypeLength == 1)
                    {
                        var dataVariableState = CreateBaseVariable(parent, regularNode.Name, regularNode.Description, DataTypeIds.UInt64, ValueRanks.Scalar, default(ulong));
                        dict_BaseDataVariableState.Add(dataVariableState.NodeId.ToString(), dataVariableState);
                    }
                    else
                    {
                        var dataVariableState = CreateBaseVariable(parent, regularNode.Name, regularNode.Description, DataTypeIds.UInt64, ValueRanks.OneDimension, new ulong[regularNode.TypeLength]);
                        dict_BaseDataVariableState.Add(dataVariableState.NodeId.ToString(), dataVariableState);
                    }
                }
                else if (regularNode.RegularCode == RegularNodeTypeItem.FDouble().Code)
                {
                    if (regularNode.TypeLength == 1)
                    {
                        var dataVariableState = CreateBaseVariable(parent, regularNode.Name, regularNode.Description, DataTypeIds.Double, ValueRanks.Scalar, default(double));
                        dict_BaseDataVariableState.Add(dataVariableState.NodeId.ToString(), dataVariableState);
                    }
                    else
                    {
                        var dataVariableState = CreateBaseVariable(parent, regularNode.Name, regularNode.Description, DataTypeIds.Double, ValueRanks.OneDimension, new double[regularNode.TypeLength]);
                        dict_BaseDataVariableState.Add(dataVariableState.NodeId.ToString(), dataVariableState);
                    }
                }
                else if (regularNode.RegularCode == RegularNodeTypeItem.FStringAscii().Code ||
                    regularNode.RegularCode == RegularNodeTypeItem.FStringUnicode().Code ||
                    regularNode.RegularCode == RegularNodeTypeItem.FStringUtf8().Code)
                {

                    var dataVariableState = CreateBaseVariable(parent, regularNode.Name, regularNode.Description, DataTypeIds.String, ValueRanks.Scalar, "");
                    dict_BaseDataVariableState.Add(dataVariableState.NodeId.ToString(), dataVariableState);
                }
            }

        }


        /// <summary>
        /// 创建一个新的节点，节点名称为字符串
        /// </summary>
        protected FolderState CreateFolder(NodeState parent, string name)
        {
            return CreateFolder(parent, name, string.Empty);
        }

        /// <summary>
        /// 创建一个新的节点，节点名称为字符串
        /// </summary>
        protected FolderState CreateFolder(NodeState parent, string name, string description)
        {
            FolderState folder = new FolderState(parent);

            folder.SymbolicName = name;
            folder.ReferenceTypeId = ReferenceTypes.Organizes;
            folder.TypeDefinitionId = ObjectTypeIds.FolderType;
            folder.Description = description;
            if (parent == null)
            {
                folder.NodeId = new NodeId(name, NamespaceIndex);
            }
            else
            {
                folder.NodeId = new NodeId(parent.NodeId.ToString() + "/" + name);
            }
            folder.BrowseName = new QualifiedName(name, NamespaceIndex);
            folder.DisplayName = new LocalizedText(name);
            folder.WriteMask = AttributeWriteMask.None;
            folder.UserWriteMask = AttributeWriteMask.None;
            folder.EventNotifier = EventNotifiers.None;

            if (parent != null)
            {
                parent.AddChild(folder);
            }

            return folder;
        }

        /// <summary>
        /// 创建一个值节点，类型需要在创建的时候指定
        /// </summary>
        protected BaseDataVariableState CreateBaseVariable(NodeState parent, string name, string description, NodeId dataType, int valueRank, object defaultValue)
        {
            BaseDataVariableState variable = new BaseDataVariableState(parent);

            variable.SymbolicName = name;
            variable.ReferenceTypeId = ReferenceTypes.Organizes;
            variable.TypeDefinitionId = VariableTypeIds.BaseDataVariableType;
            if (parent == null)
            {
                variable.NodeId = new NodeId(name, NamespaceIndex);
            }
            else
            {
                variable.NodeId = new NodeId(parent.NodeId.ToString() + "/" + name);
            }
            variable.Description = description;
            variable.BrowseName = new QualifiedName(name, NamespaceIndex);
            variable.DisplayName = new LocalizedText(name);
            variable.WriteMask = AttributeWriteMask.DisplayName | AttributeWriteMask.Description;
            variable.UserWriteMask = AttributeWriteMask.DisplayName | AttributeWriteMask.Description;
            variable.DataType = dataType;
            variable.ValueRank = valueRank;
            variable.AccessLevel = AccessLevels.CurrentReadOrWrite;
            variable.UserAccessLevel = AccessLevels.CurrentReadOrWrite;
            variable.Historizing = false;
            variable.Value = defaultValue;
            variable.StatusCode = StatusCodes.Good;
            variable.Timestamp = DateTime.Now;
            variable.OnWriteValue = OnWriteDataValue;
            if (valueRank == ValueRanks.OneDimension)
            {
                variable.ArrayDimensions = new ReadOnlyList<uint>(new List<uint> { 0 });
            }
            else if (valueRank == ValueRanks.TwoDimensions)
            {
                variable.ArrayDimensions = new ReadOnlyList<uint>(new List<uint> { 0, 0 });
            }

            if (parent != null)
            {
                parent.AddChild(variable);
            }

            return variable;
        }


    }
}
