// Scripts/Gameplay/Modes/DangerSystem.cs
using System;
using UnityEngine;

public class DangerSystem
{
    public event Action OnDangerMax;

    public float Value { get; private set; }
    public float MaxValue { get; }

    private readonly float tickRate;
    private readonly float spawnIncrease;
    private bool triggered;

    public DangerSystem(
        float maxValue = 1f,
        float tickRate = 0.002f,
        float spawnIncrease = 0.08f)
    {
        MaxValue = maxValue;
        this.tickRate = tickRate;
        this.spawnIncrease = spawnIncrease;
    }

    // Gọi từ coroutine mỗi frame
    public void Tick(float deltaTime)
    {
        if (triggered) return;

        Value = Mathf.Clamp01(Value + tickRate * deltaTime);
        EventBus<DangerChangedEvent>.Publish(new DangerChangedEvent { value = Value}); 

        if (Value >= MaxValue)
        {
            triggered = true;
            OnDangerMax?.Invoke();
        }
    }

    public void IncreaseBySpawn()
    {
        Value = Mathf.Clamp01(Value + spawnIncrease);
        EventBus<DangerChangedEvent>.Publish(new DangerChangedEvent { value = Value });

    }

    public void Decrease(float amount)
    {
        Value = Mathf.Clamp01(Value - amount);
        EventBus<DangerChangedEvent>.Publish(new DangerChangedEvent { value = Value });

    }

    public void Reset()
    {
        Value = 0f;
        triggered = false;
        EventBus<DangerChangedEvent>.Publish(new DangerChangedEvent { value = Value });

    }
}