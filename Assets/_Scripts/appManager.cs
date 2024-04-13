using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneController : MonoBehaviour
{
    // Function to change scene by name
    public void ChangeScene(string sceneName)
    {
        SceneManager.LoadScene(sceneName);
    }

    // Function to change scene by index
    public void ChangeScene(int sceneIndex)
    {
        SceneManager.LoadScene(sceneIndex);
    }

    // Function to load next scene in build settings
    public void loadAdmin()
    {
        SceneManager.LoadScene("admin");
    }
    public void loadUser()
    {
        SceneManager.LoadScene("user");
    }

    // Function to load previous scene in build settings
    public void loadHome()
    {
        SceneManager.LoadScene("mainMenu");
    }
}
