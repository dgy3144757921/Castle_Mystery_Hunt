using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAttributes : MonoBehaviour
{
    //定义游戏角色的血量
    public float hp = 100f;
    //定义游戏角色最大血量
    public float maxHp = 100f;
    //定义游戏角色的金币数
    public int coin = 0;
    //GameEnding
    public GameEnding gameEnding;

    // Start is called before the first frame update
    void Start()
    {
        hp = Mathf.Clamp(hp, 0, maxHp);
    }
    //受伤函数
    public void Hurt(float damage)
    {
        hp = MathF.Max(0, hp - damage);
        gameEnding.UpdateHp();
        if (hp <= 0)
        {
            gameEnding.Caught();
        }

    }

    //回血函数
    public void AddHp(float med)
    {
        hp = MathF.Min(hp + med, maxHp);
        gameEnding.UpdateHp();
    }

    //拾取金币函数
    public void AddCoin(int coinValue)
    {
        coin += coinValue;
        gameEnding.UpdateCoin();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
