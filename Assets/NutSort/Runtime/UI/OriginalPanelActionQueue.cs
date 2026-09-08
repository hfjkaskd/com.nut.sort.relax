using System;
using System.Collections.Generic;

namespace NutSort.UI
{
    // UIMgr.AddActionQueue (0x9F8808) / DequeueActionQueue (0x9F8860).
    public sealed class OriginalPanelActionQueue
    {
        private readonly Queue<Action> actions = new Queue<Action>();
        public int Count => actions.Count;
        public void Add(Action action) { actions.Enqueue(action); }
        public void Dequeue()
        {
            if(actions.Count<1)return;
            actions.Dequeue()?.Invoke();
        }
    }
}
