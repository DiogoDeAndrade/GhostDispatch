using JetBrains.Annotations;
using System;
using System.Collections.Generic;
using UC;
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
    [SerializeField] private TraitGroup                 emotionGroup;
    [SerializeField] private TraitGroup                 colorGroup;
    [SerializeField] private TraitGroup                 accessoryGroup;
    [SerializeField] private GhostQueue                 mainQueue;
    [SerializeField] private float                      maxGhostSpeed = 1.0f;
    [SerializeField] private float                      _signpostRotationSpeed = 360.0f;
    [SerializeField] private List<PortalInitialState>   portals;
    [SerializeField] private float                      maxSouls = 100.0f;
    [SerializeField] private float                      lossPerSoul = 20.0f;
    [SerializeField] private float                      gainPerSoul = 20.0f;
    [SerializeField] private float                      timeOfFirstOrder = 1.0f;
    [SerializeField] private Vector2                    timeBetweenOrders = new Vector2(30.0f, 45.0f);

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
    private float orderCooldown;

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
    }

    private void Update()
    {
        orderCooldown -= Time.deltaTime;
        if (orderCooldown <= 0.0f)
        {


            orderCooldown = timeBetweenOrders.Random();
        }
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
    void _SoulGain()
    {
        currentSouls += gainPerSoul;
        if (currentSouls > maxSouls) currentSouls = maxSouls;
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

    public static Portal GetRandomGate() => Instance?._GetRandomGate() ?? null;
}
