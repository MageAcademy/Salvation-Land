using System.Collections;
using Photon.Pun;
using UnityEngine;
using UnityEngine.UI;

namespace com.PROS.SalvationLand
{
    public class SelectTurnTimer : MonoBehaviour
    {
        private const float MAX_TIME_PER_TURN = 15f;

        public static SelectTurnTimer Instance;
        public static int Turn;

        public AudioSource backgroundMusic0;
        public ButtonSelectOperator[] buttonSelectOperators;
        public Transform imageTimer;
        public TextAsset jsonOperators;
        public GameObject[] meshOperators;
        [HideInInspector] public OperatorProperty[] operatorsProperty;
        public PhotonView photonView;
        public Text textOperatorName;
        public Text textPlayerNickName;

        private bool m_IsStarted;
        private float m_Time = MAX_TIME_PER_TURN;
        private int[] m_TurnOrder = {0, 1, 1, 0, 0, 1, 1, 0};
        private int m_TurnOrderIndex = 0;

        private void Awake()
        {
            Instance = this;
            Turn = -1;
        }

        private void Start()
        {
            operatorsProperty = JsonUtility.FromJson<UserData1>(jsonOperators.text).operatorsProperty;
        }

        private void Update()
        {
            if (m_IsStarted)
            {
                m_Time -= Time.deltaTime;
                if (m_Time < 0f)
                {
                    m_Time = 0f;
                    if (GameSystem.Index == Turn)
                    {
                        GameSystem.Quit();
                    }
                }

                imageTimer.localScale = new Vector3(m_Time / MAX_TIME_PER_TURN, 1f);
            }
        }

        [PunRPC]
        private void NextSelectTurnRPC(int index, PhotonMessageInfo info)
        {
            if (Turn == 0)
            {
                GameSystem.Instance.operatorPropertyListBlue.Add(operatorsProperty[index]);
            }
            else if (Turn == 1)
            {
                GameSystem.Instance.operatorPropertyListRed.Add(operatorsProperty[index]);
            }

            buttonSelectOperators[index].button.interactable = false;
            buttonSelectOperators[index].selectedBy = Turn;
            ++m_TurnOrderIndex;
            if (m_TurnOrderIndex == 8)
            {
                textPlayerNickName.text = "选择完毕  正在进入游戏";
                Turn = -1;
                StartCoroutine(WaitForGameStart());
            }
            else
            {
                float lag = Mathf.Abs((float) (PhotonNetwork.Time - info.SentServerTime));
                LogManager.Instance.Add("RPC Lag: " + lag + ".");
                ResetTimer(lag);
            }
        }

        [PunRPC]
        private void StartSelectTurnRPC(PhotonMessageInfo info)
        {
            GameSystem.Instance.operatorPropertyListBlue.Clear();
            GameSystem.Instance.operatorPropertyListRed.Clear();
            GameSystem.PlayerNickNames = new string[PhotonNetwork.PlayerList.Length];
            for (int a = 0; a < PhotonNetwork.PlayerList.Length; ++a)
            {
                GameSystem.PlayerNickNames[a] = PhotonNetwork.PlayerList[a].NickName;
                if (PhotonNetwork.LocalPlayer.ActorNumber == PhotonNetwork.PlayerList[a].ActorNumber)
                {
                    GameSystem.Index = a;
                }
            }

            foreach (ButtonSelectOperator item in buttonSelectOperators)
            {
                item.enabled = true;
                item.Initialize();
            }

            m_IsStarted = true;
            float lag = Mathf.Abs((float) (PhotonNetwork.Time - info.SentServerTime));
            LogManager.Instance.Add("RPC Lag: " + lag + ".");
            ResetTimer(lag);
        }

        public void NextSelectTurn(int index)
        {
            LogManager.Instance.Add(
                "RPC: NextSelectTurnRPC(). RPC Target: " + RpcTarget.AllViaServer + ". Index: " + index + ".");
            photonView.RPC(nameof(NextSelectTurnRPC), RpcTarget.AllViaServer, index);
        }

        private void ResetTimer(float lag)
        {
            m_Time = MAX_TIME_PER_TURN - lag;
            Turn = m_TurnOrder[m_TurnOrderIndex];
            textPlayerNickName.text = PhotonNetwork.PlayerList[Turn].NickName + "  正在选择";
        }

        private void StartSelectTurn()
        {
            if (PhotonNetwork.IsMasterClient)
            {
                LogManager.Instance.Add("RPC: StartSelectTurnRPC(). RPC Target: " + RpcTarget.AllViaServer + ".");
                photonView.RPC(nameof(StartSelectTurnRPC), RpcTarget.AllViaServer);
            }
        }

        private IEnumerator WaitForGameStart()
        {
            float startTime = Time.time;
            while (true)
            {
                yield return 0;
                float timeInterval = Time.time - startTime;
                if (timeInterval < 5f)
                {
                    backgroundMusic0.volume = Mathf.Lerp(backgroundMusic0.volume, 0f, Time.deltaTime);
                }
                else if (PhotonNetwork.IsMasterClient)
                {
                    LogManager.Instance.Add("Call: LoadLevel(). Level Name: Game_Map 0.");
                    PhotonNetwork.LoadLevel("Game_Map 0");
                    yield break;
                }
            }
        }
    }
}