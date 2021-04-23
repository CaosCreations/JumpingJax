using UnityEngine.SceneManagement;

public enum FixedScene
{
    MainMenu,
    Credits,
    LevelEditor
}

public class SceneUtils
{
    public static int GetFixedSceneIndex(FixedScene fixedScene)
    {
        int subtrahend;
        switch (fixedScene)
        {
            case FixedScene.MainMenu:
                return PlayerConstants.MainMenuSceneIndex;
            case FixedScene.LevelEditor:
                subtrahend = PlayerConstants.LevelEditorSceneIndexSubtrahend;
                break;
            case FixedScene.Credits:
                subtrahend = PlayerConstants.CreditsSceneIndexSubtrahend;
                break;
            default:
                return default;
        }

        return SceneManager.sceneCountInBuildSettings - subtrahend;
    }

    public static bool SceneUsesReplay(int buildIndex)
    {
        return buildIndex != PlayerConstants.MainMenuSceneIndex
            && buildIndex != GetFixedSceneIndex(FixedScene.Credits);
    }

    public static bool SceneIsLevel(int buildIndex)
    {
        // Level editor is the cut-off point
        int levelEditorSceneIndex = GetFixedSceneIndex(FixedScene.LevelEditor);

        return buildIndex > 0 && buildIndex < levelEditorSceneIndex;
    }
}
