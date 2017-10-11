using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameController : MonoBehaviour {
    public int score = 0; //digunakan untuk mencatat skor
    public Text ScoreLabel; //digunakan untuk menampilkan skor
    private static GameController instance; //dipakai untuk memanggil method class ini dari class lain

	void Start () {
        instance = this;
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    //Digunakan untuk menambah skor
    public void AddScore(int scr)
    {
        score += scr;
        ScoreLabel.text = "Score: " + score.ToString();
    }

    public static GameController GetInstance()
    {
        return instance;
    }
}
