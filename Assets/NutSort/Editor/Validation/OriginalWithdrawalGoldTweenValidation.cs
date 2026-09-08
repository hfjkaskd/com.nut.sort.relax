using System;
using System.Collections.Generic;
using NutSort.Content;
using NutSort.UI;
using UnityEngine;
namespace NutSort.Validation
{
    public static class OriginalWithdrawalGoldTweenValidation
    {
        public static void Validate()
        {
            var root=UnityEngine.Object.Instantiate(Resources.Load<GameObject>("Prefabs/Effects/WithdrawalGoldTween"));
            try
            {
                var tween=root.GetComponent<OriginalWithdrawalGoldTween>();Check(tween.Duration==1.5f,"Original configured duration");
                var values=new List<float>();tween.Play(4,20,tween.Duration,values.Add);Check(values.Count==0,"Creation does not synchronously display start");
                tween.Advance(0);Check(values.Count==0,"Zero scaled delta pauses updates");
                tween.Advance(.75f);Check(values.Count==1&&values[0]==16&&tween.ActiveCount==1,"OutQuad at half time, not linear interpolation");
                tween.Play(20,4,tween.Duration,values.Add);values.Clear();root.SetActive(false);tween.Advance(.75f);
                Check(values.Count==2&&values[0]==20&&values[1]==8&&tween.ActiveCount==1,"Independent tracks preserve creation order and advance while hidden");
                tween.Advance(.75f);Check(values[2]==4&&tween.ActiveCount==0,"Descending tween reaches exact endpoint and releases track");
                bool spawned=false;values.Clear();tween.Play(0,1,1,v=>{if(!spawned){spawned=true;tween.Play(10,20,1,values.Add);}});
                tween.Advance(1);Check(values.Count==0&&tween.ActiveCount==1,"Callback-created track waits until next animation advance");tween.Advance(1);Check(values[0]==20&&tween.ActiveCount==0,"New track proceeds on following advance");
                var user=new OriginalUserLocalData(Resources.Load<OriginalUserDefaults>("Configuration/OriginalUserDefaults"));user.Gold=4;user.ComeOnGold="20";int saves=0,sets=0;string display="initial";
                var formatter=new OriginalGoldFormatter(()=>"en-US");
                var pending=new OriginalWithdrawalPendingGold(user,()=>true,(v,a,b)=>{user.Gold=v;sets++;},()=>saves++,tween.Play,v=>formatter.Format(v),s=>display=s,tween.Duration);
                pending.Refresh();Check(user.Gold==20&&saves==1&&sets==1&&user.ComeOnGold==""&&display=="initial","Actual pending flow updates/saves once and launches deferred native component");
                tween.Advance(.75f);Check(display==formatter.Format(16)&&user.Gold==20,"Midpoint formats through shared formatter without changing balance");
                tween.Advance(.75f);Check(display==formatter.Format(20)&&sets==1&&saves==1&&tween.ActiveCount==0,"Endpoint does not reapply or save currency");
            }
            finally{UnityEngine.Object.DestroyImmediate(root);}
            Debug.Log("NUT_WITHDRAWAL_GOLD_TWEEN_VALIDATION_PASS native configured duration, scaled pause, OutQuad midpoint/endpoints, independent tracks, hidden updates, callback reentry and actual pending-gold composition; full TXPanel view remains pending.");
        }
        private static void Check(bool value,string message){if(!value)throw new InvalidOperationException(message);}
    }
}
