using System;
using UnityEngine;
using UnityEngine.UI;

namespace NutSort.UI
{
    public sealed class OriginalGMButton : MonoBehaviour
    {
        [SerializeField] private Button button;
        [SerializeField] private int panelId;
        private Func<bool> isTest,tryClick;
        private Action<int> openPanel;
        private Action playClick;
        public Button Button => button;

        public void Bind(Func<bool> isTest,Func<bool> tryClick,Action<int> openPanel,Action playClick)
        {
            this.isTest=isTest ?? throw new ArgumentNullException(nameof(isTest));
            this.tryClick=tryClick ?? throw new ArgumentNullException(nameof(tryClick));
            this.openPanel=openPanel ?? throw new ArgumentNullException(nameof(openPanel));
            this.playClick=playClick ?? throw new ArgumentNullException(nameof(playClick));
        }
        public void Init()
        {
            button.transition=Selectable.Transition.ColorTint;
            button.onClick.RemoveAllListeners();button.onClick.AddListener(Clicked);
            button.gameObject.SetActive(isTest());
        }
        private void Clicked()
        {
            if(!tryClick())return;
            openPanel(panelId);
            playClick();
        }
    }
}
