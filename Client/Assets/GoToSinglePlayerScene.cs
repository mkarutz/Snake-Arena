using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;

public class GoToSinglePlayerScene : MonoBehaviour
{
    public void LoadSinglePlayerScene()
    {
        SceneManager.LoadScene("singleplayer_vr");
    }
}
