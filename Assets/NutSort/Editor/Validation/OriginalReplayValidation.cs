using System.IO;
using NutSort.UI;
using UnityEditor;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
namespace NutSort.Validation
{
    public static class OriginalReplayValidation
    {
        public static void Validate()
        {
            var settings=Resources.Load<OriginalPanelSettings>("Configuration/OriginalPanels");
            Check(settings.PanelDurationTime==.25f&&settings.TitleDelayTime==.15f&&settings.TitleDurationTime==.25f&&settings.CloseDuration==.25f,"Original panel durations");
            var p=settings.PanelOpenCurve.keys;var t=settings.TitleCurve.keys;
            Check(p.Length==3&&p[1].time==.41856956f&&p[1].value==.95231426f&&p[1].inTangent==1.3587723f,"Original panel curve interior key");
            Check(t.Length==4&&t[1].time==.39306492f&&t[1].value==1.0013074f&&t[2].time==.89124197f&&t[2].value==.96792567f,"Original title overshoot and settling curve");
            Check(settings.BackOvershoot==1.70158f&&settings.BackdropAlpha==.8627451f&&settings.ClickMaskDuration==.2f,"Close InBack and source black backdrop/click mask parameters");
            var prefab=Resources.Load<GameObject>(settings.ReplayPath);
            Check(prefab.GetComponentsInChildren<Button>(true).Length==2&&prefab.GetComponentsInChildren<RectTransform>(true).Length==9,"Original replay nine-object hierarchy and two Buttons");
            var panel=prefab.GetComponent<OriginalReplayPanel>();var serialized=new SerializedObject(panel);
            Check(serialized.FindProperty("CloseBtn").objectReferenceValue==panel.ReplayButton,"Original shared close/replay reference");
            Check(panel.Main.Find("BG").GetComponent<RectTransform>().sizeDelta==new Vector2(819,841),"Original panel body dimensions");
            var instance=Object.Instantiate(prefab);
            try
            {
                var feedback=instance.GetComponent<OriginalReplayPanel>().ContinueButton.GetComponent<OriginalButtonFeedback>();
                feedback.OnEnable();
                var data=new PointerEventData(null);
                feedback.OnPointerDown(data);
                for(int i=0;i<12;i++)feedback.AdvanceFrame(.02f);
                Check(Vector3.Distance(feedback.transform.localScale,Vector3.one*.85f)<.00001f,"Original linear press reaches 85 percent");
                feedback.OnPointerUp(data);
                for(int i=0;i<9;i++)feedback.AdvanceFrame(.02f);
                Check(feedback.transform.localScale==Vector3.one,"Release restores original scale");
            }
            finally { Object.DestroyImmediate(instance); }
            Debug.Log("NUT_REPLAY_VALIDATION_PASS original curves, durations, hierarchy, Button alias, body dimensions and press/release feedback.");
        }
        private static void Check(bool condition,string message){if(!condition)throw new InvalidDataException(message);}
    }
}
