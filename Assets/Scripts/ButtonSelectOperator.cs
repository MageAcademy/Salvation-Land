using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace com.PROS.SalvationLand
{
    public class ButtonSelectOperator : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
    {
        public Button button;
        public int index;
        public Image panel;
        [HideInInspector] public int selectedBy = -1;

        private static Color _grey = new Color(128f / 255f, 128f / 255f, 128f / 255f, 1f);

        private Color m_TargetColor = _grey;

        private void Update()
        {
            panel.color = Color.Lerp(panel.color, selectedBy == -1 ? m_TargetColor : GameSystem.Colors[selectedBy],
                Time.deltaTime * 6f);
        }

        public void OnButtonClick()
        {
            if (GameSystem.Index == SelectTurnTimer.Turn)
            {
                SelectTurnTimer.Instance.NextSelectTurn(index);
            }
        }

        public void OnPointerEnter(PointerEventData eventData)
        {
            SelectTurnTimer.Instance.meshOperators[index].SetActive(true);
            SelectTurnTimer.Instance.textOperatorName.text = SelectTurnTimer.Instance.operatorsProperty[index].name;
            m_TargetColor = GameSystem.Index >= 0 ? GameSystem.Colors[GameSystem.Index] : _grey;
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            SelectTurnTimer.Instance.meshOperators[index].SetActive(false);
            SelectTurnTimer.Instance.textOperatorName.text = string.Empty;
            m_TargetColor = _grey;
        }

        public void Initialize()
        {
            button.onClick.AddListener(OnButtonClick);
            button.interactable = true;
        }
    }
}