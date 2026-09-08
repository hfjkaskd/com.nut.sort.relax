using System;
using System.Collections.Generic;
using NutSort.Content;
using NutSort.UI;
using UnityEngine;

namespace NutSort.Validation
{
    public static class OriginalToolItemControllerValidation
    {
        public static void Validate()
        {
            foreach(string name in new[]{"RevokeItem","ExchangeItem","AddScrewItem"})
            {
                var instance=UnityEngine.Object.Instantiate(Resources.Load<GameObject>("Prefabs/Panels/"+name));
                try
                {
                    var controller=instance.GetComponent<OriginalToolItemController>();var display=controller.Display;
                    var user=new OriginalUserLocalData(Resources.Load<OriginalUserDefaults>("Configuration/OriginalUserDefaults"));
                    var trace=new List<string>();bool allow=false,fail=false;int history=1;
                    controller.Bind(user,()=>{trace.Add("history");return history;},()=>trace.Add("add"),()=>{trace.Add("revoke");if(fail)throw new InvalidOperationException("fixture failure");},
                        (type,amount,refresh)=>{Check(type==display.ItemType && amount==-1 && refresh,"Original item delta and refresh flag");trace.Add("item");},
                        (active,delay)=>{Check(active && delay==0,"Exchange activation arguments");trace.Add("exchange");},
                        (panel,type)=>{Check(panel==4 && type==display.ItemType,"Tool panel and live item type");trace.Add("panel");},
                        id=>{Check(id==3,"No-history tip");trace.Add("tip");},()=>allow,()=>trace.Add("audio"));
                    display.Value.text="sentinel";controller.Init();controller.Init();
                    Check(display.Value.text=="sentinel" && display.Click.onClick.GetPersistentEventCount()==0 && display.Click.GetComponent<OriginalButtonFeedback>()!=null,"Init binds once with prefab feedback without refreshing display");
                    display.Click.onClick.Invoke();Check(trace.Count==0,"Common click gate");allow=true;
                    if(display.ItemType==2)
                    {
                        user.RevokeCount=0;display.Click.onClick.Invoke();Check(string.Join(",",trace)=="panel,audio","Zero stock bypasses history");trace.Clear();
                        user.RevokeCount=-1;display.Click.onClick.Invoke();Check(string.Join(",",trace)=="panel,audio","Negative stock uses acquisition panel");trace.Clear();
                        user.RevokeCount=1;history=0;display.Click.onClick.Invoke();Check(string.Join(",",trace)=="history,tip,audio","Stock without history shows tip");trace.Clear();
                        history=1;display.Click.onClick.Invoke();Check(string.Join(",",trace)=="history,revoke,item,audio" && user.RevokeCount==1,"Operation before manager delta; no duplicate local decrement");trace.Clear();
                        fail=true;bool failed=false;try{display.Click.onClick.Invoke();}catch(InvalidOperationException){failed=true;}
                        Check(failed && string.Join(",",trace)=="history,revoke","Operation failure stops delta and sound");
                    }
                    else if(display.ItemType==3)
                    {
                        user.ExchangeCount=0;display.Click.onClick.Invoke();Check(string.Join(",",trace)=="panel,audio","Exchange zero stock acquisition");trace.Clear();
                        user.ExchangeCount=2;display.Click.onClick.Invoke();Check(string.Join(",",trace)=="exchange,audio" && user.ExchangeCount==2,"Enter exchange without premature consumption/history read");
                    }
                    else
                    {
                        user.AddScrewCount=0;display.Click.onClick.Invoke();Check(string.Join(",",trace)=="add,audio","Add screw routes to core manager without UI stock gate");
                    }
                }
                finally { UnityEngine.Object.DestroyImmediate(instance); }
            }
            Debug.Log("NUT_TOOL_ITEM_CONTROLLER_VALIDATION_PASS original three click routes, inventory/history split, operation-before-item-manager ordering, acquisition/tip IDs, code listeners and shared click/sound behavior.");
        }
        private static void Check(bool value,string message) { if(!value)throw new InvalidOperationException(message); }
    }
}
