using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameOver : MonoBehaviour
{

    public bool gameIsOver = false;
    public GameObject gameOverUI;

    public Button playAgain, menu, quit;

    private void Awake()
    {
        gameIsOver = false;
        GameOverMenuOff();
    }

    void Start()
    {
        gameIsOver = false;
        GameOverMenuOff();

        playAgain.onClick.AddListener(PlayAgain);
        menu.onClick.AddListener(MainMenu);
        quit.onClick.AddListener(QuitGame);
    }

    private void Update()
    {
        if (GameObject.Find("Player") != null)
        {
            gameIsOver = false;
            GameOverMenuOff();

        } else if (GameObject.Find("Player") == null && !gameIsOver && GameObject.Find("Level") != null)
        {
            gameIsOver = true;
            GameOverMenuOn();
        }
    }

    private void OnDestroy() 
    {
        gameIsOver = false;
        gameOverUI.SetActive(false);
    }

    public void GameOverMenuOn()
    {
        ////play audio clip
        //FindObjectOfType<AudioManager>().Play("buttonClick"); // got annoying

        gameOverUI.SetActive(true);
    }

    public void GameOverMenuOff()
    {
        //play audio clip
        //FindObjectOfType<AudioManager>().Play("buttonClick");
        gameOverUI.SetActive(false);

    }

    public void PlayAgain()
    {
        //play audio clip
        FindObjectOfType<AudioManager>().Play("buttonClick");

        StartCoroutine(WaitForSeconds(0.4f));
        SceneManager.LoadSceneAsync("Level_1", LoadSceneMode.Single);

    }

    public void MainMenu()
    {
        //play audio clip
        FindObjectOfType<AudioManager>().Play("buttonClick");

        StartCoroutine(WaitForSeconds(0.4f));
        SceneManager.LoadSceneAsync("StartMenu", LoadSceneMode.Single);

    }

    public void QuitGame()
    {
        Debug.Log("QUITTING");
        StartCoroutine(WaitForSeconds(0.4f));
        Application.Quit();
    }

    private IEnumerator WaitForSeconds(float seconds)
    {
        yield return new WaitForSeconds(seconds);
    }
}
