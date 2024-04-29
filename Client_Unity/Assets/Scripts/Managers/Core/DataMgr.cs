using Google.Protobuf.Protocol;
using JetBrains.Annotations;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor.Experimental.GraphView;
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

/* Legacy) DataModeling 방식
public class DataMgr
{
    public Dictionary<int, Stat> SkillDictionary { get; private set; } = new Dictionary<int, Stat>();
    public Dictionary<int, Skill> SkillDictionary { get; private set; } = new Dictionary<int, Skill>();

    public void init()
    {
        StatDictionary = LoadJson<StatData, int, Stat>("SkillData").MakeDict();
        SkillDictionary = LoadJson<SkillData, int, Skill>("SkillData").MakeDict();
    }

    Loader LoadJson<Loader, key, value>(string path) where Loader : ILoader<key, value>
    {
        TextAsset textAsset = Managers.resourceMgr.Load<TextAsset>($"Data/{path}");
        return JsonUtility.FromJson<Loader>(textAsset.text);
    }
}*/