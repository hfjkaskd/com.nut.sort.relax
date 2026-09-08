using System;
using UnityEngine;
using Newtonsoft.Json.Linq;
using NutSort.Content;
namespace NutSort.UI
{
    public interface IOriginalWithdrawalProgressUI
    {
        float Fill { get; set; }
        void SetTip(int id,params object[] arguments);
        void SetProgress(string text);
        void SetProgressTip(int id,params object[] arguments);
        void SetMarkerX(float x);
    }
    // TXPanel.Refresh progress branches; one instance retains the shared completion flag.
    public sealed class OriginalWithdrawalEarlyProgress
    {
        private readonly Func<int> showLevel;
        private readonly IOriginalWithdrawalProgressUI ui;
        private readonly OriginalPanelSettings settings;
        public bool IsDoneTask { get; private set; }
        public OriginalWithdrawalEarlyProgress(Func<int> showLevel,IOriginalWithdrawalProgressUI ui,OriginalPanelSettings settings)
        {this.showLevel=showLevel;this.ui=ui;this.settings=settings;}
        public void Refresh(int stage,int level,object[] arguments)
        {
            int count,total;
            if(stage==1)
            {
                ui.SetTip(22,level);total=level;
                count=unchecked(level-(arguments.Length==0?1:0));
            }
            else if(stage==2)
            {
                total=settings.WithdrawalEarlyTotal;ui.SetTip(22,total);
                int first=showLevel();bool hasArguments=arguments.Length!=0;
                count=Math.Min(unchecked(first-1),total);
                if(!hasArguments)count=Math.Min(unchecked(showLevel()-1),unchecked(total-1));
            }
            else throw new ArgumentOutOfRangeException(nameof(stage));
            ui.Fill=(float)count/total;
            ui.SetProgress(string.Format("{0}/{1}",count,total));
            ui.SetProgressTip(25,string.Format("{0}",unchecked(total-count)));
            MoveMarker();
            Complete();
        }
        public void RefreshThird(object[] arguments,Func<int> startLevelShow,Func<int> lastLevelShow)
        {
            if(arguments.Length!=0)
            {
                int total=startLevelShow();
                ui.Fill=(float)total/total;
                ui.SetProgress(string.Format("{0}/{1}",total,total));
                MoveMarker();ui.SetTip(159);
            }
            else
            {
                int total=lastLevelShow();ui.SetTip(22,total);
                showLevel();
                int count=Math.Min(unchecked(showLevel()-1),unchecked(total-1));
                ui.Fill=(float)count/total;
                ui.SetProgress(string.Format("{0}/{1}",count,total));
                ui.SetProgressTip(25,string.Format("{0}",unchecked(total-count)));
                MoveMarker();
            }
            Complete();
        }
        // mainGoldHint reads MainPanel.Top.GoldItem.GoldHintText.text, not a recomputed description.
        public void RefreshLater(int stage,object[] arguments,OriginalUserLocalData user,
            Func<string> mainGoldHint,Action<string> setRawTip,Func<float,string> formatGold,
            Func<bool> isPlayGoldTween,Action<int> setClaimText)
        {
            setRawTip(mainGoldHint());
            JObject row=RewardRow(user);
            if(stage==4)
            {
                float target=OriginalRewardProgress.ToFloat(user.TXTargetGold);
                ui.Fill=user.Gold/target;
                ui.SetProgress(string.Concat(formatGold(user.Gold),"/",formatGold(target)));
                ui.SetProgressTip(27,formatGold(target-user.Gold)??string.Empty);
                MoveMarker();
                if(arguments.Length!=0)
                {
                    int total=Integer(RewardRow(user),"Stage2StartShowLevel");
                    ui.Fill=(float)total/total;
                    ui.SetProgress(string.Format("{0}/{1}",total,total));
                    MoveMarker();ui.SetTip(159);
                }
            }
            else if(stage==5||stage==6)
            {
                float target=OriginalRewardProgress.ToFloat(user.TXTargetGold);
                int days=Integer(RewardRow(user),"caliper_logs");
                ui.Fill=(float)user.LoginDay/days;
                ui.SetProgress(string.Format("{0}/{1}",user.LoginDay,days));
                ui.SetProgressTip(28,string.Format("{0}",unchecked(days-user.LoginDay)));
                MoveMarker();
                if(arguments.Length!=0)
                {
                    ui.Fill=user.Gold/target;
                    // The native numerator is a boxed float, only the denominator uses GoldLSSFormat.
                    ui.SetProgress(string.Format("{0}/{1}",user.Gold,formatGold(target)));
                    MoveMarker();ui.SetTip(159);
                }
            }
            else if(stage!=0)
            {
                ui.Fill=(float)user.UserLevel/Integer(row,"caliper_rank");
                ui.SetProgress(string.Format("{0}/{1}",user.UserLevel,Integer(row,"caliper_rank")));
                ui.SetProgressTip(29,string.Format("{0}",unchecked(Integer(row,"caliper_rank")-user.UserLevel)));
                MoveMarker();
            }
            Complete();
            if(!string.IsNullOrEmpty(user.TXTargetGold)&&
                (string.IsNullOrEmpty(user.ComeOnGold)||isPlayGoldTween()))setClaimText(36);
        }
        private static JObject RewardRow(OriginalUserLocalData user)
        {
            JToken list=Field(user.GoldRewardTargetS2CData,"bear_list");
            if(list==null||list.Type==JTokenType.Null)throw new NullReferenceException("bear_list");
            JToken row=((JArray)list)[2];
            return row==null||row.Type==JTokenType.Null?null:(JObject)row;
        }
        private static int Integer(JObject row,string name)
        {
            JToken value=Field(row,name);return value==null?0:(int)value;
        }
        private static JToken Field(JObject row,string name)
        {
            if(row==null)throw new NullReferenceException(name);
            JToken result=null;
            foreach(var property in row.Properties())
                if(string.Equals(property.Name,name,StringComparison.OrdinalIgnoreCase))result=property.Value;
            return result;
        }
        private void MoveMarker()=>ui.SetMarkerX(Mathf.Clamp01(ui.Fill)*settings.WithdrawalProgressTravel+settings.WithdrawalProgressOffset);
        private void Complete()
        {
            if(ui.Fill<settings.WithdrawalCompleteThreshold)return;
            IsDoneTask=true;ui.SetProgressTip(23);ui.SetTip(159);
        }
    }
}
