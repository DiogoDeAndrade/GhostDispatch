using UnityEngine;
using System.Collections.Generic;
using UC;

[System.Serializable]
public class Order
{
    public enum Type { Color, Emotion, Accessory };

    public struct OrderItem
    { 
        public Type     type;
        public Trait    trait;
    }
    public List<OrderItem>  orderItems;
    public float            duration;
    public Portal           destination;

    public static Order GetRandomOrder()
    {
        var newOrder = new Order();
        newOrder.orderItems = new();

        for (int i = 0; i < 3; i++)
        {            
            var type = (Type)Random.Range(0, 3);

            bool found = false;
            for (int j = 0; j < i; j++)
            {
                if (newOrder.orderItems[j].type == type)
                {
                    found = true; 
                    break;
                }
            }
            if (found)
            {
                break;
            }

            var traitGroup = LevelManager.GetTraitGroup(type);
            var trait = traitGroup.Random();

            OrderItem item = new OrderItem
            {
                type = type,
                trait = trait,
            };

            newOrder.orderItems.Add(item);

            if (Random.Range(0.0f, 1.0f) > 0.1f)
            {
                break;
            }
        }

        newOrder.destination = LevelManager.GetRandomGate();
        newOrder.duration = LevelManager.GetRuleDuration().Random();

        return newOrder;
    }

    public Color GetMainColor()
    {
        foreach (var item in orderItems)
        {
            if (item.type == Type.Color)
            {
                return item.trait.GetColor(Trait.TargetDisplay.MainColor);
            }
        }

        return new Color(1, 1, 1, 0);
    }

    public Trait GetExpression()
    {
        foreach (var item in orderItems)
        {
            if (item.type == Type.Emotion)
            {
                return item.trait;
            }
        }

        return null;
    }

    public Trait GetAccessory()
    {
        foreach (var item in orderItems)
        {
            if (item.type == Type.Accessory)
            {
                return item.trait;
            }
        }

        return null;
    }

    public bool IsCorrect(Ghost ghost)
    {
        foreach (var item in orderItems)
        {
            if (!ghost.HasTrait(item.trait)) return false;
        }

        return true;
    }
}
