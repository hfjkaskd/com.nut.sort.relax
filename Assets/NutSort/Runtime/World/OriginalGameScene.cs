using System;
using NutSort.Content;
using NutSort.Gameplay;
using UnityEngine;

namespace NutSort.World
{
    public sealed class OriginalGameScene : MonoBehaviour
    {
        [SerializeField] private Camera GameCamera;
        [SerializeField] private Camera TopGameCamera;
        [SerializeField] private OriginalPrefabPool pool;
        [SerializeField] private OriginalContentSettings content;
        [SerializeField] private OriginalSceneSession session;
        [SerializeField] private OriginalGameplayEffects effects;
        private OriginalLevelView level;
        private int viewportWidth, viewportHeight;
        public OriginalLevelView Level => level;
        public Camera WorldCamera => GameCamera;
        public bool InputBlocked { get; set; }
        public bool IsExchanging { get; set; }
        public event Action<ScrewOperation, ScrewState> OperationApplied;

        private void Start() { Initialize(); }

        public void Initialize()
        {
            if (level != null) return;
            if (pool == null || content == null || session == null || GameCamera == null || TopGameCamera == null)
                throw new InvalidOperationException("Original game scene references are incomplete.");
            var repository = new OriginalLevelRepository(content);
            repository.Initialize();
            // Session data is explicit configuration until original save and
            // region/server initialization are connected. No fabricated grants.
            var progress = new LevelProgressState { Level = session.Level, LevelSeed = session.Seed };
            var selector = new OriginalLevelSelector(repository, content, UnityLevelRandom.Instance);
            LevelSelection selection = selector.Select(progress, session.LSS260820, session.LSSSHSLV);
            level = pool.Rent(session.LevelPrefabPath, transform).GetComponent<OriginalLevelView>();
            level.Bind(repository.LoadBoard(selection.Loop, selection.Seed), pool, session.LongEntryDelay, session.LSSAB);
            effects.Bind(level, pool);
            level.OperationApplied += ForwardOperation;
            ResizeCameras(Screen.width, Screen.height);
        }

        private void ForwardOperation(ScrewOperation result, ScrewState target) { OperationApplied?.Invoke(result, target); }

        public void ResizeCameras(int width, int height)
        {
            viewportWidth = width; viewportHeight = height;
            level.FixCameras(GameCamera, TopGameCamera, width, height);
        }

        private void Update()
        {
            if (level == null) return;
            if (viewportWidth != Screen.width || viewportHeight != Screen.height) ResizeCameras(Screen.width, Screen.height);
            // Use the same input policy on every platform. A single touch ends
            // or a mouse button releases; no Application.isEditor alternate path.
            int touches = Input.touchCount;
            if (touches == 1)
            {
                Touch touch = Input.GetTouch(0);
                if (touch.phase == TouchPhase.Ended) TryOperateAtScreenPoint(touch.position);
            }
            else if (touches == 0 && Input.GetMouseButtonUp(0)) TryOperateAtScreenPoint(Input.mousePosition);
        }

        public ScrewOperation TryOperateAtScreenPoint(Vector2 position)
        {
            if (InputBlocked || level == null || !level.AreNutsInitialized) return default;
            Ray ray = GameCamera.ScreenPointToRay(position);
            // These are 3D gameplay objects, not UI controls. UI panels use
            // standard Buttons and set InputBlocked through their lifecycle.
            if (!Physics.Raycast(ray, out RaycastHit hit, session.RaycastDistance, session.GameplayLayers)) return default;
            if (!hit.transform.TryGetComponent(out OriginalScrewView screw) || screw.State == null) return default;
            return level.Operate(screw.State.Index, IsExchanging);
        }

        private void OnDestroy()
        {
            if (level == null) return;
            effects.Clear();
            level.OperationApplied -= ForwardOperation;
            level.Clear();
            level = null;
        }
    }
}
