using BPJsonType;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

namespace NodeUICtor.Ctor
{
    internal class ConditionUI : FunctionNodeBase<Conditionnodejson>, IRunNode, IGetValueNode                 
    {
        public ConditionUI(BPUI _bp, Conditionnodejson _d = null) : base(_bp, _d)
        {
        }
        private RectTransform _valuePoint;
        public RectTransform valuePoint { get { return this._valuePoint; } }

        private RectTransform _startPoint;
        public RectTransform startPoint{get { return _startPoint; }}
        private RectTransform _truePoint;
        private RectTransform _falsePoint;
        private RectTransform _psroot;
        private Text _pstxt;
        private RectTransform _pspoint;
        private Dropdown _dropdopwn;

        protected override Transform CreateView()
        {
            var root = GameObject.Instantiate(Resources.Load<RectTransform>("NodeUI/ConditionNode"));
            var list = root.Find("Conten/ParamsNodeList");
            _psroot = list.Find("ParameItem").GetComponent<RectTransform>();
            _pstxt = list.Find("ParameItem/psNameTxt").GetComponent<Text>();
            _pspoint = list.Find("ParameItem/Point").GetComponent<RectTransform>();
            _dropdopwn = root.Find("Conten/FunctionSelect").GetComponent<Dropdown>();

            linePoints["valuePoint"] = _valuePoint = root.Find("Conten/ValuePoint").GetComponent<RectTransform>();
            linePoints["startPoint"] = _startPoint = root.Find("Conten/StartPoint").GetComponent<RectTransform>();
            linePoints["truePoint"] = _truePoint = root.Find("Conten/True/TruePoint").GetComponent<RectTransform>();
            linePoints["falsePoint"] = _falsePoint = root.Find("Conten/False/FalsePoint").GetComponent<RectTransform>();
            PreCreateView("boolean");

            
            this.extSetNodeFlag("valuePoint", "boolean",true, _valuePoint);
            this.extSetNodeFlag("startPoint", PointConcatFix.startFlag,false, _startPoint);
            this.extSetNodeFlag("truePoint", PointConcatFix.startFlag,true, _truePoint);
            this.extSetNodeFlag("falsePoint", PointConcatFix.startFlag,true,_falsePoint);
            RegistPointClick("valuePoint");
            RegistPointClick("startPoint");
            RegistPointClick("truePoint");
            RegistPointClick("falsePoint");
            return root;
        }

        public override void OnLineSetData(string otherguid, string selfKey)
        {
            base.OnLineSetData(otherguid, selfKey);
            if(selfKey == "truePoint")
            {
                data.truenodes.Add(otherguid);
            }
            if (selfKey == "falsePoint")
            {
                data.falsenodes.Add(otherguid);
            }
        }
        public override void RefreshUI()
        {
            this.BuildParamsNodePoints();
            if (this.data.fpsnodes.Length > 0)
            {
                for (int i = 0; i < this.data.fpsnodes.Length; i++)
                {
                    var psname = this.data.f.paramsList[i];
                    var nodeguid = this.data.fpsnodes[i];
                    if (nodeguid == null) { continue; }
                    var node = bpui.allNodes[nodeguid];
                    this.DrawLine(psname, node, "valuePoint");
                }
            }
            if (this.data.truenodes.Count > 0)
            {
                for(int i = 0; i < data.truenodes.Count; i++)
                {
                    var guid = data.truenodes[i];
                    var node = bpui.allNodes[guid];
                    DrawLine("truePoint", node, "startPoint");
                }
            }
            if (this.data.falsenodes.Count > 0)
            {
                for (int i = 0; i < data.falsenodes.Count; i++)
                {
                    var guid = data.falsenodes[i];
                    var node = bpui.allNodes[guid];
                    DrawLine("falsePoint", node, "startPoint");
                }
            }
        }
        private List<string> _exnames = new List<string>() { "valuePoint", "startPoint", "truePoint", "falsePoint" };
        protected override List<string> excludeKeys
        {
            get { return _exnames; }
        }
        protected override (RectTransform root, Text txt, RectTransform point) _itemClone
        {
            get
            {
                return (_psroot, _pstxt, _pspoint);
            }
        }
        protected override Dropdown dropdownUI { get { return _dropdopwn; } }


    }


}
