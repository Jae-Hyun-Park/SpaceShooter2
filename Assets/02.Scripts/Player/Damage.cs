using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Damage : MonoBehaviour {

    // Bullet이 충돌했을 때 체크할 Bullet의 태그
    private const string bulletTag = "BULLET";
    private const string enemyTag = "ENEMY";

    private float maxHP = 100.0f;
    public float currentHP;

    public delegate void PlayerDieHandler();
    public static event PlayerDieHandler onPlayerDie;
	// Use this for initialization
	void Start () {
        currentHP = maxHP;
	}

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == bulletTag)
        {
            Destroy(other.gameObject);

            currentHP -= 5.0f;
            Debug.Log("Player HP : " + currentHP);

            // Player 체력이 0 이하
            if (currentHP <= 0.0f)
            {
                PlayerDie();
            }
        }        
    }

    void PlayerDie()
    {
        Debug.Log("Player has Dead..");

        onPlayerDie();
        //GameObject[] enemies = GameObject.FindGameObjectsWithTag(enemyTag);

        //for (int i = 0; i < enemies.Length; ++i)
        //{
        //    enemies[i].SendMessage("OnPlayerDie", SendMessageOptions.DontRequireReceiver);
        //}
    }

}
