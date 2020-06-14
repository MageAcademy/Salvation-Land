using UnityEngine;
using UnityEngine.UI;

namespace com.PROS.SalvationLand
{
    public class ButtonsOperator : MonoBehaviour
    {
        public static ButtonsOperator Instance;

        public Button[] buttons;
        public Text[] texts;

        private void Awake()
        {
            Instance = this;
        }

        public void OnButtonOperatorClick(int index)
        {
            ButtonsSkill.Instance.Cancel();
            if (GameSystem.Index == 0)
            {
                GameSystem.CurrentOperator = GameSystem.OperatorListBlue[index];
            }
            else if (GameSystem.Index == 1)
            {
                GameSystem.CurrentOperator = GameSystem.OperatorListRed[index];
            }

            CameraController.FocusTo = GameSystem.CurrentOperator.transform;
            CameraController.Instance.Focus();
            ButtonsSkill.Instance.Refresh();
        }

        public void Refresh()
        {
            for (int a = 0; a < buttons.Length; ++a)
            {
                if (GameSystem.Index == 0)
                {
                    buttons[a].interactable = GameSystem.OperatorListBlue[a].property.currentHealth > 0;
                    texts[a].text = GameSystem.OperatorListBlue[a].property.name + "  " +
                                    GameSystem.OperatorListBlue[a].property.currentHealth + "/" +
                                    GameSystem.OperatorListBlue[a].property.maxHealth;
                }
                else if (GameSystem.Index == 1)
                {
                    buttons[a].interactable = GameSystem.OperatorListRed[a].property.currentHealth > 0;
                    texts[a].text = GameSystem.OperatorListRed[a].property.name + "  " +
                                    GameSystem.OperatorListRed[a].property.currentHealth + "/" +
                                    GameSystem.OperatorListRed[a].property.maxHealth;
                }
            }
        }
    }
}