using System.Collections;
using System.IO;
using System.Text;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine;
using Hashtable = ExitGames.Client.Photon.Hashtable;

namespace com.PROS.SalvationLand
{
    public class Lobby : MonoBehaviourPunCallbacks
    {
        public const string KEY_LOADING_STATUS = "LoadingStatus";

        public Texture2D cursor;

        private void Start()
        {
            Screen.SetResolution(1280, 720, false);
            Cursor.SetCursor(cursor, new Vector2(20f, 5f), CursorMode.Auto);
            LogManager.Instance.Add("Custom Call: Lobby.Initialize().");
            Initialize();
        }

        public override void OnConnectedToMaster()
        {
            LogManager.Instance.Add("Callback: OnConnectedToMaster(). Cloud Region: " + PhotonNetwork.CloudRegion +
                                    ". Ping: " + PhotonNetwork.GetPing() + "ms.");
            LogManager.Instance.Add("Call: JoinRandomRoom().");
            PhotonNetwork.JoinRandomRoom();
        }

        public override void OnDisconnected(DisconnectCause cause)
        {
            LogManager.Instance.Add("Callback: OnDisconnected(). Disconnect Cause: " + cause + ".");
        }

        public override void OnJoinedLobby()
        {
            LogManager.Instance.Add("Callback: OnJoinedLobby().");
        }

        public override void OnLeftLobby()
        {
            LogManager.Instance.Add("Callback: OnLeftLobby().");
        }

        public override void OnJoinRandomFailed(short returnCode, string message)
        {
            LogManager.Instance.Add("Callback: OnJoinRandomFailed(). Return Code: " + returnCode + ". Message: " +
                                    message + ".");
            LogManager.Instance.Add("Call: CreateRoom().");
            PhotonNetwork.CreateRoom(string.Empty, new RoomOptions {MaxPlayers = 2});
        }

        public override void OnCreatedRoom()
        {
            LogManager.Instance.Add("Callback: OnCreatedRoom().");
        }

        public override void OnCreateRoomFailed(short returnCode, string message)
        {
            LogManager.Instance.Add("Callback: OnCreateRoomFailed(). Return Code: " + returnCode + ". Message: " +
                                    message + ".");
        }

        public override void OnJoinedRoom()
        {
            LogManager.Instance.Add("Callback: OnJoinedRoom(). Is Master Client: " + PhotonNetwork.IsMasterClient +
                                    ".");
            LogManager.Instance.Add("Call: LocalPlayer.SetCustomProperties(). Key: " + KEY_LOADING_STATUS +
                                    ". Value: 0.");
            PhotonNetwork.LocalPlayer.SetCustomProperties(new Hashtable {{KEY_LOADING_STATUS, 0}});
            LogManager.Instance.Add("Custom Call: AutoLoadLevel().");
            AutoLoadLevel();
        }

        public override void OnJoinRoomFailed(short returnCode, string message)
        {
            LogManager.Instance.Add("Callback: OnJoinRoomFailed(). Return Code: " + returnCode + ". Message: " +
                                    message + ".");
        }

        public override void OnLeftRoom()
        {
            LogManager.Instance.Add("Callback: OnLeftRoom().");
        }

        public override void OnPlayerEnteredRoom(Player newPlayer)
        {
            LogManager.Instance.Add("Callback: OnPlayerEnteredRoom(). Player Nick Name: " + newPlayer.NickName + ".");
            LogManager.Instance.Add("Custom Call: AutoLoadLevel().");
            AutoLoadLevel();
        }

        public override void OnPlayerLeftRoom(Player otherPlayer)
        {
            LogManager.Instance.Add("Callback: OnPlayerLeftRoom(). Player Nick Name: " + otherPlayer.NickName + ".");
        }

        public override void OnMasterClientSwitched(Player newMasterClient)
        {
            LogManager.Instance.Add("Callback: OnMasterClientSwitched(). Player Nick Name: " +
                                    newMasterClient.NickName + ".");
        }

        public override void OnPlayerPropertiesUpdate(Player targetPlayer, Hashtable changedProps)
        {
            string log = "Callback: OnPlayerPropertiesUpdate(). Player Nick Name: " + targetPlayer.NickName +
                         ". Changed Property Count: " + changedProps.Count + ".";
            foreach (DictionaryEntry item in changedProps)
            {
                log += "\n(Changed Property) Key: " + item.Key + ". Value: " + item.Value + ".";
            }

            LogManager.Instance.Add(log);
        }

        private void Initialize()
        {
#if UNITY_EDITOR
            string filePath = Application.dataPath + "/Text Assets/user_data_0.json";
#else
            string filePath = Application.dataPath + "/user_data_0.json";
#endif
            if (File.Exists(filePath))
            {
                PhotonNetwork.NickName =
                    JsonUtility.FromJson<UserData0>(File.ReadAllText(filePath, Encoding.UTF8)).nickName;
            }
            else
            {
                PhotonNetwork.NickName = "PC #" + Random.Range(1000, 10000);
                File.WriteAllText(filePath, JsonUtility.ToJson(new UserData0 {nickName = PhotonNetwork.NickName}, true),
                    Encoding.UTF8);
            }

            LogManager.Instance.Add("Welcome, " + PhotonNetwork.NickName + ".");
            PhotonNetwork.AutomaticallySyncScene = true;
            LogManager.Instance.Add("Call: ConnectUsingSettings().");
            PhotonNetwork.ConnectUsingSettings();
        }

        private void AutoLoadLevel()
        {
            if (PhotonNetwork.IsMasterClient && PhotonNetwork.PlayerList.Length == 2)
            {
                LogManager.Instance.Add("Call: LoadLevel(). Level Name: Select Operators_Mode 0.");
                PhotonNetwork.LoadLevel("Select Operators_Mode 0");
            }
            else
            {
                LogManager.Instance.Add("Auto Load Level Failed.");
            }
        }
    }
}