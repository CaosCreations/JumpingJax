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
    //private Dictionary<Material, Button> slotScrollViewItemButtons; // make 2d?
    private GameObject[] slotSelectors = new GameObject[] { };

    private Material selectedMaterial; // materialToUpdate
    private Material[] selectedObjectMaterialSlots; // change to GO?
    //private GameObject selectedObject; // objectToUpdate

    //public LevelImage[] LevelImages { get => levelImageContainer.levelImages; }
    public Material[] materialsToChooseFrom;
    //public List<Image> materialPreviews;
    public List<Material> materialPreviews;
    public GameObject SelectedObject { get; set; }

    // Todo: support mesh renderers with multiple materials on them 
    // New class for previews? 

    private void Awake()
    {
        imageSelectorButton.AddOnClick(() => slotContainer.ToggleActive());
        imagePreview = imageSelectorButton.GetComponent<Image>();
        scrollViewItemButtons = new Dictionary<LevelImage, Button>();
    }

    public void PopulateScrollView(Image materialPreview) // mat replacement sv 
    {
        //foreach (LevelImage levelImage in LevelImages)
        //{
        //    GameObject scrollViewItem = Instantiate(imageToSelectPrefab, scrollViewContent.transform);
        //    scrollViewItem.name = levelImage.imageName;
        //    scrollViewItem.SetText(levelImage.imageName, isChild: true);
        //    scrollViewItem.SetSprite(levelImage.previewSprite, isChild: true);
        //    scrollViewItem.GetComponent<Button>().AddOnClick(() => )
        //    //scrollViewItemButtons.Add(key: levelImage, value: scrollViewItem.GetComponentInChildren<Button>());

        //}

        foreach (Material material in materialsToChooseFrom)
        {
            GameObject scrollViewItem = Instantiate(imageToSelectPrefab, scrollViewContent.transform);
            scrollViewItem.name = material.name;
            scrollViewItem.SetText(material.name, isChild: true);
            scrollViewItem.SetMaterial(material, isChild: true); // preview
            scrollViewItem.GetComponentInChildren<Button>().AddOnClick(() => { UpdateMaterial(material); materialPreview.material = material; });
            //scrollViewItemButtons.Add(key: levelImage, value: scrollViewItem.GetComponentInChildren<Button>());

        }
    }

    public void UpdateImagePreview() // diff from materialpreviews (individual) - rename 
    {
        if (SelectedObject != null)
        {
            Material firstMaterial = SelectedObject.GetComponent<MeshRenderer>().materials.FirstOrDefault(x => x != null);
            if (firstMaterial != null)
            {
                Debug.Log("First material: " + firstMaterial);
                imagePreview.material = firstMaterial; 
            }
        }
    }

    public void PopulateSlotScrollView() // mat select sv
    {
        MeshRenderer renderer = SelectedObject.GetComponent<MeshRenderer>();
        if (renderer != null)
        {
            foreach (Material material in renderer.materials)
            {
                // Find the matching preview for that material 
                //LevelImage preview = Array.Find(LevelImages, x => x.material == material); // or compare name 
                GameObject scrollViewItem = Instantiate(imageToSelectPrefab, slotScrollViewContent.transform);
                scrollViewItem.SetText(material.name, isChild: true); // stripdown in the other SV init method?
                //scrollViewItem.SetMaterial(material);
                Image image = scrollViewItem.GetComponentInChildren<Image>(); // .ol; rename speci.
                image.material = material; //ncap
                //materialPreviews.Add(image);
                materialPreviews.Add(material);
                scrollViewItem.GetComponentInChildren<Button>().AddOnClick(() => { selectedMaterial = material; container.ToggleActive(); slotContainer.ToggleActive(); PopulateScrollView(image); });
                //scrollViewItem.AddComponent<MeshRenderer>().material = material;
                

                // either use MR + material for previews, then change tile config OR use preview screenshots 
                // keep levelimage separate SO to preview?
            }
        }
    }


    private void SelectMaterialToReplace(Material material) //rename 
    {
        selectedMaterial = material;
        container.ToggleActive();
    }

    //private Material GetMatchingMaterial(GameObject selectedObject)
    //{
    //    //return selectedObject.GetComponent<MeshRenderer>().materials.FirstOrDefault(x => x == )
    //}

    // separate class for opener?
    private void InitScrollViewItem(GameObject parent, LevelImage levelImage, UnityAction callback)
    {

    }

    //private LevelImage[] GetMaterialSlotPreviews(Material material) //rename 
    //{
    //    // just image 
    //    return Array.FindAll(LevelImages, x => x.material == material);
    //}

    public void AddListeners(GameObject selectedObject)
    {
        foreach (KeyValuePair<LevelImage, Button> kvp in scrollViewItemButtons)
        {
            kvp.Value.AddOnClick(() => SelectImage(selectedObject, kvp.Key));
        }
    }

    // make generic or use enum 
    public void AddListeners_<T>(GameObject selectedObject, Dictionary<T, Button> buttonMappings, UnityAction callback)
    {
        //Type genericType = typeof(T);
        //if (genericType is LevelImage)
        //{

        //}

        //if (T == typeof(LevelImage))
        //{

        //}


        foreach (KeyValuePair<T, Button> kvp in buttonMappings)
        {
            kvp.Value.AddOnClick(() => callback()); // generic cb
        }
    }

    private void SelectImage(GameObject selectedObject, LevelImage levelImage)
    {
        if (selectedObject != null)
        {
            selectedObject.SetMaterial(levelImage.material);
            Debug.Log("New material: " + levelImage.material.name);

            imagePreview.sprite = levelImage.previewSprite;

            Material slot = Array.Find(selectedObjectMaterialSlots, x => x.name == levelImage.material.name);
            slot = levelImage.material;
            slotScrollViewContent.ToggleActive();

        }
    }

    private void UpdateMaterial(Material material)
    {
        if (SelectedObject != null)
        {
            SelectedObject.SetMaterial(material);
            Debug.Log("New material: " + material.name);
            slotContainer.ToggleActive();
            container.ToggleActive();
        }
    }
}
