using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace com.PROS.SalvationLand
{
    public class ButtonsSkill : MonoBehaviour
    {
        public static ButtonsSkill Instance;

        public Button buttonCancel;
        public Button buttonEndTurn;
        public Button buttonMove;
        public Button[] buttonsSkill;
        public Button buttonStronghold;
        public Button buttonTeleport;
        public Button buttonUpgrade;
        [HideInInspector] public bool[] isAnimationsPlaying = new bool[4];
        [HideInInspector] public bool[] isButtonsPressed = new bool[4];
        [HideInInspector] public bool[] isSkillsEnabled = {true, true, true, true, true, true, true};
        public Text[] texts;

        private List<Block> m_ReachableBlockList;

        private void Awake()
        {
            Instance = this;
        }

        public void OnButtonMoveClick()
        {
            isButtonsPressed[0] = true;
            Refresh();
            SkillLibrary.Instance.CastSkill(GameSystem.CurrentOperator.property.index,
                GameSystem.CurrentOperator.property.skillIndex[0],
                GameSystem.CurrentOperator.property.currentSkillLevel[0]);
        }

        public void OnButtonSkill1Click()
        {
            isButtonsPressed[1] = true;
            Refresh();
            SkillLibrary.Instance.CastSkill(GameSystem.CurrentOperator.property.index,
                GameSystem.CurrentOperator.property.skillIndex[1],
                GameSystem.CurrentOperator.property.currentSkillLevel[1]);
        }

        public void OnButtonSkill2Click()
        {
            isButtonsPressed[2] = true;
            Refresh();
        }

        public void OnButtonSkill3Click()
        {
            isButtonsPressed[3] = true;
            Refresh();
        }

        public void OnButtonCancelClick()
        {
            Cancel();
        }

        public void OnButtonUpgradeClick()
        {
            if (GameSystem.CurrentOperator.property.skillPoint > 0)
            {
                for (int a = 0; a < isButtonsPressed.Length; ++a)
                {
                    if (isButtonsPressed[a])
                    {
                        if (GameSystem.CurrentOperator.property.currentSkillLevel[a] <
                            GameSystem.CurrentOperator.property.maxSkillLevel[a])
                        {
                            isSkillsEnabled[4] = false;
                            Refresh();
                            SkillLibrary.Instance.CastSkill(GameSystem.CurrentOperator.property.index, 100, 0, a);
                        }

                        break;
                    }
                }
            }
        }

        public void OnButtonStrongholdClick()
        {
            if (ActionPoint.CurrentValue > 0)
            {
                isSkillsEnabled[5] = false;
                Refresh();
                SkillLibrary.Instance.CastSkill(GameSystem.CurrentOperator.property.index, 101, 0, GameSystem.Index);
            }
        }

        public void OnButtonTeleportClick()
        {
            Teleport teleport = Teleport.GetTeleport(out int index, out bool isOneSide);
            if (teleport != null && ActionPoint.CurrentValue > 0)
            {
                isSkillsEnabled[6] = false;
                Refresh();
                RPCInt_Vector3 teleportPosition = new RPCInt_Vector3();
                teleportPosition.valueInt = isOneSide ? teleport.mapIndexOtherSide : teleport.mapIndexOneSide;
                teleportPosition.valueVector3 = teleport.GetPosition(index, isOneSide);
                SkillLibrary.Instance.CastSkill(GameSystem.CurrentOperator.property.index, 102, GameSystem.Index,
                    JsonUtility.ToJson(teleportPosition));
            }
        }

        public void OnButtonEndTurnClick()
        {
            buttonEndTurn.interactable = false;
            GameTurnTimer.Instance.NextGameTurn();
        }

        public void Cancel()
        {
            for (int a = 0; a < isButtonsPressed.Length; ++a)
            {
                isButtonsPressed[a] = false;
            }

            Refresh();
            SkillLibrary.Instance.ClearMarks();
        }

        public void Refresh()
        {
            if (GameTurnTimer.Turn == GameSystem.Index)
            {
                bool isAnyAnimationPlaying = false;
                for (int a = 0; a < isAnimationsPlaying.Length; ++a)
                {
                    if (isAnimationsPlaying[a])
                    {
                        isAnyAnimationPlaying = true;
                        break;
                    }
                }

                bool isAnyButtonSkillPressed = false;
                for (int a = 0; a < isButtonsPressed.Length; ++a)
                {
                    if (isButtonsPressed[a])
                    {
                        isAnyButtonSkillPressed = true;
                        break;
                    }
                }

                for (int a = 0; a < 4; ++a)
                {
                    isSkillsEnabled[a] = GameSystem.Instance.IsSkillEnabled(a);
                }

                if (isAnyButtonSkillPressed)
                {
                    buttonCancel.interactable = true;
                    buttonMove.interactable = false;
                    buttonsSkill[0].interactable = false;
                    buttonsSkill[1].interactable = false;
                    buttonsSkill[2].interactable = false;
                    buttonStronghold.interactable = false;
                    buttonTeleport.interactable = false;
                    buttonUpgrade.interactable = isSkillsEnabled[4];
                }
                else if (isAnyAnimationPlaying)
                {
                    buttonCancel.interactable = false;
                    buttonMove.interactable = false;
                    buttonsSkill[0].interactable = false;
                    buttonsSkill[1].interactable = false;
                    buttonsSkill[2].interactable = false;
                    buttonStronghold.interactable = false;
                    buttonTeleport.interactable = false;
                    buttonUpgrade.interactable = false;
                }
                else
                {
                    buttonCancel.interactable = false;
                    buttonMove.interactable = isSkillsEnabled[0];
                    buttonsSkill[0].interactable = isSkillsEnabled[1];
                    buttonsSkill[1].interactable = isSkillsEnabled[2];
                    buttonsSkill[2].interactable = isSkillsEnabled[3];
                    buttonStronghold.interactable = Stronghold.IsButtonStrongholdEnabled() && isSkillsEnabled[5];
                    buttonTeleport.interactable = Teleport.GetTeleport(out int index, out bool isOneSide) != null &&
                                                  isSkillsEnabled[6];
                    buttonUpgrade.interactable = false;
                }

                buttonEndTurn.interactable = true;
            }
            else
            {
                buttonCancel.interactable = false;
                buttonEndTurn.interactable = false;
                buttonMove.interactable = false;
                buttonsSkill[0].interactable = false;
                buttonsSkill[1].interactable = false;
                buttonsSkill[2].interactable = false;
                buttonStronghold.interactable = false;
                buttonTeleport.interactable = false;
                buttonUpgrade.interactable = false;
            }

            for (int a = 0; a < 4; ++a)
            {
                texts[a].text = SkillLibrary.SkillNames[GameSystem.CurrentOperator.property.skillIndex[a]] +
                                "  LV." + GameSystem.CurrentOperator.property.currentSkillLevel[a];
            }

            texts[4].text = "技能等级提升  " + GameSystem.CurrentOperator.property.skillPoint + "/3";
        }
    }
}