using System;
using NutSort.Content;
using NutSort.UI;
using UnityEngine;
using UnityEngine.UI;
namespace NutSort.Validation
{
    public static class OriginalToolRewardFlightValidation
    {
        public static void Validate()
        {
            var root=new GameObject("Flight validation");
            try
            {
                var bottom=UnityEngine.Object.Instantiate(Resources.Load<GameObject>("Prefabs/Panels/MainPanelBottom"),root.transform).GetComponent<OriginalMainBottomView>();
                var source=UnityEngine.Object.Instantiate(Resources.Load<GameObject>("Prefabs/Items/Item"),root.transform).GetComponent<OriginalRewardItemView>();
                var flight=UnityEngine.Object.Instantiate(Resources.Load<GameObject>("Prefabs/Effects/ToolRewardFlight"),root.transform).GetComponent<OriginalToolRewardFlight>();
                var user=new OriginalUserLocalData(Resources.Load<OriginalUserDefaults>("Configuration/OriginalUserDefaults")){RevokeCount=1};
                foreach(var item in bottom.Items)item.Display.Bind(user,()=>false,()=>0);
                var target=bottom.GetOtherItem(2).Display;target.Refresh();
                target.Icon.transform.position=new Vector3(100,50,0);target.Icon.transform.localScale=Vector3.one*.5f;
                source.Icon.transform.position=Vector3.zero;source.Icon.transform.localScale=Vector3.one*2;
                Action callback=null;float delay=0;int calls=0;
                flight.Bind(root.transform,()=>bottom,(seconds,done)=>{delay=seconds;callback=done;});
                flight.Fly(2,source.Icon,()=>calls++);
                var clone=root.transform.GetChild(root.transform.childCount-1);
                Check(flight.ActiveCount==1&&clone.GetComponent<Image>().sprite==source.Icon.sprite&&clone.position==Vector3.zero&&clone.localScale==Vector3.one*2,"Exact source image clone under top parent");
                Check(delay==.5f&&calls==0,"Independent callback delay");
                flight.Advance(.5f);Check(clone.position==Vector3.zero&&target.Value.text=="1","No movement through delay equality");
                callback();Check(calls==1&&flight.ActiveCount==1,"User callback runs before landing");
                target.Icon.transform.position=new Vector3(999,999,0);user.RevokeCount=7;
                flight.Advance(.3f);Check(Vector3.Distance(clone.position,new Vector3(75,37.5f,0))<.001f&&Vector3.Distance(clone.localScale,Vector3.one*.875f)<.001f,"Source default OutQuad position and scale, target captured at dispatch");
                flight.Advance(0);Check(flight.ActiveCount==1&&target.Value.text=="1","Zero delta does not complete or refresh");
                flight.Advance(.31f);Check(flight.ActiveCount==0&&target.Value.text=="7","Landing refreshes live tool count");
                int children=root.transform.childCount;callback=null;
                flight.Fly(99,source.Icon,()=>calls++);
                Check(flight.ActiveCount==0&&root.transform.childCount==children&&callback==null,"Missing HUD tool returns without clone or callback scheduling");
            }
            finally{UnityEngine.Object.DestroyImmediate(root);}
            Debug.Log("NUT_TOOL_REWARD_FLIGHT_VALIDATION_PASS source Image clone, top parent/world position, captured HUD destination and local scale, half-second callback/delay, 0.6-second OutQuad motion, live count refresh and missing-target short circuit; cash/coin flights excluded.");
        }
        private static void Check(bool ok,string message){if(!ok)throw new InvalidOperationException(message);}
    }
}
