using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using UnityEngine;
using Object = UnityEngine.Object;

namespace com.PROS.SalvationLand
{
    public class SkillLibrary : MonoBehaviour
    {
        #region VARIABLES

        public static SkillLibrary Instance;

        public static string[] SkillNames =
        {
            "移动", //0
            "", //1
            "", //2
            "技能一", //3
            "技能二", //4
            "技能三", //5
            "钝击", //6
            "", //7
        };

        public PhotonView photonView;
        public Object prefabReachableMark;
        public Object prefabEffectSkill7;
        public Object prefabSkillMark;
        public Transform reachableMarkPivot;
        public Transform skillMarkPivot;

        private Operator m_CurrentOperator;
        private List<Block> m_ReachableBlockList;

        #endregion

        private void Awake()
        {
            Instance = this;
        }

        public void CastSkill(int operatorIndex, int skillIndex, int skillLevel, object parameter = null)
        {
            CastSkillLocally(operatorIndex, skillIndex, skillLevel, parameter);
            photonView.RPC(nameof(CastSkillRPC), RpcTarget.All, operatorIndex, skillIndex, skillLevel, parameter);
        }

        private void CastSkillLocally(int operatorIndex, int skillIndex, int skillLevel, object parameter)
        {
            m_CurrentOperator = GameSystem.Instance.GetOperatorByIndex(operatorIndex);
            if (skillIndex == 0)
            {
                #region SKILL 0 移动 指示器

                ReachableMark.SkillLevel = skillLevel;
                int maxDistance = 0;
                if (skillLevel == 1)
                {
                    maxDistance = ActionPoint.CurrentValue;
                }
                else if (skillLevel == 2)
                {
                    maxDistance = ActionPoint.CurrentValue / 3 * 4 + ActionPoint.CurrentValue % 3;
                }
                else if (skillLevel == 3)
                {
                    maxDistance = ActionPoint.CurrentValue / 2 * 3 + ActionPoint.CurrentValue % 2;
                }
                else if (skillLevel == 4)
                {
                    maxDistance = ActionPoint.CurrentValue * 2;
                }

                m_ReachableBlockList = MapManager.Instance.GetReachableBlockList(
                    m_CurrentOperator.property.currentMapIndex, m_CurrentOperator.transform.position, maxDistance);
                List<ReachableMark> reachableMarkList = new List<ReachableMark>();
                for (int a = 1; a < m_ReachableBlockList.Count; ++a)
                {
                    ReachableMark reachableMark = (Instantiate(prefabReachableMark, reachableMarkPivot) as GameObject)
                        ?.GetComponent<ReachableMark>();
                    reachableMark.distance = m_ReachableBlockList[a].distance;
                    reachableMark.isOutlineExists = new[] {true, true, true, true};
                    reachableMark.transform.localScale = new Vector3(MapManager.Instance.blockSize,
                        MapManager.Instance.blockSize, 1f);
                    reachableMark.x = m_ReachableBlockList[a].x;
                    reachableMark.y = m_ReachableBlockList[a].y;
                    reachableMark.transform.position =
                        MapManager.Instance.CoordinateToPosition(m_CurrentOperator.property.currentMapIndex,
                            reachableMark.x, reachableMark.y);
                    reachableMarkList.Add(reachableMark);
                }

                for (int a = 0; a < reachableMarkList.Count - 1; ++a)
                {
                    int x = reachableMarkList[a].x;
                    int y = reachableMarkList[a].y;
                    for (int b = a + 1; b < reachableMarkList.Count; ++b)
                    {
                        if (reachableMarkList[b].x == x && reachableMarkList[b].y == y + 1)
                        {
                            reachableMarkList[a].isOutlineExists[0] = false;
                            reachableMarkList[b].isOutlineExists[3] = false;
                        }

                        if (reachableMarkList[b].x == x - 1 && reachableMarkList[b].y == y)
                        {
                            reachableMarkList[a].isOutlineExists[1] = false;
                            reachableMarkList[b].isOutlineExists[2] = false;
                        }

                        if (reachableMarkList[b].x == x + 1 && reachableMarkList[b].y == y)
                        {
                            reachableMarkList[a].isOutlineExists[2] = false;
                            reachableMarkList[b].isOutlineExists[1] = false;
                        }

                        if (reachableMarkList[b].x == x && reachableMarkList[b].y == y - 1)
                        {
                            reachableMarkList[a].isOutlineExists[3] = false;
                            reachableMarkList[b].isOutlineExists[0] = false;
                        }
                    }

                    reachableMarkList[a].SetOutline();
                }

                reachableMarkList[reachableMarkList.Count - 1].SetOutline();

                #endregion
            }
            else if (skillIndex == 1)
            {
                #region SKILL 1 移动 施放 阶段1

                ClearMarks();
                int[] value = (int[]) parameter;
                if (skillLevel == 1)
                {
                    ActionPoint.Instance.ChangeActionPoint(true, -value[0]);
                }
                else if (skillLevel == 2)
                {
                    ActionPoint.Instance.ChangeActionPoint(true, -(value[0] / 4 * 3 + value[0] % 4));
                }
                else if (skillLevel == 3)
                {
                    ActionPoint.Instance.ChangeActionPoint(true, -(value[0] / 3 * 2 + value[0] % 3));
                }
                else if (skillLevel == 4)
                {
                    ActionPoint.Instance.ChangeActionPoint(true, -(value[0] / 2 + value[0] % 2));
                }

                RPCListVector3 path = new RPCListVector3();
                path.value = MapManager.Instance.GetPath(m_ReachableBlockList,
                    m_CurrentOperator.property.currentMapIndex, value[1], value[2]);
                CastSkill(m_CurrentOperator.property.index, 2, skillLevel, JsonUtility.ToJson(path));

                #endregion
            }
            else if (skillIndex == 6)
            {
                #region SKILL 6 钝击

                SkillMark.SkillButtonIndex = 1;
                SkillMark.SkillIndex = 7;
                SkillMark.SkillLevel = skillLevel;
                Vector2Int playerCoordinate =
                    MapManager.Instance.PositionToCoordinate(m_CurrentOperator.property.currentMapIndex,
                        m_CurrentOperator.transform.position);
                if (playerCoordinate.y + 1 <
                    MapManager.Instance.maps[m_CurrentOperator.property.currentMapIndex].height)
                {
                    SkillMark skillMark = (Instantiate(prefabSkillMark, skillMarkPivot) as GameObject)
                        ?.GetComponent<SkillMark>();
                    skillMark.transform.localScale =
                        new Vector3(MapManager.Instance.blockSize, MapManager.Instance.blockSize, 1f);
                    skillMark.x = playerCoordinate.x;
                    skillMark.y = playerCoordinate.y + 1;
                    skillMark.transform.position =
                        MapManager.Instance.CoordinateToPosition(m_CurrentOperator.property.currentMapIndex,
                            skillMark.x, skillMark.y);
                }

                if (playerCoordinate.x - 1 > 0)
                {
                    SkillMark skillMark = (Instantiate(prefabSkillMark, skillMarkPivot) as GameObject)
                        ?.GetComponent<SkillMark>();
                    skillMark.transform.localScale =
                        new Vector3(MapManager.Instance.blockSize, MapManager.Instance.blockSize, 1f);
                    skillMark.x = playerCoordinate.x - 1;
                    skillMark.y = playerCoordinate.y;
                    skillMark.transform.position =
                        MapManager.Instance.CoordinateToPosition(m_CurrentOperator.property.currentMapIndex,
                            skillMark.x, skillMark.y);
                }

                if (playerCoordinate.x + 1 <
                    MapManager.Instance.maps[m_CurrentOperator.property.currentMapIndex].width)
                {
                    SkillMark skillMark = (Instantiate(prefabSkillMark, skillMarkPivot) as GameObject)
                        ?.GetComponent<SkillMark>();
                    skillMark.transform.localScale =
                        new Vector3(MapManager.Instance.blockSize, MapManager.Instance.blockSize, 1f);
                    skillMark.x = playerCoordinate.x + 1;
                    skillMark.y = playerCoordinate.y;
                    skillMark.transform.position =
                        MapManager.Instance.CoordinateToPosition(m_CurrentOperator.property.currentMapIndex,
                            skillMark.x, skillMark.y);
                }

                if (playerCoordinate.y - 1 > 0)
                {
                    SkillMark skillMark = (Instantiate(prefabSkillMark, skillMarkPivot) as GameObject)
                        ?.GetComponent<SkillMark>();
                    skillMark.transform.localScale =
                        new Vector3(MapManager.Instance.blockSize, MapManager.Instance.blockSize, 1f);
                    skillMark.x = playerCoordinate.x;
                    skillMark.y = playerCoordinate.y - 1;
                    skillMark.transform.position =
                        MapManager.Instance.CoordinateToPosition(m_CurrentOperator.property.currentMapIndex,
                            skillMark.x, skillMark.y);
                }

                #endregion
            }
        }

