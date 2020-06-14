using System.IO;
using System.Text;
using UnityEditor;
using UnityEngine;

namespace com.PROS.SalvationLand
{
    public class TextAssetCreator : MonoBehaviour
    {
        [MenuItem("PROS/Create Operator Property")]
        private static void CreateOperatorProperty()
        {
            string filePath = Application.dataPath + "/Text Assets/operator_property.json";
            UserData1 userData1 = new UserData1 {operatorsProperty = new OperatorProperty[12]};
            userData1.operatorsProperty = new[]
            {
                new OperatorProperty
                {
                    currentHealth = 3,
                    currentMapIndex = 0,
                    currentSkillLevel = new[] {1, 1, 1, 1},
                    index = 0,
                    maxHealth = 3,
                    maxSkillLevel = new[] {4, 4, 4, 4},
                    name = "爆破手",
                    skillCooldown = new[] {0, 0, 0, 0},
                    skillIndex = new[] {0, 3, 4, 5},
                    skillPoint = 3
                },
                new OperatorProperty
                {
                    currentHealth = 3,
                    currentMapIndex = 0,
                    currentSkillLevel = new[] {1, 1, 1, 1},
                    index = 1,
                    maxHealth = 3,
                    maxSkillLevel = new[] {4, 4, 4, 4},
                    name = "野兽",
                    skillCooldown = new[] {0, 0, 0, 0},
                    skillIndex = new[] {0, 3, 4, 5},
                    skillPoint = 3
                },
                new OperatorProperty
                {
                    currentHealth = 3,
                    currentMapIndex = 0,
                    currentSkillLevel = new[] {1, 1, 1, 1},
                    index = 2,
                    maxHealth = 3,
                    maxSkillLevel = new[] {4, 4, 4, 4},
                    name = "士官长",
                    skillCooldown = new[] {0, 0, 0, 0},
                    skillIndex = new[] {0, 3, 4, 5},
                    skillPoint = 3
                },
                new OperatorProperty
                {
                    currentHealth = 3,
                    currentMapIndex = 0,
                    currentSkillLevel = new[] {1, 1, 1, 1},
                    index = 3,
                    maxHealth = 3,
                    maxSkillLevel = new[] {4, 4, 4, 4},
                    name = "雷神",
                    skillCooldown = new[] {0, 0, 0, 0},
                    skillIndex = new[] {0, 3, 4, 5},
                    skillPoint = 3
                },
                new OperatorProperty
                {
                    currentHealth = 3,
                    currentMapIndex = 0,
                    currentSkillLevel = new[] {1, 1, 1, 1},
                    index = 4,
                    maxHealth = 3,
                    maxSkillLevel = new[] {4, 4, 4, 4},
                    name = "战争机器",
                    skillCooldown = new[] {0, 0, 0, 0},
                    skillIndex = new[] {0, 3, 4, 5},
                    skillPoint = 3
                },
                new OperatorProperty
                {
                    currentHealth = 3,
                    currentMapIndex = 0,
                    currentSkillLevel = new[] {1, 1, 1, 1},
                    index = 5,
                    maxHealth = 3,
                    maxSkillLevel = new[] {4, 4, 4, 4},
                    name = "星舰长",
                    skillCooldown = new[] {0, 0, 0, 0},
                    skillIndex = new[] {0, 3, 4, 5},
                    skillPoint = 3
                },
                new OperatorProperty
                {
                    currentHealth = 3,
                    currentMapIndex = 0,
                    currentSkillLevel = new[] {1, 1, 1, 1},
                    index = 6,
                    maxHealth = 3,
                    maxSkillLevel = new[] {4, 4, 4, 4},
                    name = "女猎手",
                    skillCooldown = new[] {0, 0, 0, 0},
                    skillIndex = new[] {0, 3, 4, 5},
                    skillPoint = 3
                },
                new OperatorProperty
                {
                    currentHealth = 3,
                    currentMapIndex = 0,
                    currentSkillLevel = new[] {1, 1, 1, 1},
                    index = 7,
                    maxHealth = 3,
                    maxSkillLevel = new[] {4, 4, 4, 4},
                    name = "探险者",
                    skillCooldown = new[] {0, 0, 0, 0},
                    skillIndex = new[] {0, 3, 4, 5},
                    skillPoint = 3
                },
                new OperatorProperty
                {
                    currentHealth = 3,
                    currentMapIndex = 0,
                    currentSkillLevel = new[] {1, 1, 1, 1},
                    index = 8,
                    maxHealth = 3,
                    maxSkillLevel = new[] {4, 4, 4, 4},
                    name = "药剂师",
                    skillCooldown = new[] {0, 0, 0, 0},
                    skillIndex = new[] {0, 3, 4, 5},
                    skillPoint = 3
                },
                new OperatorProperty
                {
                    currentHealth = 3,
                    currentMapIndex = 0,
                    currentSkillLevel = new[] {1, 1, 1, 1},
                    index = 9,
                    maxHealth = 3,
                    maxSkillLevel = new[] {4, 4, 4, 4},
                    name = "剑士",
                    skillCooldown = new[] {0, 0, 0, 0},
                    skillIndex = new[] {0, 3, 4, 5},
                    skillPoint = 3
                },
                new OperatorProperty
                {
                    currentHealth = 3,
                    currentMapIndex = 0,
                    currentSkillLevel = new[] {1, 1, 1, 1},
                    index = 10,
                    maxHealth = 3,
                    maxSkillLevel = new[] {4, 4, 4, 4},
                    name = "魅影",
                    skillCooldown = new[] {0, 0, 0, 0},
                    skillIndex = new[] {0, 3, 4, 5},
                    skillPoint = 3
                },
                new OperatorProperty
                {
                    currentHealth = 3,
                    currentMapIndex = 0,
                    currentSkillLevel = new[] {1, 1, 1, 1},
                    index = 11,
                    maxHealth = 3,
                    maxSkillLevel = new[] {4, 4, 4, 4},
                    name = "格斗家",
                    skillCooldown = new[] {0, 0, 0, 0},
                    skillIndex = new[] {0, 3, 4, 5},
                    skillPoint = 3
                }
            };
            File.WriteAllText(filePath, JsonUtility.ToJson(userData1, true), Encoding.UTF8);
            AssetDatabase.Refresh();
        }

