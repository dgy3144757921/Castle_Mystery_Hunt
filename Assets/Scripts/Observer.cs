using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Observer : MonoBehaviour
{
    //游戏角色
    public Transform Player;
    //PlayerAttributes
    public PlayerAttributes playerAttributes;
    //游戏角色是否进入扫描范围
    bool isInRange = false;
    //每次造成的伤害值
    public float damage = 25f;
    //攻击冷却时间
    public float damageInterval = 1f;
    //记录上次扣血时间
    private float lastDamageTime;

    private void OnTriggerEnter(Collider other)//进入扫描范围
    {
        if(other.gameObject == Player.gameObject)
        {
            isInRange = true;
        }
    }
    private void OnTriggerExit(Collider other)//离开扫描范围
    {
        if (other.gameObject == Player.gameObject)
        {
            isInRange = false;
        }
    }
    // Update is called once per frame
    void Update()
    {
        if(isInRange)
        {
            //射线检测

            Vector3 dir = Player.position - transform.position;//定义从当前触发器位置到游戏角色位置的方向
            Ray ray = new Ray(transform.position, dir);//参数1：射线起点，参数二：射线方向
            RaycastHit raycastHit;//存射线碰撞到的物体信息

            if(Physics.Raycast(ray, out raycastHit) )//参数1：射线，参数2：out碰撞信息
            {
                if(raycastHit.collider.transform == Player)
                {
                    if (Time.time - lastDamageTime >= damageInterval)
                    {
                        playerAttributes.Hurt(damage);
                        lastDamageTime = Time.time;
                    }
                }
            }
        }
    }
}
