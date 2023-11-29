using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BPJsonType;
using NodeUICtor.Ctor;
using UnityEngine;
using InputMgr;
using UnityEngine.UI;
using UnityEditor;
using FairyGUI;
using NodeUICtor.UI;
using JsonHelp;

namespace NodeUICtor
{
    internal class BPUI
    {
        /// <summary>
        /// 编辑时变量
        /// </summary>
        private readonly Dictionary<string, BPVariable> allvars = new Dictionary<string, BPVariable>();//
        public readonly Dictionary<string, INodeBaseUI> allNodes = new Dictionary<string, INodeBaseUI>();

        public readonly RectTransform MainPanel;
        private readonly RightMenu menu;
        private readonly List<KeyValuePair<string, Action>> menuList = new List<KeyValuePair<string, Action>>();
        private Vector2 createpoint = Vector2.zero;
        public BPUI(RectTransform mainPanel)
        {
            DragHandle.GetDragControl(mainPanel.gameObject);
            this.MainPanel = mainPanel;
            this.menu = new RightMenu(mainPanel.Find("RightMenu").GetComponent<RectTransform>());
            menu.active = false;
            menu.root.SetParent(mainPanel.parent);
            InputManager.instance.onMouseCursorClick += k =>
            {
                if (k == 1)
                {
                    createpoint = Input.mousePosition;
                    menu.position = createpoint;
                    menu.active = true;
                }
                if (k == 0)
                {
                    menu.active = false;
                }
            };
            InitMenu();
        }

        public void Load(BPjson data)
        {
            if (data == null) { return; }
            VariableUI.instance.ClearUI();
            this.allvars.Clear();
            data.vars.ForEach(v =>
            {
                VariableUI.instance.VarAdd(v.k, v.t, v.v);
            });
            foreach(var node in allNodes)
            {
                DeleteNode(node.Key);
            }
            data.allnodes.ForEach(node =>
            {
                this.createNode(node.type, node);
            });
            foreach(var node in allNodes.Values)
            {
                node.RefreshUI();
            }
        }


        /// <summary>
        /// 创建新节点
        /// </summary>
        /// <param name="ty"></param>
        private INodeBaseUI createNode(enodetype ty,INodejson data=null)
        {
            
            Debug.Log("创建新节点：" + ty);
            INodeBaseUI node = null;
            switch (ty)
            {
                case enodetype.actionnode:
                    node = new ActionNodeUI(this, data as Actionnodejson);
                    break;
                case enodetype.breaknode:
                    node = new BreakNodeUI(this, data as Breaknodejson);
                    break;
                case enodetype.conditionnode:
                    node = new ConditionUI(this,data as Conditionnodejson);
                    break;
                case enodetype.continuenode:
                    node = new ContinueNodeUI(this,data as Continuenodejson);
                    break;
                case enodetype.getbpvnode:
                    node = new GetBPVariableNodeUI(this,data as Getbpvnodejson);
                    break;
                case enodetype.getcvnode:
                    node = new GetConstValueNodeUI(this,data as Getcvnodejson);
                    break;
                case enodetype.getfvnode:
                    node = new GetFunctionValueNodeUI(this,data as Getfvnodejson);
                    break;
                case enodetype.lisnode:
                    node = new ListenNodeUI(this,data as Lisnodejson);
                    break;
                case enodetype.loopnode:
                    node = new LoopNodeUI(this,data as Loopnodejson);
                    break;
                case enodetype.ticknode:
                    node = new TickNodeUI(this,data as Ticknodejson);
                    break;
                case enodetype.whilenode:
                    node = new WhileNodeUI(this,data as Whilenodejson);
                    break;
            }
            this.allNodes.Add(node.data.guid, node);
            node.tra.SetParent( MainPanel);
            node.rectTra.position = data==null? createpoint : data.position.ToUnityPos2();
            node.data.position.GetXY(node.rectTra.position);
            return node;
        }
        /// <summary>
        /// 删除节点
        /// </summary>
        /// <param name="guid"></param>
        public void DeleteNode(string guid)
        {
            if (this.allNodes.ContainsKey(guid))
            {
                var node = this.allNodes[guid];
                this.allNodes.Remove(guid);
                node.Destory();
            }
        }

        private Vector2 convertWondowToUIPos(Vector2 p)
        {
            return p;
            var nowPanelPos = this.MainPanel.anchoredPosition;
            return new Vector2(p.x - nowPanelPos.x, nowPanelPos.y - p.y);
        }

        /// <summary>
        /// 转化当前节点信息导出
        /// </summary>
        /// <returns></returns>
        public BPjson BuildBPJson()
        {
            var jsonObj = new BPjson();
            foreach (var node in this.allNodes)
            {
                var nodebase = node.Value.data;
                jsonObj.allnodes.Add(nodebase);

                if (node.Value is TickNodeUI)
                {
                    jsonObj.ticks.Add(node.Key);
                }
                if (node.Value is ListenNodeUI)
                {
                    jsonObj.liss.Add(node.Key);
                }
            }
            jsonObj.vars = this.allvars.Values.ToList();

            var path = JsonHelper.openFileDialog(out var name);
            if (path != null)
            {
                JsonHelper.WriteJsonToFile(jsonObj, path);
            }
            return jsonObj;
        }

