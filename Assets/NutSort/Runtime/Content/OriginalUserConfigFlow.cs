using System;
namespace NutSort.Content
{
    // UserMgr.Config and its response/retry closures. The request manager supplies
    // its actual boolean result; neither this flow nor the view invents success.
    public sealed class OriginalUserConfigFlow
    {
        private readonly Action<Action<bool>> request;
        private readonly Action<int,Action,bool> showMessage;
        public OriginalUserConfigFlow(Action<Action<bool>> request,Action<int,Action,bool> showMessage)
        {this.request=request;this.showMessage=showMessage;}
        public void Config(Action<bool> callback,bool isPopMessageBox)
        {
            Action retry=null;
            request(success=>
            {
                if(!success&&isPopMessageBox)
                {
                    if(retry==null)retry=()=>Config(callback,true);
                    showMessage(75,retry,false);
                    return;
                }
                callback?.Invoke(success);
            });
        }
    }
}
