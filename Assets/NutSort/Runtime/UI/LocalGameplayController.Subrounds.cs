using UnityEngine;
namespace NutSort.UI
{
    public sealed partial class LocalGameplayController
    {
        [SerializeField] private string subroundPrefabPath;
        private OriginalHiddenLevelView subroundView;
        public OriginalHiddenLevelView SubroundView=>subroundView;
        private void RefreshSubrounds()
        {
            if(subroundView==null)
            {
                if(game.Tables.GetLevelInfo(game.PlayerLevel,game.PlayerLevel).SubTotalRound<=0)return;
                subroundView=Instantiate(Resources.Load<GameObject>(subroundPrefabPath),startup.MainLevel.Group.parent,false).GetComponent<OriginalHiddenLevelView>();
                subroundView.Bind(game.User,game.Tables,language);
                subroundView.Init();
            }
            // RefreshSubRound reads only the original level table. The source
            // reward milestones stay hidden in the local prefab variant.
            subroundView.RefreshSubRound();
        }
    }
}
