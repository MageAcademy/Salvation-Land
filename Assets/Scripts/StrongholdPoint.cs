using Photon.Pun;
using UnityEngine;
using UnityEngine.UI;

namespace com.PROS.SalvationLand
{
    public class StrongholdPoint : MonoBehaviour
    {
        public const int MAX_STRONGHOLD_POINT = 20;

        public static StrongholdPoint Instance;
        public static int StrongholdPointBlue;
        public static int StrongholdPointRed;

        public Transform imageBlue;
        public Transform imageRed;
        public PhotonView photonView;
        public Text textBlue;
        public Text textRed;

        private void Awake()
        {
            Instance = this;
            StrongholdPointBlue = 0;
            StrongholdPointRed = 0;
        }

        [PunRPC]
        private void SuccessRPC(int playerIndex)
        {
            Success.SuccessPlayerIndex = playerIndex;
            if (PhotonNetwork.IsMasterClient)
            {
                PhotonNetwork.LoadLevel("Success");
            }
        }

        public void AddStrongholdPoint(int playerIndex)
        {
            if (!GameSystem.IsGameEnded)
            {
                if (playerIndex == 0)
                {
                    ++StrongholdPointBlue;
                    if (StrongholdPointBlue >= MAX_STRONGHOLD_POINT)
                    {
                        GameSystem.IsGameEnded = true;
                        photonView.RPC(nameof(SuccessRPC), RpcTarget.AllViaServer, 0);
                    }
                }
                else if (playerIndex == 1)
                {
                    ++StrongholdPointRed;
                    if (StrongholdPointRed >= MAX_STRONGHOLD_POINT)
                    {
                        GameSystem.IsGameEnded = true;
                        photonView.RPC(nameof(SuccessRPC), RpcTarget.AllViaServer, 1);
                    }
                }

                Refresh();
                ButtonsSkill.Instance.isSkillsEnabled[5] = true;
            }
        }

        private void Refresh()
        {
            imageBlue.localScale = new Vector3((float) StrongholdPointBlue / MAX_STRONGHOLD_POINT, 1f);
            imageRed.localScale = new Vector3((float) StrongholdPointRed / MAX_STRONGHOLD_POINT, 1f);
            textBlue.text = StrongholdPointBlue + "/" + MAX_STRONGHOLD_POINT;
            textRed.text = StrongholdPointRed + "/" + MAX_STRONGHOLD_POINT;
        }
    }
}