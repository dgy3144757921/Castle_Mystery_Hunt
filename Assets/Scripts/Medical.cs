using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Medical : MonoBehaviour
{
    //玩家对象
    public GameObject player;
    //PlayerAttributes
    public PlayerAttributes playerAttributes;
    //恢复的血量
    public float med = 50f;
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
            playerAttributes.AddHp(med);
            isPicked = true;
            gameObject.SetActive(false);//失活血包
        }
    }
}
