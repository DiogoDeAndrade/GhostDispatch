using NaughtyAttributes;
using UnityEngine;
using UC;
using System.Collections;
using System.Collections.Generic;
using System;

public class Ghost : Interactable
{
    [System.Serializable]
    public struct EmotionConverter
    {
        public Trait sourceTrait;
        public Trait destTrait;
    }

    [SerializeField] private LayerMask          groundLayer;
    [SerializeField] private List<Trait>        activeTraits;
    [Header("Emotions")]
    [SerializeField] private EmotionConverter[] emotionConverter;
    [SerializeField] private float              restlessInterval = 10.0f;
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
    float           restlessTimer;
    Vector3         prevPos;
    HoverWithNoise  hoverWithNoise;
    Vector3?        moveTarget;

    void Start()
    {
        prevPos = transform.position;

        SetupGhost();

        bodySpriteEffect = bodyRenderer.GetComponent<SpriteEffect>();
        
        pentagram = GetComponentInChildren<Pentagram>();
        hoverWithNoise = GetComponent<HoverWithNoise>();
    }

    private void Update()
    {
        if (isDead) return;

        if ((!onQueue) && (moveTarget.HasValue))
        {
            Vector2 newPos = Vector2.MoveTowards(transform.position.xz(), moveTarget.Value.xz(), Time.deltaTime * LevelManager.ghostMoveSpeed);
            transform.position = new Vector3(newPos.x, transform.position.y, newPos.y);
        }

        if (Physics.Raycast(transform.position + Vector3.up * 1.0f, Vector3.down, out var hit, float.MaxValue, groundLayer))
        {
            transform.position = hit.point;
        }

        if (Vector3.Distance(transform.position.xz(), prevPos.xz()) < 1e-3)
        {
            restlessTimer += Time.deltaTime;
            if (restlessTimer > restlessInterval)
            {
                restlessTimer = 0.0f;

                foreach (var c in emotionConverter)
                {
                    if (HasTrait(c.sourceTrait))
                    {
                        if (c.destTrait == null)
                        {
                            // kill this one
                            Kill();
                            LevelManager.SoulLost();
                        }
                        else
                        {
                            ConvertTrait(c.sourceTrait, c.destTrait);
                        }
                        break;
                    }
                }
            }

            float tRestless = restlessTimer / restlessInterval;
            if (tRestless > 0.5f)
            {
                tRestless = (tRestless - 0.5f) * 2.0f;
                hoverWithNoise.SetMaxNoise(Vector3.one * tRestless * 0.1f);
            }
            else
            {
                hoverWithNoise.SetMaxNoise(Vector3.zero);
            }
        }       
        else
        {
            restlessTimer = 0.0f;
            hoverWithNoise.SetMaxNoise(Vector3.zero);
        }

        prevPos = transform.position;
    }

    public bool HasTrait(Trait trait)
    {
        foreach (var t in activeTraits)
        {
            if (t == trait) return true;
        }

        return false;
    }

    void ConvertTrait(Trait trait, Trait destTrait)
    {
        for (int i = 0; i < activeTraits.Count; i++)
        {
            if (activeTraits[i] == trait)
            {
                activeTraits[i] = destTrait;
                break;
            }
        }

        SetupGhost();
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

    public void Goto(Vector3 position)
    {
        if (onQueue) return;

        if (Physics.Raycast(position + Vector3.up * 50.0f, Vector3.down, out var hit, float.MaxValue, groundLayer))
        {
            position.y = hit.point.y;
        }

        moveTarget = position;
    }
}
