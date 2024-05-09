using Google.Protobuf.Protocol;
using System;
using System.Collections.Generic;
using System.Text;

namespace Server.Data
{
    #region Stat
    [Serializable]
    public class StatData : ILoader<int, StatInfo>
    {
        public List<StatInfo> Stats = new List<StatInfo>();     // !!!!!!중요!!!!!! JSON파일에서 받아오려는 list와 이름이 꼭!!! 같아야함

        public Dictionary<int, StatInfo> MakeDict()
        {
            Dictionary<int, StatInfo> dict = new Dictionary<int, StatInfo>();
            foreach (StatInfo StatInfo in Stats)
                dict.Add(StatInfo.ClassId, StatInfo);
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
            foreach (SkillInfo SkillInfo in Skills)
                dict.Add(SkillInfo.ClassId, SkillInfo);
            return dict;
        }
    }
    #endregion

    #region MonsterSkill
    [Serializable]
    public class MonsterSkillData : ILoader<int, MonsterSkillInfo>
    {
        public List<MonsterSkillInfo> MonsterSkills = new List<MonsterSkillInfo>();     // !!!!!!중요!!!!!! JSON파일에서 받아오려는 list와 이름이 꼭!!! 같아야함

        public Dictionary<int, MonsterSkillInfo> MakeDict()
        {
            Dictionary<int, MonsterSkillInfo> dict = new Dictionary<int, MonsterSkillInfo>();
            foreach (MonsterSkillInfo MonsterSkillInfo in MonsterSkills)
                dict.Add(MonsterSkillInfo.ClassId, MonsterSkillInfo);
            return dict;
        }
    }
    #endregion
}
