using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class ImageSelector : MonoBehaviour
{
    public LevelImageContainer levelImageContainer;
    public Button imageSelectorButton; 
    public Image imagePreview;
    public GameObject container;
    public GameObject slotContainer; 
    public GameObject scrollViewContent;
    public GameObject slotScrollViewContent;
    public GameObject imageToSelectPrefab;

    private Dictionary<LevelImage, Button> scrollViewItemButtons;
    //private Dictionary<LevelImage, Button> slotScrollViewItemButtons; // make 2d?

    private Material selectedMaterial; 

    public LevelImage[] LevelImages { get => levelImageContainer.levelImages; }

    // Todo: support mesh renderers with multiple materials on them 
    // New class for previews? 

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
            scrollViewItemButtons.Add(key: levelImage, value: scrollViewItem.GetComponentInChildren<Button>());
        }
    }

    public void PopulateSlotScrollView(GameObject selectedObject)
    {
        MeshRenderer renderer = selectedObject.GetComponent<MeshRenderer>();
        if (renderer != null)
        {
            foreach (Material material in renderer.materials)
            {
                // Find the matching preview for that material 
                LevelImage preview = Array.Find(LevelImages, x => x.material == material);
                GameObject scrollViewItem = Instantiate(imageToSelectPrefab, slotScrollViewContent.transform);
                scrollViewItem.SetText(material.name, isChild: true); // stripdown in the other SV init method?
                scrollViewItem.SetMaterial(material);
                scrollViewItem.GetComponentInChildren<Button>().AddOnClick(() => ChooseMaterial(material));


                // keep levelimage separate SO to preview?
            }
        }
    }

    private void ChooseMaterial(Material material) //rename 
    {
        selectedMaterial = material;
        slotScrollViewContent.ToggleActive();
    }

    private Material GetMatchingMaterial(GameObject selectedObject)
    {
        //return selectedObject.GetComponent<MeshRenderer>().materials.FirstOrDefault(x => x == )
    }

    // separate class for opener?
    private void InitScrollViewItem(GameObject parent, LevelImage levelImage, UnityAction callback)
    {

    }

    private LevelImage[] GetMaterialSlotPreviews(Material material) //rename 
    {
        // just image 
        return Array.FindAll(LevelImages, x => x.material == material);
    }

    public void AddListeners(GameObject selectedObject)
    {
        foreach (KeyValuePair<LevelImage, Button> kvp in scrollViewItemButtons)
        {
            kvp.Value.AddOnClick(() => SelectImage(selectedObject, kvp.Key)); // swap k and v
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
