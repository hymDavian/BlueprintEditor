using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

namespace NodeUICtor.UI
{
    public class DragHandle : MonoBehaviour,IBeginDragHandler,IDragHandler,IEndDragHandler
    {

        public static DragHandle GetDragControl(GameObject g)
        {
            DragHandle ret = g.GetComponent<DragHandle>();
            if (ret == null)
            {
                ret = g.AddComponent<DragHandle>();
            }
            
            return ret;
        }

        public event Action OnDragBegin;
        public event Action<Vector3> OnDragTick;
        public event Action OnDragEnd;

        private Vector3 _offset;
        //private static Queue<KeyValuePair<Transform,Vector3>> dragQueue = new Queue<KeyValuePair<Transform,Vector3>>();
        void IBeginDragHandler.OnBeginDrag(PointerEventData eventData)
        {
            Vector3 beginPos = transform.position;
            _offset = Input.mousePosition - beginPos;
            //dragQueue.Enqueue(new KeyValuePair<Transform,Vector3>(this.transform,beginPos));//记录开始拖拽的位置
            if (OnDragBegin != null)
            {
                OnDragBegin.Invoke();
            }
        }

        void IDragHandler.OnDrag(PointerEventData eventData)
        {
            transform.position = Input.mousePosition - _offset;
            if (OnDragTick != null)
            {
                OnDragTick.Invoke(transform.position);
            }
        }

        void IEndDragHandler.OnEndDrag(PointerEventData eventData)
        {
            if (OnDragEnd != null)
            {
                OnDragEnd.Invoke();
            }
        }
    }

}
