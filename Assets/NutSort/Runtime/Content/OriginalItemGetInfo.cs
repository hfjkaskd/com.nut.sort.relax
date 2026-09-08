using System;
using System.Collections.Generic;
namespace NutSort.Content
{
    public sealed class OriginalItemGetInfo
    {
        public List<OriginalItemInfo> ItemInfos;
        public bool IsMore;
        public Action<bool> CallBack;
    }
}
