using System;
using UC;
using UnityEngine;
using UnityEngine.InputSystem;

public class InteractionSystem : MonoBehaviour
{
    [SerializeField] 
    private Camera          mainCamera;
    [SerializeField] 
    private LayerMask       interactionLayers;
    [SerializeField]
    private PlayerInput     playerInput;
    [SerializeField, InputPlayer(nameof(playerInput))]
    private UC.InputControl pointerPosition;
    [SerializeField, InputPlayer(nameof(playerInput)), InputButton]
    private UC.InputControl leftClick;


    Interactable currentFocus = null;

    void Start()
    {
        pointerPosition.playerInput = playerInput;
        leftClick.playerInput = playerInput;
    }

    void Update()
    {
        var ray = mainCamera.ScreenPointToRay(pointerPosition.GetAxis2());

        bool foundInteractable = false;
        var intersections = Physics.RaycastAll(ray, float.MaxValue, interactionLayers, QueryTriggerInteraction.Collide);
        if (intersections.Length > 0)
        {
            Array.Sort(intersections, (i1, i2) => i1.distance.CompareTo(i2.distance));

            for (int i = 0; i < intersections.Length; i++)
            {
                var interactable = intersections[i].collider.GetComponent<Interactable>();  
                if (interactable)
                {
                    if (interactable.canInteract)
                    {
                        SetFocus(interactable);
                        foundInteractable = true;
                        break;
                    }
                }
            }
        }
        if (!foundInteractable)
        {
            SetFocus(null);
        }
        else
        {
            if (leftClick.IsDown())
            {
                currentFocus.Interact();
            }
        }
    }

    private void SetFocus(Interactable interactable)
    {
        if (currentFocus)
        {
            currentFocus.OnFocus(false);
        }
        currentFocus = interactable;
        currentFocus?.OnFocus(true);
    }
}
