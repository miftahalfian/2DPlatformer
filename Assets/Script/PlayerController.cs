using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerController : MonoBehaviour
{

    Rigidbody2D rigid; //rigidbody dari player
    Animator anim;

    public GameObject projectile; //obyek peluru
    public Vector2 projectileVelocity; //kecepatan peluru
    public Vector2 projectileOffset; //jarak posisi peluru dari posisi karakter
    public float cooldown = 1f; //jeda waktu untuk menembak

    public float jumpForce; //besarnya gaya untuk mengangkat karakter ke atas
    public float walkForce; //besarnya gaya untuk mendorong karakter ke samping
    public float maxSpeed; //kecepatan maksimum dari karakter utama

    public bool isGrounded = false; //untuk menyimpan state apakah karakter berada di ground
    bool isJump = false; //menyimpan kondisi sedang melompat
    bool isFall = false; //menyimpan kondisi ketika jatuh
    bool isWalking = false; //menyimpan kondisi jalan
    bool isDead = false; // menyimpan kondisi apakah sudah mati
    bool canShoot = true; //menyimpan kondisi apakah karakter dapat menembak
    bool canDoubleJump = false; //menyimpan kondisi apakah karakter bisa melakukan double jump

    float width;  //ukuran lebar karakter
    float height; //ukuran tinggi karakter
    
    public bool isFacingRight = true;   //menyimpan state apakah karakter sedang menghadap ke kanan atau kiri

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
        //jika karakter sudah ada di tanah setelah jatuh
        if ((isFall) && (isGrounded))
        {
            CharacterIdle();
            isFall = false;
        }

        //jika kecepatan karakter < 0 maka kita definisikan karakter sedang jatuh
        if (rigid.velocity.y < 0)
        {
            Fall();
        }

        //karakter bisa melakukan double jump ketika ada di tanah
        if (isGrounded) canDoubleJump = true;
    }

    void Update()
    {
        InputHandler();

        //mngecek posisi karakter
        Vector3 tmpPos = Camera.main.WorldToScreenPoint(transform.position);
        
        if (tmpPos.y < 0)
        {
            //jika karakter di luar layar maka destroy
            Destroy(this.gameObject);
            SceneManager.LoadScene(0);
        }
        else if (tmpPos.y < height && !isDead && !isGrounded)
        {
            //Jika karakter akan jatuh maka tampilkan animasi mati
            Die();
        }

        //if (rigid.velocity == Vector2.zero && !isJump && !isFall) CharacterIdle();
    }

    void InputHandler()
    {
        if (isGrounded)
        {
            if (Input.GetKeyDown(KeyCode.LeftArrow))
            {
                isWalking = true;
                if (isFacingRight) Flip();
            }
            else if (Input.GetKeyDown(KeyCode.RightArrow))
            {
                isWalking = true;
                if (!isFacingRight) Flip();
            }

            if (Input.GetKeyUp(KeyCode.LeftArrow) || Input.GetKeyUp(KeyCode.RightArrow))
            {
                isWalking = false;
                CharacterIdle();
            }

            if (isWalking)
            {
                if (isFacingRight) WalkRight();
                else WalkLeft();
            }
        }

        if (Input.GetKeyDown(KeyCode.UpArrow))
        {
            JumpClick();
        }

        //inputan untuk menembak
        if (Input.GetKeyDown(KeyCode.Space))
        {
            Fire();
        }
    }

    void CharacterIdle()
    {
        if (!isDead)
        {
            rigid.velocity = new Vector2(0, rigid.velocity.y);
            anim.SetBool("Fall", false);
            anim.SetBool("Walk", false);
            anim.SetBool("Iddle", true);
        }
    }

    void Jump()
    {
        if (!isDead)
        {
            anim.SetBool("Walk", false);
            anim.SetBool("Iddle", false);
            anim.SetBool("Jump", true);
            rigid.AddForce(Vector2.up * jumpForce);
            isJump = true;
            isGrounded = false;
            isWalking = false;
        }
    }

    void Fall()
    {
        if (!isDead)
        {
            anim.SetBool("Walk", false);
            anim.SetBool("Jump", false);
            anim.SetBool("Fall", true);
            isJump = false;
            isFall = true;
            isGrounded = false;
            isWalking = false;
        }
    }

    void WalkLeft()
    {
        if (!isDead)
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
        if (!isDead)
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
        /*
        Pada animasi die ini, karakter akan melompat terlebih dahulu sebelum jatuh.
        Mirip seperti game super mario. Agar karakter tidak menubruk objek lain,
        maka collidernya di disable dulu.
        */

        foreach (Collider2D c in GetComponents<Collider2D>())
        {
            c.enabled = false;
        }

        isDead = true;

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
        //ketika menubruk musuh maka player akan mati
        if (col.gameObject.tag == "Enemy")
        {
            Die();
        }
        else if (col.gameObject.CompareTag("Ground"))
        {
            isGrounded = true;
        }
    }

    void OnCollisionStay2D(Collision2D col)
    {
        if (col.gameObject.CompareTag("Ground"))
        {
            isGrounded = true;
        }
    }

    public void LeftDown()
    {
        if (isGrounded)
        {
            isWalking = true;
            if (isFacingRight) Flip();
        }
    }

    public void RightDown()
    {
        if (isGrounded)
        {
            isWalking = true;
            if (!isFacingRight) Flip();
        }
    }

    public void LeftUp()
    {
        if (isGrounded)
        {
            isWalking = false;
            CharacterIdle();
        }
    }

    public void RightUp()
    {
        if (isGrounded)
        {
            isWalking = false;
            CharacterIdle();
        }
    }

    public void JumpClick()
    {
        if (isGrounded) Jump();
        else if (isFall && canDoubleJump) //jika dalam posisi jatuh dan bisa double jump
        {
            //agar bisa melakukan double jump,
            //velocity y dijadikan nol dulu
            //agar kecepatan untuk lompat sama

            Vector2 vel = rigid.velocity;
            vel.y = 0;
            rigid.velocity = vel;

            isFall = false;
            anim.SetBool("Fall", false);

            Jump();

            canDoubleJump = false;
            //setelah melakukan double jump, tidak bisa lompat lagi
        }
    }

    public void Fire()
    {
        //jika karakter dapat menembak
        if (canShoot)
        {
            //Membuat projectile baru
            GameObject bullet = (GameObject)Instantiate(projectile, (Vector2)transform.position + projectileOffset * transform.localScale.x, Quaternion.identity);
            //Mengatur kecepatan dari projectile.
            bullet.GetComponent<Rigidbody2D>().velocity = new Vector2(projectileVelocity.x * transform.localScale.x, projectileVelocity.y);

            //Menyesuaikan scale dari projectile dengan scale karakter
            Vector3 scale = transform.localScale;
            bullet.transform.localScale = scale;
            CanShoot();
        }
    }

    IEnumerator CanShoot()
    {
        canShoot = false;
        yield return new WaitForSeconds(cooldown);
        canShoot = true;
    }
}
