using System;
using System.Collections.Generic;
using UnityEngine;

[NoAutoCreate]
public abstract class ScriptableRegister<TItem, TSelf> : ScriptableSingleton<TSelf>
    where TItem : UnityEngine.Object
    where TSelf : ScriptableRegister<TItem, TSelf>
{
    [SerializeField] private TItem[] registeredItems = Array.Empty<TItem>();

    private Dictionary<TItem, int> lookup;

    private void OnEnable()
    {
        BuildLookup();
    }

    private void BuildLookup()
    {
        lookup = new();

        if (registeredItems == null)
        {
            registeredItems = Array.Empty<TItem>();
            return;
        }

        for (int i = 0; i < registeredItems.Length; i++)
        {
            var item = registeredItems[i];

            if (item == null)
            {
                Debug.LogWarning($"{nameof(ScriptableRegister<TItem, TSelf>)} contains null at index {i}");
                continue;
            }

            if (lookup.ContainsKey(item))
            {
                Debug.LogWarning($"{nameof(ScriptableRegister<TItem, TSelf>)} contains duplicate item at index {i}");
                continue;
            }

            lookup.Add(item, i);
        }
    }

    public int Count => registeredItems?.Length ?? 0;

    public bool TryGetById(int id, out TItem item)
    {
        item = null;

        if (id < 0 || id >= Count)
            return false;

        item = registeredItems[id];
        return item != null;
    }

    public bool TryGetId(TItem item, out int id)
    {
        id = -1;

        if (item == null || lookup == null)
            return false;

        return lookup.TryGetValue(item, out id);
    }

    public TItem GetById(int id)
    {
        if (!TryGetById(id, out var item))
            throw new ArgumentOutOfRangeException(nameof(id), $"Invalid id {id}");

        return item;
    }

    public int GetId(TItem item)
    {
        if (!TryGetId(item, out var id))
            throw new ArgumentException("Item not found in register", nameof(item));

        return id;
    }
}