using System;
using NutSort.UI;
using UnityEngine;
namespace NutSort.Validation
{
    public static class OriginalPanelActionQueueValidation
    {
        public static void Validate()
        {
            var queue=new OriginalPanelActionQueue();int value=0;
            queue.Dequeue();queue.Add(null);queue.Add(()=>value++);
            Check(value==0 && queue.Count==2,"Enqueue does not start actions");
            queue.Dequeue();Check(value==0 && queue.Count==1,"Null action consumes one slot");
            queue.Dequeue();Check(value==1 && queue.Count==0,"One action per dequeue");
            queue.Add(()=>{Check(queue.Count==0,"Removal precedes invocation");queue.Add(()=>value+=10);});
            queue.Dequeue();Check(value==1 && queue.Count==1,"Appended action remains pending");
            queue.Dequeue();Check(value==11,"Next explicit dispatch executes appended action");
            queue.Add(()=>{throw new InvalidOperationException("fixture");});queue.Add(()=>value++);
            bool caught=false;try{queue.Dequeue();}catch(InvalidOperationException){caught=true;}
            Check(caught && queue.Count==1,"Throwing action already removed; later action retained");queue.Dequeue();
            queue.Add(()=>queue.Dequeue());queue.Add(()=>value+=100);queue.Dequeue();
            Check(value==112 && queue.Count==0,"Source allows reentrant explicit dequeue");
            var instance=UnityEngine.Object.Instantiate(Resources.Load<GameObject>("prefabs/panels/FailPanel"));
            try
            {
                var panel=instance.GetComponent<OriginalFailurePanelView>();int scheduled=0,callbacks=0;Action pending=null;
                panel.BindHide(queue,(seconds,next)=>{Check(callbacks>0 && seconds==2.5f,"Callback before scheduling");scheduled++;pending=next;},()=>callbacks++);
                panel.Hide();panel.Hide();Check(callbacks==2 && scheduled==2,"Repeated Hide has no synthetic guard");
                queue.Add(()=>value++);pending();Check(value==113,"Scheduled action uses live queue");
                panel.BindHide(queue,(seconds,next)=>scheduled++,()=>{throw new InvalidOperationException("fixture hidden callback");});
                caught=false;try{panel.Hide();}catch(InvalidOperationException){caught=true;}
                Check(caught && scheduled==2,"Throwing hide callback suppresses scheduling");
            }
            finally{UnityEngine.Object.DestroyImmediate(instance);}
            Debug.Log("NUT_PANEL_ACTION_QUEUE_VALIDATION_PASS FIFO/null/single-step/reentrant/exception semantics and original hidden-callback-before-2.5-second-schedule ordering.");
        }
        private static void Check(bool value,string message){if(!value)throw new InvalidOperationException(message);}
    }
}
