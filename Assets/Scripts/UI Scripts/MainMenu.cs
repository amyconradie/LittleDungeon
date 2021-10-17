using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{

    public void PlayGame()
    {
        StartCoroutine(WaitForSeconds(0.6f)); //for sound effect to finish
        SceneManager.LoadSceneAsync("Level_1", LoadSceneMode.Single);
    }

    public void QuitGame()
    {
        Debug.Log("QUITTING");
        StartCoroutine(WaitForSeconds(0.4f)); //for sound effect to finish
        Application.Quit();
    }
    

    private IEnumerator WaitForSeconds(float seconds)
    {
        yield return new WaitForSeconds(seconds);
    }
}
