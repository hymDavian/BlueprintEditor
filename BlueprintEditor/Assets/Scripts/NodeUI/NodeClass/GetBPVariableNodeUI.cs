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
    internal class GetBPVariableNodeUI : NodeBase<Getbpvnodejson>,IGetValueNode
    {
        private RectTransform _valuePoint;
        private InputField _inputVarNameUI;

        public GetBPVariableNodeUI(BPUI _bp, Getbpvnodejson _d = null) : base(_bp, _d)
        {
        }


        public RectTransform valuePoint { get { return _valuePoint; } }
        protected override Transform CreateView()
        {
            var root = GameObject.Instantiate(Resources.Load<RectTransform>("NodeUI/GetBPVariableNode"));
            linePoints["valuePoint"] = _valuePoint = root.Find("Conten/ValuePoint").GetComponent<RectTransform>();
            RegistPointClick("valuePoint");
            _inputVarNameUI = root.Find("Conten/InputVarName").GetComponent<InputField>();
            _inputVarNameUI.onValueChanged.AddListener(s =>
            {
                this.data.getkey = s;
                var value = bpui.GetVar(s);
                if (value!=null)
                {
                    switch (value.t)
                    {
                        case evartypes.stringValue: this.extSetNodeFlag("valuePoint", "string",true, _valuePoint); break;
                        case evartypes.numberValue: this.extSetNodeFlag("valuePoint", "number",true, _valuePoint); break;
                        case evartypes.booleanValue: this.extSetNodeFlag("valuePoint", "boolean",true, _valuePoint); break;
                        default: this.extSetNodeFlag("valuePoint", PointConcatFix.noneFlag,true, _valuePoint); break;
                    }
                }
                else
                {
                    this.extSetNodeFlag("valuePoint", PointConcatFix.noneFlag,true, _valuePoint);
                }
        
                ClearLines();
                
            });
            this.extSetNodeFlag("valuePoint", "string",true, _valuePoint);
            return root;
        }
        public override void RefreshUI()
        {
            _inputVarNameUI.text = this.data.getkey;
            return;//取值节点不会链接到其他节点，只会被其他节点链接
        }
        


    }
}
