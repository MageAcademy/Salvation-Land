using Photon.Pun;
using UnityEngine;
using UnityEngine.UI;

namespace com.PROS.SalvationLand
{
    public class GameTurnTimer : MonoBehaviour
    {
        private const float MAX_TIME_PER_TURN = 30f;

        public static GameTurnTimer Instance;
        public static int Turn;

        public AudioSource backgroundMusic0;
        public Transform imageTimer;
        [HideInInspector] public bool[] isNextGameTurnReturned = {true, true};
        public PhotonView photonView;
        public Text textPlayerNickName;

        private bool m_IsStarted;
        private float m_Time = MAX_TIME_PER_TURN;

        private void Awake()
        {
            Instance = this;
            Turn = -1;
        }

        private void Update()
        {
            if (m_IsStarted)
            {
                m_Time -= Time.deltaTime;
                if (m_Time < 0f)
                {
                    m_Time = 0f;
                    if (GameSystem.Index == Turn || (PhotonNetwork.IsMasterClient && Turn == -1))
                    {
                        NextGameTurn();
                    }
                }

                imageTimer.localScale = new Vector3(m_Time / MAX_TIME_PER_TURN, 1f);
            }
        }

        [PunRPC]
        private void NextGameTurnRPC(PhotonMessageInfo info)
        {
            if (isNextGameTurnReturned[1])
            {
                float lag = Mathf.Abs((float) (PhotonNetwork.Time - info.SentServerTime));
                LogManager.Instance.Add("RPC Lag: " + lag + ".");
                isNextGameTurnReturned[1] = false;
                GameSystem.Instance.CalculateSkillCooldown();
                ResetTimer(lag);
            }
        }

        public void NextGameTurn()
        {
            if (isNextGameTurnReturned[0] && isNextGameTurnReturned[1])
            {
                isNextGameTurnReturned[0] = false;
                LogManager.Instance.Add("RPC: NextGameTurnRPC(). RPC Target: " + RpcTarget.AllViaServer + ".");
                photonView.RPC(nameof(NextGameTurnRPC), RpcTarget.AllViaServer);
            }
        }

        private void ResetTimer(float lag)
        {
            m_Time = MAX_TIME_PER_TURN - lag;
            ++Turn;
            if (Turn == PhotonNetwork.PlayerList.Length)
            {
                Turn = 0;
            }

            ButtonsSkill.Instance.Cancel();
            textPlayerNickName.text = PhotonNetwork.PlayerList[Turn].NickName + "的回合";
            if (PhotonNetwork.IsMasterClient)
            {
                ActionPoint.Instance.Roll1D6();
            }
        }

        public void StartGameTurn(double sentServerTime)
        {
            backgroundMusic0.Play();
            m_IsStarted = true;
            float lag = Mathf.Abs((float) (PhotonNetwork.Time - sentServerTime));
            LogManager.Instance.Add("RPC Lag: " + lag + ".");
            m_Time = 5f - lag;
            imageTimer.localScale = new Vector3(m_Time / MAX_TIME_PER_TURN, 1f);
            textPlayerNickName.text = "准备时间";
            if (PhotonNetwork.IsMasterClient)
            {
                ActionPoint.Instance.Initialize();
            }

            ButtonsOperator.Instance.Refresh();
            ButtonsSkill.Instance.Refresh();
        }
    }
}