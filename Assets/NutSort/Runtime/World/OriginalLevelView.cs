using System;
using System.Collections.Generic;
using NutSort.Content;
using NutSort.Gameplay;
using UnityEngine;

namespace NutSort.World
{
    public sealed class OriginalLevelView : MonoBehaviour
    {
        [SerializeField] private OriginalScenePosGroup ScenePosGroup;
        [SerializeField] private OriginalLayoutSettings layout;
        [SerializeField] private OriginalWorldSettings world;
        [SerializeField] private OriginalScrewSettings settings;
        private OriginalPrefabPool pool;
        private OriginalScrewView[] screws;
        private readonly Dictionary<NutState, OriginalNutView> nuts = new Dictionary<NutState, OriginalNutView>();
        private OriginalScrewOperator operation;
        private OriginalDeadlockRules deadlock;
        private ScrewState selected;
        private bool pendingNuts, lssab;
        private float spawnDelay, spawnTime;
        public OriginalBoardState Board { get; private set; }
        public bool AreNutsInitialized => Board != null && !pendingNuts;
        public int NutViewCount => nuts.Count;
        public event Action<ScrewOperation, ScrewState> OperationApplied;
        public event Action<OriginalNutView> SparkRequested;
        public event Action<OriginalScrewView> DoneEffectRequested;
        public event Action<ScrewState> ScrewCompleted;
        public event Action Clearing;
        public event Action<OriginalScrewView> UnlockRequested;
        public event Action SelectionSoundRequested;
        public event Action<int> MoveSoundRequested;
        public event Action MoveAttempted;
        public event Action SaveRequested;

        public void Bind(LevelData data, OriginalPrefabPool prefabPool, bool useLongEntryDelay, bool useLssab, bool addLockedScrew = false)
        {
            BindBoard(new OriginalBoardState(data, layout, addLockedScrew), prefabPool, useLongEntryDelay, useLssab);
        }

        public void BindSaved(OriginalBoardSnapshot snapshot, OriginalPrefabPool prefabPool, bool useLongEntryDelay, bool useLssab)
        {
            if (snapshot == null) throw new ArgumentNullException(nameof(snapshot));
            BindBoard(snapshot.Restore(layout), prefabPool, useLongEntryDelay, useLssab);
            for (int i = 0; i < snapshot.OperatorInfos.Count; i++)
            {
                OriginalMoveRecord move = snapshot.OperatorInfos[i];
                operation.MoveHistory.Add(move == null ? null : new OriginalMoveRecord
                { FromScrewIndex = move.FromScrewIndex, ToScrewIndex = move.ToScrewIndex, NutCount = move.NutCount });
            }
        }

        public OriginalBoardSnapshot CaptureSnapshot()
        {
            return Board == null ? null : OriginalBoardSnapshot.Capture(Board, operation.MoveHistory);
        }

        private void BindBoard(OriginalBoardState board, OriginalPrefabPool prefabPool, bool useLongEntryDelay, bool useLssab)
        {
            Clear();
            if (layout == null || world == null || settings == null || ScenePosGroup == null || prefabPool == null)
                throw new InvalidOperationException("Original level prefab configuration is incomplete.");
            Board = board;
            pool = prefabPool; lssab = useLssab;
            operation = new OriginalScrewOperator();
            if (deadlock == null) deadlock = new OriginalDeadlockRules(PlayHiddenBreak, ForwardSave);
            operation.SelectionSoundRequested += ForwardSelectionSound;
            operation.MoveSoundRequested += ForwardMoveSound;
            operation.MoveAttempted += ForwardMoveAttempt;
            ScenePosGroup.Bind(pool, layout, settings, Board);
            screws = new OriginalScrewView[Board.Screws.Length];
            for (int i = 0; i < screws.Length; i++)
            {
                var instance = pool.Rent(settings.PrefabPath, ScenePosGroup.GetPosition(i));
                instance.transform.localPosition = Vector3.zero;
                instance.transform.localRotation = Quaternion.identity;
                var view = instance.GetComponent<OriginalScrewView>();
                view.Bind(Board.Screws[i], pool, world, settings, lssab);
                screws[i] = view;
            }
            spawnDelay = useLongEntryDelay ? settings.NutInitLongDelay : settings.NutInitDelay;
            spawnTime = 0f; pendingNuts = true;
        }

