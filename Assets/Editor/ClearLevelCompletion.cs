using System.IO;
using UnityEditor;
using UnityEngine;

public class ClearLevelCompletion : EditorWindow
{
    [SerializeField] private LevelDataContainer levelDataContainer;

    [MenuItem("Tools/CaosCreations/Clean Level Completion")]
    private static void ClearCompletion()
    {
        GetWindow<ClearLevelCompletion>();
    }

    private void OnGUI()
    {

        levelDataContainer = (LevelDataContainer)Selection.activeObject;

        GUILayout.Label("Select the level data container and hit Clean");

        if (GUILayout.Button("Clean"))
        {
            foreach(Level level in levelDataContainer.levels)
            {
                level.levelSaveData.isCompleted = false;
                level.levelSaveData.completionTime = 0;
                level.levelSaveData.collectiblesCollected = 0;
                level.levelSaveData.recordedFrames = new RecordedFrame[0];
                level.levelSaveData.ghostRun.playerName = string.Empty;
                EditorUtility.SetDirty(level);
            }

            Directory.Delete(Application.persistentDataPath, true);
        }
    }
}
