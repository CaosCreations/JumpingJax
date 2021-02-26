using UnityEditor;
using UnityEngine;

namespace DanR.JJ
{
    public class BulkPrefabCreator : MonoBehaviour
    {
        private static string assetsFolderPath = "Assets";
        private static string outputFolderPath = "BulkPrefabs";

        [MenuItem("Tools/CaosCreations/Create Bulk Prefabs")]
        static void CreateBulkPrefabs()
        {
            string completeOutputPath = assetsFolderPath + "/" + outputFolderPath;

            // If the output folder doesn't exist, create it.
            if (!AssetDatabase.IsValidFolder(completeOutputPath)) AssetDatabase.CreateFolder(assetsFolderPath, outputFolderPath);

            GameObject[] selectedObjects = Selection.gameObjects;
            foreach(GameObject currentObject in selectedObjects)
            {
                // Store transforms.
                Vector3 objectPosition = currentObject.transform.position;
                Quaternion objectRotation = currentObject.transform.rotation;
                Vector3 objectScale = currentObject.transform.localScale;

                // Reset transforms to create prefab.
                currentObject.transform.position = Vector3.zero;
                currentObject.transform.rotation = Quaternion.identity;
                currentObject.transform.localScale = Vector3.one;

                // Create prefab.
                string assetPath = AssetDatabase.GenerateUniqueAssetPath(completeOutputPath + "/" + currentObject.name + ".prefab");
                // If it is already a prefab (as fresh imported objects are), unpack it.
                if (PrefabUtility.IsPartOfAnyPrefab(currentObject)) PrefabUtility.UnpackPrefabInstance(currentObject, PrefabUnpackMode.Completely, InteractionMode.AutomatedAction);
                PrefabUtility.SaveAsPrefabAssetAndConnect(currentObject, assetPath, InteractionMode.AutomatedAction, out bool operationSuccess);

                // Restore transforms.
                currentObject.transform.position = objectPosition;
                currentObject.transform.rotation = objectRotation;
                currentObject.transform.localScale = objectScale;

                // Abort if operation failed.
                if (!operationSuccess)
                {
                    Debug.LogError("Bulk prefab creation failed with: " + currentObject);
                    return;
                }
            }
        }
    }
}