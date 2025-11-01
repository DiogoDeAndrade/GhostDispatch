using UnityEngine;

public class LevelManager : MonoBehaviour
{
    [SerializeField] private TraitSet allTraits;

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

    public static TraitSet GetCurrentTraitSet() => Instance?._GetCurrentTraitSet();
}
