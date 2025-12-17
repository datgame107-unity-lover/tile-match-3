using UnityEngine;

public class DangerSystem
{
    private float danger = 0f;
    private float maxDanger = 1f;
    private float increaseRate = 0.05f;
    private float autoIncreasePerSec = 0.01f;

    private DangerBarUI dangerBarUI;

    public System.Action OnDangerMax;

    public DangerSystem(DangerBarUI ui)
    {
        dangerBarUI = ui;
        
    }

    public void Tick(float delta)
    {
        danger += autoIncreasePerSec * delta;
        danger = Mathf.Clamp(danger, 0, maxDanger);

        dangerBarUI.SetValue(danger);

        if (danger >= maxDanger)
            OnDangerMax?.Invoke();
    }

    public void IncreaseBySpawn()
    {
        danger += increaseRate;
        danger = Mathf.Clamp(danger, 0, maxDanger);

        dangerBarUI.SetValue(danger);

        if (danger >= maxDanger)
            OnDangerMax?.Invoke();
    }
    public void ResetValue()
    {
        danger = 0f;
        dangerBarUI.SetValue(0f);
    }
    public void Decrease(float amount)
    {
        danger -= amount;
        danger = Mathf.Clamp(danger, 0, maxDanger);

        dangerBarUI.SetValue(danger);
    }

    public float GetCurrentDanger() => danger;
}
