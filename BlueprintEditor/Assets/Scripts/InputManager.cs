using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace InputMgr
{
    public enum EKeyEventCtor
    {
        down, up, tick
    }
    public class InputManager : MonoBehaviour
    {
        private readonly List<KeyCode> keys = new List<KeyCode>();
        private readonly List<InputInfo> infos = new List<InputInfo>();
        /// <summary>
        /// Êó±êÊÂ¼þ
        /// </summary>
        public event Action<int> onMouseCursorClick;

        private static InputManager _ins;
        public static InputManager instance
        {
            get
            {
                if (_ins == null)
                {
                    var g = new GameObject("InputManager");
                    _ins = g.AddComponent<InputManager>();
                }
                return _ins;
            }
        }

        private int loopIndex = 0;

        // Update is called once per frame
        void Update()
        {
            for (; loopIndex < keys.Count; loopIndex++)
            {
                var info = infos[loopIndex];
                info.excute();
            }
            loopIndex = 0;

            if (Input.GetMouseButtonUp(0) && onMouseCursorClick!=null)
            {
                onMouseCursorClick.Invoke(0);
            }
            if (Input.GetMouseButtonUp(1) && onMouseCursorClick != null)
            {
                onMouseCursorClick.Invoke(1);
            }
            if (Input.GetMouseButtonUp(2) && onMouseCursorClick != null)
            {
                onMouseCursorClick.Invoke(2);
            }
        }

        public void OnKeyEvent(KeyCode key, bool add, Action callback, EKeyEventCtor st)
        {
            InputInfo info = null;
            if (this.keys.Contains(key))
            {
                int index = infos.FindIndex(info => { return info.key == key; });
                info = infos[index];
            }
            else
            {
                info = new InputInfo(key);
                infos.Add(info);
                keys.Add(key);
            }
            switch (st)
            {
                case EKeyEventCtor.down: info.AddDown(callback, add); break;
                case EKeyEventCtor.up: info.AddUp(callback, add); break;
                case EKeyEventCtor.tick: info.AddAction(callback, add); break;
                default: info.AddAction(callback, add); break;
            }
        }
    }
}

