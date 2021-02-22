using UnityEngine;

[CreateAssetMenu(fileName = "LevelEditorImage", menuName = "ScriptableObjects/levelImages")]
public class LevelImage : ScriptableObject
{
    public string imageName;
    public Sprite previewSprite;
    public Material material;
}
