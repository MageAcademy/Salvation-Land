using System.Collections.Generic;
using Photon.Pun;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace com.PROS.SalvationLand
{
    public class GameSystem : MonoBehaviour
    {
        public static Color[] Colors =
        {
            new Color(6f / 255f, 66f / 255f, 139f / 255f, 1f),
            new Color(235f / 255f, 26f / 255f, 66f / 255f, 1f)
        };

        public static Operator CurrentOperator;
        public static int Index;
        public static GameSystem Instance;
        public static bool IsGameEnded;
        public static List<Operator> OperatorListBlue = new List<Operator>();
        public static List<Operator> OperatorListRed = new List<Operator>();
        public static string[] PlayerNickNames;

        [HideInInspector] public List<OperatorProperty> operatorPropertyListBlue = new List<OperatorProperty>();
        [HideInInspector] public List<OperatorProperty> operatorPropertyListRed = new List<OperatorProperty>();
        public PhotonView photonView;
        public Object[] prefabsCircle;
        public Object[] prefabsOperator;

        private void Awake()
        {
            if (Instance != null && Instance.gameObject != null)
            {
                DestroyImmediate(Instance.gameObject);
            }

            Instance = this;
            DontDestroyOnLoad(gameObject);
            photonView.ViewID = 998;
            Index = -1;
            IsGameEnded = false;
        }

        public void GameSystemInitialize(string parameter)
        {
            LogManager.Instance.Add("RPC: GameSystemInitializeRPC(). RPC Target: " + RpcTarget.AllViaServer + ".");
            photonView.RPC(nameof(GameSystemInitializeRPC), RpcTarget.AllViaServer, parameter);
        }

        [PunRPC]
        private void GameSystemInitializeRPC(string parameter, PhotonMessageInfo info)
        {
            RPCArrayVector2Int startCoordinates = JsonUtility.FromJson<RPCArrayVector2Int>(parameter);
            MapManager.Instance.Initialize();
            OperatorListBlue.Clear();
            for (int a = 0; a < operatorPropertyListBlue.Count; ++a)
            {
                MapManager.Instance.SetNotWalkable(0, startCoordinates.value[a].x, startCoordinates.value[a].y);
                Vector3 position =
                    MapManager.Instance.CoordinateToPosition(0, startCoordinates.value[a].x,
                        startCoordinates.value[a].y);
                OperatorListBlue.Add(
                    (Instantiate(prefabsOperator[operatorPropertyListBlue[a].index], position,
                        Quaternion.Euler(0f, 0f, 0f)) as GameObject).GetComponent<Operator>());
                (Instantiate(prefabsCircle[0], OperatorListBlue[a].transform) as GameObject).transform.localPosition =
                    Vector3.zero;
                OperatorListBlue[a].property = operatorPropertyListBlue[a];
                OperatorListBlue[a].property.currentMapIndex = 0;
                OperatorListBlue[a].viewUnit.group = 0;
            }

            OperatorListRed.Clear();
            for (int a = 0; a < operatorPropertyListRed.Count; ++a)
            {
                MapManager.Instance.SetNotWalkable(1, startCoordinates.value[a + 4].x, startCoordinates.value[a + 4].y);
                Vector3 position = MapManager.Instance.CoordinateToPosition(1, startCoordinates.value[a + 4].x,
                    startCoordinates.value[a + 4].y);
                OperatorListRed.Add(
                    (Instantiate(prefabsOperator[operatorPropertyListRed[a].index], position,
                        Quaternion.Euler(0f, 180f, 0f)) as GameObject).GetComponent<Operator>());
                (Instantiate(prefabsCircle[1], OperatorListRed[a].transform) as GameObject).transform.localPosition =
                    Vector3.zero;
                OperatorListRed[a].property = operatorPropertyListRed[a];
                OperatorListRed[a].property.currentMapIndex = 1;
                OperatorListRed[a].viewUnit.group = 1;
            }

            if (Index == 0)
            {
                CameraController.Instance.rotationY = 0f;
                CurrentOperator = OperatorListBlue[0];
                for (int a = 1; a < OperatorListBlue.Count; ++a)
                {
                    FogOfViewField.Instance.otherViewUnitList.Add(OperatorListBlue[a].viewUnit);
                }

                for (int a = 0; a < OperatorListRed.Count; ++a)
                {
                    FogOfViewField.Instance.otherViewUnitList.Add(OperatorListRed[a].viewUnit);
                }
            }
            else if (Index == 1)
            {
                CameraController.Instance.rotationY = 180f;
                CurrentOperator = OperatorListRed[0];
                for (int a = 1; a < OperatorListRed.Count; ++a)
                {
                    FogOfViewField.Instance.otherViewUnitList.Add(OperatorListRed[a].viewUnit);
                }

                for (int a = 0; a < OperatorListBlue.Count; ++a)
                {
                    FogOfViewField.Instance.otherViewUnitList.Add(OperatorListBlue[a].viewUnit);
                }
            }

            CameraController.FocusTo = CurrentOperator.transform;
            CameraController.Instance.Focus();
            FogOfViewField.Instance.mainViewUnit = CurrentOperator.viewUnit;
            FogOfViewField.Instance.Initialize();
            LogManager.Instance.Add("Custom Call: GameTurnTimer.Instance.StartGameTurn(). Sent Server Time: " +
                                    info.SentServerTime + ".");
            GameTurnTimer.Instance.StartGameTurn(info.SentServerTime);
        }

        public static void BackToLobby()
        {
            IsGameEnded = false;
            Stronghold.InstanceList = null;
            Teleport.InstanceList = null;
            SceneManager.LoadScene("Test");
        }

        public void CalculateSkillCooldown()
        {
            for (int a = 0; a < OperatorListBlue.Count; ++a)
            {
                for (int b = 0; b < OperatorListBlue[a].property.skillCooldown.Length; ++b)
                {
                    if (OperatorListBlue[a].property.skillCooldown[b] > 0)
                    {
                        --OperatorListBlue[a].property.skillCooldown[b];
                    }
                }
            }

            for (int a = 0; a < OperatorListRed.Count; ++a)
            {
                for (int b = 0; b < OperatorListRed[a].property.skillCooldown.Length; ++b)
                {
                    if (OperatorListRed[a].property.skillCooldown[b] > 0)
                    {
                        --OperatorListRed[a].property.skillCooldown[b];
                    }
                }
            }
        }

        public Operator GetOperatorByCoordinate(int mapIndex, int x, int y)
        {
            foreach (Operator item in OperatorListBlue)
            {
                if (item.property.currentMapIndex == mapIndex)
                {
                    Vector2Int coordinate = MapManager.Instance.PositionToCoordinate(mapIndex, item.transform.position);
                    if (coordinate.x == x && coordinate.y == y)
                    {
                        return item;
                    }
                }
            }

            foreach (Operator item in OperatorListRed)
            {
                if (item.property.currentMapIndex == mapIndex)
                {
                    Vector2Int coordinate = MapManager.Instance.PositionToCoordinate(mapIndex, item.transform.position);
                    if (coordinate.x == x && coordinate.y == y)
                    {
                        return item;
                    }
                }
            }

            return null;
        }

        public Operator GetOperatorByIndex(int index)
        {
            foreach (Operator item in OperatorListBlue)
            {
                if (item.property.index == index)
                {
                    return item;
                }
            }

            foreach (Operator item in OperatorListRed)
            {
                if (item.property.index == index)
                {
                    return item;
                }
            }

            return null;
        }

        public bool IsSkillEnabled(int index)
        {
            return CurrentOperator.property.skillCooldown[index] == 0;
        }

        public static void Quit()
        {
#if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
#else
            Application.Quit();
#endif
        }
    }
}