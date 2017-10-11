using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{

    // Use this for initialization
    public enum EnemyType { GROUND, FLYING, WATER };
    public EnemyType Type = EnemyType.GROUND;

    Rigidbody2D rigid;
    Animator anim;

    public float HP = 1;
    public Transform groundCheck;
    public Transform batas1;
    public Transform batas2;
    public LayerMask whatIsGround;
    public Sprite upSprite;
    public Sprite downSprite;

    float speed = 2;
    float downspeed = 7f;

    private bool isGrounded = false;
    
    bool isFacingRight = false;
    bool isMoveUp = true;
    float width = 0;
    float height = 0;
    
    void Start()
    {
        rigid = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
        width = GetComponent<SpriteRenderer>().bounds.extents.x;
        height = GetComponent<SpriteRenderer>().bounds.extents.y;
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        switch(Type)
        {
            case EnemyType.GROUND:
                isGrounded = Physics2D.Linecast(transform.position, groundCheck.position, whatIsGround);

                if (isGrounded)
                {
                    float predict = speed * Time.deltaTime;

                    if (transform.position.x + predict + width / 2 >= batas2.position.x && isFacingRight) Flip();
                    else if (transform.position.x - predict - width / 2 <= batas1.position.x && !isFacingRight) Flip();

                    if (isFacingRight) MoveRight();
                    else MoveLeft();
                }

                break;
            case EnemyType.FLYING:
                float predict2 = speed * Time.deltaTime;

                if (transform.position.x + predict2 + width / 2 >= batas2.position.x && isFacingRight) Flip();
                else if (transform.position.x - predict2 - width / 2 <= batas1.position.x && !isFacingRight) Flip();

                if (isFacingRight) MoveRight();
                else MoveLeft();

                break;
            case EnemyType.WATER:
                float predict3 = downspeed * 0.7f * Time.deltaTime;
                float predict4 = downspeed * Time.deltaTime;

                if (transform.position.y + predict3 + height / 2 >= batas2.position.y && isMoveUp) isMoveUp = false;
                else if (transform.position.y - predict4 - height / 2 <= batas1.position.y && !isMoveUp) isMoveUp = true;

                if (isMoveUp) MoveUp();
                else MoveDown();

                break;
        }
    }

    void MoveRight()
    {
        Vector3 pos = transform.position;
        pos.x += speed * Time.deltaTime;
        transform.position = pos;

        Debug.Log("MoveRight");

        //membalik arah karakter apabila menghadap ke arah yang berlawanan dari seharusya
        if (!isFacingRight)
        {
            Flip();
        }
    }

    void MoveLeft()
    {
        Vector3 pos = transform.position;
        pos.x -= speed * Time.deltaTime;
        transform.position = pos;

        Debug.Log("MoveLeft");

        //membalik arah karakter apabila menghadap ke arah yang berlawanan dari seharusya
        if (isFacingRight)
        {
            Flip();
        }
    }

    void MoveUp()
    {
        Vector3 pos = transform.position;
        pos.y += downspeed * 0.5f * Time.deltaTime;
        transform.position = pos;

        GetComponent<SpriteRenderer>().sprite = upSprite;

        Debug.Log("MoveUp");
    }

    void MoveDown()
    {
        Vector3 pos = transform.position;
        pos.y -= downspeed * Time.deltaTime;
        transform.position = pos;

        GetComponent<SpriteRenderer>().sprite = downSprite;

        Debug.Log("MoveDown");
    }

    void Flip()
    {
        Vector3 theScale = transform.localScale;
        theScale.x *= -1;
        transform.localScale = theScale;
        isFacingRight = !isFacingRight;
        Debug.Log("Flip");
    }

    void TakeDamage(float damage)
    {
        HP -= damage;
        GameController.GetInstance().AddScore(10);
        if (HP <= 0) Destroy(this.gameObject);
    }
}
