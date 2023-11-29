using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace NodeUICtor.Ctor
{
    /// <summary>
    /// 链接点条件
    /// </summary>
    internal static class PointConcatFix
    {
        private static Dictionary<string, (string,int)> _pointType = new Dictionary<string, (string, int)>();
        public static string startFlag = "run";
        public static string noneFlag = "none";

        private static int[] _mask = new int[]
        {
            0b00000001,
            0b00000010,
            0b00000100,
            0b00001000,
            0b00010000,
            0b00100000,
            0b01000000,
            0b10000000
        };
        static PointConcatFix()
        {

        }

        private static bool checkPointLine(string k1, string k2)
        {

            var flag1 = _pointType[k1];
            var flag2 = _pointType[k2];
            return (flag1.Item1 == flag2.Item1 || flag1.Item1 == "any" || flag2.Item1 == "any") && flag1.Item2 != flag2.Item2;
        }

        private static void setNodeFlag(string k1, string v,bool isOut)
        {
            _pointType[k1] = (v,isOut?1:0);
        }

        private static Dictionary<Transform, TextMeshProUGUI> pointTxts = new Dictionary<Transform, TextMeshProUGUI>();
        public static void extSetNodeFlag(this INodeBaseUI ui, string k1, string v,bool isOut, Transform tra = null)
        {
            string key = $"{ui.data.guid}-{k1}";
            PointConcatFix.setNodeFlag(key, v,isOut);
            if (tra != null)
            {
                if(!pointTxts.TryGetValue(tra,out var txt))
                {
                    txt = GameObject.Instantiate( Resources.Load<GameObject>("TypeStr")).GetComponent<TextMeshProUGUI>();
                    pointTxts[tra] = txt;
                    txt.transform.SetParent(tra);
                    txt.rectTransform.anchoredPosition = Vector3.zero;
                }
                txt.text = v.Substring(0, Math.Min(v.Length, 4));
                var img = tra.GetComponent<Image>();
                if (img != null)
                {
                    img.sprite = null;
                    img.color = isOut? Color.red : Color.green;
                }
            }
        }
        public static void extRemoveNodeFlag(this INodeBaseUI ui, string k)
        {
            string key = $"{ui.data.guid}-{k}";
            PointConcatFix._pointType.Remove(key);
        }
        public static bool extCheckPointLine(this INodeBaseUI ui, string k1, INodeBaseUI target, string k2)
        {
            if (ui == target) { return false; }//不能自己连自己
            string key1 = $"{ui.data.guid}-{k1}";
            string key2 = $"{target.data.guid}-{k2}";
            return checkPointLine(key1, key2);
        }
    }


}
