using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraFollow1 : MonoBehaviour
{
    //定义游戏角色
    private Transform player;
    //定义相机与角色之间的固定距离
    Vector3 offset;
    // Start is called before the first frame update
    void Start()
    {
        //查找游戏角色
        player = GameObject.Find("JohnLemon").transform;
        offset = transform.position - player.position;
    }

    // Update is called once per frame
    void Update()
    {
        transform.position = offset + player.position;
    }
}
