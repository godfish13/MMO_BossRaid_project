using Google.Protobuf.Protocol;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

#region Stat
// JsonWriter custom editor를 위해 Data modeling방식으로 변경
public class Stat
{
    public int ClassId;
    public string Class;
    public int MaxHp;
    public int Hp;
    public float MaxSpeed;
    public float Acceleration;
}

[Serializable]
public class StatData : ILoader<int, Stat>
{
    public List<Stat> Stats = new List<Stat>();     // !!!!!!중요!!!!!! JSON파일에서 받아오려는 list와 이름이 꼭!!! 같아야함

    public Dictionary<int, Stat> MakeDict()
    {
        Dictionary<int, Stat> dict = new Dictionary<int, Stat>();
        foreach (Stat stat in Stats)
            dict.Add(stat.ClassId, stat);
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