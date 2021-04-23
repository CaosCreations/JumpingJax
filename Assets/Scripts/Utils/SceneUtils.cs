using UnityEngine.SceneManagement;

/// <summary>
/// The scenes that will always be in the same position in the build settings 
/// relative to the others, no matter how many scenes are there.
/// </summary>
public enum FixedScene
{
    MainMenu,
    Credits,
    LevelEditor
}

public class SceneUtils
{
    /// <summary>
    /// Get the build index of a fixed scene by subtracting a pre-configured value 
    /// from the last index in the build settings.
    /// </summary>
    /// <param name="fixedScene"></param>
    public static int GetFixedSceneIndex(FixedScene fixedScene)
    {
        int subtrahend;
        switch (fixedScene)
        {
            case FixedScene.LevelEditor:
                subtrahend = PlayerConstants.LevelEditorSceneIndexSubtrahend;
                break;
            case FixedScene.Credits:
                subtrahend = PlayerConstants.CreditsSceneIndexSubtrahend;
                break;
            case FixedScene.MainMenu:
                return PlayerConstants.MainMenuSceneIndex;
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

    public static bool SceneIsFinalLevel(Level currentLevel)
    {
        return currentLevel.levelBuildIndex >= GameManager.Instance.levelDataContainer.levels.Length;
    }
}
