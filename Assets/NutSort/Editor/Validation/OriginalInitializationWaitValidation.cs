using System;
using NutSort.Gameplay;
using NutSort.World;
using UnityEngine;
namespace NutSort.Validation
{
    public static class OriginalInitializationWaitValidation
    {
        public static void Validate()
        {
            var session=Resources.Load<OriginalSceneSession>("Configuration/OriginalSceneSession");
            Check(session.MainPanelReadyDelay==1.5f,"Native post-main-panel delay");
            GameObject main=null;int checks=0,called=0;
            var routine=OriginalInitializationWait.Run(()=>{checks++;return main;},session.MainPanelReadyDelay,()=>called++);
            Check(routine.MoveNext() && routine.Current is WaitUntil && checks==0 && called==0,"Initial coroutine yield does not eagerly test or invoke");
            var wait=(WaitUntil)routine.Current;Check(wait.keepWaiting && checks==1,"Missing main panel keeps waiting");
            main=new GameObject("Initialization wait fixture");main.SetActive(false);
            Check(!wait.keepWaiting,"Inactive but existing main panel satisfies native object predicate");
            UnityEngine.Object.DestroyImmediate(main);Check(wait.keepWaiting,"Unity destroyed object compares as null");
            main=new GameObject("Replacement main panel");
            try
            {
                Check(!wait.keepWaiting,"Predicate reads replacement object");
                Check(routine.MoveNext() && routine.Current is WaitForSeconds && called==0,"WaitUntil completion yields scaled delay before continuation");
                Check(!routine.MoveNext() && called==1 && !routine.MoveNext(),"Continuation invoked once then iterator ends");
                var nullCallback=OriginalInitializationWait.Run(()=>main,1.5f,null);
                nullCallback.MoveNext();nullCallback.MoveNext();Check(!nullCallback.MoveNext(),"Optional callback can be null");
            }
            finally{UnityEngine.Object.DestroyImmediate(main);}
            Debug.Log("NUT_INITIALIZATION_WAIT_VALIDATION_PASS lazy WaitUntil, native Unity object existence including inactive/destroyed/replaced targets, scaled post-wait delay and optional one-shot continuation.");
        }
        private static void Check(bool value,string message){if(!value)throw new InvalidOperationException(message);}
    }
}
