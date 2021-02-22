using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ImageSelector : MonoBehaviour
{
    public Image imagePreview;
    public GameObject imageSelectionContainer; 

    private void ToggleImageSelectionView()
    {
        imageSelectionContainer.SetActive(!imageSelectionContainer.activeSelf);
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
