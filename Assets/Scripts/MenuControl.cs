using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuControl : MonoBehaviour
{
    public void ReloadApp()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
}