        private void InitMenu()
        {
            menuList.AddRange(new KeyValuePair<string, Action>[] {
                new KeyValuePair<string, Action>("每帧驱动",()=>{  this.createNode(enodetype.ticknode).RefreshUI();    }),
                new KeyValuePair<string, Action>("当变量修改",()=>{  this.createNode(enodetype.lisnode).RefreshUI();    }),
                new KeyValuePair<string, Action>("指定次数循环",()=>{  this.createNode(enodetype.loopnode).RefreshUI();    }),
                new KeyValuePair<string, Action>("指定条件循环",()=>{  this.createNode(enodetype.whilenode).RefreshUI();    }),
                new KeyValuePair<string, Action>("略过循环",()=>{  this.createNode(enodetype.continuenode).RefreshUI();    }),
                new KeyValuePair<string, Action>("跳出循环",()=>{  this.createNode(enodetype.breaknode).RefreshUI();    }),
                new KeyValuePair<string, Action>("执行逻辑",()=>{  this.createNode(enodetype.actionnode).RefreshUI();    }),
                new KeyValuePair<string, Action>("条件节点(取值bool)",()=>{  this.createNode(enodetype.conditionnode).RefreshUI();    }),
                new KeyValuePair<string, Action>("获取变量值",()=>{  this.createNode(enodetype.getbpvnode).RefreshUI();    }),
                new KeyValuePair<string, Action>("获取固定值",()=>{  this.createNode(enodetype.getcvnode).RefreshUI();    }),
                new KeyValuePair<string, Action>("获取函数值",()=>{  this.createNode(enodetype.getfvnode).RefreshUI();    }),
            });
            for (int i = 0; i < menuList.Count; i++)
            {
                menu.AddButton(menuList[i].Key, menuList[i].Value);
            }
        }

        public BPVariable GetVar(string key)
        {
            BPVariable var;
            if(this.allvars.TryGetValue(key, out var))
            {
                return var;
            }
            return null;
        }
        public void AddVar(string key, BPVariable var)
        {
            this.allvars[key] = var;
        }
        public void SetVar(string key, string var)
        {
            if (this.allvars.ContainsKey(key))
            {
                this.allvars[key].v = var;
            }
        }
        public void DeleteVar(string key)
        {
            this.allvars.Remove(key);
            foreach(var node in allNodes.Values)
            {
                if(node is GetBPVariableNodeUI)
                {
                    var data = node.data as Getbpvnodejson;
                    if(data.getkey == key)
                    {
                        data.getkey = "";
                        node.ClearLines();
                        node.RefreshUI();
                    }
                }
            }
        }
    }

    class RightMenu
    {
        private readonly RectTransform _root;
        private readonly MenuItem _oriItem;
        public RightMenu(RectTransform ui)
        {
            _root = ui;
            _oriItem = new MenuItem(ui.GetChild(0).GetComponent<RectTransform>());
            _oriItem.active = false;
        }
        public RectTransform root { get { return _root; } }

        public MenuItem AddButton(string name, Action callback = null)
        {
            var item = _oriItem.clone();
            item.text = name;
            item.onClick = callback;
            return item;
        }
        public Vector2 position
        {
            get { return _root.anchoredPosition; }
            set { _root.anchoredPosition = value; }
        }
        public bool active
        {
            get { return _root.gameObject.activeSelf; }
            set { _root.gameObject.SetActive(value); }
        }
    }
    class MenuItem
    {
        private readonly RectTransform m_root;
        private readonly Text _txt;
        private readonly GameObject _parent;

        public string text
        {
            get { return _txt.text; }
            set { _txt.text = value; }
        }
        public MenuItem(RectTransform ui)
        {
            m_root = ui;
            _txt = ui.GetChild(0).GetComponent<Text>();
            m_root.GetComponent<Button>().onClick.AddListener(() =>
            {
                if (this.onClick != null)
                {
                    this.onClick();
                }
                if (_parent != null)
                {
                    _parent.SetActive(false);
                }
            });
            _parent = m_root.parent.gameObject;
        }
        /// <summary>
        /// 克隆一个但不带点击回调的菜单按钮
        /// </summary>
        /// <returns></returns>
        public MenuItem clone()
        {
            var cloneObj = GameObject.Instantiate(m_root);
            cloneObj.SetParent(m_root.parent);
            var clone = new MenuItem(cloneObj);
            clone.onClick = null;
            clone.active = true;
            return clone;
        }
        public Action onClick = null;
        public bool active
        {
            get { return m_root.gameObject.activeSelf; }
            set { m_root.gameObject.SetActive(value); }
        }
    }
}
