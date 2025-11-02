using System.Collections;
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

    Material    portalMaterial;
    float       colorEffectTimer;
    Color       sourceColor;
    Tooltip     activeTooltip;

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
        return true;
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
                activeTooltip = TooltipManager.CreateTooltip();
            activeTooltip.SetText(gateName);
            activeTooltip.SetPosition(tooltipPosition.position);
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
}
