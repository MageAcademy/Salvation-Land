using UnityEngine;
using UnityEngine.UI;

namespace com.PROS.SalvationLand
{
    public class SkillMark : MonoBehaviour
    {
        public static int SkillButtonIndex;
        public static int SkillIndex;
        public static int SkillLevel;

        public Image mark;
        [HideInInspector] public int x;
        [HideInInspector] public int y;

        public void OnMouseDown()
        {
            ButtonsSkill.Instance.isAnimationsPlaying[SkillButtonIndex] = true;
            ButtonsSkill.Instance.isButtonsPressed[SkillButtonIndex] = false;
            ButtonsSkill.Instance.Refresh();
            SkillLibrary.Instance.CastSkill(GameSystem.CurrentOperator.property.index, SkillIndex, SkillLevel,
                new[] {x, y});
        }

        public void OnMouseEnter()
        {
            mark.color = new Color(0f, 1f, 0f, 0.5f);
        }

        public void OnMouseExit()
        {
            mark.color = new Color(0.8f, 0.8f, 0.8f, 0.5f);
        }
    }
}