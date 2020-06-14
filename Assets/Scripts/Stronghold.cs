using System.Collections.Generic;
using UnityEngine;

namespace com.PROS.SalvationLand
{
    public class Stronghold : MonoBehaviour
    {
        public static List<Stronghold> InstanceList;

        public int mapIndex;
        public Transform[] marks;

        private void Awake()
        {
            if (InstanceList == null)
            {
                InstanceList = new List<Stronghold>();
            }

            InstanceList.Add(this);
        }

        public static bool IsButtonStrongholdEnabled()
        {
            int currentMapIndex = GameSystem.CurrentOperator.property.currentMapIndex;
            Vector2Int playerCoordinate =
                MapManager.Instance.PositionToCoordinate(currentMapIndex,
                    GameSystem.CurrentOperator.transform.position);
            for (int a = 0; a < InstanceList.Count; ++a)
            {
                if (currentMapIndex != InstanceList[a].mapIndex)
                {
                    continue;
                }

                for (int b = 0; b < InstanceList[a].marks.Length; ++b)
                {
                    Vector2Int markCoordinate =
                        MapManager.Instance.PositionToCoordinate(currentMapIndex, InstanceList[a].marks[b].position);
                    if (markCoordinate.x == playerCoordinate.x && markCoordinate.y == playerCoordinate.y)
                    {
                        return true;
                    }
                }
            }

            return false;
        }
    }
}