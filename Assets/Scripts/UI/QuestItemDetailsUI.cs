using NUnit.Framework;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class QuestItemDetailsUI : MonoBehaviour
{
    public GameObject questRewardPrefab;
    public Button closeButton;
    private void OnEnable()
    {
        closeButton.onClick.AddListener(() =>
        {
            DestroyImmediate(gameObject);

        });
    }

    private void OnDisable()
    {
        closeButton.onClick.RemoveAllListeners();
    }


    public void LoadReward(List<QuestReward> rewards)
    {
        foreach (QuestReward reward in rewards)
        {
            Transform questReward = Instantiate(questRewardPrefab,transform.Find("Container")).transform;
            questReward.Find("RewardAmount").GetComponent<TextMeshProUGUI>().text = reward.amount.ToString();
            questReward.Find("Base/RewardIcon").GetComponent<Image>().sprite = reward.icon;
        }
    }    
}
