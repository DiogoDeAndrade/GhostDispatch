using UnityEngine;
using UC;

public abstract class Interactable : MonoBehaviour
{
    SimpleOutline3d outlineBackface;

    public virtual bool canInteract => true;

    void Awake()
    {
        outlineBackface = GetComponent<SimpleOutline3d>();
        if (outlineBackface)
        {
            outlineBackface.enabled = false;
        }
    }
    
    public virtual void OnFocus(bool focusEnable)
    {
        if (outlineBackface)
        {
            outlineBackface.enabled = focusEnable;
        }
    }

    public abstract void Interact();
}
