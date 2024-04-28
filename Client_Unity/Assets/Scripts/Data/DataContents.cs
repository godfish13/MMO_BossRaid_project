using Google.Protobuf.Protocol;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

#region Stat
[Serializable]      // �޸𸮿� ����ִ� ������ ���Ϸ� ��ȯ��Ű�� ���� �ʿ��� ���� // �׳� ���...
public class Stat
{
    // public or [SerializeField] �����ؾ��� JSON���� ������ �޾ƿ� �� ����
    // �� �׸��� �̸��̶� JSON ���� �� �׸��� �̸��� �� ���ƾ� ������ �޾ƿ� �� ����  // �ڷ��� ���� ����!
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
public class Skill
{
    // public or [SerializeField] �����ؾ��� JSON���� ������ �޾ƿ� �� ����
    // �� �׸��� �̸��̶� JSON ���� �� �׸��� �̸��� �� ���ƾ� ������ �޾ƿ� �� ����  // �ڷ��� ���� ����!
    public int ClassId;
    public string Class;
    public int SkillDamage;
    public float SkillCoolTime;
    public int SubSkillDamage;
    public float SubSkillCoolTime;
    public float JumpPower;
    public float JumpCoolTime;
}

[Serializable]
public class SkillData : ILoader<int, Skill>
{
    public List<Skill> Skills = new List<Skill>();     // !!!!!!�߿�!!!!!! JSON���Ͽ��� �޾ƿ����� list�� �̸��� ��!!! ���ƾ���

    public Dictionary<int, Skill> MakeDict()
    {
        Dictionary<int, Skill> dict = new Dictionary<int, Skill>();
        foreach (Skill skillInfo in Skills)
            dict.Add(skillInfo.ClassId, skillInfo);
        return dict;
    }
}

/*[Serializable]
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
}*/
#endregion