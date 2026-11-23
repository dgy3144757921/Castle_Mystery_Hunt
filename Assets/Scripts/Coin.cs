using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Coin : MonoBehaviour
{
    //玩家对象
    public GameObject player;
    //PlayerAttributes
    public PlayerAttributes playerAttributes;
    //金币价值
    public int coinValue = 100;
    //标记是否已经拾取
    private bool isPicked = false; 

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnTriggerEnter(Collider other)
    {
        if (isPicked)
        {
            return;
        }
        if (other.CompareTag("Player"))
        {
            playerAttributes.AddCoin(coinValue);
            isPicked = true;
            gameObject.SetActive(false);//失活金币
        }
    }
}
