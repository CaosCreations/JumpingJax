using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
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

    private async void OnEnable()
    {
        Debug.Log("enable");
        ClearButtons();
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
        hopButtonList.ForEach(x => x.gameObject.SetActive(tab == LevelSelectionTab.Hop));
        portalButtonList.ForEach(x => x.gameObject.SetActive(tab == LevelSelectionTab.Portal));
        workshopButtonList.ForEach(x => x.gameObject.SetActive(tab == LevelSelectionTab.Workshop));
    }
    
    async void LoadButtons()
    {
        Level[] levels = await GetAllLevels();
        for (int i = 0; i < levels.Length; i++)
        {
            GameObject newLevelButton = Instantiate(levelObjectPrefab, levelButtonParent);
            LevelButton levelButton = newLevelButton.GetComponentInChildren<LevelButton>();

            Level currentLevel = levels[i];
            levelButton.SetupButton(currentLevel);
            levelButton.button.onClick.AddListener(() => OnClickLevel(currentLevel));

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

    private async Task<Level[]> GetAllLevels()
    {
        List<Level> levels = GameManager.Instance.levelDataContainer.levels.ToList();
        if (GameManager.Instance.isSteamActive)
        {
            levels.AddRange( await GetWorkshopLevels());
        }

        return levels.ToArray();
    }

    private async Task<List<Level>> GetWorkshopLevels()
    {
        List<Steamworks.Ugc.Item> items = await WorkshopManager.DownloadAllSubscribedItems();
        List<Level> toReturn = new List<Level>();
        if (items != null && items.Count > 0)
        {
            foreach(Steamworks.Ugc.Item item in items)
            {
                Level newLevel = new Level();
                newLevel.levelBuildIndex = GameManager.workshopLevelIndex;
                newLevel.filePath = item.Directory;
                newLevel.levelName = item.Title;
                if(item.PreviewImageUrl != null && item.PreviewImageUrl != string.Empty)
                {
                    Texture2D texture = await LoadTextureFromUrl(item.PreviewImageUrl);
                    newLevel.previewSprite = Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), new Vector2(0.5f, 0.5f));
                }
                toReturn.Add(newLevel);
            }
        }

        return toReturn;
    }

    public async Task<Texture2D> LoadTextureFromUrl(string url)
    {
        //
        // If you're going to use this properly in production
        // you need to think about caching the texture maybe
        // so you don't download it every time.
        //

        UnityWebRequest request = UnityWebRequestTexture.GetTexture(url, true);

        var r = request.SendWebRequest();

        while (!r.isDone)
        {
            await Task.Delay(10);
        }

        if (request.isNetworkError || request.isHttpError)
            return new Texture2D(100, 100);

        DownloadHandlerTexture dh = request.downloadHandler as DownloadHandlerTexture;
        dh.texture.name = url;
        return dh.texture;
    }

    public void OnClickLevel(Level level)
    {
        levelPreview.Init(level);
    }
}
