using BPJsonType;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

namespace NodeUICtor.Ctor
{
    internal class TickNodeUI : NodeBase<Ticknodejson>
    {
        private RectTransform _openPoint;//打开时执行节点口
        private RectTransform _closePoint;//关闭时执行节点口
        private RectTransform _tickPoint;//帧运行节点口
        private Toggle _initrunUI;
        private Dropdown _csStateUI;

        public TickNodeUI(BPUI _bp, Ticknodejson _d = null) : base(_bp, _d)
        {
        }


        public override void RefreshUI()
        {
            _initrunUI.isOn = this.data.init == 1;
            _csStateUI.value = this.data.csstate;
            if (this.data.opennodes.Count > 0)
            {
                //连线到打开点
                this.data.opennodes.ForEach(guid =>
                {
                    var node = this.bpui.allNodes[guid];
                    this.DrawLine("openPoint", node, "startPoint");
                });
            }
            if(this.data.closenodes.Count > 0)
            {
                this.data.closenodes.ForEach(guid =>
                {
                    var node = this.bpui.allNodes[guid];
                    this.DrawLine("closePoint", node, "startPoint");
                });
            }
            if (this.data.ticknodes.Count > 0)
            {
                this.data.ticknodes.ForEach(guid =>
                {
                    var node = this.bpui.allNodes[guid];
                    this.DrawLine("tickPoint", node, "startPoint");
                });
            }
        }
        public override void OnLineSetData(string otherguid, string selfKey)
        {
            if (selfKey == "openPoint")
            {
                data.opennodes.Add(otherguid);
            }
            if (selfKey == "closePoint")
            {
                data.closenodes.Add(otherguid);
            }
            if (selfKey == "tickPoint")
            {
                data.ticknodes.Add(otherguid);
            }
        }

        private readonly List<string> _ops = new List<string>() { "客户端", "服务器", "双端" };
        protected override Transform CreateView()
        {
            var root = GameObject.Instantiate(Resources.Load<RectTransform>("NodeUI/TickNode"));
            _initrunUI = root.Find("Conten/InitRun").GetComponent<Toggle>();
            _initrunUI.onValueChanged.AddListener(v =>
            {
                this.data.init = v ? 1 : 0;
            });
            _csStateUI = root.Find("Conten/CSState").GetComponent<Dropdown>();
            _csStateUI.ClearOptions();
            _csStateUI.AddOptions(_ops);
            _csStateUI.onValueChanged.AddListener(n =>
            {
                this.data.csstate = n;
            });

            this._openPoint = root.Find("Conten/Open/OpenPoint").GetComponent<RectTransform>();
            this._tickPoint = root.Find("Conten/Tick/TickPoint").GetComponent<RectTransform>();
            this._closePoint = root.Find("Conten/Close/ClosePoint").GetComponent<RectTransform>();
            this.linePoints["openPoint"] = this._openPoint;
            this.linePoints["tickPoint"] = this._tickPoint;
            this.linePoints["closePoint"] = this._closePoint;
            this.extSetNodeFlag("openPoint", PointConcatFix.startFlag,true,_openPoint);
            this.extSetNodeFlag("tickPoint", PointConcatFix.startFlag,true,_tickPoint);
            this.extSetNodeFlag("closePoint", PointConcatFix.startFlag,true,_closePoint);
            RegistPointClick("openPoint");
            RegistPointClick("tickPoint");
            RegistPointClick("closePoint");
            return root;
        }
    }
}
