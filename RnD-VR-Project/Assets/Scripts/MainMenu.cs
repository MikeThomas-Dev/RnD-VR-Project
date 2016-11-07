using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour {

    public Canvas MainCanvas;

    void Awake()
    {

    }

  public void LoadOn()
    {
        //Application.LoadLevel(1);
        SceneManager.LoadScene(1);

    }  


}
