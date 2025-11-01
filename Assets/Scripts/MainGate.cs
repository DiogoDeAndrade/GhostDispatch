using System;
using System.Collections;
using UnityEngine;

public class MainGate : Interactable, IQueueHandler
{
    [SerializeField] private GhostQueue nextQueue;

    Animator    animator;
    Coroutine   toggleCR;
    bool        open = false;

    public override bool canInteract => (toggleCR == null);

    private void Start()
    {
        animator = GetComponent<Animator>();
        open = false;
    }

    public override void Interact()
    {
        if (toggleCR != null) return;

        toggleCR = StartCoroutine(ToggleCR());
    }

    private IEnumerator ToggleCR()
    {
        animator.SetTrigger("Toggle");

        yield return new WaitForSeconds(1.0f);

        open = !open;
        toggleCR = null;
    }

    public GhostQueue GetNextQueue()
    {
        if (open) return nextQueue;

        return null;
    }
}
