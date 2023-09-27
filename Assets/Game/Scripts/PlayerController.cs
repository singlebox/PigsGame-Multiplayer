using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
using System;
using TMPro;
using UnityEngine.UI;

public class PlayerController :FSM
{
    public enum FSMState            //��ɫ�İ˸�״̬
    {
        None,Idle,Moving,Jump,Fall,Attack,Hit,Dead
    }
    

    [Header("------- ��ҿؼ����� -------")]

    [SerializeField] Animator animator;
    [SerializeField] Rigidbody2D rb2d = null;
    [SerializeField] Transform pigT = null;
    [SerializeField] Transform attackPosOrigin;     //���������
    [SerializeField] Slider healthSldr;     //����������
    [SerializeField] TextMeshProUGUI displayNameTMP;    //��ɫ����
    [SerializeField] PhysicsMaterial2D noFriction;      //��Ħ��������
    [SerializeField] PhysicsMaterial2D haveFriction;    //��Ħ��������

    [Header("------- ������� -------")]

    public FSMState curState = FSMState.None;       //Ĭ��״̬����None��ʼ
    [SerializeField] float speed = 400.0f;
    [SerializeField] float jumpForce = 20.0f;       //��Ծ������
    [SerializeField] float attackRadius = 1.0f;     //�����İ뾶
    [SerializeField] bool isPlayerDead = false;     //������״̬

    [Header("------- ������ -------")]

    [SerializeField] float distance = 1;        //�����ľ���
    [SerializeField] LayerMask layerMask;       //��Ӧͼ�������ײ���

    private float horizontalAxis = 0;
    private bool jump = false;                  //��ǰ�Ƿ�����Ծ״̬
    private bool attack = false;                //����

    //ͬ��
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
    private void CmdHitEnemy(GameObject target,int damage)      //���߷�������Ҫ�Զ�������˺�
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

    public override void OnStartLocalPlayer()       //����Start ֻ�ڿͻ���ִ�У�һ�����ڳ�ʼ�������
    {
        if(GameManager.GetInstance().localPlayer==null)
        {
            GameManager.GetInstance().localPlayer = this;
        }
    }

    //�ع�����
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
        //��ɫ����  ��ɫ����������ߣ�����Ϊ��ʱ��ת��Ϊ��ʱ���䣬Ϊ0ʱά�ֵ�ǰ
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
        curState = FSMState.Idle;       //����ʼ״̬��ΪIdle
    }

    private void UpdateIdleState()
    {
        transform.GetComponent<CircleCollider2D>().sharedMaterial  = haveFriction;
        animator.Play("Pig_Idle");      //���Ŷ���
        //״̬ת��
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
        if(Mathf.Abs(horizontalAxis)>Mathf.Epsilon)     //���벻Ϊ0
        {
            curState = FSMState.Moving;
        }
        if(!IsGround())//�жϲ��ڵ���
        {
            curState = FSMState.Fall;
        }
    }

    private void UpdateMovingState()
    {
        animator.Play("Pig_Run");
        rb2d.velocity = new Vector2(horizontalAxis * Time.fixedDeltaTime * speed, rb2d.velocity.y);
        //������һ���ٶ�
        //״̬ת��
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
        if (!IsGround())//�жϲ��ڵ���
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


        if (IsGround())//��Ծ�����ڵ���ʱ��һ�����ϵ��ٶ�
        {
            rb2d.velocity = Vector2.up * jumpForce;     //���ϵķ����Լ�һ����
        }
        if (attack)
        {
            curState = FSMState.Attack;
            attack = false;
            return;
        }
        if(rb2d.velocity.y<0)       //������ʱ�ı�״̬
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
        if (!animator.GetCurrentAnimatorStateInfo(0).IsName("Pig_Attack"))          //����0��ʾ��ǰĬ��ͼ��
        {
            animator.Play("Pig_Attack");
            //����Թ�����ΪԲ�ģ�arrackRadiusΪ�뾶����Ӧͼ���ϵ����е���
            Collider2D[] enemies = Physics2D.OverlapCircleAll(attackPosOrigin.position, attackRadius, LayerMask.GetMask("Enemy"));

            foreach(Collider2D enemy in enemies)
            {//����˺�
                PlayerController playerController = enemy.GetComponent<PlayerController>();
                
                if(playerController.netId!=this.netId)      //�жϽӴ�������˭
                {
                    CmdHitEnemy(playerController.gameObject,10);
                }
            }
        }

        if(animator.GetCurrentAnimatorStateInfo(0).normalizedTime>0.9f)  //��������Ϊ������ѭ��������С������Ϊ��ǰѭ���İٷֱȣ����жϵ�ǰ�����Ƿ�ִ�����
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
    {//�������߽����ж�
        return Physics2D.Raycast(transform.position, Vector2.down, distance, layerMask);
        //�ڶ�Ӧͼ������жϣ��ӵ���λ�÷��������������£�����Ϊ1
    }

    /// <summary>
    ///�޸Ľ�ɫ���Ƶķ���
    /// </summary>
    /// <param name="name"></param>
    public void SetNickName(string name)        
    {
        nickName = name;
    }

    /// <summary>
    /// ��ȡ��ɫ����
    /// </summary>
    /// <returns></returns>
    public string GetNickName()
    {
        return nickName;
    }
}
