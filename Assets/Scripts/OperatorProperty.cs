using System;

namespace com.PROS.SalvationLand
{
    [Serializable]
    public class OperatorProperty
    {
        public int currentHealth;
        public int currentMapIndex;
        public int[] currentSkillLevel;
        public int index;
        public int maxHealth;
        public int[] maxSkillLevel;
        public string name;
        public int[] skillCooldown;
        public int[] skillIndex;
        public int skillPoint;
    }
}