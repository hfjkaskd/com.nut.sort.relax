using System;
using Newtonsoft.Json.Linq;
using NutSort.Content;
using NutSort.Gameplay;
using NutSort.UI;
using NutSort.World;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
namespace NutSort.Validation
{
    [InitializeOnLoad]
    public static class OriginalSuccessSettlementPlayValidation
    {
        private const string Key="NutSort.SuccessSettlementPlay";
        private static double timeout;
        static OriginalSuccessSettlementPlayValidation(){if(SessionState.GetBool(Key,false)){timeout=EditorApplication.timeSinceStartup+60;EditorApplication.update+=Tick;}}
        public static void Run(){OriginalPreferenceFixture.Begin();EditorSceneManager.OpenScene("Assets/Scenes/LuoSiSortGame.unity");SessionState.SetBool(Key,true);EditorApplication.EnterPlaymode();}
        private static void Tick()
        {
            if(!EditorApplication.isPlaying)return;
            try
            {
                Check(EditorApplication.timeSinceStartup<timeout,"Settlement Play timeout");
                var game=UnityEngine.Object.FindObjectOfType<OriginalGameScene>();
                if(game==null||game.Level==null||game.IsRestarting||game.InputBlocked)return;
                var user=game.User;user.Level=2;user.GoldRewardTargetS2CData=JObject.Parse(@"{""bear_list"":[{""psi_value"":""42""}]}");
                float gold=user.Gold;double coin=user.Coin;var board=game.Level.Board;
                var queue=new OriginalPanelActionQueue();int closed=0,shown=0;
                OriginalItemGetInfo result=null;
                var settle=game.CreateSuccessSettlement(id=>{Check(id==11,"Native modal ID");return true;},queue.Add,()=>closed++,
                    (id,info)=>{Check(id==9&&closed==1,"Close before actual settlement consumer");result=info;shown++;});
                var response=JObject.Parse(@"{""kinetic_data"":{""hg_amt"":"""",""hg_zs_amt"":""8""}}");
                settle(response);
                Check(user.Level1Gold==42&&queue.Count==1&&closed==0&&shown==0,"Real early-level field and deferred queue");
                var saved=OriginalUserDataJson.Read(PlayerPrefs.GetString(OriginalUserStore.Key),Resources.Load<OriginalUserDefaults>("Configuration/OriginalUserDefaults"));
                Check(saved.Level1Gold==42&&saved.LevelInfo==OriginalBoardSnapshotJson.Write(game.Level.CaptureSnapshot()),"Existing scene save persists early reward field and current board");
                response["kinetic_data"]["hg_zs_amt"]="999";queue.Dequeue();
                Check(shown==1&&result.ItemInfos.Count==2&&result.ItemInfos[0].Count==42&&result.ItemInfos[1].Count==8,"Actual queue releases previously built gold and coin payload");
                Check(user.Gold==gold&&user.Coin==coin&&game.Level.Board==board,"Settlement presentation does not grant balance or rebuild board");
                Debug.Log("NUT_SUCCESS_SETTLEMENT_PLAY_PASS real scene field/board save, existing action queue, delayed ordered settlement payload and unchanged balances/board; explicit response and panel consumer, rendered settlement panel pending.");Finish(0);
            }
            catch(Exception error){Debug.LogException(error);Finish(1);}
        }
        private static void Check(bool value,string message){if(!value)throw new InvalidOperationException(message);}
        private static void Finish(int code){OriginalPreferenceFixture.Restore();SessionState.SetBool(Key,false);EditorApplication.update-=Tick;EditorApplication.Exit(code);}
    }
}
