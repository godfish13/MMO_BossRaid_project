using Google.Protobuf.Protocol;
using JetBrains.Annotations;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface ILoader<key, value>            // DataContents에서 활용
{
    Dictionary<key, value> MakeDict();
}

public class DataMgr
{
    public Dictionary<int, StatInfo> StatDictionary { get; private set; } = new Dictionary<int, StatInfo>();
    public Dictionary<int, SkillInfo> SkillDictionary { get; private set; } = new Dictionary<int, SkillInfo>();

    public void init()
    {
        StatDictionary = LoadJson<StatData, int, StatInfo>("StatData").MakeDict();
        SkillDictionary = LoadJson<SkillData, int, SkillInfo>("SkillData").MakeDict();
    }

    Loader LoadJson<Loader, key, value>(string path) where Loader : ILoader<key, value>
    {
        TextAsset textAsset = Managers.resourceMgr.Load<TextAsset>($"Data/{path}");
        return Newtonsoft.Json.JsonConvert.DeserializeObject<Loader>(textAsset.ToString());
    }
}