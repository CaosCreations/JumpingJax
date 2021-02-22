using System.Collections;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ImageSelector : MonoBehaviour, IPointerClickHandler
{
    public LevelImage[] levelImages; 
    public Image imagePreview;
    public Material materialPreview; 
    public GameObject imageSelectionContainer;
    public GameObject scrollViewContent;
    public GameObject imageToSelectPrefab;

    private void ToggleImageSelectionView()
    {
        imageSelectionContainer.ToggleActive();
    }

    private void PopulateScrollView()
    {

    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void UpdateImagePreview(GameObject objectToUpdate)
    {
        // convert material to image (UI)

        //imagePreview.sprite = objectToUpdate.GetComponent<MeshRenderer>().material;
    }
    
    private void SelectImage(GameObject selectedObject, Material newMaterial)
    {
        selectedObject.SetMaterial(newMaterial);
        Debug.Log("New material: " + newMaterial.name);
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (eventData.button == PointerEventData.InputButton.Left)
        {
            //SelectImage(selectedObject, newMaterial);
            ToggleImageSelectionView();
            
        }

    }
}
