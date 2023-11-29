using BPJsonType;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEditor.Experimental.GraphView;
using UnityEngine;

namespace BPJsonType
{
    enum enodetype
    {
        ticknode, lisnode, whilenode, loopnode, getbpvnode, getcvnode, getfvnode, conditionnode, actionnode, breaknode, continuenode
    }
    enum evartypes
    {
        stringValue, numberValue, booleanValue
    }
    [Serializable]
    class NodePosition
    {
        public double x = 0;
        public double y = 0;
        public Vector2 ToUnityPos2()
        {
            return new Vector2((float)x, (float)y);
        }
        public void GetXY(Vector2 pos)
        {
            this.x = pos.x;
            this.y = pos.y;
        }
    }
    [Serializable]
    internal abstract class INodejson
    {
        public NodePosition position { get; set; } = new NodePosition();
        public string guid { get; set; }
        public enodetype type { get; set; }

        public abstract enodetype getNodeType();

        protected abstract void DefaultData();
        public INodejson() 
        {
            DefaultData();
        }

    }
    [Serializable]
    internal class Ticknodejson : INodejson
    {
        public List<string> opennodes { get; set; }
        public List<string> closenodes { get; set; }
        public List<string> ticknodes { get; set; }
        public int init { get; set; }//是否初始化运作，0:false
        public int csstate { get; set; }//0客户端1服务器2双端



        public override enodetype getNodeType()
        {
            return enodetype.ticknode;
        }

        protected override void DefaultData()
        {
            this.opennodes = new List<string>();
            this.closenodes = new List<string>();
            this.ticknodes = new List<string>();
            this.init = 0;
            this.csstate = 0;
        }
    }
    [Serializable]
    internal class Lisnodejson : INodejson
    {
        public List<string> runnodes { get; set; }
        public string liskey { get; set; }
        public int init { get; set; }//是否初始化运作，0:false
        public int csstate { get; set; }//0客户端1服务器2双端

        public override enodetype getNodeType()
        {
            return enodetype.lisnode;
        }

        protected override void DefaultData()
        {
            runnodes = new List<string>();
            liskey = "";
            this.init = 0;
            this.csstate = 0;
        }
    }
    [Serializable]
    internal class Whilenodejson : INodejson //irun
    {
        public string conditionnode { get; set; }
        public List<string> runnodes { get; set; }
        public override enodetype getNodeType()
        {
            return enodetype.whilenode;
        }

        protected override void DefaultData()
        {
            conditionnode = "";
            runnodes = new List<string>();
        }
    }
    [Serializable]
    internal class Loopnodejson : INodejson //irun
    {
        public int loop { get; set; }
        public List<string> runnodes { get; set; }
        public override enodetype getNodeType()
        {
            return enodetype.loopnode;
        }

        protected override void DefaultData()
        {
            loop = 1;
            runnodes = new List<string>();
        }
    }
    [Serializable]
    internal class Getbpvnodejson : INodejson //valuePoint
    {
        public string getkey { get; set; }
        public override enodetype getNodeType()
        {
            return enodetype.getbpvnode;
        }

        protected override void DefaultData()
        {
            getkey = "";
        }
    }
    [Serializable]
    internal class Getcvnodejson : INodejson //valuePoint
    {
        public string v { get; set; }
        public evartypes t { get; set; }
        public override enodetype getNodeType()
        {
            return enodetype.getcvnode;
        }

        protected override void DefaultData()
        {
            v = "";
            t = evartypes.stringValue;
        }
    }
    [Serializable]
    internal class Getfvnodejson : INodejson //valuePoint
    {
        public FuncDescription f { get; set; }
        public string[] fpsnodes { get; set; }
        public override enodetype getNodeType()
        {
            return enodetype.getfvnode;
        }

        protected override void DefaultData()
        {
            f = new FuncDescription();
            fpsnodes = new string[0];
        }
    }
    [Serializable]
    internal class Conditionnodejson : Getfvnodejson //irun valuePoint
    {
        public List<string> truenodes { get; set; }
        public List<string> falsenodes { get; set; }
        public override enodetype getNodeType()
        {
            return enodetype.conditionnode;
        }
        protected override void DefaultData()
        {
            base.DefaultData();
            truenodes = new List<string>();
            falsenodes = new List<string>();
        }
    }
    [Serializable]
    internal class Actionnodejson : Getfvnodejson //irun
    {
        public override enodetype getNodeType()
        {
            return enodetype.actionnode;
        }
    }
    [Serializable]
    internal class Breaknodejson : INodejson //irun
    {
        public override enodetype getNodeType()
        {
            return enodetype.breaknode;
        }

        protected override void DefaultData()
        {
            ;
        }
    }
    [Serializable]
    internal class Continuenodejson : INodejson //irun
    {
        public override enodetype getNodeType()
        {
            return enodetype.continuenode;
        }

        protected override void DefaultData()
        {
            ;
        }
    }
    [Serializable]
    internal class BPVariable
    {
        public string k { get; set; }
        public string v { get; set; }
        public evartypes t { get; set; }
    }
    [Serializable]
    internal class BPjson
    {
        public List<INodejson> allnodes { get; set; }
        public List<string> ticks { get; set; }
        public List<string> liss { get; set; }
        public List<BPVariable> vars { get; set; }

        public BPjson()
        {
            allnodes = new List<INodejson>();
            ticks = new List<string>();
            liss = new List<string>();
            vars = new List<BPVariable>();
        }
    }
}

