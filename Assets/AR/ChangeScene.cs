using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ChangeScene : MonoBehaviour
{
    // Change to scene
    public string sceneName;
    public void ChangeToScene()
    {
        SceneManager.LoadScene(sceneName);
    }
}