        private void Update() { AdvanceInitialization(Time.deltaTime); }
        private void ForwardSelectionSound() { SelectionSoundRequested?.Invoke(); }
        private void ForwardMoveSound(int count) { MoveSoundRequested?.Invoke(count); }
        private void ForwardMoveAttempt() { MoveAttempted?.Invoke(); }
        private void ForwardSave() { SaveRequested?.Invoke(); }
        private void PlayHiddenBreak(int index) { screws[index].TypeView.PlayHiddenBreak(); }

        // This is stateful: hidden-rod transitions can request visual changes
        // and immediate saves. Do not call it from an Update polling loop.
        public bool IsCannotMove() => deadlock != null && deadlock.IsCannotMove(Board);

        public void AdvanceInitialization(float deltaTime)
        {
            if (!pendingNuts) return;
            if (deltaTime < 0f) throw new ArgumentOutOfRangeException(nameof(deltaTime));
            spawnTime += deltaTime;
            if (spawnTime < spawnDelay) return;
            pendingNuts = false;
            for (int i = 0; i < screws.Length; i++)
            {
                ScrewState state = Board.Screws[i];
                for (int j = 0; j < state.Slots.Length; j++)
                {
                    NutSlot slot = state.Slots[j];
                    if (slot.Nut == null) continue;
                    var instance = pool.Rent(world.NutPath(slot.Nut.Color), screws[i].GetTile(slot.Coordinate.y).transform);
                    instance.transform.localPosition = Vector3.zero;
                    instance.transform.localRotation = Quaternion.identity;
                    var view = instance.GetComponent<OriginalNutView>();
                    view.BindVisual(slot, world, pool);
                    nuts.Add(slot.Nut, view);
                    view.PlayEntry(state, screws[i].InitialPosition, slot.Coordinate.y);
                }
            }
        }

        public void FixCameras(Camera gameCamera, Camera topGameCamera, int width, int height)
        {
            float size = layout.CameraSize(width, height, ScenePosGroup.MaxColumnCount);
            gameCamera.orthographicSize = topGameCamera.orthographicSize = size;
        }

        public ScrewOperation Operate(int targetIndex, bool isExchanging = false)
        {
            if (!AreNutsInitialized) return default;
            ScrewState target = Board.Screws[targetIndex];
            ScrewOperation result = operation.Operate(target, selected, isExchanging);
            if (result.Kind == ScrewOperationKind.Ready)
                nuts[result.AnimatedSlot.Nut].PlayReady(screws[targetIndex].ReadyPosition);
            else if (result.Kind == ScrewOperationKind.Reverted)
            {
                OriginalNutView view = nuts[result.AnimatedSlot.Nut];
                view.PlayRevert(null, () => SparkRequested?.Invoke(view));
            }
            else if (result.Kind == ScrewOperationKind.Moved)
            {
                NutMoveBatch batch = result.Move;
                OriginalScrewView destination = screws[batch.Destination.Index];
                for (int i = 0; i < batch.Transfers.Length; i++)
                {
                    int moveIndex = i;
                    NutTransfer transfer = batch.Transfers[i];
                    OriginalNutView view = nuts[transfer.Nut];
                    view.PlayTransfer(transfer.Destination, destination.GetTile(transfer.Destination.Coordinate.y).transform,
                        destination.ReadyPosition, i, transfer.WasReady, () => OnLanded(batch, moveIndex),
                        () => SparkRequested?.Invoke(view));
                }
                foreach (NutSlot slot in batch.Source.Slots)
                    if (slot.Nut != null) nuts[slot.Nut].RefreshVisual();
            }
            if(result.Kind==ScrewOperationKind.ExchangeRequested)exchangeRequested?.Invoke(target);
            if(result.Kind==ScrewOperationKind.AddScrewRequested)addScrewRequested?.Invoke();
            // Level.Update remembers the clicked target on every true return,
            // including an invalid destination that just reverted the source.
            if (result.OriginalReturnValue) selected = target;
            OperationApplied?.Invoke(result, target);
            return result;
        }

