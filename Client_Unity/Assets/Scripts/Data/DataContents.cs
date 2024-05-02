using Google.Protobuf.Protocol;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

#region Stat
[Serializable]
public class StatData : ILoader<int, StatInfo>
{
    public List<StatInfo> Stats = new List<StatInfo>();     // !!!!!!�߿�!!!!!! JSON���Ͽ��� �޾ƿ����� list�� �̸��� ��!!! ���ƾ���

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
    public List<SkillInfo> Skills = new List<SkillInfo>();     // !!!!!!�߿�!!!!!! JSON���Ͽ��� �޾ƿ����� list�� �̸��� ��!!! ���ƾ���

    public Dictionary<int, SkillInfo> MakeDict()
    {
        Dictionary<int, SkillInfo> dict = new Dictionary<int, SkillInfo>();
        foreach (SkillInfo skillInfo in Skills)
            dict.Add(skillInfo.ClassId, skillInfo);
        return dict;
    }
}
#endregion