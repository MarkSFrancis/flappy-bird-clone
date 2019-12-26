using UnityEngine.SceneManagement;

public static class SceneLoader
{
    private static Scene _requestedScene;

    public static void RequestScene(Scene scene)
    {
        _requestedScene = scene;
        SceneManager.LoadScene(Scene.Loading.ToString());
    }

    public static void StartLoading()
    {
        SceneManager.LoadScene(_requestedScene.ToString());
    }
}
