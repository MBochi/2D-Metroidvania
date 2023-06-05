using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Heart : Collectable
{
    private int healAmount = 10;

    protected override void PlayerCollected()
    {
        base.PlayerCollected();
        playerObj.GetComponent<PlayerCombat>().Heal(healAmount);
    }
}
