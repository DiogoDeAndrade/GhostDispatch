using UnityEngine;

public class LevelManager : MonoBehaviour
{
    [SerializeField] private TraitSet   allTraits;
    [SerializeField] private GhostQueue mainQueue;
    [SerializeField] private float      maxGhostSpeed = 1.0f;
    [SerializeField] private float      _signpostRotationSpeed = 360.0f;

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

    void Start()
    {
        if (Instance != this)
        {
            Destroy(gameObject);
            return;
        }
    }

    void Update()
    {
        
    }

    TraitSet _GetCurrentTraitSet()
    {
        return allTraits;
    }

    GhostQueue _GetMainQueue() => mainQueue;

    public static TraitSet GetCurrentTraitSet() => Instance?._GetCurrentTraitSet();
    public static GhostQueue GetMainQueue() => Instance?._GetMainQueue();

    public static float ghostMoveSpeed => Instance?.maxGhostSpeed ?? 0.0f;
    public static float signpostRotationSpeed => Instance?._signpostRotationSpeed ?? 0.0f;
}
