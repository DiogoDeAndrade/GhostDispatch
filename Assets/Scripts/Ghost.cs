using NaughtyAttributes;
using UnityEngine;
using UC;
using System.Collections;
using System.Collections.Generic;

public class Ghost : Interactable
{
    [SerializeField] private LayerMask          groundLayer;
    [SerializeField] private List<Trait>        activeTraits;
    [Header("Visuals")]
    [SerializeField] private SpriteRenderer3D   bodyRenderer;
    [SerializeField] private SpriteRenderer3D   hatRenderer;
    [SerializeField] private SpriteRenderer3D   eyesRenderer;
    [SerializeField] private SpriteRenderer3D   mouthRenderer;
    [SerializeField] private Pentagram          pentagram;
    [SerializeField] private ParticleSystem     hellfirePS;

    public SpriteRenderer3D mainBody => bodyRenderer;
    public SpriteRenderer3D eyes => eyesRenderer;
    public SpriteRenderer3D mouth => mouthRenderer;
    public SpriteRenderer3D accessoryHat => hatRenderer;

    SpriteEffect    bodySpriteEffect;
    bool            onQueue = false;
    bool            isDead = false;

    void Start()
    {
        SetupGhost();

        bodySpriteEffect = bodyRenderer.GetComponent<SpriteEffect>();
        
        pentagram = GetComponentInChildren<Pentagram>();
    }

    private void Update()
    {
        if (Physics.Raycast(transform.position + Vector3.up * 1.0f, Vector3.down, out var hit, float.MaxValue, groundLayer))
        {
            transform.position = hit.point;
        }
    }

    void SetupGhost()
    {
        if ((activeTraits == null) || (activeTraits.Count == 0))
        {
            GenerateTraits(false);
        }

        UpdateTraitVisuals();
    }

    void UpdateTraitVisuals()
    {
        SetDefaults();

        foreach (var trait in activeTraits)
        {
            if (trait == null) continue;
            trait.ApplyTraitVisuals(this);
        }
    }

    void SetDefaults()
    {
        bodyRenderer.color = Color.white;
        hatRenderer.enabled = false;
    }

    [Button("Generate Traits")]
    void GenerateTraits(bool updateVisuals = true)
    {
        var allTraits = LevelManager.GetCurrentTraitSet();

        activeTraits = new List<Trait>();
        foreach (var traitGroup in allTraits.traits)
        {
            if (traitGroup == null) continue;

            var trait = traitGroup.Get();
            if (trait != null)
                activeTraits.Add(trait);
        }   

        if (updateVisuals)
        {
            SetDefaults();
            UpdateTraitVisuals();
        }
    }

    public override void OnFocus(bool focusEnable)
    {
        bodySpriteEffect?.SetOutline(focusEnable ? (3.0f) : (0.0f), Color.yellow);
    }

    public override bool canInteract => !onQueue && !isDead;

    public override void Interact()
    {
        if (onQueue) return;

        onQueue = true;

        // Find main queue
        var queue = LevelManager.GetMainQueue();
        queue.Add(this);
    }

    [Button("Kill")]
    public void Kill()
    {
        isDead = true;
        StartCoroutine(KillCR());
    }

    IEnumerator KillCR()
    { 
        pentagram.enabled = true;

        yield return new WaitForSeconds(0.25f);

        hellfirePS.Play();

        yield return new WaitForSeconds(0.1f);

        bodyRenderer.gameObject.SetActive(false);

        yield return new WaitForSeconds(1.0f);

        Destroy(gameObject);
    }
}
