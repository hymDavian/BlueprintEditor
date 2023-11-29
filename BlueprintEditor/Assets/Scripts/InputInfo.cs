using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace InputMgr
{
    internal class InputInfo
    {
        public readonly KeyCode key;
        private readonly List<Action> downCallback = new List<Action>();
        private readonly List<Action> upCallback = new List<Action>();
        private readonly List<Action> actions = new List<Action>();

        public InputInfo(KeyCode _k) 
        {
            this.key = _k;
        }

        public void AddDown(Action action, bool add = true)
        {
            if (add)
            {

            this.downCallback.Add(action);
            }
            else
            {
                this.downCallback.Remove(action);
            }
        }
        public void AddUp(Action action, bool add = true)
        {
            if (add)
            {

                this.upCallback.Add(action);
            }
            else
            {
                this.upCallback.Remove(action);
            }
        }
        public void AddAction(Action action, bool add = true)
        {
            if (add)
            {

                this.actions.Add(action);
            }
            else
            {
                this.actions.Remove(action);
            }
        }

        public void excute()
        {
            if (Input.GetKeyDown(key))
            {
                downCallback.ForEach(act => {  act.Invoke(); });
            }
            if (Input.GetKeyUp(key))
            {
                upCallback.ForEach(act => { act.Invoke(); });
            }
            if (Input.GetKey(key))
            {
                actions.ForEach(act => { act.Invoke(); });
            }
        }
    }
}
