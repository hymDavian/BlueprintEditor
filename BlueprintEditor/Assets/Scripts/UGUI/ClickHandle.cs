using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace NodeUICtor.UI
{
    internal class ClickHandle : MonoBehaviour, IPointerClickHandler
    {
        public static (string,INodeBaseUI,RectTransform) beforeClick = (null,null,null);
        private static Dictionary<RectTransform, GameObject> clickFlagImg = new Dictionary<RectTransform, GameObject>();
        public static void SetIsClickFlag(RectTransform point, bool click)
        {
            var flag = GetClickFlag(point);
            flag.SetActive(click);
        }
        private static GameObject GetClickFlag(RectTransform point)
        {
            GameObject g = null;
            if (!clickFlagImg.ContainsKey(point))
            {
                g = GameObject.Instantiate(Resources.Load<Image>("clickFlag")).gameObject;
                g.transform.SetParent(point);
                g.GetComponent<RectTransform>().anchoredPosition = new Vector2(0, 0);
                clickFlagImg[point] = g;
            }
            else
            {
                g = clickFlagImg[point];
            }
            return g;
        }
        public static ClickHandle GetClickHandle(GameObject g)
        {
            ClickHandle ret = g.GetComponent<ClickHandle>();
            if(ret == null)
            {
                ret = g.AddComponent<ClickHandle>();
            }
            return ret;
        }

        public Action OnClick;


        void IPointerClickHandler.OnPointerClick(PointerEventData eventData)
        {
            Debug.Log("点击了" + this.name);
            if(OnClick != null)
            {
                OnClick.Invoke();
            }
        }
    }
}
