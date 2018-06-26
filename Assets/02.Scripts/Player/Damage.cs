using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Damage : MonoBehaviour {

    // Bullet이 충돌했을 때 체크할 Bullet의 태그
    private const string bulletTag = "BULLET";
    private const string enemyTag = "ENEMY";

    private float maxHP = 100.0f;
    public float currentHP;

    public delegate void PlayerDieHandler();
    public static event PlayerDieHandler onPlayerDie;

    public Image bloodScreen;

    public Image hpBar;

    Coroutine bloodCoroutine = null;

    Color startColor = new Color(0.0f, 1.0f, 0.0f, 1.0f);
    Color middleColor = new Color(1.0f, 0.78f, 0.0f, 1.0f);
    Color endColor = new Color(0.88f, 0.0f, 0.0f, 1.0f);
   
    // Use this for initialization
    void Start () {
        currentHP = maxHP;
        DisplayHPBar();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == bulletTag)
        {
            if (bloodCoroutine != null)
                StopCoroutine(bloodCoroutine);

            bloodCoroutine = StartCoroutine(ShowBloodScreen(0.5f));

            Destroy(other.gameObject);

            currentHP -= 5.0f;
            Debug.Log("Player HP : " + currentHP);

            DisplayHPBar();

            // Player 체력이 0 이하
            if (currentHP <= 0.0f)
            {
                PlayerDie();
            }
        }        
    }

    void DisplayHPBar()
    {
        float amount = currentHP / maxHP;
        Color newColor;

        if (amount > 0.5f)
        {
            newColor = Color.Lerp(middleColor, startColor, (amount * 2.0f) - 1.0f);
        }
        else
        {
            newColor = Color.Lerp(endColor, middleColor, amount * 2.0f);
        }
        hpBar.color = newColor;
        hpBar.fillAmount = amount;
    }
    IEnumerator ShowBloodScreen(float duration = 0.5f)
    {
        float elapsedTime = 0.0f;
        Color currentColor = bloodScreen.color;
        float alpha = 1.0f;

        while (alpha <= 0.0f)
        {
            alpha = Mathf.Lerp(1.0f, 0.0f, elapsedTime / duration);
            currentColor.a = alpha;
            bloodScreen.color = currentColor;

            elapsedTime += Time.deltaTime;
            yield return null;
        }

        elapsedTime = 0.0f;
        currentColor.a = 0.0f;
        bloodScreen.color = currentColor;
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
