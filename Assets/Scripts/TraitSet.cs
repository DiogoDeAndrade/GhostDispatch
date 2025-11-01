using System.Collections.Generic;
using UC;
using UnityEditor;
using UnityEngine;

[CreateAssetMenu(fileName = "TraitSet", menuName = "Ghost Dispatch/Trait Set")]
public class TraitSet : ScriptableObject
{
    [System.Serializable]
    public class TraitsGroup : ProbList<Trait>
    {
    }

    public List<TraitsGroup> traits;
}

