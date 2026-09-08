using System;
using System.Collections.Generic;
using NutSort.Content;
using NutSort.UI;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

namespace NutSort.Validation
{
    public static class OriginalMarqueeItemValidation
    {
        public static void Validate()
        {
            var tables=new OriginalTables(Resources.Load<OriginalTableSettings>("Configuration/OriginalTables"));
            var instance=UnityEngine.Object.Instantiate(Resources.Load<GameObject>("Prefabs/Panels/PMDItem"));
            var defaults=ScriptableObject.CreateInstance<OriginalUserDefaults>();
            try
            {
                var view=instance.GetComponent<OriginalMarqueeItemView>();
                Check(view!=null && instance.GetComponentsInChildren<Transform>(true).Length==3,"Original hierarchy");
                foreach(var node in instance.GetComponentsInChildren<Transform>(true))
                    Check(GameObjectUtility.GetMonoBehavioursWithMissingScriptCount(node.gameObject)==0,"No missing scripts");
                Check(view.HeadImage.sprite!=null && view.Tip.font!=null && view.Tip.fontSharedMaterial!=null,"Icon/font references");
                Check(view.HeadImage.material.shader.name=="UI/CircleImage" && !ShaderUtil.ShaderHasError(view.HeadImage.material.shader),"Circle shader");
                Check(Mathf.Approximately(view.HeadImage.material.GetFloat("_Radius"),.49f),"Source radius");
                var fitter=view.Tip.GetComponent<ContentSizeFitter>();
                Check(fitter.horizontalFit==ContentSizeFitter.FitMode.PreferredSize && fitter.verticalFit==ContentSizeFitter.FitMode.Unconstrained,"Original fitter");
                Check(view.Tip.enableAutoSizing && view.Tip.fontSizeMin==18 && view.Tip.fontSizeMax==30 && !view.Tip.enableWordWrapping,"Original font sizing");
                var trace=new List<string>();var pending=new List<Action>();
                var text=new OriginalMarqueeText(new OriginalUserLocalData(defaults),tables,()=>{trace.Add("name");return "Player_AI29";},n=>{trace.Add("format");return "$7";},(a,b)=>{trace.Add("amount");return 7;});
                var oldIcon=view.HeadImage.sprite;
                view.Bind(text,tables.PayChannels,"en",()=>{trace.Add("country");return "US";},(delay,done)=>{trace.Add("delay");Check(delay==.01f,"Original delay");pending.Add(done);},path=>{trace.Add("icon");Check(path=="Atlas/PaySimple/PayPal","Original path");return null;},(a,b)=>{trace.Add("channel");Check(a==0 && b==3,"Channel bounds");return 2;});
                Vector2 before=view.Rect.sizeDelta;view.IsReady=true;
                view.Init(new OriginalMarqueeItem { Minimum=5,Maximum=9,IsGold=true },3);
                Check(string.Join(",",trace)=="name,amount,format,country,channel,icon,delay","Native call order");
                Check(view.HeadImage.sprite==oldIcon && view.Rect.sizeDelta==before && view.IsReady,"Retain missing icon, width and ready");
                Check(view.Tip.text==tables.Text.GetText(35,"en","Player_AI29","$7"),"Text connection");
                view.Init(new OriginalMarqueeItem(),3);Check(pending.Count==2,"Separate delays");
                view.Tip.rectTransform.sizeDelta=new Vector2(321,64);view.Rect.sizeDelta=new Vector2(before.x,77);pending[0]();
                Check(view.Rect.sizeDelta==new Vector2(461,77),"Read current text width and preserve height");
                view.Tip.rectTransform.sizeDelta=new Vector2(400,64);pending[1]();
                Check(view.Rect.sizeDelta==new Vector2(540,77),"Repeated callbacks retained");
                Debug.Log("NUT_MARQUEE_ITEM_VALIDATION_PASS original prefab/font/fitter/circle material, text-icon-delay order, retained missing sprite and delayed current width.");
            }
            finally { UnityEngine.Object.DestroyImmediate(instance);UnityEngine.Object.DestroyImmediate(defaults); }
        }
        private static void Check(bool condition,string message) { if(!condition)throw new InvalidOperationException(message); }
    }
}
