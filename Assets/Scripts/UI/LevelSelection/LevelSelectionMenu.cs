using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

public enum LevelSelectionTab
{
    Hop, Portal, Workshop
}

public class LevelSelectionMenu : MonoBehaviour
{
    public Transform levelButtonParent;
    public GameObject levelCardPrefab;
    public LevelPreview levelPreview;

    public TabButton hopTabButton;
    public TabButton portalTabButton;
    public TabButton workshopTabButton;

    public List<LevelSelectionCard> hopCardList;
    public List<LevelSelectionCard> portalCardList;
    public List<LevelSelectionCard> workshopCardList;

    public Button backToMenuButton;
    public MainMenuController mainMenuController;

    public LevelSelectionTab currentTab;

    public Text groupTimeText;
    public Text totalTimeText;

    void Start()
    {
        mainMenuController = GetComponentInParent<MainMenuController>();
        backToMenuButton.onClick.RemoveAllListeners();
        backToMenuButton.onClick.AddListener(mainMenuController.Init);

        levelPreview = GetComponentInChildren<LevelPreview>();
        levelPreview.gameObject.SetActive(false);

        SetupTabButtons();
        LoadCards(GameManager.Instance.levelDataContainer.levels);
        SetTab(LevelSelectionTab.Hop);
        LoadWorkshopCards();
    }

    void SetupTabButtons()
    {
        hopTabButton.Init("Hop", () => SetTab(LevelSelectionTab.Hop));
        portalTabButton.Init("Portal", () => SetTab(LevelSelectionTab.Portal));
        workshopTabButton.Init("Workshop", () => SetTab(LevelSelectionTab.Workshop));
    }

    void SetTab(LevelSelectionTab tab)
    {
        hopCardList.ForEach(x => x.gameObject.SetActive(tab == LevelSelectionTab.Hop));
        portalCardList.ForEach(x => x.gameObject.SetActive(tab == LevelSelectionTab.Portal));
        workshopCardList.ForEach(x => x.gameObject.SetActive(tab == LevelSelectionTab.Workshop));

        totalTimeText.text = $"Total Time: {GameManager.Instance.levelDataContainer.GetTotalTime(TotalTimeType.All)}";
        switch (tab)
        {
            case LevelSelectionTab.Hop:
                hopTabButton.SelectTab();
                portalTabButton.UnselectTab();
                workshopTabButton.UnselectTab();
                groupTimeText.text = $"Hop Total Time: {GameManager.Instance.levelDataContainer.GetTotalTime(TotalTimeType.Hop)}";
                break;
            case LevelSelectionTab.Portal:
                hopTabButton.UnselectTab();
                portalTabButton.SelectTab();
                workshopTabButton.UnselectTab();
                groupTimeText.text = $"Portal Total Time: {GameManager.Instance.levelDataContainer.GetTotalTime(TotalTimeType.Portal)}";
                break;
            case LevelSelectionTab.Workshop:
                hopTabButton.UnselectTab();
                portalTabButton.UnselectTab();
                workshopTabButton.SelectTab();
                groupTimeText.text = "";
                break;
        }

        currentTab = tab;
    }
    
    void LoadCards(Level[] levels)
    {
        for (int i = 0; i < levels.Length; i++)
        {
            GameObject newLevelCard = Instantiate(levelCardPrefab, levelButtonParent);
            LevelSelectionCard levelCard = newLevelCard.GetComponentInChildren<LevelSelectionCard>();

            Level currentLevel = levels[i];
            levelCard.Init(currentLevel);
            levelCard.selectButton.onClick.AddListener(() => OnClickLevel(currentLevel));

            if (levelCard.tab == LevelSelectionTab.Hop)
            {
                hopCardList.Add(levelCard);
            }
            else if(levelCard.tab == LevelSelectionTab.Portal)
            {
                portalCardList.Add(levelCard);
            }
            else
            {
                workshopCardList.Add(levelCard);
            }
        }
    }

    private async void LoadWorkshopCards()
    {
        List<Level> levels = await GetWorkshopLevels();
        LoadCards(levels.ToArray());
        // Re-toggle the correct buttons once the levels have loaded from Steam
        SetTab(currentTab);
    }

    private async Task<List<Level>> GetWorkshopLevels()
    {
        List<Steamworks.Ugc.Item> items = await WorkshopManager.DownloadAllSubscribedItems();
        List<Level> toReturn = new List<Level>();
        if (items != null && items.Count > 0)
        {
            foreach(Steamworks.Ugc.Item item in items)
            {
                Level newLevel = ScriptableObject.CreateInstance<Level>();
                newLevel.InitFromWorkshopItem(item);
                toReturn.Add(newLevel);
            }
        }

        return toReturn;
    }

    public void OnClickLevel(Level level)
    {
        levelPreview.gameObject.SetActive(true);
        levelPreview.Init(level);
    }
}
