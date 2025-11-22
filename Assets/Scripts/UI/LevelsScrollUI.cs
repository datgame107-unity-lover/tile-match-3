using DG.Tweening;
using NUnit.Framework;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class LevelsScrollUI : MonoBehaviour
{

    //[SerializeField]
    public Transform levelsContent;
    public GameObject levelCreatePrefab;
    public GameObject tilePrefab;
    public Button newLevelButton;
    //public WarningPanel warningPanel;
    //public List<Button> levelButtons;
    //int maxLevel = 0;
    //int currentLevel;
    private Button selectingButton;
    private int totalLevel;
   

    private void Start()
    {
        totalLevel = LevelDataManager.GetTotalLevel();
        LoadLevel();
       

    }
    private void OnEnable()
    {
        EventManager.OnSavedNewLevel += SavedNewLevelHandler;
        newLevelButton.onClick.AddListener(() =>
        {
            CreateNewLevelButton(totalLevel+1);

            newLevelButton.interactable = false;
        });
      
    }
    private void OnDisable()
    {
        EventManager.OnSavedNewLevel -= SavedNewLevelHandler;

        newLevelButton.onClick.RemoveAllListeners();
    }

    private void SavedNewLevelHandler()
    {
        print("ádasd");
        totalLevel++;
        newLevelButton.interactable = true;
    }
    //private void SaveNewLevelHandler()
    //{
    //    newLevelButton.interactable = true;
    //    LevelManager.Instance.isChanged = false;
    //    maxLevel++;
    //}
    //private void CreateNewLevel()
    //{
    //    print("hhr");
    //    newLevelButton.interactable = false;
    //    if (currentLevel == maxLevel + 1)
    //    {
    //        if (!LevelDataManager.SaveToSO(grid, currentLevel))
    //        {
    //            warningPanel.gameObject.SetActive(true);
    //            warningPanel.gameObject.GetComponentInChildren<TextMeshProUGUI>().text = "Can not save empty level!!!";
    //            return;
    //        }
    //        else
    //        {
    //            warningPanel.gameObject.SetActive(true);
    //        }

    //    }
    //    currentLevel = maxLevel + 1;
    //    GameObject level = Instantiate(levelCreatePrefab, levelsContent);

    //    level.transform.localScale = Vector2.zero;
    //    level.GetComponent<RectTransform>().DOScale(1.2f, 0.3f).SetEase(Ease.InExpo).OnComplete(() =>
    //    {
    //        level.GetComponent<RectTransform>()
    //           .DOScale(1f, 0.7f).SetEase(Ease.OutExpo);

    //    });
    //    level.transform.Find("Container/Center/Level").GetComponent<TextMeshProUGUI>().text = currentLevel.ToString();
    //    level.transform.SetSiblingIndex(1);
    //    levelButtons.Add(level.GetComponent<Button>());
    //    level.GetComponent<Button>().onClick.AddListener(() =>
    //    {
    //        ChangeSelectingButton(level.GetComponent<Button>());
    //    });
    //    ChangeSelectingButton(level.GetComponent<Button>());
    //    LevelManager.Instance.selectingLevel = GetLevel(level.GetComponent<Button>());


    //}

    //private void LoadLevel(int level)
    //{
    //    List<Tile> tiles = grid.GetComponentsInChildren<Tile>().ToList();
    //    if (tiles.Count > 0)
    //    {
    //        foreach (Tile child in tiles)
    //        {
    //            Destroy(child);
    //        }
    //    }

    //    LevelDataManager.LoadFromSO(level, tilePrefab, grid);
    //}
    //private void ChangeSelectingButton(Button button)
    //{
    //    if (selectingButton == null)
    //    {
    //        selectingButton = button;
    //        button.transform.Find("SelectImage").gameObject.SetActive(true);

    //    }
    //    else if (selectingButton == button)
    //    {
    //        print("heh");
    //        if (LevelManager.Instance.isChanged)
    //        {
    //            WarningData data = new WarningData()
    //            {
    //                warningType = WarningType.Delete,
    //                message = "You haven't saved current edit!",
    //                agreeText = "Discard",
    //                refuseText = "Return",

    //                agreeAction = () =>
    //                {
    //                    selectingButton.transform.Find("SelectImage").gameObject.SetActive(false);
    //                    selectingButton = null;

    //                    foreach (Tile child in grid.GetComponentsInChildren<Tile>())
    //                    {
    //                        Destroy(child.gameObject);
    //                    }
    //                    LevelManager.Instance.isChanged = false;
    //                },
    //                refuseAction = () => Debug.Log("Đã HỦY")
    //            };

    //            warningPanel.ShowWarning(data);
    //        }

    //    }
    //    else if (selectingButton != button)
    //    {
    //        if (LevelManager.Instance.isChanged)
    //        {
    //            WarningData data = new WarningData()
    //            {
    //                warningType = WarningType.Delete,
    //                message = "You haven't saved current edit!",
    //                agreeText = "Discard",
    //                refuseText = "Return",

    //                agreeAction = () =>
    //                {
    //                    ApplySelect(button);
    //                    foreach (Tile child in grid.GetComponentsInChildren<Tile>())
    //                    {
    //                        Destroy(child.gameObject);
    //                    }
    //                    LoadLevel(GetLevel(button));
    //                    LevelManager.Instance.isChanged = false;
    //                    LevelManager.Instance.selectingLevel = GetLevel(button);

    //                },
    //                refuseAction = () => Debug.Log("Đã HỦY")
    //            };

    //            warningPanel.ShowWarning(data);
    //        }
    //        else
    //        {
    //            ApplySelect(button);
    //        }


    //    }

    //}


    //void ApplySelect(Button button)
    //{
    //    selectingButton.transform.Find("SelectImage").gameObject.SetActive(false);
    //    selectingButton = button;
    //    button.transform.Find("SelectImage").gameObject.SetActive(true);
    //}



    int GetLevel(Button button)
    {
        TextMeshProUGUI levelText = button.transform.Find("Container/Center/Level")
                                     .GetComponent<TextMeshProUGUI>();

        int levelNumber = int.Parse(levelText.text);
        return levelNumber;
    }
 
    public void LoadLevel()
    {
        for (int i = 1; i <= totalLevel; i++)
        {
            CreateNewLevelButton(i);
        }

    }
    public void CreateNewLevelButton(int level)
    {
        GameObject levelButton = Instantiate(levelCreatePrefab, levelsContent);
        levelButton.transform.SetSiblingIndex(1);
        levelButton.transform.Find("Container/Center/Level").GetComponent<TextMeshProUGUI>().text = level.ToString();
        levelButton.GetComponent<Button>().onClick.AddListener(() =>
        {
            ChangeSelectingLevelButton(levelButton.GetComponent<Button>());

        });
        ChangeSelectingLevelButton(levelButton.GetComponent<Button>());
    }
    public void ChangeSelectingLevelButton(Button button)
    {
        if (selectingButton == null)
        {
            selectingButton = button;
            selectingButton.transform.Find("SelectImage").gameObject.SetActive(true);
            EventManager.OnChoseLevel?.Invoke(GetLevel(button));
        }
        //else if (selectingButton == button)
        //{   
        //    selectingButton = null;
        //    selectingButton.transform.Find("SelectImage").gameObject.SetActive(false);
        //}
        else
        {
            selectingButton.transform.Find("SelectImage").gameObject.SetActive(false);
            selectingButton = button;
            selectingButton.transform.Find("SelectImage").gameObject.SetActive(true);
            EventManager.OnChoseLevel?.Invoke(GetLevel(button));


        }


    }
}
