using System.Collections;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine;
using UnityEngine.Playables;
using Hashtable = ExitGames.Client.Photon.Hashtable;

namespace com.PROS.SalvationLand
{
    public class SelectOperators : MonoBehaviourPunCallbacks
    {
        public AudioSource backgroundMusic0;
        public PlayableDirector timeline0;

        private void Start()
        {
            LogManager.Instance.Add("Call: LocalPlayer.SetCustomProperties(). Key: " + Lobby.KEY_LOADING_STATUS +
                                    ". Value: 1.");
            PhotonNetwork.LocalPlayer.SetCustomProperties(new Hashtable {{Lobby.KEY_LOADING_STATUS, 1}});
            LogManager.Instance.Add("Custom Call: StartCoroutine(CheckAllStart()).");
            StartCoroutine(CheckAllStart());
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
                        if (loadingStatus < 1)
                        {
                            allStart = false;
                            break;
                        }
                    }
                }

                if (allStart)
                {
                    backgroundMusic0.Play();
                    timeline0.Play();
                    yield break;
                }
            }
        }
    }
}