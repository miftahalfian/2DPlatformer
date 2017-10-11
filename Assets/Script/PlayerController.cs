using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{

    Rigidbody2D rigid;
    Animator anim;

    public Transform groundCheck;
    public LayerMask whatIsGround;
    public GameObject projectile;
    public Vector2 velocity;
    public Vector2 offset;

    public float jumpForce; //besarnya gaya untuk mengangkat karakter ke atas
    public float walkForce; //besarnya gaya untuk mendorong karakter ke samping
    public float maxSpeed; //kecepatan maksimum dari karakter utama

    private bool isGrounded = false; //untuk menyimpan state apakah karakter berada di ground
    private bool isJump = false; //menyimpan kondisi sedang melompat atau tidak
    private bool isFall = false;
    private bool isDead = false; // menyimpan kondisi apakah sudah mati atau belum
    private bool canShoot = true;
    private bool IsDoubleJump = false;
    private bool canDoubleJump = false;
    public float cooldown = 1f;

    private float width;
    private float height;

    bool IsDie = false;

    [HideInInspector]
    public bool isFacingRight = true;    //menyimpan state apakah karakter sedang menghadap ke kanan atau kiri

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
        //isGrounded = Physics2D.Linecast(transform.position, groundCheck.position, whatIsGround);
        isGrounded = Physics2D.BoxCast(new Vector2(transform.position.x, transform.position.y), new Vector2(width, 2), 0, Vector2.down, height/2, whatIsGround);
        if ((isFall) && (isGrounded))
        {
            CharacterIdle();
            isFall = false;
        }

        if (rigid.velocity.y < 0)
        {
            Fall();
        }

        if (isGrounded) canDoubleJump = true;
    }

    void Update()
    {
        InputHandler();

        Vector3 tmpPos = Camera.main.WorldToScreenPoint(transform.position);
        if (tmpPos.y < 0)
        {
            Debug.Log("Destroy");
            Destroy(this.gameObject);
        }
        else if (tmpPos.y < height && !IsDie)
        {
            Die();
        }
    }

    void InputHandler()
    {
        float move = Input.GetAxis("Horizontal");

        if (isGrounded)
        {
            if (move > 0) WalkRight();
            else if (move < 0) WalkLeft();
            else if (!isJump && !isFall) CharacterIdle();
        }

        if (Input.GetKeyDown(KeyCode.UpArrow))
        {
            if (isGrounded) Jump();
            else if (isFall && canDoubleJump)
            {
                Vector2 vel = rigid.velocity;
                vel.y = 0;
                rigid.velocity = vel;

                Jump();

                Debug.Log("Double Jump");

                canDoubleJump = false;
            }
        }

        if (Input.GetKeyDown(KeyCode.Space))
        {
            if (canShoot)
            {
                GameObject go = (GameObject)Instantiate(projectile, (Vector2)transform.position + offset * transform.localScale.x, Quaternion.identity);
                go.GetComponent<Rigidbody2D>().velocity = new Vector2(velocity.x * transform.localScale.x, velocity.y);
                Vector3 scale = transform.localScale;
                go.transform.localScale = scale;
                CanShoot();
            }
        }
    }

    void CharacterIdle()
    {
        if (!IsDie)
        {
            rigid.velocity = new Vector2(0, rigid.velocity.y);
            anim.SetBool("Fall", false);
            anim.SetBool("Walk", false);
            anim.SetBool("Iddle", true);
            //Debug.Log("Iddle");
        }
    }

    void Jump()
    {
        if (!IsDie)
        {
            anim.SetBool("Walk", false);
            anim.SetBool("Iddle", false);
            anim.SetBool("Jump", true);
            rigid.AddForce(Vector2.up * jumpForce);
            isJump = true;
        }
    }

    void Fall()
    {
        if (!IsDie)
        {
            anim.SetBool("Walk", false);
            anim.SetBool("Jump", false);
            anim.SetBool("Fall", true);
            isJump = false;
            isFall = true;
        }
    }

    void WalkLeft()
    {
        if (!IsDie)
        {
            if (rigid.velocity.x * -1 < maxSpeed)
                rigid.AddForce(Vector2.left * walkForce);
            anim.SetBool("Walk", true);
            anim.SetBool("Iddle", false);

            //membalik arah karakter apabila menghadap ke arah yang berlawanan dari seharusya
            if (isFacingRight)
            {
                Flip();
            }
        }
    }

    void WalkRight()
    {
        if (!IsDie)
        {
            if (rigid.velocity.x < maxSpeed)
                rigid.AddForce(Vector2.right * walkForce);
            anim.SetBool("Walk", true);
            anim.SetBool("Iddle", false);

            //membalik arah karakter apabila menghadap ke arah yang berlawanan dari seharusya
            if (!isFacingRight)
            {
                Flip();
            }
        }
    }

    void Flip()
    {
        Vector3 theScale = transform.localScale;
        theScale.x *= -1;
        transform.localScale = theScale;
        isFacingRight = !isFacingRight;
    }

    void Die()
    {
        foreach (Collider2D c in GetComponents<Collider2D>())
        {
            c.enabled = false;
        }

        IsDie = true;

        anim.SetBool("Die", true);
        anim.SetBool("Walk", false);
        anim.SetBool("Iddle", false);
        anim.SetBool("Jump", false);
        anim.SetBool("Fall", false);
        rigid.velocity = Vector2.zero;
        rigid.AddForce(Vector2.up * jumpForce);
    }

    void OnCollisionEnter2D(Collision2D col)
    {
        if (col.gameObject.tag == "Enemy")
        {
            Die();
        }
    }

    IEnumerator CanShoot()
    {
        anim.SetBool("Shoot", true);
        canShoot = false;
        yield return new WaitForSeconds(cooldown);
        canShoot = true;
        anim.SetBool("Shoot", false);
    }
}
