using UnityEngine;

public class GameManagerQuit : MonoBehaviour
{
    void Update()
    {
        // Check if the ESC key is pressed
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            QuitGame();
        }
    }

    void QuitGame()
    {
        // If we are running in the editor
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        // If we are running in a build
        Application.Quit();
#endif
    }
}
