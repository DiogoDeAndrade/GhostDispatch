using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using UC;
using System;
using TMPro;

public class Messenger : Interactable
{
    [SerializeField] private SpriteRenderer3D   demonSR; 
    [SerializeField] private ParticleSystem     hellfirePS;
    [SerializeField] private Light              hellfireLight;
    [Header("UI")]
    [SerializeField] private CanvasGroup        messageDisplay;    
    [SerializeField] private TextMeshProUGUI    baseText;
    [SerializeField] private GameObject         orderContainer;
    [SerializeField] private Image              ghostBody;
    [SerializeField] private Image              ghostFace;
    [SerializeField] private Image              ghostAccessory;
    [SerializeField] private TextMeshProUGUI    targetGateText;

    SpriteEffect spriteEffect;
    Order        currentOrder;
    float        targetIntensity;

    void Start()
    {
        messageDisplay.alpha = 0.0f;
        targetIntensity = hellfireLight.intensity;
        spriteEffect = demonSR.GetComponent<SpriteEffect>();
        StartCoroutine(ShowMessageCR());        
    }

    IEnumerator ShowMessageCR()
    {
        hellfireLight.enabled = true;
        hellfireLight.intensity = 0.0f;
        hellfireLight.FadeTo(targetIntensity, 0.1f);

        demonSR.color = Color.white.ChangeAlpha(0.0f);

        hellfirePS.Play();

        yield return new WaitForSeconds(0.2f);

        hellfireLight.FadeTo(0.0f, 0.2f).Done(() => hellfireLight.enabled = false);
        messageDisplay.FadeIn(0.2f);

        demonSR.FadeTo(Color.white, 0.2f);
    }

    internal void SetMessage(Order newOrder)
    {
        orderContainer.SetActive(true);
        baseText.text = "New Order!";
        targetGateText.text = newOrder.destination.displayName;

        var color = newOrder.GetMainColor();
        if (color.a == 0) ghostBody.enabled = false;
        else
        {
            ghostBody.color = color;
            ghostBody.enabled = true;
        }

        var expression = newOrder.GetExpression();
        if (expression == null) ghostFace.enabled = false;
        else
        {
            ghostFace.enabled = true;
            ghostFace.sprite = expression.uiFaceSprite;
        }

        var accessory = newOrder.GetAccessory();
        if (accessory == null) ghostAccessory.enabled = false;
        else
        {
            ghostAccessory.enabled = true;
            ghostAccessory.sprite = accessory.uiAccessorySprite;
        }

        currentOrder = newOrder;
    }

    internal void CommitOrder()
    {
        StartCoroutine(CommitOrderCR());
    }

    IEnumerator CommitOrderCR()
    {
        currentOrder.destination.AddRule(currentOrder);
        currentOrder = null;

        messageDisplay.FadeOut(0.3f);

        hellfireLight.enabled = true;
        hellfireLight.intensity = 0.0f;
        hellfireLight.FadeTo(targetIntensity, 0.1f);

        demonSR.color = Color.white;

        hellfirePS.Play();

        yield return new WaitForSeconds(0.2f);

        hellfireLight.FadeTo(0.0f, 0.2f).Done(() => hellfireLight.enabled = false);

        demonSR.FadeTo(new Color(1, 1, 1, 0), 0.4f);

        yield return new WaitForSeconds(1.0f);

        Destroy(gameObject);
    }

    internal void Kill()
    {
        Destroy(gameObject);
    }

    public override void OnFocus(bool focusEnable)
    {
        spriteEffect?.SetOutline((focusEnable) && (currentOrder != null) ? (3.0f) : (0.0f), Color.yellow);
    }

    public override void Interact()
    {
        if (currentOrder == null) return;
        CommitOrder();
    }
}
