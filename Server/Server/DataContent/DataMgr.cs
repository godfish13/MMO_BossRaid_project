﻿using Google.Protobuf.Protocol;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace Server.Data
{
    public interface ILoader<key, value>            // DataContents에서 활용
    {
        Dictionary<key, value> MakeDict();
    }

    public class DataMgr
    {
        public static Dictionary<int, StatInfo> StatDictionary { get; private set; } = new Dictionary<int, StatInfo>();
        public static Dictionary<int, SkillInfo> SkillDictionary { get; private set; } = new Dictionary<int, SkillInfo>();
        public static Dictionary<int, MonsterSkillInfo> MonsterSkillDictionary { get; private set; } = new Dictionary<int, MonsterSkillInfo>();

        public static void LoadData()
        {
            StatDictionary = LoadJson<StatData, int, StatInfo>("StatData").MakeDict();
            SkillDictionary = LoadJson<SkillData, int, SkillInfo>("SkillData").MakeDict();
            MonsterSkillDictionary = LoadJson<MonsterSkillData, int, MonsterSkillInfo>("MonsterSkillData").MakeDict();
        }

        static Loader LoadJson<Loader, key, value>(string path) where Loader : ILoader<key, value>
        {
            string text = File.ReadAllText($"{ConfigMgr.Config.dataPath}/{path}.json"); // C#내에서는 .json 붙여줘야 읽어짐
            return Newtonsoft.Json.JsonConvert.DeserializeObject<Loader>(text);

            //TextAsset textAsset = Managers.resourceMgr.Load<TextAsset>($"Data/{path}");   //유니티 어셋용
            //return JsonUtility.FromJson<Loader>(textAsset.text);                          // Newtonsoft nuget이랑 비교해보라고 남겨둠
        }
    }
}
