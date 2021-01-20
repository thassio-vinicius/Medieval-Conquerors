using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;


public class PlayerPanelController : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void ExitGame()
    {
        Destroy(gameObject);
        Application.Quit();
    }

    public void BackToMenu()
    {
        Destroy(gameObject);
        SceneManager.LoadScene("MainMenu");
    }

    public void ClosePanel()
    {
        gameObject.SetActive(false);
    }
}
