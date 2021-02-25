using UnityEngine;
using UnityEngine.UI;

public class LevelEditorMaterialSlot : MonoBehaviour
{
    public Material material;
    public Image imagePreview;

    public void Init(Material material, Image imagePreview)
    {
        this.material = material;
        this.imagePreview = imagePreview;
    }

    public void SetImagePreview(Material material)
    {
        imagePreview.material.mainTexture = material.mainTexture;

        // Or just set the color of the albedo 
    }

    // Handle material setting here too?
}