using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
using System;
using TMPro;
using UnityEngine.UI;

public class PlayerController :FSM
{
    public enum FSMState            //角色的八个状态
    {
        None,Idle,Moving,Jump,Fall,Attack,Hit,Dead
    }
    

    [Header("------- 玩家控件参数 -------")]

    [SerializeField] Animator animator;
    [SerializeField] Rigidbody2D rb2d = null;
    [SerializeField] Transform pigT = null;
    [SerializeField] Transform attackPosOrigin;     //攻击的起点
    [SerializeField] Slider healthSldr;     //生命滑动条
    [SerializeField] TextMeshProUGUI displayNameTMP;    //角色名称
    [SerializeField] PhysicsMaterial2D noFriction;      //无摩擦力材质
    [SerializeField] PhysicsMaterial2D haveFriction;    //有摩擦力材质

    [Header("------- 玩家属性 -------")]

    public FSMState curState = FSMState.None;       //默认状态，从None开始
    [SerializeField] float speed = 400.0f;
    [SerializeField] float jumpForce = 20.0f;       //跳跃的力度
    [SerializeField] float attackRadius = 1.0f;     //攻击的半径
    [SerializeField] bool isPlayerDead = false;     //标记玩家状态

    [Header("------- 地面检测 -------")]

    [SerializeField] float distance = 1;        //距地面的距离
    [SerializeField] LayerMask layerMask;       //对应图层进行碰撞检测

    private float horizontalAxis = 0;
    private bool jump = false;                  //当前是否处于跳跃状态
    private bool attack = false;                //攻击

    //同步
    [SyncVar(hook = nameof(OnHealthChanged))]
    float health = 100;
    [SyncVar(hook =nameof(OnNickNameChanged))]
    string nickName = "";

    private void OnHealthChanged(float oldHealth,float newHealth)
    {
        healthSldr.value = newHealth;
    }

    private void OnNickNameChanged(string oldNickName,string newNickName)
    {
        displayNameTMP.text = newNickName;
    }

    [Command]
    private void CmdTakeDamage(int damage)
    {
        health -= damage;
    }

    [Command]
    private void CmdHitEnemy(GameObject target,int damage)      //告诉服务器需要对对象造成伤害
    {
        target.GetComponent<PlayerController>().curState = FSMState.Hit;
        target.GetComponent<PlayerController>().health -=damage;
    }

    [Command]
    private void CmdDeadPlayer()            
    {
        GameManager.GetInstance().RemovePlayerName(this.nickName);
        if(GameManager.GetInstance().IsGameOver())
        {
            GameManager.GetInstance().GameOver();
        }
    }

    public override void OnStartLocalPlayer()       //类似Start 只在客户端执行，一般用于初始化摄像机
    {
        if(GameManager.GetInstance().localPlayer==null)
        {
            GameManager.GetInstance().localPlayer = this;
        }
    }

    //重构函数
    protected override void Initialize()
    {
        Invoke("SetDefaultState", 3f);
    }

    protected override void FSMUpdate()
    {
        if (!isLocalPlayer) return;
        horizontalAxis = Input.GetAxisRaw("Horizontal");
        if(Input.GetButtonDown("Jump"))
        {
            jump = true;
            Debug.Log("Jump");
            Debug.Log("-----");
        }
        if(Input.GetButtonDown("Fire1"))
        {
            attack = true;
        }
        if (Input.GetKeyDown(KeyCode.M))
        {
            CmdTakeDamage(10);
        }
        //角色朝向  角色本来朝向左边，输入为右时反转，为左时不变，为0时维持当前
        int lookAt = horizontalAxis > 0 ? -1 : horizontalAxis < 0 ? 1 : (int)pigT.localScale.x;
        pigT.localScale = new Vector2(lookAt, 1);
    }

    protected override void FSMFixedUpdate()
    {
        if (!isLocalPlayer) return;
        if (GameManager.GetInstance().gameState != GameManager.GameState.GameStart) return;
        switch (curState)
        {
            case FSMState.Idle:
                UpdateIdleState();
                break;
            case FSMState.Moving:
                UpdateMovingState();
                break;
            case FSMState.Jump:
                UpdateJumpState();
                break;
            case FSMState.Fall:
                UpdateFallState();
                break;
            case FSMState.Hit:
                UpdateHitState();
                break;
            case FSMState.Dead:
                UpdateDeadState();
                break;
            case FSMState.Attack:
                UpdateAttackState();
                break;
        }
        if (health <= 0) curState = FSMState.Dead;
    }

    private void SetDefaultState()
    {
        curState = FSMState.Idle;       //将初始状态赋为Idle
    }

