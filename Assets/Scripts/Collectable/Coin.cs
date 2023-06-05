using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Coin : Collectable
{
    private int coinValue = 1;

    protected override void PlayerCollected()
    {
        base.PlayerCollected();
        playerObj.GetComponent<Inventory>().AddMoney(coinValue);
    }
}