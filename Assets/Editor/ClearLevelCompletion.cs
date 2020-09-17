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
                level.isCompleted = false;
                level.completionTime = 0;
                level.ghostRunPositions = new Vector3[0];
                level.ghostRunKeys = new KeysPressed[0];
                
                foreach(Collectible collectible in level.collectibles)
                {
                    collectible.isCollected = false;
                }
            }
        }
    }
}
