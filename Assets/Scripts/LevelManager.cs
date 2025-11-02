using System.Collections.Generic;
using UnityEngine;

public class LevelManager : MonoBehaviour
{
    [System.Serializable]
    struct PortalInitialState
    {
        public bool     enabled;
        public Portal   portal;
    }

    [SerializeField] private TraitSet                   allTraits;
    [SerializeField] private GhostQueue                 mainQueue;
    [SerializeField] private float                      maxGhostSpeed = 1.0f;
    [SerializeField] private float                      _signpostRotationSpeed = 360.0f;
    [SerializeField] private List<PortalInitialState>   portals;
    [SerializeField] private float                      maxSouls;
    [SerializeField] private float                      lossPerSoul;

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

    private float currentSouls;

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
}
