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
    internal class ActionNodeUI : FunctionNodeBase<Actionnodejson>, IRunNode
    {
        public ActionNodeUI(BPUI _bp, Actionnodejson _d = null) : base(_bp, _d)
        {
        }
        private RectTransform _startPoint;
        public RectTransform startPoint { get { return _startPoint; } }

        private RectTransform _psroot;
        private Text _pstxt;
        private RectTransform _pspoint;
        private Dropdown _dropdopwn;
     


        protected override Transform CreateView()
        {
            var root = GameObject.Instantiate(Resources.Load<RectTransform>("NodeUI/ActionNode"));
            var list = root.Find("Conten/ParamsNodeList");
            _psroot = list.Find("ParameItem").GetComponent<RectTransform>();
            _pstxt = list.Find("ParameItem/psNameTxt").GetComponent<Text>();
            _pspoint = list.Find("ParameItem/Point").GetComponent<RectTransform>();
            _dropdopwn = root.Find("Conten/FunctionSelect").GetComponent<Dropdown>(); 
            this._startPoint = linePoints["startPoint"] = root.Find("Conten/StartPoint").GetComponent<RectTransform>();
            this.extSetNodeFlag("startPoint", PointConcatFix.startFlag,false, _startPoint);
            RegistPointClick("startPoint");
            PreCreateView("void");
            return root;
        }
        public override void RefreshUI()
        {
            this.BuildParamsNodePoints();
            if (data.fpsnodes.Length > 0)
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
        }

        private List<string> _exnames = new List<string>() { "startPoint" };
        protected override List<string> excludeKeys
        {
            get { return _exnames; }
        }

        protected override (RectTransform root, Text txt, RectTransform point) _itemClone
        {
            get
            {
                return (_psroot,_pstxt,_pspoint );
            }
        }
        protected override Dropdown dropdownUI { get { return _dropdopwn; } }
    }
}
