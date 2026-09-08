using System;
using System.Collections.Generic;
using UnityEngine;

namespace NutSort.UI
{
    // UIMgr.ShowPanel (0x9E872C) / HideLssPanel (0x9F8294).
    // Factory owns resource instantiation/parenting/activation; adapters own
    // concrete native view methods. Explicit names avoid enum reflection.
    public sealed class OriginalPanelRegistry<T> where T : class
    {
        private readonly Dictionary<int,T> panels=new Dictionary<int,T>();
        private readonly Func<int,string> name;
        private readonly Func<int,T> create;
        private readonly Action<int,T> initialize;
        private readonly Action<T> refresh,hide,destroy;
        public int Count => panels.Count;
        public bool Contains(int id) => panels.ContainsKey(id);
        public T Get(int id) => panels[id];

        public OriginalPanelRegistry(Func<int,string> name,Func<int,T> create,Action<int,T> initialize,
            Action<T> refresh,Action<T> hide,Action<T> destroy)
        {
            this.name=name ?? throw new ArgumentNullException(nameof(name));
            this.create=create ?? throw new ArgumentNullException(nameof(create));
            this.initialize=initialize ?? throw new ArgumentNullException(nameof(initialize));
            this.refresh=refresh ?? throw new ArgumentNullException(nameof(refresh));
            this.hide=hide ?? throw new ArgumentNullException(nameof(hide));
            this.destroy=destroy ?? throw new ArgumentNullException(nameof(destroy));
        }

        public T Show(int id)
        {
            if(panels.ContainsKey(id)) {Debug.LogError("panel exist :"+name(id));return null;}
            T panel=create(id);
            panels.Add(id,panel);
            initialize(id,panel);
            refresh(panel);
            return panel;
        }

        public void Hide(int id)
        {
            if(!panels.ContainsKey(id)) {Debug.LogError("panel not exist :"+name(id));return;}
            hide(panels[id]);
            // Source reads the dictionary again after the view callback.
            destroy(panels[id]);
            panels.Remove(id);
        }
    }
}
