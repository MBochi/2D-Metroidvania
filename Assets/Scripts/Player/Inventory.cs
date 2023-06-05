using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Inventory : MonoBehaviour
{

    [SerializeField] private int money;
    // Start is called before the first frame update
    void Start()
    {
        money = 0;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public int getMoneyValue()
    {
        return money;
    }

    public void AddMoney(int amount)
    {
        money += amount;
    }
}
