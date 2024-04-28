using Google.Protobuf.Protocol;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

#region Stat
[Serializable]      // 메모리에 들고있는 정보를 파일로 변환시키기 위해 필요한 선언 // 그냥 써라...
public class Stat
{
    // public or [SerializeField] 선언해야지 JSON에서 데이터 받아올 수 있음
    // 각 항목의 이름이랑 JSON 파일 내 항목의 이름이 꼭 같아야 데이터 받아올 수 있음  // 자료형 또한 주의!
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
public class Skill
{
    // public or [SerializeField] 선언해야지 JSON에서 데이터 받아올 수 있음
    // 각 항목의 이름이랑 JSON 파일 내 항목의 이름이 꼭 같아야 데이터 받아올 수 있음  // 자료형 또한 주의!
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
    public List<Skill> Skills = new List<Skill>();     // !!!!!!중요!!!!!! JSON파일에서 받아오려는 list와 이름이 꼭!!! 같아야함

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
    public List<SkillInfo> Skills = new List<SkillInfo>();     // !!!!!!중요!!!!!! JSON파일에서 받아오려는 list와 이름이 꼭!!! 같아야함

    public Dictionary<int, SkillInfo> MakeDict()
    {
        Dictionary<int, SkillInfo> dict = new Dictionary<int, SkillInfo>();
        foreach (SkillInfo skillInfo in Skills)
            dict.Add(skillInfo.ClassId, skillInfo);
        return dict;
    }
}*/
#endregion