    private void UpdateIdleState()
    {
        transform.GetComponent<CircleCollider2D>().sharedMaterial  = haveFriction;
        animator.Play("Pig_Idle");      //播放动画
        //状态转换
        if(jump)
        {
            curState = FSMState.Jump;
            jump = false;
            return;
        }
        if(attack)
        {
            curState = FSMState.Attack;
            attack = false;
        }
        if(Mathf.Abs(horizontalAxis)>Mathf.Epsilon)     //输入不为0
        {
            curState = FSMState.Moving;
        }
        if(!IsGround())//判断不在地面
        {
            curState = FSMState.Fall;
        }
    }

    private void UpdateMovingState()
    {
        animator.Play("Pig_Run");
        rb2d.velocity = new Vector2(horizontalAxis * Time.fixedDeltaTime * speed, rb2d.velocity.y);
        //给刚体一个速度
        //状态转换
        if (jump)
        {
            curState = FSMState.Jump;
            jump = false;
            return;
        }
        if (attack)
        {
            curState = FSMState.Attack;
            attack = false;
            return;
        }
        if (!IsGround())//判断不在地面
        {
            curState = FSMState.Fall;
        }
        if(Mathf.Abs(horizontalAxis)<1&&IsGround())
        {
            curState = FSMState.Idle;
        }
    }

    private void UpdateJumpState()
    {
        jump = false;
        transform.GetComponent<CircleCollider2D>().sharedMaterial = noFriction;
        animator.Play("Pig_Jump");
        rb2d.velocity = new Vector2(horizontalAxis * speed * Time.fixedDeltaTime, rb2d.velocity.y);


        if (IsGround())//跳跃，还在地面时给一个向上的速度
        {
            rb2d.velocity = Vector2.up * jumpForce;     //向上的方向以及一个力
        }
        if (attack)
        {
            curState = FSMState.Attack;
            attack = false;
            return;
        }
        if(rb2d.velocity.y<0)       //当向下时改变状态
        {
            curState = FSMState.Fall;       
        }

    }

    private void UpdateFallState()
    {
        jump = false;
        transform.GetComponent<CircleCollider2D>().sharedMaterial = noFriction;
        animator.Play("Pig_Fall");

        rb2d.velocity = new Vector2(horizontalAxis * speed * Time.fixedDeltaTime, rb2d.velocity.y);

        if(attack)
        {
            curState = FSMState.Attack;
            attack = false;
            return;
        }
        if(IsGround())
        {
            curState = FSMState.Idle;
        }

    }

    private void UpdateAttackState()
    {
        if (!animator.GetCurrentAnimatorStateInfo(0).IsName("Pig_Attack"))          //参数0表示当前默认图层
        {
            animator.Play("Pig_Attack");
            //检测以攻击点为圆心，arrackRadius为半径，对应图层上的所有敌人
            Collider2D[] enemies = Physics2D.OverlapCircleAll(attackPosOrigin.position, attackRadius, LayerMask.GetMask("Enemy"));

            foreach(Collider2D enemy in enemies)
            {//造成伤害
                PlayerController playerController = enemy.GetComponent<PlayerController>();
                
                if(playerController.netId!=this.netId)      //判断接触对象是谁
                {
                    CmdHitEnemy(playerController.gameObject,10);
                }
            }
        }

        if(animator.GetCurrentAnimatorStateInfo(0).normalizedTime>0.9f)  //整数部分为动画的循环次数，小数部分为当前循环的百分比，即判断当前动画是否执行完成
        {
            if(IsGround())
            {
                curState = FSMState.Idle;
            }
            if(rb2d.velocity.y<0)
            {
                curState = FSMState.Fall;
            }
        }
        attack = false;
    }

    private void UpdateHitState()
    {
        animator.Play("Pig_Hit");

        if(IsGround())
        {
            curState = FSMState.Idle;
        }
        if(rb2d.velocity.y<0)
        {
            curState = FSMState.Fall;
        }
    }

    private void UpdateDeadState()
    {
        if (!isPlayerDead)
        {
            animator.Play("Pig_Dead");
            isPlayerDead = true;
            CmdDeadPlayer();
        }
    }

    private bool IsGround()
    {//发出射线进行判断
        return Physics2D.Raycast(transform.position, Vector2.down, distance, layerMask);
        //在对应图层进行判断，从当先位置发出，方向是向下，长度为1
    }

    /// <summary>
    ///修改角色名称的方法
    /// </summary>
    /// <param name="name"></param>
    public void SetNickName(string name)        
    {
        nickName = name;
    }

    /// <summary>
    /// 获取角色名称
    /// </summary>
    /// <returns></returns>
    public string GetNickName()
    {
        return nickName;
    }
}
