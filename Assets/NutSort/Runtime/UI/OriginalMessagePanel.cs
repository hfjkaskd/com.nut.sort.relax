using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
namespace NutSort.UI
{
    public sealed class OriginalMessagePanel : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI Content;
        [SerializeField] private Button Ok;
        private Func<int,string> text;
        public Action OkCallbackAction;
        public TextMeshProUGUI ContentText=>Content;
        public Button ConfirmButton=>Ok;
        public void Bind(Func<int,string> getText){text=getText;}
        // Source adds a listener each Init; it does not replace prior listeners.
        public void Init(){Ok.onClick.AddListener(OkCallback);}
        public void OkCallback()
        {
            gameObject.SetActive(false);
            OkCallbackAction?.Invoke();
        }
        public void Show(int id,Action callback,bool isCanClose=false)
        {
            foreach(var label in GetComponentsInChildren<TextMeshProUGUI>(true))
            {
                if(!label.name.StartsWith("label_"))continue;
                string token=label.name.Trim().Split('_')[1];
                if(!int.TryParse(token,out int labelId)){Debug.LogError("ToInt fail s:"+token);labelId=0;}
                label.text=text(labelId);
            }
            Show(text(id),callback,false);
        }
        // isCanClose is unused in both source overloads.
        public void Show(string value,Action callback,bool isCanClose=false)
        {
            Content.text=value;
            OkCallbackAction=callback;
            gameObject.SetActive(true);
        }
    }
}
