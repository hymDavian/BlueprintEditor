using BPJsonType;
using NodeUICtor;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace NodeUICtor.Ctor
{
    internal class WhileNodeUI : NodeBase<Whilenodejson>, IRunNode
    {
        public WhileNodeUI(BPUI _bp, Whilenodejson _d = null) : base(_bp, _d)
        {
        }

        private RectTransform _startPoint;

        public RectTransform startPoint
        {
            get { return _startPoint; }
        }

        public override void RefreshUI()
        {
            if (this.data.runnodes.Count>0)
            {
                this.data.runnodes.ForEach(guid =>
                {
                    var node = this.bpui.allNodes[guid];
                    this.DrawLine("runPoint", node, "startPoint");
                });
            }
            if (!String.IsNullOrEmpty(this.data.conditionnode))
            {
                var node = this.bpui.allNodes[this.data.conditionnode];
                this.DrawLine("conditionPoint", node, "valuePoint");
            }
        }
        public override void OnLineSetData(string otherguid, string selfKey)
        {
            if(selfKey == "runPoint")
            {
                data.runnodes.Add(otherguid);
            }
            if(selfKey == "conditionPoint")
            {
                data.conditionnode = otherguid;
            }
        }

        protected override Transform CreateView()
        {
            var root = GameObject.Instantiate(Resources.Load<RectTransform>("NodeUI/WhileNode"));
            
            this._startPoint = this.linePoints["startPoint"] = root.Find("Conten/StartPoint").GetComponent<RectTransform>();
            var _conditionpoint = this.linePoints["conditionPoint"] = root.Find("Conten/ConditionPoint").GetComponent<RectTransform>();
            var _runpoint = this.linePoints["runPoint"] = root.Find("Conten/RunPoint").GetComponent<RectTransform>();
            this.extSetNodeFlag("startPoint", PointConcatFix.startFlag,false,_startPoint);
            this.extSetNodeFlag("conditionPoint", "boolean",false,_conditionpoint);
            this.extSetNodeFlag("runPoint", PointConcatFix.startFlag,true,_runpoint);
            RegistPointClick("startPoint");
            RegistPointClick("conditionPoint");
            RegistPointClick("runPoint");
            return root;
        }
    }
}
