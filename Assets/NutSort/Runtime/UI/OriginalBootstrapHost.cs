using System.Globalization;
using System.Threading;
using NutSort.Gameplay;
using UnityEngine;
namespace NutSort.UI
{
    public sealed class OriginalBootstrapHost:MonoBehaviour,IOriginalBootstrapActions
    {
        [SerializeField] private OriginalBootLoadingPanel loading;
        [SerializeField] private int targetFrameRate;
        [SerializeField] private string culture;
        [SerializeField] private float mainPanelDelay;
        private IOriginalBootstrapActions actions;
        public OriginalBootLoadingPanel Loading=>loading;
        public void Bind(IOriginalBootstrapActions value){actions=value;}
        private void Start()
        {
            Application.targetFrameRate=targetFrameRate;
            Thread.CurrentThread.CurrentCulture=new CultureInfo(culture);
            Thread.CurrentThread.CurrentUICulture=new CultureInfo(culture);
            StartCoroutine(OriginalBootstrap.Run(this,mainPanelDelay));
        }
        public string CountryCode=>actions.CountryCode;
        public bool IsUserInitDone=>actions.IsUserInitDone;
        public bool HasLevelInfo=>actions.HasLevelInfo;
        public void SetLoading(bool visible)=>loading.SetState(visible);
        public void InitializeUI()=>actions.InitializeUI();
        public void InitializeTables()=>actions.InitializeTables();
        public void InitializeSdkBoundary()=>actions.InitializeSdkBoundary();
        public void InitializePool()=>actions.InitializePool();
        public void InitializeUser()=>actions.InitializeUser();
        public void InitializeAudio()=>actions.InitializeAudio();
        public void InitializeScene()=>actions.InitializeScene();
        public void ShowAndAssignMainPanel()=>actions.ShowAndAssignMainPanel();
        public void InitializeRequests()=>actions.InitializeRequests();
        public void PlayBgm()=>actions.PlayBgm();
    }
}
