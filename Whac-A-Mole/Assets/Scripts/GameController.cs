using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using UnityEngine;
using TMPro;

public class GameController : MonoBehaviour
{
    public static GameController instance;
    public GameObject mainMenu, inGameUI,endScreen,recordPanel, pauseMenu;

    public Transform molesParent;
    public MoleBehaviour[] moles;

    public bool playing = false;

    public float gameDuration;
    public float timePlayed = 60f;

    public int points = 0;
    public float clicks = 0;
    public float failedClicks = 0;

    public TMP_InputField nameField;
    public string playerName;

    public TextMeshProUGUI infoGame;
   // public TextMeshProUGUI infoGame2;

    /*AÑADIDO POR CRISTIAN (Además de los public int points, clicks & failedClicks y modificar el timePlayed = 60f, cono gameDuration = 0f.
    El pauseMenu es un extra, para poder reiniciar y ver que funciona el tiempo y puntos correctamente). También he añadido un inforGame2 para el mensaje de NO record.*/
    public TextMeshProUGUI timeText;
    public TextMeshProUGUI pointsText;
    public GameObject gameController;
    public GameObject moleContainer;

    public TextMeshProUGUI recordText;

    float clicksPorcentaje;
    float totalClicks;

    public bool pups;
    public float timeBooleana;
    public GameObject puBomb;
    public GameObject puTime;

    public float timerTime = 5f;


    void Awake()
    {
        if (GameController.instance == null)
        {
            ConfigureInstance();
        }
        else
        {
            Destroy(this);
        }

        
    }

    void ConfigureInstance()
    {
        //Configura acceso a moles
        moles = new MoleBehaviour[molesParent.childCount];
        for (int i = 0; i < molesParent.childCount; i++)
        {
            moles[i] = molesParent.GetChild(i).GetComponent<MoleBehaviour>();
        }

        //Inicia los puntos
        points = 0;
        clicks = 0;
        failedClicks = 0;

        //Activa la UI inicial
        inGameUI.SetActive(false);
        mainMenu.SetActive(true);
        endScreen.SetActive(false);
        recordPanel.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        if (playing == true)
        {
            timePlayed -= Time.deltaTime;
            timeText.text = timePlayed.ToString("00");
            pointsText.text = points.ToString("00");


            if (timePlayed <= gameDuration)
            {

                ShowEndScreen();
                playing = false;
                for (int i = 0; i < moles.Length; i++)
                {
                    moles[i].StopMole();
                }
                
            }
            else
            {
                CheckClicks();
                totalClicks = clicks + failedClicks;
                clicksPorcentaje = (clicks / totalClicks) * 100f;
            }
            
        }

        

        if (puTime.activeInHierarchy == true)
        {
            timerTime -= Time.deltaTime;
        }
    }


    void ShowEndScreen()
    {
        endScreen.SetActive(true);
        infoGame.text = " Total points : " + points + "\n Record actual: " + recordText + "\n GoodShot: " + clicksPorcentaje + "%" + "\n BadShots: " + failedClicks;

        bool isRecord = true;
        //si hay nuevo record mostrar el panel recordPanel
        recordPanel.SetActive(isRecord);
        SaveRecord();
        recordText.text += PlayerPrefs.GetInt("Record").ToString("0000");
        recordText.text += PlayerPrefs.GetString(playerName);
    }

    /// <summary>
    /// Function called from End Screen when players hits Retry button
    /// </summary>
    public void Retry()
    {
        //Guardar record si es necesario
        SaveRecord();
        Debug.Log("Record guardado con éxito");
        //Acceso al texto escrito
        playerName = nameField.text;
        Debug.Log("Record de " + playerName);

        //Reinicia información del juego
        ResetGame();
        //Cambia las pantallas
        inGameUI.SetActive(true);
        mainMenu.SetActive(false);
        endScreen.SetActive(false);
        //Activa juego
        playing = true;
        EnterOnGame();

        //Reinicia moles
        for (int i = 0; i < moles.Length; i++)
        {
            moles[i].ResetMole();
        }
    }

    /// <summary>
    /// Restarts all info game
    /// </summary>
    void ResetGame()
    {
        for (int i = 0; i < moles.Length; i++)
        {
            moles[i].StopMole();
        }

        timePlayed = 60.0f;
        points = 0;
        clicks = 0f;
        failedClicks = 0f;
    }

    public void EnterMainScreen()
    {
        //Reinicia información del juego
        ResetGame();
        //Cambia las pantallas
        inGameUI.SetActive(false);
        mainMenu.SetActive(true);
        endScreen.SetActive(false);
        recordPanel.SetActive(false);

        EnterOnGame();
        Debug.Log("Record guardado con éxito");

    }

    /// <summary>
    /// Used to check if players hits or not the moles/powerups
    /// </summary>
    public void CheckClicks()
    {
        if ((Input.touchCount >= 1 && Input.GetTouch(0).phase == TouchPhase.Ended) || (Input.GetMouseButtonDown(0)))
        {
          
            Vector3 pos = Input.mousePosition;
            if (Application.platform == RuntimePlatform.Android)
            {
                pos = Input.GetTouch(0).position;
            }

            Ray rayo = Camera.main.ScreenPointToRay(pos);
            RaycastHit hitInfo;
            if (Physics.Raycast(rayo, out hitInfo))
            {
                if (hitInfo.collider.tag.Equals("Mole"))
                {
                    MoleBehaviour mole = hitInfo.collider.GetComponent<MoleBehaviour>();
                    if (mole != null)
                    {
                        mole.OnHitMole();
                        points += 100;
                        clicks += 1f;
                    }
                }
                else if (hitInfo.collider.tag.Equals("PowerUp1"))
                {
                    clicks += 1f;

                    for (int i = 0; i < moles.Length; i++)
                    {
                        points += 100;
                        moles[i].OnHitMole();
                    }
                }
                else if (hitInfo.collider.tag.Equals("PowerUp2"))
                {
                    clicks += 1f;
                    if(timerTime > 0)
                    {
                        timePlayed -= (Time.deltaTime - 0.5f);
                    }
                    else
                    {
                        timePlayed -= Time.deltaTime;
                    }
                }
                else if (hitInfo.collider.tag.Equals("Menu"))
                {
                    clicks += 0f;
                    failedClicks += 0f;
                }
                else
                {
                    failedClicks += 1f;
                }
            }
        }
    }

    public void OnGameStart()
    {
        mainMenu.SetActive(false);
        inGameUI.SetActive(true);
        points = 0;
        failedClicks = 0f;
        for (int i = 0; i < moles.Length; i++)
        {
            moles[i].ResetMole(moles[i].initTimeMin, moles[i].initTimeMax);
        }
        playing = true;
    }

    /// <summary>
    /// Funcion para entrar en pausa, pone playing en false y muestra la pantalla de pausa.
    /// </summary>
    public void EnterOnPause()
    {
        pauseMenu.SetActive(true);
        gameController.SetActive(false);
        moleContainer.SetActive(false);
    }
    public void EnterOnGame()
    {
        pauseMenu.SetActive(false);
        gameController.SetActive(true);
        moleContainer.SetActive(true);
    }

    public void SaveRecord()
    {
        if(points > PlayerPrefs.GetInt("Record"))
        {
            PlayerPrefs.SetInt("Record", points);
            PlayerPrefs.SetString("playerName", nameField.text);
            recordPanel.SetActive(true);
        }
        else
        {
            recordPanel.SetActive(false);
            //infoGame2.text = "Record actual: " + recordText + " por Cristian";
        }
    }
}
