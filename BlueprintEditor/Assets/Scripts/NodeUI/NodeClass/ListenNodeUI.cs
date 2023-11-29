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
    internal class ListenNodeUI : NodeBase<Lisnodejson>
    {
        private RectTransform _runPoint;//监听运行的节点
        private Toggle _initrunUI;
        private Dropdown _csStateUI;
        private InputField _lisKeyUI;


        public ListenNodeUI(BPUI _bp, Lisnodejson _d = null) : base(_bp, _d)
        {
        }
        private readonly List<string> _ops = new List<string>() { "客户端", "服务器", "双端" };
        protected override Transform CreateView()
        {
            var root = GameObject.Instantiate(Resources.Load<RectTransform>("NodeUI/ListenNode"));
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
            _lisKeyUI = root.Find("Conten/InputListenKey").GetComponent<InputField>();
            _lisKeyUI.onValueChanged.AddListener(s =>
            {
                this.data.liskey = s;
            });

            this._runPoint = root.Find("Conten/Run/RunPoint").GetComponent<RectTransform>();
            this.linePoints["runPoint"] = this._runPoint;
            this.extSetNodeFlag("runPoint", PointConcatFix.startFlag,true, _runPoint);
            RegistPointClick("runPoint");
            return root;
        }
        public override void RefreshUI()
        {
            _initrunUI.isOn = this.data.init == 1;
            _csStateUI.value = this.data.csstate;
            _lisKeyUI.text = this.data.liskey;
            if (this.data.runnodes.Count > 0)
            {
                this.data.runnodes.ForEach(guid =>
                {
                    var node = this.bpui.allNodes[guid];
                    this.DrawLine("runPoint", node, "startPoint");
                });
            }
        }

        public override void OnLineSetData(string otherguid, string selfKey)
        {
            if(selfKey == "runPoint")
            {
                this.data.runnodes.Add(otherguid);
            }
        }


    }
}
