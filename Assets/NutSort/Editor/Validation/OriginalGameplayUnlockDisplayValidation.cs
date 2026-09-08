using System;
using System.Collections.Generic;
using NutSort.Content;
using NutSort.UI;
using UnityEngine;
namespace NutSort.Validation
{
    public static class OriginalGameplayUnlockDisplayValidation
    {
        public static void Validate()
        {
            var trace=new List<string>();int count=4;bool throws=false;string text=null;
            var table=new OriginalTables(Resources.Load<OriginalTableSettings>("Configuration/OriginalTables"));
            var display=new OriginalGameplayUnlockDisplay(()=>count,(i,active)=>
            {trace.Add(i+":"+(active?1:0));if(throws && i==1)throw new InvalidOperationException("fixture");},
                id=>{trace.Add("text:"+id);text=table.Text.GetText(id,"en");});
            display.Refresh(0);
            Check(string.Join(",",trace)=="0:1,1:0,2:0,3:0,text:63" && text=="Hidden nuts are revealed when they are at the top","Icon order before original hidden-nut text");
            string[] expected={"Complete a set of matching nuts to uncover the curtain","Bolts blocked by rocks cannot have nuts removed","Fill all surrounding bolts to unlock the blocked bolt"};
            for(int i=1;i<4;i++){trace.Clear();display.Refresh(i);Check(text==expected[i-1] && trace[i]==i+":1","Original gameplay description and matching icon");}
            trace.Clear();display.Refresh(5);Check(string.Join(",",trace)=="0:0,1:0,2:0,3:0,text:68","Out-of-range index hides every icon without clamping text id");
            trace.Clear();count=0;display.Refresh(-1);Check(string.Join(",",trace)=="text:62","Empty icon collection still refreshes text");
            count=4;throws=true;trace.Clear();bool caught=false;try{display.Refresh(0);}catch(InvalidOperationException){caught=true;}
            Check(caught && string.Join(",",trace)=="0:1,1:0","Icon callback exception stops later icons and text");
            int visits=0;count=4;
            var changing=new OriginalGameplayUnlockDisplay(()=>count,(i,active)=>{visits++;count=1;},id=>{});
            changing.Refresh(0);Check(visits==1,"Count is read again after each icon callback");
            Debug.Log("NUT_GAMEPLAY_UNLOCK_DISPLAY_VALIDATION_PASS icon-before-text order, actual four original text entries, invalid/empty index behavior, live count and exception boundary.");
        }
        private static void Check(bool value,string message){if(!value)throw new InvalidOperationException(message);}
    }
}
