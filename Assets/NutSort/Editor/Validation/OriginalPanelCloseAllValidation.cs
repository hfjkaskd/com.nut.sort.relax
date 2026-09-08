using System;
using System.Collections.Generic;
using NutSort.UI;
using UnityEngine;
namespace NutSort.Validation
{
    public static class OriginalPanelCloseAllValidation
    {
        private sealed class Panel { public int Id; }
        public static void Validate()
        {
            OriginalPanelRegistry<Panel> registry=null;
            var hidden=new List<int>();var destroyed=new List<int>();
            var queue=new OriginalPanelActionQueue();var delayed=new List<Action>();
            bool spawn=false,throws=false;int events=0;
            registry=new OriginalPanelRegistry<Panel>(id=>id.ToString(),id=>new Panel {Id=id},(id,p)=>{},p=>{},p=>
            {
                hidden.Add(p.Id);
                if(throws)throw new InvalidOperationException("fixture");
                delayed.Add(queue.Dequeue);
                if(spawn){spawn=false;registry.Show(17);}
            },p=>destroyed.Add(p.Id));
            Check(!registry.IsExistPanel,"Empty registry has no panel");registry.CloseAll();
            registry.Show(10);Check(!registry.IsExistPanel,"Single non-main registration still returns false");registry.CloseAll();
            registry.Show(1);registry.CloseAll();Check(registry.Count==1 && !registry.IsExistPanel,"MainPanel survives alone");
            hidden.Clear();destroyed.Clear();delayed.Clear();
            var main=registry.Get(1);registry.Show(10);registry.Show(5);spawn=true;
            queue.Add(()=>events++);queue.Add(()=>events++);
            Check(registry.IsExistPanel,"Multiple registrations satisfy source predicate");
            registry.CloseAll();
            Check(registry.Count==2 && registry.Get(1)==main && registry.Contains(17),"Main and newly-opened panel survive snapshot close");
            Check(hidden.Count==2 && hidden.Contains(10) && hidden.Contains(5) && destroyed.Count==2,"Only original non-main IDs closed");
            Check(events==0 && delayed.Count==2,"Each close schedules separately without draining queue");
            foreach(var callback in delayed)callback();Check(events==2 && queue.Count==0,"Each delayed close consumes one queue entry");
            registry.CloseAll();Check(registry.Count==1 && registry.Get(1)==main,"Next close pass handles newly opened panel");
            registry.Show(10);registry.Show(5);throws=true;hidden.Clear();destroyed.Clear();
            bool caught=false;try{registry.CloseAll();}catch(InvalidOperationException){caught=true;}
            Check(caught && hidden.Count==1 && destroyed.Count==0 && registry.Count==3,"First lifecycle exception stops bulk close without rollback");
            throws=false;registry.CloseAll();Check(registry.Count==1,"Later explicit pass can finish remaining panels");
            Debug.Log("NUT_PANEL_CLOSE_ALL_VALIDATION_PASS source main retention, snapshot exclusion of new panels, count-greater-than-one predicate, per-panel delayed queue progression and exception stop.");
        }
        private static void Check(bool value,string message){if(!value)throw new InvalidOperationException(message);}
    }
}
