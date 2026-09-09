using System;
using NutSort.Content;
using UnityEngine;
namespace NutSort.World
{
    public sealed class OriginalUserSession : MonoBehaviour
    {
        [SerializeField] private OriginalUserDefaults defaults;
        private OriginalUserStore loadedStore;
        private OriginalUserStartup startup;
        public OriginalUserStore Store=>startup!=null&&startup.Store!=null?startup.Store:loadedStore;
        public bool IsUserInitDone=>startup!=null&&startup.IsInitDone;
        public OriginalUserLocalData Data=>Store.Data;
        public void StartUser(OriginalUserStartup value)
        {
            if(value==null)throw new ArgumentNullException(nameof(value));
            loadedStore=Store;
            startup=value;
            // Bind before invoking: synchronous request callbacks must see published startup data.
            startup.Initialize();
        }
        private void Awake() { Initialize(); }
        public void Initialize()
        {
            if(Store==null)loadedStore=new OriginalUserStore(defaults,new UnityUserPreferences());
        }
    }
}
