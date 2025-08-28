using System.Linq;
using UnityEngine;

public class LevelEditorUtils : MonoBehaviour
{
    public static LevelEditorObject[] GetObjectsByType(ObjectType type)
    {
        return FindObjectsByType<LevelEditorObject>(FindObjectsSortMode.InstanceID)
            .Where(x => x.objectType == type)
            .ToArray();
    }

    public static Checkpoint GetFirstCheckpoint()
    {
        return FindObjectsByType<Checkpoint>(FindObjectsSortMode.InstanceID).FirstOrDefault(x => x.isFirstCheckpoint);
    }
}
