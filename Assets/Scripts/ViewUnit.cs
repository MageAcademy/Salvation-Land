using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace com.PROS.SalvationLand
{
    public class ViewUnit : MonoBehaviour
    {
        public class FogCache
        {
            public List<Coordinate> fogCoordinateList = new List<Coordinate>();
            public Coordinate obstructionCoordinate;
        }

        [HideInInspector] public List<FogCache> fogCacheList;
        public int group;
        [HideInInspector] public List<Coordinate> offsetCacheList;
        public Renderer[] renderers;
        public float viewFieldRadius;

        private static int _halfOffsetLength;
        private static float _maxViewFieldRadius = 10;
        private static Coordinate[,] _offsets;

        public void Initialize()
        {
            if (_offsets == null)
            {
                _halfOffsetLength = (int) (_maxViewFieldRadius / FogOfViewField.Instance.gridHeight);
                int offsetLength = _halfOffsetLength * 2 + 1;
                _offsets = new Coordinate[offsetLength, offsetLength];
                for (int a = 0; a < offsetLength; ++a)
                {
                    for (int b = 0; b < offsetLength; ++b)
                    {
                        _offsets[a, b] = new Coordinate {x = a - _halfOffsetLength, y = b - _halfOffsetLength};
                    }
                }
            }

            GetCache();
        }

        private void GetCache()
        {
            //缓存所有视野范围内的小格坐标相对于该视野单元坐标的偏移
            int distance = 0;
            Coordinate offset = _offsets[_halfOffsetLength, _halfOffsetLength];
            offset.distance = distance;
            offset.isVisited = true;
            offsetCacheList = new List<Coordinate> {offset};
            float targetDistance = viewFieldRadius / FogOfViewField.Instance.gridHeight;
            targetDistance *= targetDistance;
            for (int a = 0; a < offsetCacheList.Count; ++a)
            {
                int x = offsetCacheList[a].x;
                int y = offsetCacheList[a].y;
                distance = (x - 1) * (x - 1) + y * y;
                offset = _offsets[_halfOffsetLength + x - 1, _halfOffsetLength + y];
                if (distance < targetDistance && !offset.isVisited)
                {
                    offset.distance = distance;
                    offset.isVisited = true;
                    offsetCacheList.Add(offset);
                }

                distance = (x + 1) * (x + 1) + y * y;
                offset = _offsets[_halfOffsetLength + x + 1, _halfOffsetLength + y];
                if (distance < targetDistance && !offset.isVisited)
                {
                    offset.distance = distance;
                    offset.isVisited = true;
                    offsetCacheList.Add(offset);
                }

                distance = x * x + (y - 1) * (y - 1);
                offset = _offsets[_halfOffsetLength + x, _halfOffsetLength + y - 1];
                if (distance < targetDistance && !offset.isVisited)
                {
                    offset.distance = distance;
                    offset.isVisited = true;
                    offsetCacheList.Add(offset);
                }

                distance = x * x + (y + 1) * (y + 1);
                offset = _offsets[_halfOffsetLength + x, _halfOffsetLength + y + 1];
                if (distance < targetDistance && !offset.isVisited)
                {
                    offset.distance = distance;
                    offset.isVisited = true;
                    offsetCacheList.Add(offset);
                }
            }

            offsetCacheList = offsetCacheList.OrderByDescending(item => item.distance).ToList();

            //缓存1/8个圆内障碍物产生迷雾的情况
            fogCacheList = new List<FogCache>();
            for (int a = 0; a < offsetCacheList.Count - 1; ++a)
            {
                offset = offsetCacheList[a];
                if (offset.x >= offset.y && offset.y >= 0)
                {
                    FogCache fogCache = new FogCache {obstructionCoordinate = offset};
                    float slopeLeftBottom = (offset.y - 0.5f) / (offset.x - 0.5f);
                    float slopeLeftUp = (offset.y + 0.5f) / (offset.x - 0.5f);
                    float slopeRightBottom = (offset.y - 0.5f) / (offset.x + 0.5f);
                    float maxSlope = Mathf.Max(slopeLeftBottom, slopeLeftUp, slopeRightBottom);
                    float minSlope = Mathf.Min(slopeLeftBottom, slopeLeftUp, slopeRightBottom);
                    for (int b = 0; b < a; ++b)
                    {
                        Coordinate fog = offsetCacheList[b];
                        if (fog.x > 0)
                        {
                            float slope = (float) fog.y / fog.x;
                            if (maxSlope > slope && minSlope < slope)
                            {
                                fogCache.fogCoordinateList.Add(fog);
                            }
                        }
                    }

                    fogCacheList.Add(fogCache);
                }
            }

            //缓存完毕，部分中间量恢复初始值
            foreach (Coordinate item in offsetCacheList)
            {
                item.distance = 0;
                item.isVisited = false;
            }
        }

        public List<Coordinate> GetFogCoordinateList(Coordinate obstructionOffsetCoordinate)
        {
            //将1/8个圆形的信息解压为1个圆形的信息
            Coordinate coordinate = new Coordinate
                {x = obstructionOffsetCoordinate.x, y = obstructionOffsetCoordinate.y};
            int symmetryAxis0 = coordinate.x < 0 ? -1 : 1;
            int symmetryAxis1 = coordinate.y < 0 ? -1 : 1;
            coordinate.x *= symmetryAxis0;
            coordinate.y *= symmetryAxis1;
            bool symmetryAxis2 = coordinate.x >= coordinate.y;
            if (!symmetryAxis2)
            {
                int a = coordinate.x;
                coordinate.x = coordinate.y;
                coordinate.y = a;
            }

            List<Coordinate> cachedFogCoordinateList = null;
            foreach (FogCache item in fogCacheList)
            {
                if (item.obstructionCoordinate.x == coordinate.x &&
                    item.obstructionCoordinate.y == coordinate.y)
                {
                    cachedFogCoordinateList = item.fogCoordinateList;
                    break;
                }
            }

            List<Coordinate> fogCoordinateList = new List<Coordinate>();
            if (symmetryAxis2)
            {
                foreach (Coordinate item in cachedFogCoordinateList)
                {
                    fogCoordinateList.Add(new Coordinate {x = item.x * symmetryAxis0, y = item.y * symmetryAxis1});
                }
            }
            else
            {
                foreach (Coordinate item in cachedFogCoordinateList)
                {
                    fogCoordinateList.Add(new Coordinate {x = item.y * symmetryAxis0, y = item.x * symmetryAxis1});
                }
            }

            return fogCoordinateList;
        }
    }
}