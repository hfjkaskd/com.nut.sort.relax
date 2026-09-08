using System;
using System.Collections.Generic;
using NutSort.Content;
using UnityEngine;
using UnityEngine.UI;
namespace NutSort.UI
{
    public sealed class OriginalRewardPanel : MonoBehaviour
    {
        [SerializeField] private Transform items;
        [SerializeField] private float grantDelay,closeDelay,queueDelay;
        private OriginalItemGetInfo info;
        private List<OriginalRewardItemView> views;
        private OriginalRewardItemFactory factory;
        private OriginalItemManager itemManager;
        private Action<int,Image> flyItem;
        private Action<float,Action> schedule;
        private Action close,hidden;
        private OriginalPanelActionQueue queue;
        public Transform Items=>items;
        public IReadOnlyList<OriginalRewardItemView> Views=>views;
        public void Bind(OriginalRewardItemFactory factory,OriginalItemManager itemManager,Action<int,Image> flyItem,
            Action<float,Action> schedule,Action close,OriginalPanelActionQueue queue,Action hidden)
        {
            this.factory=factory;this.itemManager=itemManager;this.flyItem=flyItem;this.schedule=schedule;
            this.close=close;this.queue=queue;this.hidden=hidden;
        }
        public void Init(OriginalItemGetInfo info)
        {
            // Source has no main child or label_* descendants; custom alpha
            // preserves its serialized backdrop without a base panel tween.
            this.info=info;
        }
        public void Refresh()
        {
            info.IsMore=true;
            views=factory.GenerateItem(info,items,false,true);
            schedule(grantDelay,CollectAndClose);
        }
        private void CollectAndClose()
        {
            // Delayed callbacks read the current view list, as the source does.
            foreach(var view in views)
            {
                itemManager.Add(view.ItemInfo,true);
                flyItem(view.ItemInfo.ItemType,view.Icon);
            }
            schedule(closeDelay,Close);
        }
        public void Close()=>close();
        public void Hide()
        {
            hidden?.Invoke();
            schedule(queueDelay,queue.Dequeue);
        }
    }
}
