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
        public event Action SelectionSoundRequested;
        public event Action<int> MoveSoundRequested;
        public event Action MoveAttempted;

        public void Bind(LevelData data, OriginalPrefabPool prefabPool, bool useLongEntryDelay, bool useLssab)
        {
            BindBoard(new OriginalBoardState(data, layout), prefabPool, useLongEntryDelay, useLssab);
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
            operation.SelectionSoundRequested += ForwardSelectionSound;
            operation.MoveSoundRequested += ForwardMoveSound;
            operation.MoveAttempted += ForwardMoveAttempt;
            ScenePosGroup.Bind(pool, layout, settings, Board.Screws.Length);
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
            float size = layout.CameraSize(width, height, layout.MaxColumnCount(Board.Screws.Length));
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
            // Level.Update remembers the clicked target on every true return,
            // including an invalid destination that just reverted the source.
            if (result.OriginalReturnValue) selected = target;
            OperationApplied?.Invoke(result, target);
            return result;
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
