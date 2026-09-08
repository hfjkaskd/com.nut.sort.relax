using System;
using NutSort.Content;
using NutSort.UI;
using UnityEngine;
namespace NutSort.Validation
{
    public static class OriginalGuideRoutingValidation
    {
        private sealed class Branches:IOriginalGuideBranches
        {
            public Action<string> Run;
            public void ShowSuccess()=>Run("success");
            public void ShowTargetCompletion()=>Run("target");
            public void ShowWithdrawal()=>Run("withdrawal");
            public void ShowCoin()=>Run("coin");
            public void ShowGoldEntry(int index)=>Run("gold"+index);
            public void ShowWithdrawalStage(int index)=>Run("stage"+index);
        }
        public static void Validate()
        {
            var previous=OriginalNewbieGuideView.CallbackAction;
            var root=UnityEngine.Object.Instantiate(Resources.Load<GameObject>("prefabs/panels/NewbieGuidePanel"));
            try
            {
                var user=new OriginalUserLocalData(Resources.Load<OriginalUserDefaults>("Configuration/OriginalUserDefaults"));
                var view=root.GetComponent<OriginalNewbieGuideView>();bool canOperate=true;int closes=0,skipReads=0;
                var tables=new OriginalTables(Resources.Load<OriginalTableSettings>("Configuration/OriginalTables"));
                view.BindTeaching(tables,"en",value=>canOperate=value,()=>closes++);
                var branches=new Branches();string called=null;branches.Run=name=>{Check(!canOperate,"Teaching reset precedes branch");called=name;};
                view.BindGuide(user,()=>{skipReads++;return false;},branches);
                string[] expected={"success","target","withdrawal","coin",null,null,null,null,null,null,"gold10","stage11","gold12","stage13","gold14","stage15"};
                for(int i=0;i<16;i++)
                {
                    user.Level=2;user.GuideIndex=i;view.InitializeInteractions(user,null,()=>true,()=>{});
                    user.GuideIndex=99;canOperate=true;called=null;view.ShowGuide();
                    Check(called==expected[i] && !canOperate,"Native cached-index dispatch table");
                }
                foreach(int index in new[]{-1,int.MinValue,16,int.MaxValue})
                {
                    user.GuideIndex=index;view.InitializeInteractions(user,null,()=>true,()=>{});canOperate=true;called=null;view.ShowGuide();
                    Check(called==null && !canOperate,"Out-of-range index still processes teaching reset");
                }
                Check(skipReads==0,"Higher levels do not evaluate teaching skip configuration");
                user.Level=1;user.LevelSeed=0;user.GuideIndex=1;view.InitializeInteractions(user,null,()=>true,()=>{});called=null;view.ShowGuide();
                Check(canOperate && called==null && skipReads==1 && view.Tip.text==tables.Text.GetText(70,"en"),"Real teaching view short-circuits target branch");
                user.Level=2;user.GuideIndex=1;view.InitializeInteractions(user,null,()=>true,()=>{});
                branches.Run=name=>
                {
                    Check(name=="target","Target dispatch");
                    view.ShowTargetCompletion(user,value=>{},(id,level)=>{Check(id==35 && level==1 && closes==1,"Dispatcher reaches existing target handoff");called="handoff";});
                };
                view.ShowGuide();Check(called=="handoff","Actual handoff invoked");
                user.Level=1;view.BindGuide(user,()=>true,branches);view.ShowGuide();Check(closes==2,"Skip teaching closes without branch dispatch");
            }
            finally{OriginalNewbieGuideView.CallbackAction=previous;UnityEngine.Object.DestroyImmediate(root);}
            Debug.Log("NUT_GUIDE_ROUTING_VALIDATION_PASS native 16-entry cached-index routing, out-of-range teaching reset, real teaching short-circuit and target-completion handoff; remaining branch bodies and production host still pending.");
        }
        private static void Check(bool value,string message){if(!value)throw new InvalidOperationException(message);}
    }
}
