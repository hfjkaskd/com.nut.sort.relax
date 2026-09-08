using System;
using System.Collections.Generic;
using Newtonsoft.Json.Linq;
using NutSort.Content;
using NutSort.UI;
using NutSort.World;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
namespace NutSort.Validation
{
    [InitializeOnLoad]
    public static class OriginalSuccessRewardPlayValidation
    {
        private const string Key="NutSort.SuccessRewardPlay";
        private static double timeout;
        static OriginalSuccessRewardPlayValidation(){if(SessionState.GetBool(Key,false)){timeout=EditorApplication.timeSinceStartup+60;EditorApplication.update+=Tick;}}
        public static void Run(){OriginalPreferenceFixture.Begin();EditorSceneManager.OpenScene("Assets/Scenes/LuoSiSortGame.unity");SessionState.SetBool(Key,true);EditorApplication.EnterPlaymode();}
        private static void Tick()
        {
            if(!EditorApplication.isPlaying)return;
            try
            {
                Check(EditorApplication.timeSinceStartup<timeout,"Success reward Play timeout");
                var game=UnityEngine.Object.FindObjectOfType<OriginalGameScene>();
                if(game==null||game.Level==null||game.IsRestarting||game.InputBlocked)return;
                Transform parent=null;foreach(var c in UnityEngine.Object.FindObjectsOfType<Canvas>())if(c.name=="UICanvas")parent=c.transform;
                Check(parent!=null,"Actual scene UI canvas");
                var user=game.User;float gold=user.Gold;double coin=user.Coin;var board=game.Level.Board;
                var factory=new OriginalRewardItemFactory(Resources.Load<OriginalRewardItemSettings>("Configuration/OriginalRewardItem"),new OriginalGoldFormatter(()=>"en-US"),()=>"US");
                Action<JObject> respond=null;List<OriginalRewardItemView> views=null;int shown=0,closed=0;
                var get=game.CreateSuccessReward((more,done)=>{Check(more,"Captured claim mode forwarded");respond=done;},
                    (id,info)=>{Check(id==8&&closed==0&&info.IsMore,"Reward consumer before settlement close");views=factory.GenerateItem(info,parent);shown++;},()=>{Check(shown==1,"Show completes before close");closed++;});
                get(true);Check(respond!=null&&shown==0&&closed==0,"No presentation before explicit response");
                respond(JObject.Parse(@"{""kinetic_data"":{""hg_amt"":""5.25"",""hg_psi"":""100"",""hg_zs_amt"":""0"",""hg_zs_psi"":""16777217.25""}}"));
                Check(shown==1&&closed==1&&views.Count==2,"Ordered actual reward entries including zero coin");
                Check(views[0].Count.text=="$5.25"&&views[1].Count.text=="0"&&views[0].Icon.sprite!=null&&views[1].Icon.sprite!=null,"Real reward prefabs render received amounts, not current balances");
                Check(views[1].ItemInfo.DoubleCurrentCount==16777217.25,"Response balance retains double precision through actual view");
                Check(user.Gold==gold&&user.Coin==coin&&game.Level.Board==board,"UI response flow does not pretend to grant balance or advance the board");
                foreach(var view in views)UnityEngine.Object.Destroy(view.gameObject);
                Debug.Log("NUT_SUCCESS_REWARD_PLAY_PASS actual scene response adapter and reward item prefabs, deferred display, cash and zero-coin text/icons, preserved double balance and show-before-close; explicit response and panel consumer, original RewardPanel lifecycle/SDK excluded.");Finish(0);
            }
            catch(Exception error){Debug.LogException(error);Finish(1);}
        }
        private static void Check(bool ok,string message){if(!ok)throw new InvalidOperationException(message);}
        private static void Finish(int code){OriginalPreferenceFixture.Restore();SessionState.SetBool(Key,false);EditorApplication.update-=Tick;EditorApplication.Exit(code);}
    }
}
