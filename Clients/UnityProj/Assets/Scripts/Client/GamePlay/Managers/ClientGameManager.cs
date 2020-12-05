using BiangStudio.DragHover;
using BiangStudio.GamePlay;
using BiangStudio.GamePlay.UI;
using BiangStudio.Singleton;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ClientGameManager : MonoSingleton<ClientGameManager>
{
    #region Managers

    #region Mono

    private ConfigManager ConfigManager => ConfigManager.Instance;
    private AudioManager AudioManager => AudioManager.Instance;
    private CameraManager CameraManager => CameraManager.Instance;
    private UIManager UIManager => UIManager.Instance;

    #endregion

    #region TSingletonBaseManager

    #region Resources

    private LayerManager LayerManager => LayerManager.Instance;
    private PrefabManager PrefabManager => PrefabManager.Instance;
    private GameObjectPoolManager GameObjectPoolManager => GameObjectPoolManager.Instance;

    #endregion

    #region Framework

    private GameStateManager GameStateManager => GameStateManager.Instance;
    private RoutineManager RoutineManager => RoutineManager.Instance;

    #endregion

    #region GamePlay

    private DragManager DragManager => DragManager.Instance;
    private MouseHoverManager MouseHoverManager => MouseHoverManager.Instance;
    private DragExecuteManager DragExecuteManager => DragExecuteManager.Instance;

    #region Level

    private LevelManager LevelManager => LevelManager.Instance;
    private FXManager FXManager => FXManager.Instance;
    private ProjectileManager ProjectileManager => ProjectileManager.Instance;

    #endregion

    #endregion

    #endregion

    #endregion

    private void Awake()
    {
        UIManager.Init(
            (prefabName) => Instantiate(PrefabManager.GetPrefab(prefabName)),
            Debug.LogError,
            () => Input.GetMouseButtonDown(0),
            () => Input.GetMouseButtonDown(1),
            () => Input.GetKeyDown(KeyCode.Escape),
            () => Input.GetKeyDown(KeyCode.Return),
            () => Input.GetKeyDown(KeyCode.Tab)
        );

        ConfigManager.Awake();
        LayerManager.Awake();
        PrefabManager.Awake();
        if (!GameObjectPoolManager.IsInit)
        {
            Transform root = new GameObject("GameObjectPool").transform;
            DontDestroyOnLoad(root.gameObject);
            GameObjectPoolManager.Init(root);
            GameObjectPoolManager.Awake();
        }

        RoutineManager.LogErrorHandler = Debug.LogError;
        RoutineManager.Awake();
        GameStateManager.Awake();

        DragManager.Awake();
        MouseHoverManager.Awake();
        MouseHoverManager.Init(
            () => Input.GetMouseButtonDown(0),
            () => Input.GetMouseButtonUp(0),
            () => Input.mousePosition);
        InitMouseHoverManager();
        DragExecuteManager.Init();
        DragExecuteManager.Awake();

        FXManager.Awake();
        ProjectileManager.Awake();
        ProjectileManager.Init(new GameObject("ProjectileRoot").transform);
    }

    private void Start()
    {
        ConfigManager.Start();
        LayerManager.Start();
        PrefabManager.Start();
        GameObjectPoolManager.Start();

        RoutineManager.Start();
        GameStateManager.Start();

        DragManager.Start();
        MouseHoverManager.Start();
        DragExecuteManager.Start();

        FXManager.Start();
        ProjectileManager.Start();

        UIManager.Instance.ShowUIForms<DebugPanel>();
#if !DEBUG
        UIManager.Instance.CloseUIForm<DebugPanel>();
#endif
        StartGame();
    }

    private void Update()
    {
        if (Input.GetKeyUp(KeyCode.F10))
        {
            ReloadGame();
            return;
        }
        else
        {
            ConfigManager.Update(Time.deltaTime);
            LayerManager.Update(Time.deltaTime);
            PrefabManager.Update(Time.deltaTime);
            GameObjectPoolManager.Update(Time.deltaTime);

            RoutineManager.Update(Time.deltaTime, Time.frameCount);
            GameStateManager.Update(Time.deltaTime);

            DragManager.Update(Time.deltaTime);
            MouseHoverManager.Update(Time.deltaTime);
            DragExecuteManager.Update(Time.deltaTime);

            FXManager.Update(Time.deltaTime);
            ProjectileManager.Update(Time.deltaTime);
        }
    }

    void LateUpdate()
    {
        ConfigManager.LateUpdate(Time.deltaTime);
        LayerManager.LateUpdate(Time.deltaTime);
        PrefabManager.LateUpdate(Time.deltaTime);
        GameObjectPoolManager.LateUpdate(Time.deltaTime);

        RoutineManager.LateUpdate(Time.deltaTime);
        GameStateManager.LateUpdate(Time.deltaTime);

        DragManager.LateUpdate(Time.deltaTime);
        MouseHoverManager.LateUpdate(Time.deltaTime);
        DragExecuteManager.LateUpdate(Time.deltaTime);

        FXManager.LateUpdate(Time.deltaTime);
        ProjectileManager.LateUpdate(Time.deltaTime);
    }

    void FixedUpdate()
    {
        ConfigManager.FixedUpdate(Time.fixedDeltaTime);
        LayerManager.FixedUpdate(Time.fixedDeltaTime);
        PrefabManager.FixedUpdate(Time.fixedDeltaTime);
        GameObjectPoolManager.FixedUpdate(Time.fixedDeltaTime);

        RoutineManager.FixedUpdate(Time.fixedDeltaTime);
        GameStateManager.FixedUpdate(Time.fixedDeltaTime);

        DragManager.FixedUpdate(Time.fixedDeltaTime);
        MouseHoverManager.FixedUpdate(Time.fixedDeltaTime);
        DragExecuteManager.FixedUpdate(Time.fixedDeltaTime);

        FXManager.FixedUpdate(Time.fixedDeltaTime);
        ProjectileManager.FixedUpdate(Time.fixedDeltaTime);
    }

    private void StartGame()
    {
        LevelManager.StartGame();
    }

    public void ReloadGame()
    {
        ShutDownGame();
        SceneManager.LoadScene("MainScene");
    }

    private void ShutDownGame()
    {
        ProjectileManager.ShutDown();
        FXManager.ShutDown();

        DragExecuteManager.ShutDown();
        MouseHoverManager.ShutDown();
        DragManager.ShutDown();

        GameStateManager.ShutDown();
        RoutineManager.ShutDown();

        GameObjectPoolManager.ShutDown();
        PrefabManager.ShutDown();
        LayerManager.ShutDown();
        ConfigManager.ShutDown();
    }

    private void InitMouseHoverManager()
    {
        MouseHoverManager.Instance.AddHoverAction(new MouseHoverManager.Hover<MouseHoverUI>(LayerManager.LayerMask_UI, MouseHoverManager.StateMachine.States.UI, UIManager.Instance.UICamera));
        MouseHoverManager.Instance.AddHoverAction(new MouseHoverManager.Hover<Building>(LayerManager.LayerMask_BuildingHitBox, MouseHoverManager.StateMachine.States.Building, CameraManager.Instance.MainCamera));
    }
}