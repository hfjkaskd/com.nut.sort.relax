using System;
using NutSort.Content;
using UnityEngine;
using UnityEngine.UI;

namespace NutSort.UI
{
    public sealed class OriginalPlayerGoldHintIcons
    {
        private readonly OriginalPayChannels channels;
        private readonly Func<string> country;
        private readonly Func<int,int,int> range;
        private readonly Func<string,Sprite> load;
        public OriginalPlayerGoldHintIcons(OriginalPayChannels channels,Func<string> country,
            Func<string,Sprite> load=null,Func<int,int,int> range=null)
        {
            this.channels=channels ?? throw new ArgumentNullException(nameof(channels));
            this.country=country ?? throw new ArgumentNullException(nameof(country));
            this.load=load ?? Resources.Load<Sprite>;
            this.range=range ?? UnityEngine.Random.Range;
        }
        private Sprite RandomIcon()
        {
            string channel=channels.GetPayChannel(string.Empty,country).RandomChannel(range);
            return load(OriginalChannelInfos.IconPath(channel));
        }
        public void ApplyPush(Image head)
        {
            Sprite icon=RandomIcon();
            if(icon!=null)head.sprite=icon;
        }
        public void RefreshRandom(Image head) { head.sprite=RandomIcon(); }
        public void ApplySelf(Image head,OriginalChannelInfo selected) { head.sprite=load(OriginalChannelInfos.IconPath(selected.Channel)); }
    }
}
