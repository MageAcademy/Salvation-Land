using UnityEngine;
using UnityEngine.UI;

namespace com.PROS.SalvationLand
{
    public class ReachableMark : MonoBehaviour
    {
        public static int SkillLevel;

        [HideInInspector] public int distance;
        [HideInInspector] public bool[] isOutlineExists;
        public Image mark;
        public RawImage outline;
        [HideInInspector] public int x;
        [HideInInspector] public int y;

        public void OnMouseDown()
        {
            ButtonsSkill.Instance.isAnimationsPlaying[0] = true;
            ButtonsSkill.Instance.isButtonsPressed[0] = false;
            ButtonsSkill.Instance.Refresh();
            SkillLibrary.Instance.CastSkill(GameSystem.CurrentOperator.property.index, 1, SkillLevel,
                new[] {distance, x, y});
        }

        public void OnMouseEnter()
        {
            mark.color = new Color(0f, 1f, 0f, 0.5f);
        }

        public void OnMouseExit()
        {
            mark.color = new Color(0.8f, 0.8f, 0.8f, 0.5f);
        }

        public void SetOutline()
        {
            int a = 1;
            int index = 0;
            for (int b = 0; b < 4; ++b)
            {
                if (isOutlineExists[b])
                {
                    index += a;
                }

                a *= 2;
            }

            if (index == 0)
            {
                outline.color = Color.clear;
            }
            else
            {
                outline.texture = MapManager.Instance.outlines[index];
            }
        }
    }
}