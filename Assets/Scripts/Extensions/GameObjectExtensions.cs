using UnityEngine;

public static class GameObjectExtensions
{
    public static bool ToggleActive(this GameObject self)
    {
        self.SetActive(!self.activeSelf);
        return self.activeSelf;
    }

    public static Material SetMaterial(this GameObject self, Material newMaterial)
    {
        return self.GetComponent<MeshRenderer>().material = newMaterial;
    }
}
