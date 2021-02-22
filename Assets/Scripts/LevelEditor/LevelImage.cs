using UnityEngine;

[CreateAssetMenu(fileName = "X Image", menuName = "ScriptableObjects/levelImages")]
public class LevelImage : ScriptableObject
{
    public Sprite sprite;
    public Texture texture; 
    public Color colour;
    public string imageName; 
}
