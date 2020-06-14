using Photon.Pun;
using Photon.Realtime;

namespace com.PROS.SalvationLand
{
    public class PlayerDisconnect : MonoBehaviourPunCallbacks
    {
        public override void OnDisconnected(DisconnectCause cause)
        {
            if (!GameSystem.IsGameEnded)
            {
                LogManager.Instance.Add("Callback: OnDisconnected(). Disconnect Cause: " + cause + ".");
                LogManager.Instance.Add("Custom Call: GameSystem.BackToLobby()");
                GameSystem.BackToLobby();
            }
        }

        public override void OnPlayerLeftRoom(Player otherPlayer)
        {
            if (!GameSystem.IsGameEnded)
            {
                LogManager.Instance.Add("Callback: OnPlayerLeftRoom(). Player Nick Name: " + otherPlayer.NickName +
                                        ".");
                LogManager.Instance.Add("Call: Disconnect().");
                PhotonNetwork.Disconnect();
            }
        }
    }
}