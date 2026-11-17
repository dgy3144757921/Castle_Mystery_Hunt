using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    //定义游戏角色旋转的速度
    public float turnSpeed = 20f;

    //定义游戏角色身上的组件
    Animator m_Animator;
    Rigidbody m_Rigidbody;

    //定义游戏角色移动的矢量
    Vector3 m_Movement;
    //定义游戏角色旋转的角度
    Quaternion m_Quaternion  = Quaternion.identity;
    // Start is called before the first frame update
    void Start()
    {
        //获取游戏角色身上的动画控制机、刚体组件
        m_Animator = GetComponent<Animator>();
        m_Rigidbody = GetComponent<Rigidbody>();
    }

    private void FixedUpdate()
    {
        //获取水平、垂直方向是否有键值输入
        float horizontal = Input.GetAxis("Horizontal");
        float vertical = Input.GetAxis("Vertical");

        //设置游戏角色移动的方向
        m_Movement.Set(horizontal,0f,vertical);
        m_Movement.Normalize();//做归一化，避免斜向移动速度变大

        //定义游戏角色是否移动的 bool 值
        bool hasHorizontalInput = !Mathf.Approximately(horizontal,0f);//Approximately比较输入的两个浮点数是否近似相等
        bool hasVerticalInput = !Mathf.Approximately(vertical,0f);

        bool isWalking = hasHorizontalInput || hasVerticalInput;
        //将角色是否移动的bool值传入动画控制机里的IsWalking值
        m_Animator.SetBool("IsWalking",isWalking);

        //旋转的过渡，通过三维向量转四元数的方式获取游戏角色的目标朝向
        //作用:将当前方向向量旋转到目标方向向量
        //参数1：当前朝向 参数二：目标朝向 参数三：每帧旋转的最大角度 参数4：每帧向量改变的最大长度，只进行旋转传入0
        Vector3 desirForward = Vector3.RotateTowards(transform.forward, m_Movement, Time.fixedDeltaTime * turnSpeed, 0f);

        //作用:让旋转角度对齐传入的方向向量
        m_Quaternion = Quaternion.LookRotation(desirForward);
    }

    private void OnAnimatorMove()//管理Animator动画产生的的位移和旋转的回调函数
    {
        //m_Animator.deltaPosition.magnitude 当前帧根运动动画位移的距离
        m_Rigidbody.MovePosition(m_Rigidbody.position +  m_Movement*m_Animator.deltaPosition.magnitude);
        m_Rigidbody.MoveRotation(m_Quaternion);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
