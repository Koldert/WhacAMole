using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class PowerUps : MonoBehaviour
{
    public GameObject PowerUp01;
    public GameObject PowerUp02;

    public float timerPU;
    public float timeScale = 5f;
    public float velocity;
    public float endPosition;

    public float posibility;
    public GameObject puBomb;
    public GameObject puTime;


    // Start is called before the first frame update
    void Awake()
    {
        posibility = Random.Range(0f, 100f);
        timerPU = Random.Range(10f, 50f);
        velocity = Random.Range(2f, 4f);
        endPosition = Random.Range(-4f, 22.5f);
    }

    // Update is called once per frame
    void Update()
    {
        timerPU -= Time.deltaTime;
        timeScale -= (Time.deltaTime * 0.5f);

        if(timerPU < 0)
        {
            LeftToRight();
        }
    }
    public void LeftToRight()
    {
        LeanTween.moveX(gameObject.GetComponent<RectTransform>(), endPosition, velocity);
    }

    /*public void RightToLeft()
    {
        LeanTween.moveX(gameObject.GetComponent<RectTransform>(), endPosition, velocity);
    }*/
    public void PowerUp()
    {
        if (posibility <= 20f)
        {
            Debug.Log("Existe una posibilidad de 20%");
            puBomb.SetActive(true);
            puTime.SetActive(true);
        }
        else if (posibility <= 50f)
        {
            Debug.Log("Existe una posibilidad de 50%");
            puBomb.SetActive(true);
            puTime.SetActive(false);
        }
        else
        {
            Debug.Log("No existe una posibilidad");
            puBomb.SetActive(false);
            puTime.SetActive(false);
        }


    }
}
