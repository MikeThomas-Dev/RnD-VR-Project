using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using UnityEngine.SceneManagement;
using System.Threading;

public class MainMenu : MonoBehaviour {

    public Canvas MainCanvas;
    public Button Start;
    public Button Reset;

    
    void Awake() //everything inside awake gets called before the start function
    {

        
    }

  public void LoadOn()
    {
        //load the specified scene where the indices e.g. (1) represent a certain scene in Unity
        //use File>Build Settings... to check which scene has which index or to change or add scenes to be able to index them
        //Scene 1 is specified as the TestScene so the main play area
        SceneManager.LoadScene(1, LoadSceneMode.Additive);
       // SceneManager.LoadSceneAsync(1);
       // SceneManager.UnloadScene(1);
        Start.enabled = false;
        Reset.enabled = true;

    }

    
    public void onButtonResetEnvClick()
    {
        SceneManager.UnloadScene(1);
        SceneManager.LoadScene(1,LoadSceneMode.Additive);
        Reset.enabled = true;
        
    }  


}
