using System.Linq;
using UnityEngine;

public class LevelEditorUtils : MonoBehaviour
{
    public static LevelEditorObject[] GetObjectsByType(ObjectType type)
    {
        return FindObjectsOfType<LevelEditorObject>()
            .Where(x => x.objectType == type)
            .ToArray();
    }
}
