using Photon.Pun;
using UnityEngine;
using UnityEngine.UI;

namespace com.PROS.SalvationLand
{
    public class ActionPoint : MonoBehaviour
    {
        public static float C;
        public static int CurrentValue;
        public static ActionPoint Instance;

        public Animation animation0;
        public PhotonView photonView;
        public Text text;

        private float m_Probability6Blue;
        private float m_Probability6Red;
        private System.Random m_Random;

        private void Awake()
        {
            Instance = this;
        }

        public void ChangeActionPoint(bool isDelta, int value)
        {
            photonView.RPC(nameof(ChangeActionPointRPC), RpcTarget.AllViaServer, isDelta, value);
        }

        [PunRPC]
        private void ChangeActionPointRPC(bool isDelta, int value)
        {
            if (isDelta)
            {
                CurrentValue += value;
            }
            else
            {
                CurrentValue = value;
            }

            CurrentValue = CurrentValue > 0 ? CurrentValue : 0;
            animation0.Play();
            text.text = CurrentValue.ToString();
            ButtonsSkill.Instance.isSkillsEnabled[5] = true;
            ButtonsSkill.Instance.isSkillsEnabled[6] = true;
            GameTurnTimer.Instance.isNextGameTurnReturned[0] = true;
            GameTurnTimer.Instance.isNextGameTurnReturned[1] = true;
            ButtonsSkill.Instance.Cancel();
        }

        public void Initialize()
        {
            C = RandomNumber.CFromP(1f / 6f);
            m_Probability6Blue = C;
            m_Probability6Red = C;
            int seed = Random.Range(1000, 10000);
            LogManager.Instance.Add("Seed: " + seed + ".");
            m_Random = new System.Random(seed);
        }

        public void Roll1D6()
        {
            float random = (float) m_Random.NextDouble();
            if (GameTurnTimer.Turn == 0)
            {
                if (m_Probability6Blue < random)
                {
                    m_Probability6Blue += C;
                    ChangeActionPoint(false, m_Random.Next() % 5 + 1);
                }
                else
                {
                    m_Probability6Blue = C;
                    ChangeActionPoint(false, 6);
                }
            }
            else if (GameTurnTimer.Turn == 1)
            {
                if (m_Probability6Red < random)
                {
                    m_Probability6Red += C;
                    ChangeActionPoint(false, m_Random.Next() % 5 + 1);
                }
                else
                {
                    m_Probability6Red = C;
                    ChangeActionPoint(false, 6);
                }
            }
        }
    }
}