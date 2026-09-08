using System;
using System.Collections.Generic;
using Newtonsoft.Json.Linq;
using NutSort.Content;
using NutSort.UI;
using UnityEngine;
using UnityEngine.UI;
namespace NutSort.Validation
{
    public static class OriginalWithdrawalLaterProgressValidation
    {
        private sealed class View:IOriginalWithdrawalProgressUI
        {
            public Image Image;public string Progress,Tip;public int Hint;public object Remaining;public float X;
            public readonly List<string> Trace=new List<string>();public Action<string> Changed;
            private void Record(string value){Trace.Add(value);Changed?.Invoke(value);}
            public float Fill{get=>Image.fillAmount;set{Image.fillAmount=value;Record("fill");}}
            public void SetTip(int id,params object[] args){Tip=id.ToString();Record("tip:"+id);}
            public void SetProgress(string text){Progress=text;Record("progress");}
            public void SetProgressTip(int id,params object[] args){Hint=id;Remaining=args.Length==0?null:args[0];Record("hint:"+id);}
            public void SetMarkerX(float x){X=x;Record("marker");}
            public void RawTip(string text){Tip=text;Record("raw");}
        }
        public static void Validate()
        {
            var go=new GameObject("WithdrawalProgressFixture",typeof(RectTransform),typeof(CanvasRenderer),typeof(Image));
            try
            {
                var view=new View{Image=go.GetComponent<Image>()};
                var settings=Resources.Load<OriginalPanelSettings>("Configuration/OriginalPanels");
                var user=new OriginalUserLocalData(Resources.Load<OriginalUserDefaults>("Configuration/OriginalUserDefaults"));
                var formatter=new OriginalGoldFormatter(()=>"en-US");Func<float,string> money=value=>formatter.Format(value);
                var flow=new OriginalWithdrawalEarlyProgress(()=>throw new Exception("No level table read"),view,settings);
                user.GoldRewardTargetS2CData=JObject.Parse(@"{""bear_list"":[null,null,{""caliper_logs"":5,""caliper_rank"":10,""Stage2StartShowLevel"":8}],""cal_cfg"":""unused""}");
                user.TXTargetGold="100";user.ComeOnGold="";user.Gold=25;user.LoginDay=2;user.UserLevel=3;
                int flags=0,claim=0;bool tween=false;
                Action<int,object[]> refresh=(stage,args)=>flow.RefreshLater(stage,args,user,()=>"HUD live hint",view.RawTip,money,()=>{flags++;return tween;},id=>{claim=id;view.Trace.Add("claim");});
                refresh(4,Array.Empty<object>());
                Check(view.Fill==.25f&&view.Progress==money(25)+"/"+money(100)&&view.Hint==27&&(string)view.Remaining==money(75)&&view.X==-200&&!flow.IsDoneTask,"Amount, remaining and marker use configured target");
                Check(view.Tip=="HUD live hint"&&claim==36&&flags==0,"Copies actual HUD hint and empty pending amount bypasses tween flag");
                view.Trace.Clear();refresh(4,new object[]{null});
                Check(view.Fill==1&&view.Progress=="8/8"&&flow.IsDoneTask&&view.Hint==23,"Stage four settlement overrides amount with StartShowLevel/itself");
                Check(string.Join(",",view.Trace)=="raw,fill,progress,hint:27,marker,fill,progress,marker,tip:159,hint:23,tip:159,claim","Two display passes and duplicated success tip preserve native order");
                foreach(int stage in new[]{5,6})
                {
                    user.TodayPassLevelCount=999;user.Gold=25;user.LoginDay=2;
                    refresh(stage,Array.Empty<object>());
                    Check(view.Progress=="2/5"&&view.Fill==.4f&&view.Hint==28&&(string)view.Remaining=="3"&&Mathf.Approximately(view.X,-80),"Both later daily branches display login days");
                    refresh(stage,new object[]{null});
                    Check(view.Progress==string.Format("{0}/{1}",25f,money(100))&&view.Fill==.25f&&view.Tip=="159"&&view.Hint==28,"Settlement amount override has raw numerator, formatted denominator and retained day hint below threshold");
                }
                refresh(7,null);Check(view.Progress=="3/10"&&view.Fill==.3f&&view.Hint==29&&(string)view.Remaining=="7","Rank branch does not inspect arguments");
                JObject retained=(JObject)((JArray)user.GoldRewardTargetS2CData["bear_list"])[2];
                view.Changed=operation=>{if(operation=="fill"){retained["caliper_rank"]=12;user.UserLevel=4;user.GoldRewardTargetS2CData=JObject.Parse(@"{""bear_list"":[null,null,{""caliper_rank"":99}]}");}};
                refresh(7,null);view.Changed=null;
                Check(view.Fill==.3f&&view.Progress=="4/12"&&(string)view.Remaining=="8","Rank label and remainder reread retained row, not replaced document");
                user.GoldRewardTargetS2CData=new JObject(new JProperty("bear_list",new JArray(null,null,retained)));
                retained=(JObject)((JArray)user.GoldRewardTargetS2CData["bear_list"])[2];
                user.LoginDay=2;
                view.Changed=operation=>{if(operation=="fill"){retained["caliper_logs"]=99;user.LoginDay=3;}};
                refresh(5,Array.Empty<object>());view.Changed=null;
                Check(view.Fill==.4f&&view.Progress=="3/5"&&(string)view.Remaining=="2","Login denominator cached; label and remainder read current LoginDay");
                user.ComeOnGold="20";claim=0;flags=0;refresh(0,null);
                Check(claim==0&&flags==1,"Pending gold without tween preserves button caption");
                tween=true;refresh(0,null);Check(claim==36&&flags==2,"Tween flag allows caption 36");
                user.TXTargetGold="";claim=0;refresh(0,null);Check(claim==0&&flags==2,"Empty target skips pending/tween conditions");
                view.Trace.Clear();user.GoldRewardTargetS2CData=null;bool failed=false;
                try{refresh(0,null);}catch(NullReferenceException){failed=true;}
                Check(failed&&string.Join(",",view.Trace)=="raw","Missing data fails after copied HUD hint, even stage zero");
                user.GoldRewardTargetS2CData=JObject.Parse(@"{""bear_list"":[null,null,null]}");
                view.Image.fillAmount=1;view.Trace.Clear();refresh(0,null);
                Check(string.Join(",",view.Trace)=="raw,hint:23,tip:159","Stage zero retains prior fill and runs completion without dereferencing row");
                user.TXTargetGold="100";user.Gold=25;view.Trace.Clear();failed=false;
                try{refresh(4,null);}catch(NullReferenceException){failed=true;}
                Check(failed&&string.Join(",",view.Trace)=="raw,fill,progress,hint:27,marker","Null argument array fails only after ordinary amount display");
                Debug.Log("NUT_WITHDRAWAL_LATER_PROGRESS_VALIDATION_PASS actual Unity Image, stages 0/4/5/6/7, settlement overrides, live/cached reads, inherited HUD text, completion/caption gates and failure boundaries; full TXPanel composition pending.");
            }
            finally{UnityEngine.Object.DestroyImmediate(go);}
        }
        private static void Check(bool value,string message){if(!value)throw new InvalidOperationException(message);}
    }
}
