using BPJsonType;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

namespace NodeUICtor.Ctor
{
    internal class GetFunctionValueNodeUI : FunctionNodeBase<Getfvnodejson>,IGetValueNode
    {
        public GetFunctionValueNodeUI(BPUI _bp, Getfvnodejson _d = null) : base(_bp, _d)
        {
        }
        private RectTransform _valuePoint;
        public RectTransform valuePoint { get { return this._valuePoint; } }
        private RectTransform _psroot;
        private Text _pstxt;
        private RectTransform _pspoint;
        private Dropdown _dropdopwn;

        private Dropdown _tydpUI;

        protected override Transform CreateView()
        {
            var root = GameObject.Instantiate(Resources.Load<RectTransform>("NodeUI/GetFunctionValueNode"));
            var list = root.Find("Conten/ParamsNodeList");
            _psroot = list.Find("ParameItem").GetComponent<RectTransform>();
            _pstxt = list.Find("ParameItem/psNameTxt").GetComponent<Text>();
            _pspoint = list.Find("ParameItem/Point").GetComponent<RectTransform>();
            _dropdopwn = root.Find("Conten/FunctionSelect").GetComponent<Dropdown>();
            _tydpUI = root.Find("Conten/RetTypeSelect").GetComponent <Dropdown>();
            var tys = NodeFuncJsonClass.allRetFunction_order.Keys.ToList();
            _tydpUI.ClearOptions();
            _tydpUI.AddOptions(tys);
            _tydpUI.onValueChanged.AddListener(n =>
            {
                string ty = tys[n];
                this.ondropdownTypeChange(ty);
                this.extSetNodeFlag("valuePoint", ty,true, _valuePoint);
            });
            linePoints["valuePoint"] = _valuePoint = root.Find("Conten/ValuePoint").GetComponent<RectTransform>();
            this.extSetNodeFlag("valuePoint", tys[0],true, _valuePoint);

            RegistPointClick("valuePoint");
            PreCreateView(tys[0]);
            return root;
        }
        public override void RefreshUI()
        {
            this.BuildParamsNodePoints();
            if (this.data.fpsnodes.Length > 0)
            {
                for(int i = 0;i< this.data.fpsnodes.Length;i++)
                {
                    var psname = this.data.f.paramsList[i];
                    var nodeguid = this.data.fpsnodes[i];
                    if(nodeguid == null) { continue; }
                    var node = bpui.allNodes[nodeguid];
                    this.DrawLine(psname, node, "valuePoint");
                }
            }
        }




        private List<string> _exnames = new List<string>() { "valuePoint" };
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
