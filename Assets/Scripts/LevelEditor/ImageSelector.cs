using UnityEngine;
using UnityEngine.UI;

public class ImageSelector : MonoBehaviour
{
    public Material[] availableMaterials;

    public Button imageSelectorButton; 
    public Image imageSelectorPreview;
    public GameObject container;
    public GameObject slotContainer; 
    public GameObject scrollViewContent;
    public GameObject slotScrollViewContent;
    public GameObject scrollViewItemPrefab;
    public GameObject SelectedObject { get; set; }

    private void Awake()
    {
        imageSelectorButton.AddOnClick(() => slotContainer.SetActive(true));
        imageSelectorPreview = imageSelectorButton.GetComponent<Image>();
        
        // Populate on start (not in inspector event)
        // Populate once 
    }

    public void PopulateMaterialScrollView(LevelEditorMaterialSlot materialSlot) // rename SVs
    {
        foreach (Material material in availableMaterials)
        {
            GameObject scrollViewItem = Instantiate(scrollViewItemPrefab, scrollViewContent.transform);
            scrollViewItem.name = material.name;
            scrollViewItem.SetText(material.name, isChild: true);
            scrollViewItem.SetMaterial(material, isChild: true); // set image maintexture instead

            scrollViewItem.GetComponentInChildren<Button>().AddOnClick(() => 
            {
                HandleMaterialSelection(material, materialSlot);
            });
        }
    }

    public void PopulateMaterialSlotScrollView()
    {
        MeshRenderer renderer = SelectedObject.GetComponent<MeshRenderer>();
        if (renderer != null)
        {
            foreach (Material material in renderer.materials)
            {
                GameObject scrollViewItem = Instantiate(scrollViewItemPrefab, slotScrollViewContent.transform);
                scrollViewItem.SetText(material.name, isChild: true); 

                LevelEditorMaterialSlot materialSlot = scrollViewItem.AddComponent<LevelEditorMaterialSlot>(); // or separate prefab
                materialSlot.Init(material, scrollViewItem.GetComponent<Image>());
                materialSlot.SetImagePreview(material);

                scrollViewItem.GetComponentInChildren<Button>().AddOnClick(() => 
                { 
                    HandleMaterialSlotSelection(materialSlot); 
                });
            }
        }
    }

    private void HandleMaterialSlotSelection(LevelEditorMaterialSlot materialSlot) // slot own class 
    {
        slotContainer.SetActive(false);
        PopulateMaterialScrollView(materialSlot); //only do this once on startup
    }

    private void HandleMaterialSelection(Material material, LevelEditorMaterialSlot materialSlot) // preview to update
    {
        UpdateMaterial(material);
        materialSlot.SetImagePreview(material);
        container.SetActive(true);
        slotContainer.SetActive(false);
    }

    private void UpdateMaterial(Material material)
    {
        if (SelectedObject != null)
        {
            SelectedObject.SetMaterial(material);
            Debug.Log("New material: " + material.name);
            slotContainer.SetActive(true);
            container.SetActive(false);
        }
    }
}
