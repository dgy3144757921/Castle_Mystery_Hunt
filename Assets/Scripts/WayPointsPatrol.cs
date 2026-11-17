using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class WayPointsPatrol : MonoBehaviour
{
    //导航组件
    private NavMeshAgent navMeshAgent;
    //导航点数组
    public Transform[] wayPoints;
    //当前巡逻的目标导航点索引
    int m_currentPointIndex = 0;
    // Start is called before the first frame update
    void Start()
    {
        navMeshAgent = GetComponent<NavMeshAgent>();
        m_currentPointIndex = 0;
        navMeshAgent.SetDestination(wayPoints[m_currentPointIndex].position);//设置第一个目标点
    }

    // Update is called once per frame
    void Update()
    {
        //到达目标点，前往下一个导航点
        if(navMeshAgent.remainingDistance < navMeshAgent.stoppingDistance)//到达判断：距离目标点的距离小于停止距离
        {
            m_currentPointIndex = (m_currentPointIndex +1) % wayPoints.Length; //索引+1，去下一个目标点；求模实现循环切换巡逻点索引
            navMeshAgent.SetDestination(wayPoints[m_currentPointIndex].position);//设置目标点坐标
        }
    }
}
