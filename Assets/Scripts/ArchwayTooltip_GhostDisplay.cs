using UnityEngine;
using UnityEngine.UI;

public class ArchwayTooltip_GhostDisplay : MonoBehaviour
{
    [SerializeField] private Image ghostBody;
    [SerializeField] private Image ghostFace;
    [SerializeField] private Image ghostAccessory;

    public void SetOrder(Order order)
    {
        var color = order.GetMainColor();
        if (color.a == 0) ghostBody.enabled = false;
        else
        {
            ghostBody.color = color;
            ghostBody.enabled = true;
        }

        var expression = order.GetExpression();
        if (expression == null) ghostFace.enabled = false;
        else
        {
            ghostFace.enabled = true;
            ghostFace.sprite = expression.uiFaceSprite;
        }

        var accessory = order.GetAccessory();
        if (accessory == null) ghostAccessory.enabled = false;
        else
        {
            ghostAccessory.enabled = true;
            ghostAccessory.sprite = accessory.uiAccessorySprite;
        }
    }
}
