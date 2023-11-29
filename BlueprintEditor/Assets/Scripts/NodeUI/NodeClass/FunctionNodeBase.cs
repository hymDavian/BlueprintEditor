using BPJsonType;
using NodeUICtor;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.Versioning;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

namespace NodeUICtor.Ctor
{
    internal abstract class FunctionNodeBase<T> : NodeBase<T>
                where T : Getfvnodejson, new()
    {
        protected FunctionNodeBase(BPUI _bp, T _d = default) : base(_bp, _d)
        {

        }
        /// <summary>
        /// 下拉UI
        /// </summary>
        protected abstract Dropdown dropdownUI { get; }
        /// <summary>
        /// 克隆源
        /// </summary>
        protected abstract (RectTransform root, Text txt, RectTransform point) _itemClone { get; }

        private string _tempDptype = "void";//默认选返回void的函数
        /// <summary>
        /// 重新构建参数节点时排除的节点名称
        /// </summary>
        protected abstract List<string> excludeKeys { get; }

        /// <summary>
        /// 当选择需要的函数返回类型变更时调用,需要子类主动调用
        /// </summary>
        protected void ondropdownTypeChange(string toType)
        {
            _tempDptype = toType;
            dropdownUI.value = 0;
            dropdownUI.onValueChanged.RemoveAllListeners();
            dropdownUI.ClearOptions();
            _tempfslist = NodeFuncJsonClass.allRetFunction_order[this._tempDptype];
            var fslistname = _tempfslist.ConvertAll<string>(val => { return val.funcdes; });
            dropdownUI.AddOptions(fslistname);
            dropdownUI.onValueChanged.AddListener(DpOnvalueChange);
            DpOnvalueChange(0);

        }
        private List<FuncDescription> _tempfslist = null;
        private void DpOnvalueChange(int n)
        {
            this.data.f = _tempfslist[n];
            this.data.fpsnodes = new string[_tempfslist[n].paramsList.Count];
            ClearLines();//重置使用函数时候需要清理所有之前的连线
            RefreshUI();
        }


        protected void PreCreateView(string retty)
        {
            ondropdownTypeChange(retty);
            _itemClone.root.gameObject.SetActive(false);
        }

        private List<string> _tempRemoveKeys = new List<string>();
        private Queue<NodeParamsPoint> _pool = new Queue<NodeParamsPoint>();


        public override void OnLineSetData(string otherguid, string selfKey)
        {

            int index = -1;
            for (int i = 0; i < this.data.f.paramsList.Count; i++)
            {
                if (this.data.f.paramsList[i] == selfKey)
                {
                    index = i; break;
                }
            }
            if (index >= 0)
            {

                this.data.fpsnodes[index] = otherguid;
            }
        }



        //创建新参数节点
        private NodeParamsPoint _BuildNewPsPoint()
        {
            var c_root = _itemClone.root;
            var c_txt = _itemClone.txt;
            var c_point = _itemClone.point;


            var root = GameObject.Instantiate(c_root, c_root.parent);
            var txt = root.Find(c_txt.name).GetComponent<Text>();
            var point = root.Find(c_point.name).GetComponent<RectTransform>();
            return new NodeParamsPoint(root, txt, point);
        }
        //隐藏不用的参数节点
        private void _HidePsPoints(NodeParamsPoint point)
        {
            point.active = false;
        }
        //参数节点映射集
        private Dictionary<string, NodeParamsPoint> _psnodes = new Dictionary<string, NodeParamsPoint>();
        /// <summary>
        /// 根据当前使用函数信息生成参数节点UI
        /// </summary>
        protected void BuildParamsNodePoints()
        {
            
            _tempRemoveKeys.Clear();
            foreach (var v in this.linePoints)
            {
                if (!excludeKeys.Contains(v.Key))
                {
                    _tempRemoveKeys.Add(v.Key);
                }
            }
            _tempRemoveKeys.ForEach(uikey =>
            {
                var nodetra = linePoints[uikey];
                var psnode = _psnodes[uikey];
                _HidePsPoints(psnode);
                _psnodes.Remove(uikey);
                linePoints.Remove(uikey);
                _pool.Enqueue(psnode);
                this.extRemoveNodeFlag(uikey);
            });
            var pslist = this.data.f.paramsList;
            for (int i = 0; i < pslist.Count; i++)
            {
                int index = i;
                var psname = pslist[i];
                var point = _pool.Count > 0 ? _pool.Dequeue() : _BuildNewPsPoint();
                point.txt.text = psname;
                point.active = true;
                point.root.SetAsLastSibling();
                _psnodes[psname] = point;
                linePoints[psname] = point.point;
                this.extSetNodeFlag(psname, this.data.f.paramsTypes[i],false,point.point);
                RegistPointClick(psname);
            }
        }
    }

    class NodeParamsPoint
    {
        public readonly RectTransform root;
        public readonly Text txt;
        public readonly RectTransform point;
        public NodeParamsPoint(RectTransform root, Text txt, RectTransform point)
        {
            this.root = root;
            this.txt = txt;
            this.point = point;
            txt.alignment = TextAnchor.MiddleCenter;
        }

        public bool active
        {
            get { return root.gameObject.activeSelf; }
            set { root.gameObject.SetActive(value); }
        }
    }
}


