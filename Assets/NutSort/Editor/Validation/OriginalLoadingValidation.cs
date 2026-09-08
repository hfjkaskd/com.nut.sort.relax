using System;
using System.IO;
using NutSort.UI;
using UnityEngine;

namespace NutSort.Validation
{
    public static class OriginalLoadingValidation
    {
        public static void Validate(OriginalLoadingView view)
        {
            Check(view!=null,"Loading view restored");
            view.SetState(true);view.Advance(.25f);
            Near(view.Value,.125f,"Original auto progress rate");
            Check(view.PercentageLabel.text==string.Format("{0:F0}%",view.Value*100f),"Source F0 rounding");
            Near(view.Marker.anchoredPosition.x,93.75f,"Original 750-unit marker travel");
            view.Advance(10f);Near(view.Value,.9f,"Automatic progress caps at 90 percent");
            view.SetState(false);view.Advance(.25f);Near(view.Value,.975f,"Default OutQuad completion midpoint");
            Check(view.IsVisible,"Still visible during completion");
            view.Advance(.25f);Near(view.Value,1f,"Completion reaches 100 percent");
            Check(view.PercentageLabel.text=="100%","Completion label");
            view.Advance(.199f);Check(view.IsVisible,"Original completed hold");
            view.Advance(.002f);Check(!view.IsVisible,"Hide after completed hold");
            view.SetState(true);Near(view.Value,0f,"Reopen resets progress");
            view.Advance(0f);Near(view.Value,0f,"Scaled pause does not advance loading");
            Check(view.PercentageLabel.font.name=="zh_Custom SDF","Original font retained");
            Near(view.PercentageLabel.fontSize,44.5f,"Original percentage font size");
            view.SetState(false);view.Advance(.501f);view.Advance(.201f);
            Debug.Log("NUT_LOADING_VALIDATION_PASS original 90-percent cap, half-second OutQuad completion, 0.2-second hold, marker position, F0 label, source font and repeated loading cycle.");
        }
        private static void Check(bool value,string message){if(!value)throw new InvalidDataException(message);}
        private static void Near(float a,float b,string message){Check(Mathf.Abs(a-b)<.00001f,message+": "+a);}
    }
}
