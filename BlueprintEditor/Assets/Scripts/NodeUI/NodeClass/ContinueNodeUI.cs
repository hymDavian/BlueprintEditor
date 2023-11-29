using BPJsonType;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace NodeUICtor.Ctor
{
    internal class ContinueNodeUI : NodeBase<Continuenodejson>, IRunNode
    {
        public ContinueNodeUI(BPUI _bp, Continuenodejson _d = null) : base(_bp, _d)
        {
        }
        private RectTransform _startPoint;
        public RectTransform startPoint { get { return _startPoint; } }

        public override void RefreshUI()
        {
            return;
        }

        protected override Transform CreateView()
        {
            var root = GameObject.Instantiate(Resources.Load<RectTransform>("NodeUI/ContinueNode"));
            linePoints["startPoint"] = _startPoint = root.Find("Conten/StartPoint").GetComponent<RectTransform>();
            this.extSetNodeFlag("startPoint", PointConcatFix.startFlag,false, _startPoint);
            RegistPointClick("startPoint");
            return root;
        }
    }
}