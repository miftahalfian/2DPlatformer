using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    /*
    Untuk enemy, kita ada beberapa jenis tipe:
    1. GROUND : Enemy yg ada di darat. Enemy ini punya behaviour gerak kanan kiri
    2. FLYING : Enemy yg ada di udara. Behaviour enemy ini seperti enemy ground
    3. WATER : Enemy yg ada di air. Enemy ini punya behaviour muncul dari bawah ke atas kemudian turun lagi.
    */

    public enum EnemyType { GROUND, FLYING, WATER };
    public EnemyType Type = EnemyType.GROUND;

    Rigidbody2D rigid;
    Animator anim;

    public float HP = 1; //HP dari enemy.
    public Transform batas1; //untuk enemy ground atau flying digunakan untuk batas bergerak ke kiri, untuk enemy water digunakan untuk batas ketika jatuh
    public Transform batas2; //untuk enemy ground atau flying digunakan untuk batas bergerak ke kanan, untuk enemy water digunakan untuk batas ketika naik
    public Sprite upSprite; //sprite yg ditampilkan ketika enemy muncul ke atas (khusus tipe water)
    public Sprite downSprite; //sprite yg ditampilkan ketika enemy jatuh (khusus tipe water)
    public float delay; //digunakan untuk delay sebelum loncat kembali (khusus tipe water)

    float speed = 2; //kecepatan enemy bergerak
    float downspeed = 7f; //kecepatan enemy jatuh (khusus tipe water)

    bool isGrounded = false; //untuk menyimpan state apakah enemy berada di ground
    bool isFacingRight = false;  //menyimpan state apakah karakter sedang menghadap ke kanan atau kiri
    bool isMoveUp = true; //menyimpan state apakah karakter sedang menghadap melompat (khusus tipe water)
    float width = 0;  //lebar enemy
    float height = 0; //tinggi enemy
    
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
        if (HP <= 0)
        {
            GameController.GetInstance().AddScore(10);
            Destroy(this.gameObject);
        }
    }

    void OnCollisionEnter2D(Collision2D col)
    {
        if (col.gameObject.CompareTag("Ground"))
        {
            isGrounded = true;
        }
    }
}
