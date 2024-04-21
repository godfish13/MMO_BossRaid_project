using JetBrains.Annotations;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public interface ILoader<key, value>            // DataContents에서 활용
{
    Dictionary<key, value> MakeDict();
}

public class DataMgr
{
    public Dictionary<int, Stat> StatDictionary { get; private set; } = new Dictionary<int, Stat>();

    public void init()
    {
        StatDictionary = LoadJson<StatData, int, Stat>("StatData").MakeDict();
    }

    Loader LoadJson<Loader, key, value>(string path) where Loader : ILoader<key, value>
    {
        TextAsset textAsset = Managers.resourceMgr.Load<TextAsset>($"Data/{path}");
        return JsonUtility.FromJson<Loader>(textAsset.text);
    }
}