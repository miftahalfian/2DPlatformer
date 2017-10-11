using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour {

    // Use this for initialization
    Rigidbody2D rigid;
    public GameObject explosion;

	void Start () {
        Destroy(this.gameObject, 10);
        rigid = GetComponent<Rigidbody2D>();
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    void OnCollisionEnter2D (Collision2D col)
    {
        if (col.gameObject.CompareTag("Character")) return;

        if (col.gameObject.tag == "Enemy")
        {
            col.gameObject.SendMessage("TakeDamage", 1);
            Explode();
        }
        else Explode();

        Debug.Log("Triggered");
    }

    void Explode()
    {
        Debug.Log("Explode");
        Instantiate(explosion, transform.position, Quaternion.identity);
        Destroy(this.gameObject);
    }
}
