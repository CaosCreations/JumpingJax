using UnityEngine;
using UnityEngine.UI;

public static class GameObjectExtensions
{
    public static bool ToggleActive(this GameObject self)
    {
        self.SetActive(!self.activeSelf);
        return self.activeSelf;
    }

    #region Set Component Values
    public static GameObject SetText(this GameObject self, string value, bool isChild = false)
    {
        Text text = isChild ? self.GetComponentInChildren<Text>() : self.GetComponent<Text>();
        if (text != null)
        {
            text.text = value;
        }
        return self;
    }

    public static GameObject SetSprite(this GameObject self, Sprite value, bool isChild = false)
    {
        Image image = isChild ? self.GetComponentInChildren<Image>() : self.GetComponent<Image>();
        if (image != null)
        {
            image.sprite = value;
        }
        return self;
    }

    public static GameObject SetMaterial(this GameObject self, Material value, bool isChild = false)
    {
        MeshRenderer renderer = isChild ? self.GetComponentInChildren<MeshRenderer>() : self.GetComponent<MeshRenderer>();
        if (renderer != null)
        {
            self.GetComponent<MeshRenderer>().material = value;
        }
        return self;
    }

    //public static GameObject AddComponents(this GameObject self, params Component[] values)
    //{ 
    //    foreach (Component component in values)
    //    {
    //        if (component != null)
    //        {
    //            self.AddComponent<component>();
    //        }
    //    }
    //    return self;
    //}

    #endregion
}
