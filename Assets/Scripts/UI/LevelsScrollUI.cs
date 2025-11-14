using DG.Tweening;
using NUnit.Framework;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class LevelsScrollUI : MonoBehaviour
{

    [SerializeField]
    Transform levelsContent;
    [SerializeField]
    GameObject levelCreatePrefab;
    [SerializeField] Transform grid;
    [SerializeField]
    Button newLevelButton;
    public Image warningImage;
    public List<Button> levelButtons;
    int maxLevel = 0;
    int currentLevel;
    private Button selectingButton;
    private void Start()
    {
        string folderPath = "Assets/Levels";
        if (!Directory.Exists(folderPath))
        {
            return;
        }

        string[] files = Directory.GetFiles(folderPath, "Level_*.asset");

        if (files.Length == 0)
        {
            return;
        }

        var levelNumbers = files
            .Select(f => Path.GetFileNameWithoutExtension(f))
            .Select(name =>
            {
                string[] parts = name.Split('_');
                if (parts.Length > 1 && int.TryParse(parts[1], out int n))
                    return n;
                return 0;
            })
            .ToList();

        maxLevel = levelNumbers.Max();
        for (int i = maxLevel; i > 0; i--)
        {
            GameObject level = Instantiate(levelCreatePrefab, levelsContent);
            level.transform.Find("Container/Center/Level").GetComponent<TextMeshProUGUI>().text = i.ToString();
            levelButtons.Add(level.GetComponent<Button>());
        }
        foreach (Button button in levelButtons)
        {
            button.onClick.AddListener(() =>
            {   
                ChangeSelectingButton(button);
                TextMeshProUGUI levelText = button.transform.Find("Container/Center/Level")
                                      .GetComponent<TextMeshProUGUI>();

                int levelNumber = int.Parse(levelText.text); // Nếu chắc chắn text là số
                LoadLevel(levelNumber);
            });
        }

        newLevelButton.onClick.AddListener(() =>
        {       
            CreateNewLevel();
        });
    }

    private void CreateNewLevel()
    {
        newLevelButton.interactable = false;
        if (currentLevel == maxLevel + 1)
        {
            if (!LevelDataManager.SaveToSO(grid, currentLevel))
            {
                warningImage.gameObject.SetActive(true);
                warningImage.gameObject.GetComponentInChildren<TextMeshProUGUI>().text = "Can not save empty level!!!";
                return;
            }
            else
            {
                warningImage.gameObject.SetActive(true);
            }

        }
        currentLevel = maxLevel + 1;
        GameObject level = Instantiate(levelCreatePrefab, levelsContent);
        
        level.transform.localScale = Vector2.zero;
        level.GetComponent<RectTransform>().DOScale(1.2f, 0.3f).SetEase(Ease.InExpo).OnComplete(() =>
        {
            level.GetComponent<RectTransform>()
               .DOScale(1f, 0.7f).SetEase(Ease.OutExpo);

        });
        level.transform.Find("Container/Center/Level").GetComponent<TextMeshProUGUI>().text = currentLevel.ToString();
        level.transform.SetSiblingIndex(1);
        levelButtons.Add(level.GetComponent<Button>());
        level.GetComponent<Button>().onClick.AddListener(() =>
        {
            ChangeSelectingButton(level.GetComponent<Button>());
        });
        ChangeSelectingButton(level.GetComponent<Button>());

    }

    private void LoadLevel(int level)
    {

    }
    private void ChangeSelectingButton(Button button)
    {
        if (selectingButton == null)
        {
            selectingButton = button;
            button.transform.Find("SelectImage").gameObject.SetActive(true);

        }
        else if (selectingButton != button)
        {
            selectingButton.transform.Find("SelectImage").gameObject.SetActive(false);
            selectingButton = button;
            button.transform.Find("SelectImage").gameObject.SetActive(true);

        }
        else if (selectingButton == button)
        {
            selectingButton.transform.Find("SelectImage").gameObject.SetActive(false);
            selectingButton = null;
        }
    }
 
}
