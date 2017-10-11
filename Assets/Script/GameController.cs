using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameController : MonoBehaviour {

    // Use this for initialization
    public int score = 0;
    public Text ScoreLabel;
    private static GameController instance;

	void Start () {
        instance = this;
	}
	
	// Update is called once per frame
	void Update () {
		
	}

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
