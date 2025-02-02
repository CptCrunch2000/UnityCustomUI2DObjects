using UnityEngine;
using UnityEngine.SceneManagement;

public class CloseApp : MonoBehaviour
{
    public void Close()
    {
        SceneManager.LoadScene(0);
    }

    private void Update()
    {
        if(Input.GetKeyDown(KeyCode.Escape)) { SceneManager.LoadScene(0); }
    }
}
