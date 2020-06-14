using System.Collections.Generic;
using UnityEngine;

namespace com.PROS.SalvationLand
{
    public class FogOfViewField : MonoBehaviour
    {
        public static FogOfViewField Instance;
        public static int[,] ObstructionGrid;

        public Rect area;
        public Color32[] fogColors; //0:厚迷雾 1:薄迷雾 2:无迷雾
        public float fogUpdateTimeInterval;
        public int gridColumnCount;
        [HideInInspector] public float gridHeight;
        public int gridRowCount;
        [HideInInspector] public float gridWidth;
        public ViewUnit mainViewUnit;
        public List<ViewUnit> otherViewUnitList = new List<ViewUnit>();
        public Object prefabFogOfViewField;

        private int[,] m_FogGrid; //0:厚迷雾 1:薄迷雾 2:无迷雾 3:厚迷雾（中间量） 4:薄迷雾（中间量） 5:无迷雾（中间量）
        private int m_LayerMaskObstruction;
        private Material m_Material;
        private Texture2D m_TextureFog;
        private Color32[] m_TextureFogColors;

        private void Awake()
        {
            Instance = this;
        }

        private Coordinate ConvertPositionToCoordinate(Vector3 position)
        {
            return new Coordinate
                {x = (int) ((position.x - area.x) / gridWidth), y = (int) ((position.z - area.y) / gridHeight)};
        }

        public void Initialize()
        {
            //初始化变量
            gridHeight = area.height / gridRowCount;
            gridWidth = area.width / gridColumnCount;
            m_FogGrid = new int[gridColumnCount, gridRowCount];
            m_TextureFogColors = new Color32[gridColumnCount * gridRowCount];
            for (int a = 0; a < m_TextureFogColors.Length; ++a)
            {
                m_TextureFogColors[a] = fogColors[0];
            }

            m_TextureFog = new Texture2D(gridColumnCount, gridRowCount, TextureFormat.ARGB32, false)
                {filterMode = FilterMode.Point, wrapMode = TextureWrapMode.Clamp};
            m_TextureFog.SetPixels32(m_TextureFogColors);
            m_TextureFog.Apply();
            GameObject fogOfViewField = Instantiate(prefabFogOfViewField,
                new Vector3(area.x + area.width / 2f, 0f, area.y + area.height / 2f),
                Quaternion.identity) as GameObject;
            fogOfViewField.transform.localScale = new Vector3(area.width, 1f, area.height);
            m_Material = fogOfViewField.GetComponentInChildren<Renderer>().material;
            m_Material.SetTexture("_MainTex", m_TextureFog);
            ObstructionGrid = new int[gridColumnCount, gridRowCount];

            //区分空地与障碍物
            m_LayerMaskObstruction = 1 << 8;
            Vector3 halfExtents = new Vector3(gridWidth / 2f, 0.5f, gridHeight / 2f);
            for (int a = 0; a < gridColumnCount; ++a)
            {
                for (int b = 0; b < gridRowCount; ++b)
                {
                    Collider[] obstructions =
                        Physics.OverlapBox(
                            new Vector3((a + 0.5f) * gridWidth + area.x, 0.5f, (b + 0.5f) * gridHeight + area.y),
                            halfExtents, Quaternion.identity, m_LayerMaskObstruction);
                    ObstructionGrid[a, b] = obstructions.Length == 0 ? 0 : 1;
                }
            }

            //初始化所有视野单元
            mainViewUnit.Initialize();
            foreach (ViewUnit item in otherViewUnitList)
            {
                item.Initialize();
            }

            //以一定频率更新迷雾贴图
            InvokeRepeating(nameof(FogUpdate), 0f, fogUpdateTimeInterval);
        }

        private bool IsInRange(Coordinate coordinate)
        {
            return coordinate.x >= 0 && coordinate.x < gridColumnCount && coordinate.y >= 0 &&
                   coordinate.y < gridRowCount;
        }

