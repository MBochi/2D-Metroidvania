using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class Inventory : MonoBehaviour
{
    public TMP_Text coinText;
    [SerializeField] private int money;
    // Start is called before the first frame update
    void Start()
    {
        money = 0;
        coinText.text = getMoneyValue().ToString();
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
        coinText.text = getMoneyValue().ToString();
    }
}
