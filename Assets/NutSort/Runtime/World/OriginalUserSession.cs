using NutSort.Content;
using UnityEngine;
namespace NutSort.World
{
    public sealed class OriginalUserSession : MonoBehaviour
    {
        [SerializeField] private OriginalUserDefaults defaults;
        public OriginalUserStore Store { get; private set; }
        public OriginalUserLocalData Data=>Store.Data;
        private void Awake() { Initialize(); }
        public void Initialize()
        {
            if(Store==null)Store=new OriginalUserStore(defaults,new UnityUserPreferences());
        }
    }
}