        private void FogUpdate()
        {
            //计算自己产生的迷雾
            for (int a = 0; a < gridColumnCount; ++a)
            {
                for (int b = 0; b < gridRowCount; ++b)
                {
                    if (m_FogGrid[a, b] == 2)
                    {
                        m_FogGrid[a, b] = 1;
                        m_TextureFogColors[a + gridColumnCount * b] = fogColors[1];
                    }
                }
            }

            Coordinate viewUnitCoordinate = ConvertPositionToCoordinate(mainViewUnit.transform.position);
            if (!IsInRange(viewUnitCoordinate))
            {
                return;
            }

            List<Coordinate> offsetList = mainViewUnit.offsetCacheList;
            for (int a = 0; a < offsetList.Count - 1; ++a)
            {
                Coordinate coordinate = new Coordinate
                    {x = offsetList[a].x + viewUnitCoordinate.x, y = offsetList[a].y + viewUnitCoordinate.y};
                if (IsInRange(coordinate))
                {
                    if (m_FogGrid[coordinate.x, coordinate.y] == 0)
                    {
                        m_FogGrid[coordinate.x, coordinate.y] = 3;
                    }
                    else
                    {
                        m_FogGrid[coordinate.x, coordinate.y] = 4;
                    }
                }
            }

            for (int a = 0; a < offsetList.Count - 1; ++a)
            {
                Coordinate coordinate = new Coordinate
                    {x = offsetList[a].x + viewUnitCoordinate.x, y = offsetList[a].y + viewUnitCoordinate.y};
                if (IsInRange(coordinate))
                {
                    if (ObstructionGrid[coordinate.x, coordinate.y] == 1)
                    {
                        if (m_FogGrid[coordinate.x, coordinate.y] == 3)
                        {
                            m_FogGrid[coordinate.x, coordinate.y] = 0;
                        }
                        else if (m_FogGrid[coordinate.x, coordinate.y] == 4)
                        {
                            m_FogGrid[coordinate.x, coordinate.y] = 1;
                        }

                        List<Coordinate> fogCoordinateList = mainViewUnit.GetFogCoordinateList(offsetList[a]);
                        foreach (Coordinate item in fogCoordinateList)
                        {
                            coordinate = new Coordinate
                                {x = item.x + viewUnitCoordinate.x, y = item.y + viewUnitCoordinate.y};
                            if (IsInRange(coordinate))
                            {
                                if (m_FogGrid[coordinate.x, coordinate.y] == 3)
                                {
                                    m_FogGrid[coordinate.x, coordinate.y] = 0;
                                }
                                else if (m_FogGrid[coordinate.x, coordinate.y] == 4)
                                {
                                    m_FogGrid[coordinate.x, coordinate.y] = 1;
                                }
                            }
                        }
                    }
                }
            }

            for (int a = 0; a < gridColumnCount; ++a)
            {
                for (int b = 0; b < gridRowCount; ++b)
                {
                    if (m_FogGrid[a, b] > 2)
                    {
                        m_FogGrid[a, b] = 2;
                        m_TextureFogColors[a + gridColumnCount * b] = fogColors[2];
                    }
                }
            }

            m_FogGrid[viewUnitCoordinate.x, viewUnitCoordinate.y] = 2;
            m_TextureFogColors[gridColumnCount * viewUnitCoordinate.y + viewUnitCoordinate.x] = fogColors[2];

            //叠加其他视野单元产生的迷雾
            foreach (ViewUnit item in otherViewUnitList)
            {
                if (item.group == mainViewUnit.group)
                {
                    viewUnitCoordinate = ConvertPositionToCoordinate(item.transform.position);
                    if (!IsInRange(viewUnitCoordinate))
                    {
                        continue;
                    }

                    offsetList = item.offsetCacheList;
                    for (int a = 0; a < offsetList.Count - 1; ++a)
                    {
                        Coordinate coordinate = new Coordinate
                            {x = offsetList[a].x + viewUnitCoordinate.x, y = offsetList[a].y + viewUnitCoordinate.y};
                        if (IsInRange(coordinate))
                        {
                            if (m_FogGrid[coordinate.x, coordinate.y] == 0)
                            {
                                m_FogGrid[coordinate.x, coordinate.y] = 3;
                            }
                            else if (m_FogGrid[coordinate.x, coordinate.y] == 1)
                            {
                                m_FogGrid[coordinate.x, coordinate.y] = 4;
                            }
                        }
                    }

                    for (int a = 0; a < offsetList.Count - 1; ++a)
                    {
                        Coordinate coordinate = new Coordinate
                            {x = offsetList[a].x + viewUnitCoordinate.x, y = offsetList[a].y + viewUnitCoordinate.y};
                        if (IsInRange(coordinate))
                        {
                            if (ObstructionGrid[coordinate.x, coordinate.y] == 1)
                            {
                                if (m_FogGrid[coordinate.x, coordinate.y] == 3)
                                {
                                    m_FogGrid[coordinate.x, coordinate.y] = 0;
                                }
                                else if (m_FogGrid[coordinate.x, coordinate.y] == 4)
                                {
                                    m_FogGrid[coordinate.x, coordinate.y] = 1;
                                }

                                List<Coordinate> fogCoordinateList = item.GetFogCoordinateList(offsetList[a]);
                                foreach (Coordinate item0 in fogCoordinateList)
                                {
                                    coordinate = new Coordinate
                                        {x = item0.x + viewUnitCoordinate.x, y = item0.y + viewUnitCoordinate.y};
                                    if (IsInRange(coordinate))
                                    {
                                        if (m_FogGrid[coordinate.x, coordinate.y] == 3)
                                        {
                                            m_FogGrid[coordinate.x, coordinate.y] = 0;
                                        }
                                        else if (m_FogGrid[coordinate.x, coordinate.y] == 4)
                                        {
                                            m_FogGrid[coordinate.x, coordinate.y] = 1;
                                        }
                                    }
                                }
                            }
                        }
                    }

                    for (int a = 0; a < gridColumnCount; ++a)
                    {
                        for (int b = 0; b < gridRowCount; ++b)
                        {
                            if (m_FogGrid[a, b] > 2)
                            {
                                m_FogGrid[a, b] = 2;
                                m_TextureFogColors[a + gridColumnCount * b] = fogColors[2];
                            }
                        }
                    }

                    m_FogGrid[viewUnitCoordinate.x, viewUnitCoordinate.y] = 2;
                    m_TextureFogColors[gridColumnCount * viewUnitCoordinate.y + viewUnitCoordinate.x] = fogColors[2];
                }
            }

            //剔除在迷雾中的视野单元的模型
            foreach (ViewUnit item in otherViewUnitList)
            {
                if (item.group != mainViewUnit.group)
                {
                    viewUnitCoordinate = ConvertPositionToCoordinate(item.transform.position);
                    bool isRendererEnabled = m_FogGrid[viewUnitCoordinate.x, viewUnitCoordinate.y] == 2;
                    foreach (Renderer renderer in item.renderers)
                    {
                        renderer.enabled = isRendererEnabled;
                    }
                }
            }

            m_TextureFog.SetPixels32(m_TextureFogColors);
            m_TextureFog.Apply();
        }
    }
}