using Google.Protobuf.Protocol;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

#region Stat
// JsonWriter custom editor�� ���� Data modeling������� ����
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
    public List<Stat> Stats = new List<Stat>();     // !!!!!!�߿�!!!!!! JSON���Ͽ��� �޾ƿ����� list�� �̸��� ��!!! ���ƾ���

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