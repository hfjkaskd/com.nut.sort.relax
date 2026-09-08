namespace NutSort.UI
{
    // Typed UI operations selected by the original ShowGuide jump table.
    public interface IOriginalGuideBranches
    {
        void ShowSuccess();
        void ShowTargetCompletion();
        void ShowWithdrawal();
        void ShowCoin();
        void ShowGoldEntry(int guideIndex);
        void ShowWithdrawalStage(int guideIndex);
    }
}
