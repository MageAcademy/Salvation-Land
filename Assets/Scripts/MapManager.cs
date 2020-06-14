using System.Collections.Generic;
using UnityEngine;

namespace com.PROS.SalvationLand
{
    public class MapManager : MonoBehaviour
    {
        public static MapManager Instance;

        public float blockSize;
        [HideInInspector] public Map[] maps = new Map[3];
        public Transform[] mapsPivot;
        public Texture2D[] outlines;

        private bool m_IsInitialized;

        private void Awake()
        {
            Instance = this;
        }

        public void Initialize()
        {
            if (m_IsInitialized)
            {
                return;
            }

            #region Map 0 Initialize

            maps[0] = new Map
            {
                height = 10,
                width = 10
            };
            maps[0].blocks = new Block[maps[0].width, maps[0].height];
            for (int a = 0; a < maps[0].height; ++a)
            {
                for (int b = 0; b < maps[0].width; ++b)
                {
                    maps[0].blocks[b, a] = new Block
                    {
                        distance = 0,
                        isVisited = false,
                        isWalkable = true,
                        x = b,
                        y = a
                    };
                    maps[0].walkableBlockList.Add(maps[0].blocks[b, a]);
                }
            }

            #endregion

            #region Map 1 Initialize

            maps[1] = new Map
            {
                height = 10,
                width = 10
            };
            maps[1].blocks = new Block[maps[1].width, maps[1].height];
            for (int a = 0; a < maps[1].height; ++a)
            {
                for (int b = 0; b < maps[1].width; ++b)
                {
                    maps[1].blocks[b, a] = new Block
                    {
                        distance = 0,
                        isVisited = false,
                        isWalkable = true,
                        x = b,
                        y = a
                    };
                    maps[1].walkableBlockList.Add(maps[1].blocks[b, a]);
                }
            }

            #endregion

            #region Map 2 Initialize

            maps[2] = new Map
            {
                height = 20,
                width = 20
            };
            maps[2].blocks = new Block[maps[2].width, maps[2].height];
            for (int a = 0; a < maps[2].height; ++a)
            {
                for (int b = 0; b < maps[2].width; ++b)
                {
                    maps[2].blocks[b, a] = new Block
                    {
                        distance = 0,
                        isVisited = false,
                        isWalkable = true,
                        x = b,
                        y = a
                    };
                    maps[2].walkableBlockList.Add(maps[2].blocks[b, a]);
                }
            }

            #endregion

            m_IsInitialized = true;
        }

        public Vector3 CoordinateToPosition(int mapIndex, int x, int y)
        {
            return new Vector3(mapsPivot[mapIndex].position.x + blockSize * (x + 0.5f), 0f,
                mapsPivot[mapIndex].position.z - blockSize * (y + 0.5f));
        }

        public Vector2Int PositionToCoordinate(int mapIndex, Vector3 position)
        {
            Vector3 localPosition = (position - mapsPivot[mapIndex].position) / blockSize;
            return new Vector2Int(Mathf.FloorToInt(localPosition.x), Mathf.FloorToInt(-localPosition.z));
        }

        public void SetNotWalkable(int mapIndex, int x, int y)
        {
            for (int a = 0; a < maps[mapIndex].walkableBlockList.Count; ++a)
            {
                if (maps[mapIndex].walkableBlockList[a].x == x && maps[mapIndex].walkableBlockList[a].y == y)
                {
                    maps[mapIndex].blocks[x, y].isWalkable = false;
                    maps[mapIndex].notWalkableBlockList.Add(maps[mapIndex].blocks[x, y]);
                    maps[mapIndex].walkableBlockList.RemoveAt(a);
                    return;
                }
            }
        }

        public void SetWalkable(int mapIndex, int x, int y)
        {
            for (int a = 0; a < maps[mapIndex].notWalkableBlockList.Count; ++a)
            {
                if (maps[mapIndex].notWalkableBlockList[a].x == x && maps[mapIndex].notWalkableBlockList[a].y == y)
                {
                    maps[mapIndex].blocks[x, y].isWalkable = true;
                    maps[mapIndex].walkableBlockList.Add(maps[mapIndex].blocks[x, y]);
                    maps[mapIndex].notWalkableBlockList.RemoveAt(a);
                    return;
                }
            }
        }

        public List<Block> GetReachableBlockList(int mapIndex, Vector3 playerPosition, int maxDistance)
        {
            Vector2Int playerCoordinate = PositionToCoordinate(mapIndex, playerPosition);
            Block playerBlock = maps[mapIndex].blocks[playerCoordinate.x, playerCoordinate.y];
            playerBlock.distance = 0;
            playerBlock.isVisited = true;
            List<Block> reachableBlockList = new List<Block> {playerBlock};
            for (int a = 0; a < reachableBlockList.Count; ++a)
            {
                int currentDistance = reachableBlockList[a].distance + 1;
                if (currentDistance > maxDistance)
                {
                    break;
                }

                int currentX = reachableBlockList[a].x;
                int currentY = reachableBlockList[a].y;
                if (currentY < maps[mapIndex].height - 1)
                {
                    Block block = maps[mapIndex].blocks[currentX, currentY + 1];
                    if (!block.isVisited && block.isWalkable)
                    {
                        block.distance = currentDistance;
                        block.isVisited = true;
                        block.lastIndex = a;
                        reachableBlockList.Add(block);
                    }
                }

                if (currentX > 0)
                {
                    Block block = maps[mapIndex].blocks[currentX - 1, currentY];
                    if (!block.isVisited && block.isWalkable)
                    {
                        block.distance = currentDistance;
                        block.isVisited = true;
                        block.lastIndex = a;
                        reachableBlockList.Add(block);
                    }
                }

                if (currentX < maps[mapIndex].width - 1)
                {
                    Block block = maps[mapIndex].blocks[currentX + 1, currentY];
                    if (!block.isVisited && block.isWalkable)
                    {
                        block.distance = currentDistance;
                        block.isVisited = true;
                        block.lastIndex = a;
                        reachableBlockList.Add(block);
                    }
                }

                if (currentY > 0)
                {
                    Block block = maps[mapIndex].blocks[currentX, currentY - 1];
                    if (!block.isVisited && block.isWalkable)
                    {
                        block.distance = currentDistance;
                        block.isVisited = true;
                        block.lastIndex = a;
                        reachableBlockList.Add(block);
                    }
                }
            }

            foreach (Block item in reachableBlockList)
            {
                item.isVisited = false;
            }

            return reachableBlockList;
        }

        public List<Vector3> GetPath(List<Block> reachableBlockList, int mapIndex, int targetX, int targetY)
        {
            List<int> indexList = new List<int>();
            for (int a = 0; a < reachableBlockList.Count; ++a)
            {
                if (reachableBlockList[a].x == targetX && reachableBlockList[a].y == targetY)
                {
                    indexList.Add(a);
                    break;
                }
            }

            for (int a = 0;; ++a)
            {
                if (indexList[a] == 0)
                {
                    break;
                }

                indexList.Add(reachableBlockList[indexList[a]].lastIndex);
            }

            List<Vector3> path = new List<Vector3>();
            for (int a = indexList.Count - 1; a >= 0; --a)
            {
                path.Add(CoordinateToPosition(mapIndex, reachableBlockList[indexList[a]].x,
                    reachableBlockList[indexList[a]].y));
            }

            return path;
        }
    }
}