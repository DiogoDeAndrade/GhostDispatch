using UnityEngine;
using UC;
using System.Collections.Generic;

public class ArchwayTooltip : Tooltip
{
    [SerializeField] private ArchwayTooltip_GhostDisplay[] rules;

    public void SetRule(List<Order> orders)
    {
        for (int i = 0; i < Mathf.Min(rules.Length, orders.Count); i++)
        {
            rules[i].gameObject.SetActive(true);
            rules[i].SetOrder(orders[i]);
        }
        for (int i = Mathf.Min(rules.Length, orders.Count); i < rules.Length; i++)
        {
            rules[i].gameObject.SetActive(false);
        }
    }
}
