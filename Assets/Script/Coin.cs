using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Coin : MonoBehaviour {

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    void OnTriggerEnter2D (Collider2D col)
    {
        if (col.gameObject.CompareTag("Character"))
        {
            GameController.GetInstance().AddScore(50);
            Destroy(this.gameObject);
        }
    }
}
