using System.Collections;
using UnityEngine;
namespace NutSort.Gameplay
{
    public interface IOriginalBootstrapActions
    {
        string CountryCode{get;}
        bool IsUserInitDone{get;}
        bool HasLevelInfo{get;}
        void SetLoading(bool visible);
        void InitializeUI();
        void InitializeTables();
        void InitializeSdkBoundary();
        void InitializePool();
        void InitializeUser();
        void InitializeAudio();
        void InitializeScene();
        void ShowAndAssignMainPanel();
        void InitializeRequests();
        void PlayBgm();
    }
    // LuoSiSort.Init coroutine. SDK port keeps the caller's existing handling.
    public static class OriginalBootstrap
    {
        public static IEnumerator Run(IOriginalBootstrapActions actions,float mainPanelDelay)
        {
            actions.SetLoading(true);yield return null;
            actions.InitializeUI();yield return null;
            actions.InitializeTables();yield return null;
            actions.InitializeSdkBoundary();yield return null;
            actions.InitializePool();
            yield return new WaitUntil(()=>!string.IsNullOrEmpty(actions.CountryCode));
            actions.InitializeUser();yield return null;
            actions.InitializeAudio();
            yield return new WaitUntil(()=>actions.IsUserInitDone);
            yield return null;
            actions.InitializeScene();
            yield return new WaitUntil(()=>actions.HasLevelInfo);
            actions.ShowAndAssignMainPanel();
            yield return new WaitForSeconds(mainPanelDelay);
            actions.SetLoading(false);yield return null;
            actions.InitializeRequests();actions.PlayBgm();
        }
    }
}
