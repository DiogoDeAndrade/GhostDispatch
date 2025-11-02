using UnityEngine;

[System.Serializable]
public class Order
{
    public enum Type { Color, Emotion, Accessory };

    public Type     type;
    public Trait    trait;
    public Portal   destination;

    public static Order GetRandomOrder()
    {
        var newOrder = new Order();

        newOrder.type = (Type)Random.Range(0, 3);

        var traitGroup = LevelManager.GetTraitGroup(newOrder.type);
        newOrder.trait = traitGroup.Random();

        var destination = LevelManager.GetRandomGate();

        return newOrder;
    }
}
