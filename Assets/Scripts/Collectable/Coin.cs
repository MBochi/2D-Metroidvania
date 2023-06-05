using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Coin : Collectable
{
    private int coinValue = 5;

    protected override void PlayerCollected()
    {
        base.PlayerCollected();
        coinValue += 10; // Dummy
    }
}