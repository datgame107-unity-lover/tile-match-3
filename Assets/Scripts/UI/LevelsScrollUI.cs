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
   

    private void Start()
    {
        LoadLevel();
       

    }
    private void OnEnable()
    {
        newLevelButton.onClick.AddListener(() =>
        {
            CreateNewLevelButton(LevelDataManager.GetTotalLevelEditor() + 1);

            newLevelButton.interactable = false;
        });
      
    }
    private void OnDisable()
    {

        newLevelButton.onClick.RemoveAllListeners();
    }

 


    int GetLevel(Button button)
    {
        TextMeshProUGUI levelText = button.transform.Find("Container/Center/Level")
                                     .GetComponent<TextMeshProUGUI>();

        int levelNumber = int.Parse(levelText.text);
        return levelNumber;
    }
 
    public void LoadLevel()
    {
        for (int i = 1; i <= LevelDataManager.GetTotalLevelEditor(); i++)
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
