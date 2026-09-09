using System;
namespace NutSort.Content
{
    public sealed class OriginalRegistrationRequests
    {
        private readonly Func<OriginalUserLocalData> user;
        private readonly Func<OriginalCountryInfo> country;
        private readonly Action<string,Action<string,byte[]>> request;
        public OriginalRegistrationRequests(Func<OriginalUserLocalData> user,Func<OriginalCountryInfo> country,
            Action<string,Action<string,byte[]>> request)
        {this.user=user;this.country=country;this.request=request;}
        public void Register(Action<bool> callback)
        {
            Request(response=>
            {
                if(response!=null&&response.IsSuccess)
                {
                    user().UserId=response.kinetic_data.copp_uuid;
                    callback?.Invoke(true);
                }
                else callback?.Invoke(false);
            });
        }
        public void Request(Action<OriginalRegistrationResponse> callback=null)
        {
            // Native checkSystemProp/checkNetProp stay at the excluded SDK boundary.
            request(country().Area,(json,bytes)=>
            {
                if(string.IsNullOrEmpty(json)){callback?.Invoke(null);return;}
                var response=OriginalRegistrationResponse.Read(json);
                if(response==null)throw new NullReferenceException("RegisterS2C");
                response.Init();
                callback?.Invoke(response);
            });
        }
    }
}
