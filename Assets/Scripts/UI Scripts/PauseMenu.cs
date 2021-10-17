using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class PauseMenu : MonoBehaviour
{
    private bool GameIsPaused = false;

    public GameObject pauseMenuUI;
    public Button @continue, restart, menu, quit;
    public GameObject pauseMenuButtonUI;
    public Button pauseMenuButton;


    void Start()
    {
        @continue.onClick.AddListener(ContinueGame);
        restart.onClick.AddListener(Restart);
        menu.onClick.AddListener(MainMenu);
        quit.onClick.AddListener(QuitGame);
        pauseMenuButton.onClick.AddListener(PauseToggle);
    }


    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            PauseToggle();
        }
    }


    private void OnDestroy()
    {
        GameIsPaused = false;
        pauseMenuUI.SetActive(false);
    }

    public void PauseToggle()
    {
        if (GameIsPaused)
        {
            ContinueGame();
        }
        else
        {
            Pause();
        }
    }

    public void Pause()
    {
        //play audio clip
        FindObjectOfType<AudioManager>().Play("buttonClick");

        pauseMenuButtonUI.SetActive(false);
        pauseMenuUI.SetActive(true);
        Time.timeScale = 0f;
        GameIsPaused = true;
    }

    public void ContinueGame()
    {
        //play audio clip
        FindObjectOfType<AudioManager>().Play("buttonClick");

        pauseMenuButtonUI.SetActive(true);
        pauseMenuUI.SetActive(false);
        Time.timeScale = 1f;
        GameIsPaused = false;
    }

    public void Restart()
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
        //play audio clip
        FindObjectOfType<AudioManager>().Play("buttonClick");

        Debug.Log("QUITTING");
        StartCoroutine(WaitForSeconds(0.4f));
        Application.Quit();
    }

    private IEnumerator WaitForSeconds(float seconds)
    {
        yield return new WaitForSeconds(seconds);
    }
}
