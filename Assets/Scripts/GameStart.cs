using System.Collections;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine;
using Hashtable = ExitGames.Client.Photon.Hashtable;

namespace com.PROS.SalvationLand
{
    public class GameStart : MonoBehaviourPunCallbacks
    {
        private void Start()
        {
            LogManager.Instance.Add("Call: LocalPlayer.SetCustomProperties(). Key: " + Lobby.KEY_LOADING_STATUS +
                                    ". Value: 2.");
            PhotonNetwork.LocalPlayer.SetCustomProperties(new Hashtable {{Lobby.KEY_LOADING_STATUS, 2}});
            if (PhotonNetwork.IsMasterClient)
            {
                LogManager.Instance.Add("Custom Call: StartCoroutine(CheckAllStart()).");
                StartCoroutine(CheckAllStart());
            }
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

        private IEnumerator CheckAllStart()
        {
            while (true)
            {
                yield return 0;
                bool allStart = true;
                foreach (Player item in PhotonNetwork.PlayerList)
                {
                    if (item.CustomProperties.TryGetValue(Lobby.KEY_LOADING_STATUS, out object value))
                    {
                        int loadingStatus = (int) value;
                        if (loadingStatus < 2)
                        {
                            allStart = false;
                            break;
                        }
                    }
                }

                if (allStart)
                {
                    MapManager.Instance.Initialize();
                    RPCArrayVector2Int startCoordinates = new RPCArrayVector2Int {value = new Vector2Int[8]};
                    for (int a = 0; a < 4; ++a)
                    {
                        Block block = MapManager.Instance.maps[0]
                            .walkableBlockList[Random.Range(0, MapManager.Instance.maps[0].walkableBlockList.Count)];
                        MapManager.Instance.SetNotWalkable(0, block.x, block.y);
                        startCoordinates.value[a] = new Vector2Int(block.x, block.y);
                    }

                    for (int a = 4; a < 8; ++a)
                    {
                        Block block = MapManager.Instance.maps[1]
                            .walkableBlockList[Random.Range(0, MapManager.Instance.maps[1].walkableBlockList.Count)];
                        MapManager.Instance.SetNotWalkable(1, block.x, block.y);
                        startCoordinates.value[a] = new Vector2Int(block.x, block.y);
                    }

                    LogManager.Instance.Add("Custom Call: GameSystem.Instance.GameSystemInitialize().");
                    GameSystem.Instance.GameSystemInitialize(JsonUtility.ToJson(startCoordinates));
                    yield break;
                }
            }
        }
    }
}