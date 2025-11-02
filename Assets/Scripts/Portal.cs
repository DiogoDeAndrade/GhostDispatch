using System;
using System.Collections;
using System.Collections.Generic;
using UC;
using UnityEngine;

public class Portal : Interactable, IQueueHandler
{
    [SerializeField] private string         gateName;
    [SerializeField] private Light          portalLight;
    [SerializeField] private MeshRenderer   portalObject;
    [SerializeField] private Transform      tooltipPosition;

    [SerializeField, ColorUsage(true, true)] 
    private Color          colorDisabled;
    [SerializeField, ColorUsage(true, true)] 
    private Color          colorEnabled;
    [SerializeField, ColorUsage(true, true)] 
    private Color          colorFail;
    [SerializeField, ColorUsage(true, true)] 
    private Color          colorSuccess;

    Material        portalMaterial;
    float           colorEffectTimer;
    Color           sourceColor;
    ArchwayTooltip  activeTooltip;
    List<Order>     rules = new();

    public string displayName => gateName;

    private void Awake()
    {
        portalMaterial = new Material(portalObject.material);
        portalObject.material = portalMaterial;
    }

    void OnEnable()
    {
        portalLight.color = colorEnabled;
        portalMaterial.SetColor("_EmissionColor", colorEnabled);
        portalObject.enabled = true;
        colorEffectTimer = 0.0f;
    }

    void OnDisable()
    {
        portalLight.color = colorDisabled;
        portalMaterial.SetColor("_EmissionColor", colorDisabled);
        portalObject.enabled = false;
        colorEffectTimer = 0.0f;
    }

    void Update()
    {
        if (colorEffectTimer > 0.0f)
        {
            colorEffectTimer -= Time.deltaTime;
            if (colorEffectTimer < 0.0f)
            {
                portalMaterial.SetColor("_EmissionColor", colorEnabled);
                portalLight.color = colorEnabled;
            }
            else if (colorEffectTimer < 1.0f)
            {
                var color = Color.Lerp(colorEnabled, sourceColor, colorEffectTimer);
                portalMaterial.SetColor("_EmissionColor", color);
                portalLight.color = color;
            }
        }

        bool updateUI = false;
        foreach (var rule in rules)
        {
            rule.duration -= Time.deltaTime;
            if (rule.duration <= 0.0f) updateUI = true;
        }

        if (updateUI)
        {
            rules.RemoveAll((r) => r.duration <= 0.0f);

            activeTooltip?.SetRule(rules);
        }
    }

    public bool IsGoal() => true;

    public void ReachGoal(Ghost ghost)
    {
        if (enabled)
        {
            if (IsCorrect(ghost))
            {
                sourceColor = colorSuccess;
                LevelManager.SoulGain();
            }
            else
            {
                sourceColor = colorFail;
                LevelManager.SoulLost();
            }

            portalMaterial.SetColor("_EmissionColor", sourceColor);
            portalLight.color = sourceColor;

            colorEffectTimer = 1.15f;

            StartCoroutine(KillGhostCR(ghost));
        }
        else
        {
            ghost.Kill();
            LevelManager.SoulLost();
        }
    }

    bool IsCorrect(Ghost ghost)
    {
        if (rules.Count == 0) return true;

        foreach (var r in rules)
        {
            if (r.IsCorrect(ghost)) return true;
        }

        return false;
    }

    IEnumerator KillGhostCR(Ghost ghost)
    {
        yield return null;

        Destroy(ghost.gameObject);
    }

    public GhostQueue GetNextQueue()
    {
        return null;
    }

    public override void OnFocus(bool focusEnable)
    {
        if (focusEnable)
        {
            if (activeTooltip == null)
                activeTooltip = TooltipManager.CreateTooltip() as ArchwayTooltip;
            activeTooltip.SetText(gateName);
            activeTooltip.SetPosition(tooltipPosition.position);
            activeTooltip.SetRule(rules);
        }
        else
        {
            if (activeTooltip)
            {
                activeTooltip.SetText("");
            }
        }
    }

    public override void Interact()
    {
    }

    public void AddRule(Order currentOrder)
    {
        rules.Add(currentOrder);
        activeTooltip?.SetRule(rules);
    }
}
