using Photon.Pun;
using UnityEngine;
using UnityEngine.UI;

namespace com.PROS.SalvationLand
{
    public class Success : MonoBehaviour
    {
        public static int SuccessPlayerIndex;

        public Image background;
        public Text text;

        private void Start()
        {
            PhotonNetwork.Disconnect();
            background.color = GameSystem.Colors[SuccessPlayerIndex];
            text.text = GameSystem.PlayerNickNames[SuccessPlayerIndex] + " 获胜";
        }

        public void OnButtonBackToLobbyClick()
        {
            if (!PhotonNetwork.IsConnected)
            {
                GameSystem.BackToLobby();
            }
        }
    }
}