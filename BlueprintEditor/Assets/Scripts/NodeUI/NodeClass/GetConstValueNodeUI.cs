using BPJsonType;
using FairyGUI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

namespace NodeUICtor.Ctor
{
    internal class GetConstValueNodeUI : NodeBase<Getcvnodejson>,IGetValueNode
    {
        public GetConstValueNodeUI(BPUI _bp, Getcvnodejson _d = null) : base(_bp, _d)
        {
        }

        private RectTransform _valuePoint;
        private InputField _inputValueUI;
        private Dropdown _selectTypeUI;



        public RectTransform valuePoint { get { return _valuePoint; } }
        private readonly List<string> types = new List<string>() {"string","number","bool" };
        protected override Transform CreateView()
        {
            var root = GameObject.Instantiate(Resources.Load<RectTransform>("NodeUI/GetConstValueNode"));
            linePoints["valuePoint"] = _valuePoint = root.Find("Conten/ValuePoint").GetComponent<RectTransform>();
            RegistPointClick("valuePoint");
            _inputValueUI = root.Find("Conten/InputValue").GetComponent<InputField>();
            _inputValueUI.onValueChanged.AddListener(s =>
            {
                this.data.v = s;
            });
            _selectTypeUI = root.Find("Conten/SelectType").GetComponent<Dropdown>();
            _selectTypeUI.ClearOptions();
            _selectTypeUI.AddOptions(types);
            _selectTypeUI.onValueChanged.AddListener(n =>
            {
                this.data.t = (evartypes)n;
                switch (data.t)
                {
                    case evartypes.stringValue: this.extSetNodeFlag("valuePoint", "string",true, _valuePoint); break;
                    case evartypes.numberValue: this.extSetNodeFlag("valuePoint", "number",true, _valuePoint); break;
                    case evartypes.booleanValue: this.extSetNodeFlag("valuePoint", "boolean",true, _valuePoint); break;
                }
                ClearLines();
            });
            this.extSetNodeFlag("valuePoint", "string",true, _valuePoint);
            return root;
        }
        public override void RefreshUI()
        {
            _inputValueUI.text = data.v;
            _selectTypeUI.value = (int)data.t;
            return;
        }


    }
}