        // LevelInfo.Unlock 0x9FAD44 selects the first locked or capacity<=3 rod.
        public bool Unlock(Func<bool> singleTile)
        {
            for(int i=0;i<Board.Screws.Length;i++)
            {
                var state=Board.Screws[i];
                if(!state.IsLocked && state.Capacity>3)continue;
                state.IsLocked=false;
                screws[i].TypeView.RefreshLock(state,singleTile);
                UnlockRequested?.Invoke(screws[i]);
                screws[i].AddTile(singleTile());
                return true;
            }
            return false;
        }

        // LevelInfo.AddNullScrew 0x9FA0A4 initializes after row layout and camera refresh.
        public OriginalScrewView AddNullScrew(Func<bool> singleTile,Action fixCameras)
        {
            var state=Board.AppendNullScrew(singleTile);
            Transform position=ScenePosGroup.Append(Board,layout,settings);
            fixCameras();
            var expanded=new OriginalScrewView[screws.Length+1];Array.Copy(screws,expanded,screws.Length);screws=expanded;
            var instance=pool.Rent(settings.PrefabPath,position);
            instance.transform.localPosition=Vector3.zero;instance.transform.localRotation=Quaternion.identity;
            var view=instance.GetComponent<OriginalScrewView>();screws[screws.Length-1]=view;
            view.Bind(state,pool,world,settings,lssab);
            UnlockRequested?.Invoke(view);
            return view;
        }

        // IsCanAddTile 0xA07E48 always returns true. Manager AddTile
        // (0x9FE6B0) stops after the first eligible entry, including full rods.
        public void AddTile(Func<bool> singleTile)
        {
            if(Board.Screws.Length==0)return;
            screws[0].AddTile(singleTile());
        }

        private Action addScrewRequested;
        public void BindAddScrew(Action addScrew) { addScrewRequested=addScrew ?? throw new ArgumentNullException(nameof(addScrew)); }

        private Action<ScrewState> exchangeRequested;
        public void BindExchange(Action<ScrewState> exchange) { exchangeRequested=exchange; }

        // ScrewInfo.Exchange 0xA08A18 rotates the occupied prefix by the top
        // visible same-color group. It reorders slot objects, unlike normal transfers.
        public void Exchange(ScrewState target,Action<int,float,bool> applyItem,Action fail)
        {
            int count=0;
            foreach(var slot in target.Slots)if(slot.Nut!=null)count++;
            var top=new List<NutSlot>();target.GetTopSame(top);
            var reordered=new NutSlot[count];
            for(int i=0;i<count;i++)reordered[i]=target.Slots[i<top.Count?count+i-top.Count:i-top.Count];
            for(int i=0;i<count;i++)
            {
                target.Slots[i]=reordered[i];
                var slot=target.Slots[i];slot.RefreshPosition(i);
                nuts[slot.Nut].RefreshPosition(slot,screws[target.Index].GetTile(i).transform);
            }
            target.GetTopSame(top);
            foreach(var slot in top)
            {
                if(slot.Nut.Type!=NutType.Hidden)continue;
                slot.Nut.Type=NutType.Normal;nuts[slot.Nut].RefreshVisual();
            }
            applyItem(3,-1,true);
            if(IsCannotMove())fail();
            // Original SDK vibration/analytics excluded; no automatic mode exit.
        }