        [MenuItem("PROS/Create Outlines")]
        private static void CreateOutlines()
        {
            for (int a = 0; a < 16; ++a)
            {
                int b = a;
                bool up = b >= 8;
                b -= up ? 8 : 0;
                bool right = b >= 4;
                b -= right ? 4 : 0;
                bool left = b >= 2;
                b -= left ? 2 : 0;
                bool down = b == 1;
                Texture2D tex = new Texture2D(116, 116, TextureFormat.ARGB32, false);
                for (int c = 0; c < 116; ++c)
                {
                    for (int d = 0; d < 116; ++d)
                    {
                        if (down && d > 7 && d < 16 && (!left || c > 7) && (!right || c < 108))
                        {
                            tex.SetPixel(c, d, Color.white);
                        }
                        else if (left && c > 7 && c < 16 && (!down || d > 7) && (!up || d < 108))
                        {
                            tex.SetPixel(c, d, Color.white);
                        }
                        else if (right && c > 99 && c < 108 && (!down || d > 7) && (!up || d < 108))
                        {
                            tex.SetPixel(c, d, Color.white);
                        }
                        else if (up && d > 99 && d < 108 && (!left || c > 7) && (!right || c < 108))
                        {
                            tex.SetPixel(c, d, Color.white);
                        }
                        else
                        {
                            tex.SetPixel(c, d, Color.clear);
                        }
                    }
                }

                string filePath = Application.dataPath + "/Textures/Outline " + a + ".png";
                File.WriteAllBytes(filePath, tex.EncodeToPNG());
            }

            AssetDatabase.Refresh();
        }

        private static int _fileCount;
        private static int _lineCount;

        [MenuItem("PROS/Print Scripts' Line Count")]
        private static void PrintScriptsLineCount()
        {
            _fileCount = 0;
            _lineCount = 0;
            GetAllDirectories(Application.dataPath + "/Scripts");
            print("File Count: " + _fileCount + "\nLine Count: " + _lineCount);
        }

        [MenuItem("PROS/Refresh Asset Database")]
        private static void RefreshAssetDatabase()
        {
            AssetDatabase.Refresh();
        }

        private static void GetAllDirectories(string currentPath)
        {
            GetAllFiles(currentPath);
            DirectoryInfo[] directoryInfos = new DirectoryInfo(currentPath).GetDirectories();
            foreach (DirectoryInfo item in directoryInfos)
            {
                GetAllDirectories(item.FullName);
            }
        }

        private static void GetAllFiles(string currentPath)
        {
            FileInfo[] fileInfos = new DirectoryInfo(currentPath).GetFiles();
            foreach (FileInfo item in fileInfos)
            {
                if (item.Name.Substring(item.Name.Length - 3, 3).Equals(".cs"))
                {
                    ++_fileCount;
                    _lineCount += File.ReadAllLines(item.FullName).Length;
                }
            }
        }
    }
}