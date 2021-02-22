using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ImageSelector : MonoBehaviour
{
    public LevelImageContainer levelImageContainer;
    public Button imageSelectorButton; 
    public Image imagePreview;
    public GameObject container;
    public GameObject scrollViewContent;
    public GameObject imageToSelectPrefab;

    private Dictionary<LevelImage, Button> scrollViewItemButtons;
    public LevelImage[] LevelImages { get => levelImageContainer.levelImages; }

    // Todo: support mesh renderers with multiple materials on them 

    private void Awake()
    {
        imageSelectorButton.AddOnClick(() => container.ToggleActive());
        imagePreview = imageSelectorButton.GetComponent<Image>();
        scrollViewItemButtons = new Dictionary<LevelImage, Button>();
    }

    public void PopulateScrollView()
    {
        foreach (LevelImage levelImage in LevelImages)
        {
            GameObject scrollViewItem = Instantiate(imageToSelectPrefab, scrollViewContent.transform);
            scrollViewItem.name = levelImage.imageName;
            scrollViewItem.SetText(levelImage.imageName, isChild: true);
            scrollViewItem.SetSprite(levelImage.previewSprite, isChild: true);
            scrollViewItemButtons.Add(levelImage, scrollViewItem.GetComponentInChildren<Button>());
        }
    }

    public void AddListeners(GameObject objectToInspect)
    {
        foreach (KeyValuePair<LevelImage, Button> kvp in scrollViewItemButtons)
        {
            kvp.Value.AddOnClick(() => SelectImage(objectToInspect, kvp.Key)); // swap k and v
        }
    }

    private void SelectImage(GameObject selectedObject, LevelImage levelImage)
    {
        if (selectedObject != null)
        {
            selectedObject.SetMaterial(levelImage.material);
            Debug.Log("New material: " + levelImage.material.name);

            imagePreview.sprite = levelImage.previewSprite;

        }
    }
}
