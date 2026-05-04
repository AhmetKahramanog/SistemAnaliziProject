using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GoldCoin : MonoBehaviour, ICollectable
{
    private int goldAmount;

    private void Start()
    {
        goldAmount = Random.Range(2, 50);
    }
    public void Interact(PlayerMovement player)
    {
        if (player != null)
        {
            player.GetGold(goldAmount);
            Destroy(gameObject);
        }
    }
}
