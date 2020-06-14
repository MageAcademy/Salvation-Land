using System.Collections.Generic;
using UnityEngine;

namespace com.PROS.SalvationLand
{
    public class Teleport : MonoBehaviour
    {
        public static List<Teleport> InstanceList;

        public int mapIndexOneSide;
        public int mapIndexOtherSide;
        public Transform[] marksOneSide;
        public Transform[] marksOtherSide;

        private void Awake()
        {
            if (InstanceList == null)
            {
                InstanceList = new List<Teleport>();
            }

            InstanceList.Add(this);
        }

        public Vector3 GetPosition(int index, bool isOneSide)
        {
            return isOneSide ? marksOtherSide[index].position : marksOneSide[index].position;
        }

        public static Teleport GetTeleport(out int index, out bool isOneSide)
        {
            int currentMapIndex = GameSystem.CurrentOperator.property.currentMapIndex;
            Vector2Int playerCoordinate =
                MapManager.Instance.PositionToCoordinate(currentMapIndex,
                    GameSystem.CurrentOperator.transform.position);
            for (int a = 0; a < InstanceList.Count; ++a)
            {
                if (currentMapIndex == InstanceList[a].mapIndexOneSide)
                {
                    for (int b = 0; b < InstanceList[a].marksOneSide.Length; ++b)
                    {
                        Vector2Int markCoordinate =
                            MapManager.Instance.PositionToCoordinate(currentMapIndex,
                                InstanceList[a].marksOneSide[b].position);
                        if (markCoordinate.x == playerCoordinate.x && markCoordinate.y == playerCoordinate.y)
                        {
                            isOneSide = true;
                            index = b;
                            return InstanceList[a];
                        }
                    }
                }

                if (currentMapIndex == InstanceList[a].mapIndexOtherSide)
                {
                    for (int b = 0; b < InstanceList[a].marksOtherSide.Length; ++b)
                    {
                        Vector2Int markCoordinate =
                            MapManager.Instance.PositionToCoordinate(currentMapIndex,
                                InstanceList[a].marksOtherSide[b].position);
                        if (markCoordinate.x == playerCoordinate.x && markCoordinate.y == playerCoordinate.y)
                        {
                            isOneSide = false;
                            index = b;
                            return InstanceList[a];
                        }
                    }
                }
            }

            isOneSide = false;
            index = -1;
            return null;
        }
    }
}