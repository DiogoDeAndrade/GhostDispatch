using System.Collections;
using System.Collections.Generic;
using UC;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class LevelManager : MonoBehaviour
{
    [System.Serializable]
    struct PortalInitialState
    {
        public bool     enabled;
        public Portal   portal;
    }

    [Header("Traits")]
    [SerializeField] private TraitSet                   allTraits;
    [SerializeField] private TraitGroup                 emotionGroup;
    [SerializeField] private TraitGroup                 colorGroup;
    [SerializeField] private TraitGroup                 accessoryGroup;
    [Header("Queues & Paths")]
    [SerializeField] private GhostQueue                 mainQueue;
    [SerializeField] private float                      maxGhostSpeed = 1.0f;
    [SerializeField] private float                      _signpostRotationSpeed = 360.0f;
    [SerializeField] private List<PortalInitialState>   portals;
    [Header("Soul system")]
    [SerializeField] private float                      maxSouls = 100.0f;
    [SerializeField] private float                      lossPerSoul = 20.0f;
    [SerializeField] private float                      gainPerSoul = 20.0f;
    [Header("Order system")]
    [SerializeField] private float                      timeOfFirstOrder = 1.0f;
    [SerializeField] private Vector2                    timeBetweenOrders = new Vector2(30.0f, 45.0f);
    [SerializeField] private Transform[]                messengerSlots;
    [SerializeField] private Messenger                  messengerPrefab;
    [SerializeField] private Vector2                    ruleDuration = new Vector2(30.0f, 45.0f);
    [Header("Input")]
    [SerializeField] private PlayerInput                playerInput;
    [SerializeField, InputPlayer(nameof(playerInput)), InputButton] 
    private UC.InputControl                             anyKey;
    [SerializeField, InputPlayer(nameof(playerInput)), InputButton] 
    private UC.InputControl                             cheatNewOrder;
    [SerializeField, InputPlayer(nameof(playerInput)), InputButton] 
    private UC.InputControl                             cheatAddSouls;
    [SerializeField, InputPlayer(nameof(playerInput)), InputButton] 
    private UC.InputControl                             cheatRestart;
    [Header("UI")]
    [SerializeField] private CanvasGroup                gameOverGroup;
    [Header("Spawns")]
    [SerializeField] private BoxCollider                spawnArea;
    [SerializeField] private BoxCollider                waitingArea;
    [SerializeField] private Ghost                      ghostPrefab;
    [SerializeField] private Vector2                    spawnInterval = new Vector2(5.0f, 10.0f);
    [SerializeField] private int                        initialSpawnCount = 10;
    [SerializeField] private bool                       spawnOnFail;
    [SerializeField] private bool                       spawnOnSuccess;

    static LevelManager Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = FindFirstObjectByType<LevelManager>();
            }
            return _instance;
        }
    }
    private static LevelManager _instance;

    private float           currentSouls;
    private float           orderCooldown;
    private List<Messenger> activeMessengers;
    private int             ruleCount = 0;
    private bool            gameOver = false;
    private float           spawnTimer;

    void Start()
    {
        if (Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        foreach (var portal in portals)
        {
            portal.portal.enabled = portal.enabled;
        }

        currentSouls = maxSouls;
        orderCooldown = timeOfFirstOrder;

        cheatNewOrder.playerInput = playerInput;
        cheatAddSouls.playerInput = playerInput;
        anyKey.playerInput = playerInput;
        cheatRestart.playerInput = playerInput;

        spawnTimer = spawnInterval.Random();

        for (int i = 0; i < initialSpawnCount; i++)
        {
            SpawnGhost();
        }
    }

    private void Update()
    {
        if (gameOver)
        {
            if (anyKey.IsDown())
            {
                FullscreenFader.FadeOut(0.5f, Color.black, () =>
                {
                    SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
                });
            }
            return;
        }

        spawnTimer -= Time.deltaTime;
        if (spawnTimer <= 0.0f)
        {
            SpawnGhost();

            spawnTimer = spawnInterval.Random();
        }

        activeMessengers?.RemoveAll((m) => m == null);

        orderCooldown -= Time.deltaTime;
        if (orderCooldown <= 0.0f)
        {
            SpawnOrder();
        }

#if CHEATMODE_ENABLED
        if (cheatNewOrder.IsDown())
        {
            SpawnOrder();
        }
        if (cheatAddSouls.IsDown())
        {
            currentSouls = maxSouls;
        }
        if (cheatRestart.IsDown())
        {
            FullscreenFader.FadeOut(0.5f, Color.black, () =>
            {
                SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
            });
        }
#endif

        if (currentSouls <= 0.0f)
        {
            gameOver = true;
            gameOverGroup.FadeIn(0.25f);
        }
    }

    void SpawnOrder()
    {
        ruleCount++;
        if ((ruleCount % 3) == 0)
        {
            List<Portal> disabledPortals = new();
            for (int i = 0; i < portals.Count; i++)
            {
                if (!portals[i].portal.enabled) disabledPortals.Add(portals[i].portal);
            }
            if (disabledPortals.Count > 0)
            {
                var portal = disabledPortals.Random();
                portal.enabled = true;
            }
        }

        var newOrder = Order.GetRandomOrder();
        var messengerPosition = GetMessengerSlot();
        if (messengerPosition == null)
        {
            // No space for more messengers, kill the existing ones!
            KillMessengers();
            StartCoroutine(SpawnOrderCR());
            return;
        }
        var newMessenger = Instantiate(messengerPrefab, messengerPosition.position, messengerPosition.rotation);
        newMessenger.SetMessage(newOrder);
        if (activeMessengers == null) activeMessengers = new();
        activeMessengers.Add(newMessenger);

        orderCooldown = timeBetweenOrders.Random();
    }

    IEnumerator SpawnOrderCR()
    {
        yield return new WaitForSeconds(1.0f);

        SpawnOrder();
    }

    void SpawnGhost()
    {
        var position = spawnArea.bounds.Random();

        var newGhost = Instantiate(ghostPrefab, position, Quaternion.identity);       

        position = waitingArea.bounds.Random();
        newGhost.Goto(position);
    }

    private Transform GetMessengerSlot()
    {
        foreach (var slot in messengerSlots)
        {
            if (!IsSlotInUse(slot))
            {
                return slot;
            }
        }

        return null;
    }

    private bool IsSlotInUse(Transform slot)
    {
        if (activeMessengers == null) return false;

        foreach (var msg in activeMessengers)
        {
            if (Vector3.Distance(msg.transform.position, slot.transform.position) < 0.1f)
            {
                return true;
            }
        }

        return false;
    }

    void KillMessengers()
    {
        foreach (var msg in activeMessengers)
        {
            msg.CommitOrder();
            msg.Kill();
        }

        activeMessengers = new();
    }

    TraitSet _GetCurrentTraitSet()
    {
        return allTraits;
    }

    GhostQueue _GetMainQueue() => mainQueue;
    float _soulPercentage => currentSouls / maxSouls;

    void _SoulLost()
    {
        currentSouls -= lossPerSoul;
        if (currentSouls < 0) currentSouls = 0;

        if (spawnOnFail)
        {
            SpawnGhost();
        }
    }
    void _SoulGain()
    {
        if (currentSouls <= 0) return;

        currentSouls += gainPerSoul;
        if (currentSouls > maxSouls) currentSouls = maxSouls;

        if (spawnOnSuccess)
        {
            SpawnGhost();
        }
    }
    TraitGroup _GetTraitGroup(Order.Type type)
    {
        switch (type)
        {
            case Order.Type.Color: return colorGroup;
            case Order.Type.Emotion: return emotionGroup;
            case Order.Type.Accessory: return accessoryGroup;
        }

        return null;
    }

    Portal _GetRandomGate()
    {
        int count = 0;
        foreach (var p in portals)
        {
            if (p.portal.enabled) count++;
        }

        int n = UnityEngine.Random.Range(0, count);

        foreach (var p in portals)
        {
            if (p.portal.enabled)
            {
                if (n == 0) return p.portal;
                else n--;
            }
        }

        return null;
    }

    public static TraitSet GetCurrentTraitSet() => Instance?._GetCurrentTraitSet();
    public static GhostQueue GetMainQueue() => Instance?._GetMainQueue();

    public static float ghostMoveSpeed => Instance?.maxGhostSpeed ?? 0.0f;
    public static float signpostRotationSpeed => Instance?._signpostRotationSpeed ?? 0.0f;

    public static float soulPercentage => Instance?._soulPercentage ?? 0.0f ;

    public static void SoulLost()
    {
        Instance?._SoulLost();
    }
    public static void SoulGain()
    {
        Instance?._SoulGain();
    }

    public static TraitGroup GetTraitGroup(Order.Type type) => Instance?._GetTraitGroup(type) ?? null;

    public static Portal    GetRandomGate() => Instance?._GetRandomGate() ?? null;
    public static Vector2   GetRuleDuration() => Instance?.ruleDuration ?? Vector2.one;
}
