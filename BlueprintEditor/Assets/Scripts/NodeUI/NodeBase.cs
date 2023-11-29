using BPJsonType;
using NodeUICtor.Ctor;
using NodeUICtor.UI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;


namespace NodeUICtor
{
    interface INodeBaseUI
    {
        /// <summary>
        /// 设置显示对象的坐标
        /// </summary>
        public Transform tra { get; }
        /// <summary>
        /// UI的transform
        /// </summary>
        public RectTransform rectTra { get; }
        /// <summary>
        /// 刷新UI
        /// </summary>
        public void RefreshUI();

        public INodejson data {  get; }
        /// <summary>
        /// 连线UI点
        /// </summary>
        public Dictionary<string, RectTransform> linePoints { get; }

        public void OnLineSetData(string otherguid, string selfKey);
        /// <summary>
        /// 删除自身UI
        /// </summary>
        public void Destory();
        /// <summary>
        /// 清理自身所有连线
        /// </summary>
        public void ClearLines();
    }
    /// <summary>
    /// 节点UI基类
    /// </summary>
    internal abstract class NodeBase<T> : INodeBaseUI
        where T : INodejson, new()
    {
        private readonly Transform _tra;
        private readonly RectTransform _rectTra;
        private readonly T _data;
        /// <summary>
        /// 自身数据对象
        /// </summary>
        public T data { get { return _data; } }
        INodejson INodeBaseUI.data {  get { return _data; } }
        public readonly BPUI bpui;
        public NodeBase(BPUI _bp, T _d = null)
        {
            this.bpui = _bp;
            if (_d == null)
            {
                this._data = new T();
                this._data.guid = Guid.NewGuid().ToString();
                this._data.type = this._data.getNodeType();
            }
            else
            {
                this._data = _d;
            }
            this._tra = this.CreateView();
            var deleteBtn = _tra.Find("Delete").gameObject.GetComponent<Button>();
            deleteBtn.image.color = Color.red;
            deleteBtn.onClick.AddListener(() =>
            {
                bpui.DeleteNode(this.data.guid);
            });
            this._rectTra = this.tra.GetComponent<RectTransform>();
            DragHandle.GetDragControl(this._tra.gameObject).OnDragEnd += ()=>
            {
                this.RefreshUI();
                foreach(string s in linePoints.Keys)
                {
                    NodeLine.lineMoveFlags.Adds(this.data.guid+ s);
                }
                this.data.position.GetXY(this._rectTra.anchoredPosition);
            };
        }


        public Transform tra { get { return _tra; } }
        public RectTransform rectTra { get { return _rectTra; } }
        /// <summary>
        /// 创建UI显示对象
        /// </summary>
        protected abstract Transform CreateView();
        public abstract void RefreshUI();


        private readonly Dictionary<string,RectTransform> _linePoints = new Dictionary<string,RectTransform>();
        public Dictionary<string, RectTransform> linePoints { get { return this._linePoints; } }



        /// <summary>
        /// 创建一条线到目标节点
        /// </summary>
        /// <param name="selflineFlag">自身出线口</param>
        /// <param name="toNodeUI">目标节点</param>
        protected void DrawLine(string selflineFlag,INodeBaseUI toNodeUI,string toLineFlag)
        {
            RectTransform r1 = this.linePoints[selflineFlag];
            RectTransform r2 = toNodeUI.linePoints[toLineFlag];
            if (r1==null || r2 == null)
            {
                return;
            }
            NodeLine.BuildLine(r1, r2, this.data.guid+ selflineFlag, toNodeUI.data.guid+toLineFlag);
        }
        public void ClearLines()
        {
            foreach (string s in linePoints.Keys)
            {
                NodeLine.lineHideFlags.Adds(this.data.guid + s);
            }
        }
        protected void RegistPointClick(string key)
        {
            var point = linePoints[key];
            if(point == null) { return; }
            var click = ClickHandle.GetClickHandle(point.gameObject);
            click.OnClick = () =>
            {
                if (ClickHandle.beforeClick.Item2 == null)
                {
                    ClickHandle.beforeClick = (key,this,point);
                    ClickHandle.SetIsClickFlag(point, true);
                }
                else
                {
                    bool check = this.extCheckPointLine(key, ClickHandle.beforeClick.Item2, ClickHandle.beforeClick.Item1);
                    if (check)
                    {
                        this.OnLineSetData(ClickHandle.beforeClick.Item2.data.guid, key);
                        ClickHandle.beforeClick.Item2.OnLineSetData(this.data.guid, ClickHandle.beforeClick.Item1);
                        this.DrawLine(key, ClickHandle.beforeClick.Item2, ClickHandle.beforeClick.Item1);

                    }
                    ClickHandle.SetIsClickFlag(point, false);
                    ClickHandle.SetIsClickFlag(ClickHandle.beforeClick.Item3, false);
                    ClickHandle.beforeClick = (null,null,null);
                }
            };
        }

        /// <summary>
        /// 某个外部节点连接到了自身的某个接收点
        /// </summary>
        public virtual void OnLineSetData(string otherguid,string selfKey ) { }

        public void Destory()
        {
            this.ClearLines();
            GameObject.Destroy(this.tra.gameObject);
        }

    }

    interface IRunNode
    {
        /// <summary>
        /// 被外部驱动的脉冲入口
        /// </summary>
        public RectTransform startPoint { get; }
    }

    interface IGetValueNode
    {
        /// <summary>
        /// 外部接收值的出口
        /// </summary>
        public RectTransform valuePoint { get; }
    }

    

}
