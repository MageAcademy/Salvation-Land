using UnityEngine;
using UnityEngine.EventSystems;

namespace com.PROS.SalvationLand
{
    public class ButtonsSkillEvent : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
    {
        public int index;

        public void OnPointerEnter(PointerEventData eventData)
        {
            SkillDescription.Instance.Show(index);
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            SkillDescription.Instance.Hide();
        }
    }
}