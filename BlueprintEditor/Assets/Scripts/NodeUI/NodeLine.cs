using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

namespace NodeUICtor
{
    

    class NodeLine
    {
        class lineSaveInfo
        {
            public string k1;
            public string k2;
            public NodeLine line;
            public lineSaveInfo(string _k1,string _k2,NodeLine _line)
            {
                this.k1 = _k1;
                this.k2 = _k2;
                this.line = _line;
            }
        }

        private static List<lineSaveInfo> _lineDic = new List<lineSaveInfo>();

        private readonly Image _img;
        private NodeLine(RectTransform parent)
        {
            this._img = GameObject.Instantiate<Image>(Resources.Load<Image>("Image"));
            _img.transform.SetParent(parent);
        }

        //必然是个新建线
        private void SetLine(RectTransform r1, RectTransform r2, string guid1, string guid2)
        {
            _lineDic.Add(new lineSaveInfo(guid1, guid2, this));

            this.r1 = r1;
            this.r2 = r2;
            RefreshLine();
            this._img.gameObject.SetActive(true);
        }

        private RectTransform r1 = null;
        private RectTransform r2 = null;
        private void RefreshLine()
        {
            if (r1 == null || r2 == null) { return; }
            Vector2 p1 = r1.position;
            Vector2 p2 = r2.position;
            float distance = Vector2.Distance(p1, p2);
            float angle = Vector2.SignedAngle(p1 - p2, Vector2.left);
            var tra = _img.GetComponent<RectTransform>();
            tra.position = (p1 + p2) / 2;
            tra.sizeDelta = new Vector2(distance, 5);
            tra.localRotation = Quaternion.AngleAxis(-angle, Vector3.forward);
        }

        /// <summary>
        /// 标记被移动了的线
        /// </summary>
        public static Lists<string> lineMoveFlags = new Lists<string>();
        /// <summary>
        /// 标记被隐藏了的线
        /// </summary>
        public static Lists<string> lineHideFlags = new Lists<string>();
        /// <summary>
        /// 双端点判断移除线
        /// </summary>
        public static List<(string,string)> lineHideDoubleFlags = new List<(string,string)>();

        private static readonly Queue<NodeLine> _linePool = new Queue<NodeLine>();
        public static RectTransform mainPanel = null;
        public static void BuildLine(RectTransform r1, RectTransform r2, string guid1, string guid2)
        {
            NodeLine line = null;
            var find = _lineDic.Find(info => { return (info.k1 == guid1 && info.k2 == guid2) || (info.k2 == guid1 && info.k1 == guid2); });//先找一下已运行的线
            if (find !=null )//如果是个已运行的线
            {
                line = find.line;
                line.r1 = r1;
                line.r2 = r2;
                line.RefreshLine();
                return;
            }
            else if (_linePool.Count > 0)
            {
                line = _linePool.Dequeue();
            }
            else
            {
                line = new NodeLine(mainPanel);
            }
            line.SetLine(r1, r2, guid1, guid2);

        }

        private static void ForeachLines( string k,Action<NodeLine,int> callback)
        {
            for(int i = _lineDic.Count-1; i >=0; i--)
            {
                var info = _lineDic[i];
                if(info.k1 == k || info.k2 == k)
                {
                    callback(info.line, i);
                }
            }
        }
        private static void ForeachLines(string k1,string k2, Action<NodeLine,int> callback)
        {
            for (int i = _lineDic.Count-1; i >= 0; i--)
            {
                var info = _lineDic[i];
                if ((info.k1 == k1 && info.k2 == k2) || (info.k1 == k2 && info.k2 == k1))
                {
                    callback(info.line,i);
                }
            }
        }
        //静态构造函数 初始注册update事件
        static NodeLine()
        {
            ProgramMain.OnUpdate += (dt) =>
            {
                for (int i = 0; i < lineMoveFlags.Count; i++)
                {
                    string k = lineMoveFlags[i];
                    ForeachLines(k, (line,index) =>
                    {
                        line.RefreshLine();
                    });
                }
                lineMoveFlags.Clear();
                for (int i = 0; i < lineHideFlags.Count; i++)
                {
                    string k = lineHideFlags[i];
                    ForeachLines(k, (line,index) =>
                    {
                        line._img.gameObject.SetActive(false);

                        _lineDic.RemoveAt(index);
                        _linePool.Enqueue(line);
                    });
                }
                lineHideFlags.Clear();
                for (int i = 0; i < lineHideDoubleFlags.Count; i++)
                {
                    var k = lineHideDoubleFlags[i];
                    ForeachLines(k.Item1,k.Item2, (line,index) =>
                    {
                        line._img.gameObject.SetActive(false);
                        _lineDic.RemoveAt(index);
                        _linePool.Enqueue(line);
                    });
                }
                lineHideDoubleFlags.Clear();

            };
        }

        public class Lists<T>:List<T>
        {
            public void Adds(T item)
            {
                if (!this.Contains(item))
                {
                    this.Add(item);
                }
            }
        }
    }

}
