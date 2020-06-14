using TMPro;
using UnityEngine;

namespace com.PROS.SalvationLand
{
    public class SkillDescription : MonoBehaviour
    {
        public static SkillDescription Instance;

        public static string[] Values =
        {
            "消耗1点行动值，干员移动到距离自己1格的位置。\n\n" +
            "#2#-等级2：连续移动4格仅消耗3点行动值。###\n" +
            "#3#-等级3：连续移动3格仅消耗2点行动值。###\n" +
            "#4#-等级4：连续移动2格仅消耗1点行动值。###", //0
            "", //1
            "", //2
            "", //3
            "", //4
            "", //5
            "", //6
            "" //7
        };

        public RectTransform panel;
        public TextMeshProUGUI text;

        private void Awake()
        {
            Instance = this;
        }

        public void Show(int index)
        {
            int currentSkillLevel = GameSystem.CurrentOperator.property.currentSkillLevel[index];
            int maxSkillLevel = GameSystem.CurrentOperator.property.maxSkillLevel[index];
            int skillIndex = GameSystem.CurrentOperator.property.skillIndex[index];
            string description = Values[skillIndex];
            for (int a = 1; a <= maxSkillLevel; ++a)
            {
                string b = "#" + a + "#";
                if (a <= currentSkillLevel)
                {
                    description = description.Replace(b, "<color=#ff8000>");
                }
                else
                {
                    description = description.Replace(b, "<color=#808080>");
                }
            }

            text.text = description.Replace("###", "</color>");
            panel.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, text.preferredHeight + 40f);
            panel.gameObject.SetActive(true);
        }

        public void Hide()
        {
            panel.gameObject.SetActive(false);
        }
    }
}