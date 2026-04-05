using System;
using System.Collections.Generic;
using UnityEngine;

    public class EventBus<T> where T : struct
    {
        private static readonly List<Action<T>> handlers = new();

        public static void Subscribe(Action<T> action)
        {
            if (!handlers.Contains(action))
                handlers.Add(action);
        }

        public static void Unsubscribe(Action<T> action)
        {
            handlers.Remove(action);
        }

        public static void Publish(T evt)
        {
            for (int i = handlers.Count - 1; i >= 0; i--)
            {
                try
                {
                    handlers[i]?.Invoke(evt);
                }
                catch (Exception e)
                {
                    Debug.LogError($"[EventBus<{typeof(T).Name}>] {e}");
                }
            }
        }

        public static void Clear() => handlers.Clear();
    }
