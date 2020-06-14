using System.Collections.Generic;
using UnityEngine;

namespace com.PROS.SalvationLand
{
    public class RandomNumber
    {
        public static void Disrupt<T>(ref List<T> list)
        {
            int length = list.Count;
            for (int a = length; a > 1; --a)
            {
                int index = Random.Range(0, length);
                T item = list[a - 1];
                list[a - 1] = list[index];
                list[index] = item;
            }
        }

        public static float PFromC(float valueC)
        {
            int length = Mathf.CeilToInt(1f / valueC); //概率刚达到1或以上时的实验次数
            float currentP; //重置后，这次试验事件发生的概率
            float previousP = 0f; //重置后，一直到这次试验累计的事件发生概率
            float valueE = 0f; //事件发生的数学期望
            for (int a = 1; a <= length; ++a)
            {
                currentP = Mathf.Min(a * valueC, 1f) * (1 - previousP);
                previousP += currentP;
                valueE += a * currentP;
            }

            return 1 / valueE;
        }

        public static float CFromP(float valueP)
        {
            float currentP; //这次猜测的C值对应的P值
            float lastP = 1f; //上次的P值
            float maxC = valueP; //这次猜测的区间上限
            float minC = 0f; //这次猜测的区间下限
            float valueC; //这次猜测的C值
            while (true)
            {
                valueC = (maxC + minC) / 2f;
                currentP = PFromC(valueC);
                if (Mathf.Approximately(currentP, lastP))
                {
                    return valueC;
                }

                if (currentP < valueP)
                {
                    minC = valueC;
                }
                else
                {
                    maxC = valueC;
                }

                lastP = currentP;
            }
        }
    }
}