using UnityEngine;

[CreateAssetMenu(fileName = "TraitGroup", menuName = "Ghost Dispatch/Trait Group")]
public class TraitGroup : ScriptableObject
{
    public Trait[] traits;

    public Trait Random()
    {
        int n = UnityEngine.Random.Range(0, traits.Length);

        return traits[n];
    }
}
