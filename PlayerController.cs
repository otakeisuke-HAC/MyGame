using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    private GameObject gameManagerObj;
    public GameManager gameManager;
    public GameObject GameOverCanvas;
    
    public float m_WalkSpeed = 8f;      // 歩く速度（メートル/秒）
    public float m_RotateSpeed = 180.0f;   // 回転速度（度/秒）
    public float m_Gravity = -25f;      // 重力加速度
    public float m_JumpPower = 10.0f; // ジャンプ力

    CharacterController m_Controller; // CharacterControllerの参照
    float m_VelocityY = 0; // y軸方向の速度
    [HideInInspector]
    public Animator m_Animator; //アニメーション

    private int m_HP = 3; //HP
    bool m_Invincibility = false; //無敵フラグ
    float m_InvincibilityTime = 0; //無敵時間

    void Start()
    {
        //GameManagerObjectのGameManagerを取得
        gameManagerObj = GameObject.Find("GameManager");
        gameManager = gameManagerObj.GetComponent<GameManager>();
        // CharacterControllerの参照を取得
        m_Controller = GetComponent<CharacterController>();
        //Animatorの参照を取得
        m_Animator = GetComponent<Animator>();
        
    }

    void Update()
    {

        // カメラの正面向きのベクトルを取得
        Vector3 forward = Camera.main.transform.forward;

        // y成分を無視して水平にする
        forward.y = 0;

        // 正規化（ベクトルの長さを1にする）
        forward.Normalize();


        // 速度を求める（1秒あたりの移動量）
        Vector3 velocity =
              forward * Input.GetAxis("Vertical") * m_WalkSpeed
            + Camera.main.transform.right * Input.GetAxis("Horizontal") * m_WalkSpeed;



        //MoveSpeedに応じてアニメーションを再生
        m_Animator.SetFloat("MoveSpeed", velocity.magnitude);

        // 前を向かせる
        if (velocity.magnitude > 0)
        {
            transform.LookAt(transform.position + velocity);
        }

        // 接地しているなら
        if (m_Controller.isGrounded)
        {
            m_VelocityY = 0; // 落下を止める
        }

        // 重力で下方向に加速
        m_VelocityY += m_Gravity * Time.deltaTime;

        // 接地中に、ジャンプボタンでジャンプする
        if (m_Controller.isGrounded && Input.GetButtonDown("Jump"))
        {
            m_VelocityY = m_JumpPower;
            m_Animator.SetTrigger("Jump");
        }

        // y方向の速度を適用
        velocity.y = m_VelocityY;

        // 今フレームの移動量を求める
        Vector3 movement = velocity * Time.deltaTime;

        // CharacterControllerに命令して移動する
        m_Controller.Move(movement);

        m_Animator.SetBool("IsGrounded", m_Controller.isGrounded);

        Attack();
        Block();
        WalkingConditions();

        if (m_InvincibilityTime >= 0)
        {
            m_InvincibilityTime -= Time.deltaTime;
        }
        else if(m_InvincibilityTime <= 0)
        {
            m_Invincibility = false;
        }
    }

    public void Damage()
    {
        if (m_Invincibility == false)
        {
            m_HP--;
            m_InvincibilityTime = 3f;
            if (m_HP <= 0)
            {
                //死亡処理
                Die();
            }
            else if (m_HP > 0)
            {
                //ダメージ処理
                m_Animator.SetTrigger("Damage");
                m_Animator.SetBool("BlockBool", false);
                gameManager.CallInoperable(1.5f); //指定した秒数このスクリプトを無効化する
                m_Invincibility = true;
            }
        }
    }

    void Attack()
    {
        //アニメーションがAttackになったら移動スピードを0にする
        if (Input.GetButtonDown("Attack") && m_Controller.isGrounded)
        {
            m_Animator.SetTrigger("Attack");
            m_WalkSpeed = 0;
        }
        
    }

    void Block()
    {
        if (Input.GetButtonDown("Block") && m_Animator.GetBool("BlockBool") == false)
        {
            m_Animator.SetTrigger("BlockTrigger");
            m_WalkSpeed = 0;
            m_Animator.SetBool("BlockBool", true);
        }
        else if(Input.GetButtonDown("Block") && m_Animator.GetBool("BlockBool") == true)
        {
            m_Animator.SetBool("BlockBool" , false);
        }
        
    }

    public void BlockDamage()
    {
        m_Animator.SetTrigger("BlockDamage");
    }

    void WalkingConditions()
    {
        if (m_Animator.GetCurrentAnimatorStateInfo(0).IsName("Move") && !m_Animator.IsInTransition(0) && m_Animator.GetBool("BlockBool") == false
            && m_Animator.GetBool("Attack") == false)
        {
            m_WalkSpeed = 8;
        }
    }

    void Die()
    {
        m_Animator.SetTrigger("Die");
        enabled = false;
        gameObject.tag = "Untagged";
        GameOverCanvas.SetActive(true);
    }
}
