using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneLoader : MonoBehaviour
{
    private static string sceneToLoad;

    public static string SceneToLoad { get => sceneToLoad; }

    //Load
    public static void Load(string sceneName)
    {
        SceneManager.LoadScene(sceneName);
    }
    //Progress Loading
    public static void ProgressLoad(string sceneName)
    {
        sceneToLoad = sceneName;
        // SceneManager.LoadScene("LoadingProgress");
    }
    //ReloadLevel
    public static void ReloadLevel()
    {
        var currentScene = SceneManager.GetActiveScene().name;
        ProgressLoad(currentScene);
    }
    //LoadNextLevel
    public static void LoadNextLevel()
    {
        var currentSceneName = SceneManager.GetActiveScene().name;
        int nextLevel = int.Parse(currentSceneName.Split("Level")[1]) + 1;
        string nextSceneName = "Level" + nextLevel;

        if (SceneUtility.GetBuildIndexByScenePath(nextSceneName) == -1)
        {
            Debug.LogError(nextSceneName + "does not exists");
            return;
        }

        ProgressLoad(nextSceneName);
    }
}