        // SetExchangeState applies ScrewInfo.SetScrewState(!isExchanging) in list order.
        public void SetScrewState(bool visible)
        {
            for(int i=0;i<Board.Screws.Length;i++)
            {
                ScrewState state=Board.Screws[i];
                GameObject target=screws[i].gameObject;
                // Native checks activeSelf, not effective hierarchy visibility.
                if(target.activeSelf==visible)continue;
                if(!visible && (state.IsNull||state.IsColorMask||state.IsHidden||state.IsNutColorSame))
                {
                    target.SetActive(false);
                    continue;
                }
                target.SetActive(true);
                if(!visible && state.IsReadyMove)RevertScrew(state);
            }
        }

        public int MoveHistoryCount => operation.MoveHistory.Count;

        // LuoSiSortMgr.Revoke acts on live history, then checks the resulting
        // board even when a record rejects. Item cost/save belong to ItemMgr.
        public void Revoke(Action<int> refreshButtonState, Action fail)
        {
            new OriginalRevokeFlow(() => operation.MoveHistory, RevokeRecord,
                refreshButtonState, IsCannotMove, fail).Revoke();
        }

        // Record operation only; history removal, failure dispatch and tool cost
        // remain owned by their respective manager flows.
        public bool RevokeRecord(OriginalMoveRecord record)
        {
            return new OriginalRecordRevoke(FindScrew, RevertScrew, ReverseMove,
                state => screws[Array.IndexOf(Board.Screws, state)].RefreshCap()).Revoke(record);
        }

        private ScrewState FindScrew(int index)
        {
            foreach (ScrewState state in Board.Screws)
                if (state.Index == index) return state;
            return null;
        }

        private readonly List<NutSlot> revokeGroup = new List<NutSlot>();
        private void RevertScrew(ScrewState state)
        {
            state.GetTopSame(revokeGroup);
            if (revokeGroup.Count == 0) { Debug.LogError("nutInfos.NutMaxCount == 0"); return; }
            NutSlot slot = revokeGroup[0];
            if (!nuts.TryGetValue(slot.Nut, out OriginalNutView view) || view == null) return;
            slot.IsReady = false;
            view.PlayRevert(null, () => SparkRequested?.Invoke(view));
        }

        private void ReverseMove(ScrewState target, NutTransfer transfer, int index, Action completed)
        {
            if (!nuts.TryGetValue(transfer.Nut, out OriginalNutView view) || view == null) return;
            OriginalScrewView destination = screws[Array.IndexOf(Board.Screws, target)];
            view.PlayTransfer(transfer.Destination, destination.GetTile(transfer.Destination.Coordinate.y).transform,
                destination.ReadyPosition, index, transfer.WasReady, completed, () => SparkRequested?.Invoke(view));
        }

        private void OnLanded(NutMoveBatch batch, int index)
        {
            batch.CompleteMovement(index);
            if (index != batch.Transfers.Length - 1 || !batch.RequiresDoneAnimation) return;
            var view = screws[batch.Destination.Index];
            view.PlayDone(() => { batch.CompleteDoneAnimation(); ScrewCompleted?.Invoke(batch.Destination); },
                () => DoneEffectRequested?.Invoke(view));
        }

        public OriginalScrewView GetScrew(int index) => screws[index];
        public OriginalNutView GetNut(NutState state) => nuts[state];

        public void Clear()
        {
            Clearing?.Invoke();
            pendingNuts = false;
            foreach (OriginalNutView nut in nuts.Values) { nut.ReleaseVisual(); pool.Return(nut.gameObject); }
            nuts.Clear();
            if (screws != null)
                for (int i = 0; i < screws.Length; i++) { screws[i].Release(); pool.Return(screws[i].gameObject); }
            if (ScenePosGroup != null) ScenePosGroup.Clear();
            screws = null; Board = null; pool = null; operation = null; selected = null;
        }
    }
}
