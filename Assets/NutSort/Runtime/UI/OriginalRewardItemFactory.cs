using System;
using System.Collections.Generic;
using NutSort.Content;
using UnityEngine;
namespace NutSort.UI
{
    // ItemGetInfo.GenerateItem (9F8A98). The owning panel controls disposal.
    public sealed class OriginalRewardItemFactory
    {
        private readonly OriginalRewardItemSettings settings;
        private readonly OriginalGoldFormatter formatter;
        private readonly Func<string> country;
        public OriginalRewardItemFactory(OriginalRewardItemSettings settings,OriginalGoldFormatter formatter,Func<string> country)
        { this.settings=settings;this.formatter=formatter;this.country=country; }
        public List<OriginalRewardItemView> GenerateItem(OriginalItemGetInfo info,Transform parent,bool isBig=false,bool isShowMax=true)
        {
            var result=new List<OriginalRewardItemView>();
            foreach(OriginalItemInfo item in info.ItemInfos)
            {
                string path=isBig?settings.BigItemPath:settings.ItemPath;
                var prefab=Resources.Load<GameObject>(path);
                if(prefab==null)Debug.LogError("InstanceGameObject not find path: "+path);
                var instance=prefab==null?null:UnityEngine.Object.Instantiate(prefab,parent,false);
                var view=instance.GetComponent<OriginalRewardItemView>();
                result.Add(view);
                view.Bind(formatter,country);
                view.Init(item,isBig,isShowMax);
            }
            return result;
        }
    }
}
