using UnityEngine;

public class ProgressData
{
    public string questID;
    public int currentAmount;
    public bool isClaimed;

    public ProgressData(string id)
    {
        questID = id;
        currentAmount = 0;
        isClaimed = false;
    }
}
