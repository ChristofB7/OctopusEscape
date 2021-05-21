using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class LevelSelector : MonoBehaviour
{
    public GameObject levelHolder; // Panel 
    public GameObject levelIcon; // Size of each level icon
    public GameObject thisCanvas;
    private int numberOfLevels = 11;
    public Vector2 iconSpacing;

    private Rect iconDimensions;
    private Rect panelDimensions;
    private int amountPerPage;
    private int currentLevelCount;
    // Start is called before the first frame update

    void Start()
    {
        PlayerPrefs.SetInt("maxLevel", 11);
        panelDimensions = levelHolder.GetComponent<RectTransform>().rect;
        iconDimensions = levelIcon.GetComponent<RectTransform>().rect;
        int maxInARow = Mathf.FloorToInt((panelDimensions.width + iconSpacing.x) / (iconDimensions.width + iconSpacing.x));
        int maxInACol = Mathf.FloorToInt((panelDimensions.height + iconSpacing.y) / (iconDimensions.height + iconSpacing.y));
        amountPerPage = maxInARow * maxInACol;
        int totalPages = Mathf.CeilToInt((float)numberOfLevels / amountPerPage);
        LoadPanels(totalPages);
    }
    void LoadPanels(int numberOfPanels)
    {
        GameObject panelClone = Instantiate(levelHolder) as GameObject;
        PageSwiper swiper = levelHolder.AddComponent<PageSwiper>();
        swiper.totalPages = numberOfPanels;

        for (int i = 1; i <= numberOfPanels; i++)
        {
            GameObject panel = Instantiate(panelClone) as GameObject;
            panel.transform.SetParent(thisCanvas.transform, false);
            panel.transform.SetParent(levelHolder.transform);
            panel.name = "Page-" + i;
            panel.GetComponent<RectTransform>().localPosition = new Vector2(panelDimensions.width * (i - 1), 0);
            SetUpGrid(panel);
            //if i is the same as the last page (we are populating the last page
            //then set number of icons to levels - current level count otherwise
            //the value of number of icons is amount per page
            int numberOfIcons = i == numberOfPanels ? numberOfLevels - currentLevelCount : amountPerPage;
            LoadIcons(numberOfIcons, panel);
        }
        Destroy(panelClone);
    }
    void SetUpGrid(GameObject panel)
    {
        GridLayoutGroup grid = panel.AddComponent<GridLayoutGroup>();
        grid.cellSize = new Vector2(iconDimensions.width, iconDimensions.height);
        grid.childAlignment = TextAnchor.MiddleCenter;
        grid.spacing = iconSpacing;
    }
    void LoadIcons(int numberOfIcons, GameObject parentObject)
    {
        for (int i = 1; i <= numberOfIcons; i++)
        {
            currentLevelCount++;
            GameObject icon = Instantiate(levelIcon) as GameObject;
            icon.transform.SetParent(thisCanvas.transform, false);
            icon.transform.SetParent(parentObject.transform);
            icon.name = i.ToString();
            icon.GetComponentInChildren<TextMeshProUGUI>().SetText(currentLevelCount.ToString());
            if(i> PlayerPrefs.GetInt("levelAt"))
            {
                icon.GetComponentInChildren<TextMeshProUGUI>().SetText("");
                Button button = icon.GetComponentInChildren<Button>();
                button.interactable = false;
                Debug.Log(icon.GetComponentInChildren<Button>().interactable);
                button.GetComponent<Image>().sprite = button.spriteState.disabledSprite;
            }

        }
    }
    // Update is called once per frame


}
