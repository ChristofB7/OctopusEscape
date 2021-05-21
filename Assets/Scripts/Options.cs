using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class Options : MonoBehaviour
{
    private ControlQueue player;

    public static bool GameIsPaused = false;

    [SerializeField] GameObject pauseUI = null;

    [SerializeField] private Button enterPracticeModeButton;
    [SerializeField] private Button exitPracticeModeButton;
    [SerializeField] private GameObject practiceModePosition;



    // Start is called before the first frame update
    void Start()
    {
        player = FindObjectOfType<ControlQueue>();
        pauseUI.SetActive(false);

        if (PlayerPrefs.GetInt("Practice") == 1)
        {
            player.EnablePracticeMode();
            EnableButton(exitPracticeModeButton);
            Debug.Log("enabling exit practice mode button");
            DisableButton(enterPracticeModeButton);
            Debug.Log("disabling enter practice mode button");
        }

    }

    // Update is called once per frame
    void Update()
    {

        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (GameIsPaused)
            {
                ResumeGame();
            }
            else
            {
                PauseGame();
            }
        }
    }

    public void ReturnToMenu()
    {
        Time.timeScale = 1f;
        GameIsPaused = false;
        SceneManager.LoadScene("Main Menu");
    }

    public void LoadLevelSelect()
    {
        GameIsPaused = false;
        Time.timeScale = 1f;
        SceneManager.LoadScene("Level Select");
    }

    public void QuitGame()
    {
        Application.Quit();
    }

    public void EnterPracticeMode()
    {
        Time.timeScale = 1f;
        GameIsPaused = false;
        PlayerPrefs.SetInt("Practice", 1);
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    public void SetExitPModeSprite()
    {
        exitPracticeModeButton.GetComponent<Image>().sprite = exitPracticeModeButton.spriteState.pressedSprite;
    }
    public void SetEnterPModeSprite()
    {
        enterPracticeModeButton.GetComponent<Image>().sprite = enterPracticeModeButton.spriteState.pressedSprite;
    }

    public void ExitPracticeMode()
    {
        Time.timeScale = 1f;
        GameIsPaused = false;
        PlayerPrefs.SetInt("Practice", 0);
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    private void PauseGame()
    {
        pauseUI.SetActive(true);
        Time.timeScale = 0;
        GameIsPaused = true;
    }

    public void ResumeGame()
    {
        pauseUI.SetActive(false);
        Time.timeScale = 1f;
        GameIsPaused = false;
    }

    private void DisableButton(Button button)
    {
        button.transform.position = new Vector3(-10000, -10000, -10000);
    }

    private void EnableButton(Button button)
    {
        button.transform.position = practiceModePosition.transform.position;
    }

    internal bool isPaused()
    {
        return GameIsPaused;
    }
}
