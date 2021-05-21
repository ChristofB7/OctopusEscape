using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Exit : MonoBehaviour
{
    private int nextScene;
    // Start is called before the first frame update
    void Start()
    {
        int level = PlayerPrefs.GetInt("levelAt");
        if(level<1||level>11)
        {
            PlayerPrefs.SetInt("levelAt",1);
        }   
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void LoadLevelSelect()
    {
        SceneManager.LoadScene("Level Select");
    }

    public void QuitApplication()
    {
        Application.Quit();
    }

    public void LoadNextScene()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex+1);
    }

    public void ReturnToMenu()
    {
        SceneManager.LoadScene("Main Menu");
    }

    void OnCollisionEnter2D(Collision2D col)
    {
        Debug.Log("OnCollisionEnter2D");
        int level = PlayerPrefs.GetInt("levelAt");
        int max = PlayerPrefs.GetInt("maxLevels");
        if (level>0 || level< max)
        {
            if(SceneManager.GetActiveScene().buildIndex < level)
            {
                Debug.Log("Didn't add one");
            }
            else
            {
                PlayerPrefs.SetInt("levelAt", level + 1);
            }

        }
        LoadNextScene();

    }

    public void ResetAllProgress()
    {
        PlayerPrefs.SetInt("levelAt", 1);
    }
    public void AddOneLevel()
    {
        int level = PlayerPrefs.GetInt("levelAt");
        PlayerPrefs.SetInt("levelAt", level+1);
    }
}