        [PunRPC]
        private void CastSkillRPC(int operatorIndex, int skillIndex, int skillLevel, object parameter,
            PhotonMessageInfo info)
        {
            m_CurrentOperator = GameSystem.Instance.GetOperatorByIndex(operatorIndex);
            if (skillIndex == 2)
            {
                #region SKILL 2 移动 施放 阶段2

                string json = (string) parameter;
                RPCListVector3 path = JsonUtility.FromJson<RPCListVector3>(json);
                Vector2Int coordinate =
                    MapManager.Instance.PositionToCoordinate(m_CurrentOperator.property.currentMapIndex, path.value[0]);
                MapManager.Instance.SetWalkable(m_CurrentOperator.property.currentMapIndex, coordinate.x, coordinate.y);
                coordinate = MapManager.Instance.PositionToCoordinate(m_CurrentOperator.property.currentMapIndex,
                    path.value[path.value.Count - 1]);
                MapManager.Instance.SetNotWalkable(m_CurrentOperator.property.currentMapIndex, coordinate.x,
                    coordinate.y);
                StartCoroutine(Skill2(path.value));

                #endregion
            }
            else if (skillIndex == 7)
            {
                #region SKILL 7 钝击 施放

                ClearMarks();
                int[] coordinate = (int[]) parameter;
                Instantiate(prefabEffectSkill7,
                    MapManager.Instance.CoordinateToPosition(m_CurrentOperator.property.currentMapIndex, coordinate[0],
                        coordinate[1]), Quaternion.identity);
                Operator target =
                    GameSystem.Instance.GetOperatorByCoordinate(m_CurrentOperator.property.currentMapIndex,
                        coordinate[0], coordinate[1]);
                if (target != null)
                {
                    target.property.currentHealth = Mathf.Clamp(target.property.currentHealth - skillLevel, 0,
                        target.property.maxHealth);
                }
                ButtonsOperator.Instance.Refresh();
                ButtonsSkill.Instance.isAnimationsPlaying[1] = false;
                ButtonsSkill.Instance.Refresh();

                #endregion
            }
            else if (skillIndex == 100)
            {
                #region SKILL 100 技能等级提升

                int value = (int) parameter;
                ++m_CurrentOperator.property.currentSkillLevel[value];
                --m_CurrentOperator.property.skillPoint;
                ButtonsSkill.Instance.isSkillsEnabled[4] = true;
                ButtonsSkill.Instance.Cancel();

                #endregion
            }
            else if (skillIndex == 101)
            {
                #region SKILL 101 据点争夺

                int playerIndex = (int) parameter;
                StrongholdPoint.Instance.AddStrongholdPoint(playerIndex);
                if (GameSystem.Index == playerIndex)
                {
                    ActionPoint.Instance.ChangeActionPoint(true, -1);
                }

                #endregion
            }
            else if (skillIndex == 102)
            {
                #region SKILL 102 传送

                string json = (string) parameter;
                RPCInt_Vector3 teleportPosition = JsonUtility.FromJson<RPCInt_Vector3>(json);
                m_CurrentOperator.property.currentMapIndex = teleportPosition.valueInt;
                m_CurrentOperator.transform.position = teleportPosition.valueVector3;
                if (GameSystem.Index == skillLevel)
                {
                    ActionPoint.Instance.ChangeActionPoint(true, -1);
                }

                #endregion
            }
        }

