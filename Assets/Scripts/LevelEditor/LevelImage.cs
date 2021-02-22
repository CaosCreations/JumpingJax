using UnityEngine;

[CreateAssetMenu(fileName = "X Image", menuName = "ScriptableObjects/levelImages")]
public class LevelImage : ScriptableObject
{
    public Sprite sprite;
    public Material material;
    public Texture texture; 
    public Color colour;
    public string imageName; 
}
