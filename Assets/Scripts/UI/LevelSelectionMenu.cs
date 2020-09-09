using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public enum LevelSelectionTab
{
    Hop, Portal, Workshop
}

public class LevelSelectionMenu : MonoBehaviour
{
    public Transform levelButtonParent;
    public GameObject levelObjectPrefab;
    public LevelPreview levelPreview;

    public Button hopTabButton;
    public Button portalTabButton;
    public Button workshopTabButton;

    public List<LevelButton> hopButtonList;
    public List<LevelButton> portalButtonList;
    public List<LevelButton> workshopButtonList;

    void Start()
    {
        levelPreview = GetComponentInChildren<LevelPreview>();
        SetupTabButtons();
    }

    private void OnEnable()
    {
        LoadButtons();
        SetTab(LevelSelectionTab.Hop);
    }

    private void OnDisable()
    {
        ClearButtons();
    }

    void ClearButtons()
    {
        hopButtonList.ForEach(x => Destroy(x.gameObject));
        hopButtonList = new List<LevelButton>();

        portalButtonList.ForEach(x => Destroy(x.gameObject));
        portalButtonList = new List<LevelButton>();

        workshopButtonList.ForEach(x => Destroy(x.gameObject));
        workshopButtonList = new List<LevelButton>();
    }

    void SetupTabButtons()
    {
        hopTabButton.onClick.RemoveAllListeners();
        hopTabButton.onClick.AddListener(() => SetTab(LevelSelectionTab.Hop));

        portalTabButton.onClick.RemoveAllListeners();
        portalTabButton.onClick.AddListener(() => SetTab(LevelSelectionTab.Portal));

        workshopTabButton.onClick.RemoveAllListeners();
        workshopTabButton.onClick.AddListener(() => SetTab(LevelSelectionTab.Workshop));
    }

    void SetTab(LevelSelectionTab tab)
    {
        Debug.Log("set level tab");
        hopButtonList.ForEach(x => x.gameObject.SetActive(tab == LevelSelectionTab.Hop));
        portalButtonList.ForEach(x => x.gameObject.SetActive(tab == LevelSelectionTab.Portal));
        workshopButtonList.ForEach(x => x.gameObject.SetActive(tab == LevelSelectionTab.Workshop));
    }
    
    void LoadButtons()
    {
        Level[] levels = GameManager.Instance.levelDataContainer.levels;
        for (int i = 0; i <= levels.Length - 1; i++)
        {
            GameObject newLevelButton = Instantiate(levelObjectPrefab, levelButtonParent);
            LevelButton levelButton = newLevelButton.GetComponentInChildren<LevelButton>();
            levelButton.SetupButton(levels[i]);

            if (levelButton.tab == LevelSelectionTab.Hop)
            {
                hopButtonList.Add(levelButton);
            }
            else if(levelButton.tab == LevelSelectionTab.Portal)
            {
                portalButtonList.Add(levelButton);
            }
            else
            {
                workshopButtonList.Add(levelButton);
            }
        }
    }
}
