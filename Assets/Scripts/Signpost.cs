using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Signpost : Interactable, IQueueHandler
{
    [System.Serializable]
    struct Direction
    {
        public float        yRotation;
        public GhostQueue   queue;
    }

    [SerializeField] private List<Direction>    directions;
    [SerializeField] private int                currentDirection;

    void Start()
    {
        transform.rotation = Quaternion.Euler(0, directions[currentDirection].yRotation, 0);
    }

    void Update()
    {
        transform.rotation = Quaternion.RotateTowards(transform.rotation, Quaternion.Euler(0, directions[currentDirection].yRotation, 0), Time.deltaTime * LevelManager.signpostRotationSpeed);
    }

    public GhostQueue GetNextQueue()
    {
        return directions[currentDirection].queue;
    }

    public override void Interact()
    {
        currentDirection = (currentDirection + 1) % directions.Count;
    }
    public override bool canInteract => Quaternion.Angle(transform.rotation, Quaternion.Euler(0, directions[currentDirection].yRotation, 0)) < 1.0f;

}