        public void ClearMarks()
        {
            List<GameObject> markList = new List<GameObject>();
            for (int a = 0; a < reachableMarkPivot.childCount; ++a)
            {
                markList.Add(reachableMarkPivot.GetChild(a).gameObject);
            }

            for (int a = 0; a < skillMarkPivot.childCount; ++a)
            {
                markList.Add(skillMarkPivot.GetChild(a).gameObject);
            }

            foreach (GameObject item in markList)
            {
                Destroy(item);
            }
        }

        private IEnumerator Skill2(List<Vector3> path)
        {
            m_CurrentOperator.animator.SetInteger("Status", 1);
            float startTime = Time.time;
            float totalTime = (path.Count - 1) * 0.3f;
            while (true)
            {
                float timeInterval = Time.time - startTime;
                if (timeInterval < totalTime)
                {
                    int index = Mathf.FloorToInt(timeInterval / 0.3f);
                    m_CurrentOperator.transform.position =
                        Vector3.Lerp(path[index], path[index + 1], timeInterval / 0.3f - index);
                    m_CurrentOperator.transform.forward = path[index + 1] - path[index];
                }
                else
                {
                    m_CurrentOperator.animator.SetInteger("Status", 0);
                    m_CurrentOperator.transform.position = path[path.Count - 1];
                    m_CurrentOperator.transform.forward = path[path.Count - 1] - path[path.Count - 2];
                    ButtonsSkill.Instance.isAnimationsPlaying[0] = false;
                    ButtonsSkill.Instance.Refresh();
                    yield break;
                }

                yield return 0;
            }
        }
    }
}