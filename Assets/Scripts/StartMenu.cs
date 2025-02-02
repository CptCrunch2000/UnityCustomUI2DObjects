using UnityEngine;
using UnityEngine.SceneManagement;

public class StartMenu : MonoBehaviour
{
    public void StartPieChart()
    {
        SceneManager.LoadScene(1);
    }

    public void StartFancyText()
    {
        SceneManager.LoadScene(2);
    }

    public void StartFpsCounter()
    {
        SceneManager.LoadScene(3);
    }

    public void Exit()
    {
        Application.Quit();
    }
}
