using Google.Protobuf.Protocol;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

#region Stat
[Serializable]
public class StatData : ILoader<int, StatInfo>
{
    public List<StatInfo> Stats = new List<StatInfo>();     // !!!!!!중요!!!!!! JSON파일에서 받아오려는 list와 이름이 꼭!!! 같아야함

    public Dictionary<int, StatInfo> MakeDict()
    {
        Dictionary<int, StatInfo> dict = new Dictionary<int, StatInfo>();
        foreach (StatInfo statInfo in Stats)
            dict.Add(statInfo.ClassId, statInfo);
        return dict;
    }
}
#endregion

#region Skill
[Serializable]
public class SkillData : ILoader<int, SkillInfo>
{
    public List<SkillInfo> Skills = new List<SkillInfo>();     // !!!!!!중요!!!!!! JSON파일에서 받아오려는 list와 이름이 꼭!!! 같아야함

    public Dictionary<int, SkillInfo> MakeDict()
    {
        Dictionary<int, SkillInfo> dict = new Dictionary<int, SkillInfo>();
        foreach (SkillInfo skillInfo in Skills)
            dict.Add(skillInfo.ClassId, skillInfo);
        return dict;
    }
}
#endregion