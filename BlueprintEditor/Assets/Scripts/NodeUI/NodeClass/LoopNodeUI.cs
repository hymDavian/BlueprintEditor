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
    internal class LoopNodeUI : NodeBase<Loopnodejson>, IRunNode
    {
        private RectTransform _startPoint;
        private RectTransform _runPoint;
        private InputField _inputUI;


        public LoopNodeUI(BPUI _bp, Loopnodejson _d = null) : base(_bp, _d)
        {
        }

        public RectTransform startPoint
        {
            get { return _startPoint; }
        }

        public override void RefreshUI()
        {
            _inputUI.text = data.loop.ToString();
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
                data.runnodes.Add(otherguid);
            }
        }

        protected override Transform CreateView()
        {
            var root = GameObject.Instantiate(Resources.Load<RectTransform>("NodeUI/LoopNode"));

            this._startPoint = root.Find("Conten/StartPoint").GetComponent<RectTransform>();
            this._runPoint = root.Find("Conten/RunPoint").GetComponent<RectTransform>();
            _inputUI = root.Find("Conten/InputLoopNum").GetComponent<InputField>();
            _inputUI.onValueChanged.AddListener(s =>
            {
                int n = 1;
                int.TryParse(s, out n);
                this.data.loop = n;
            });


            this.linePoints["startPoint"] = this._startPoint;
            this.linePoints["runPoint"] = this._runPoint;
            this.extSetNodeFlag("startPoint", PointConcatFix.startFlag,false, _startPoint);
            this.extSetNodeFlag("runPoint", PointConcatFix.startFlag,true,_runPoint);
            RegistPointClick("startPoint");
            RegistPointClick("runPoint");
            return root;
        }
    }
}
