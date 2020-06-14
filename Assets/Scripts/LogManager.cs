using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace com.PROS.SalvationLand
{
    public class LogManager : MonoBehaviour
    {
        public static LogManager Instance;

        [Header("Components")] public Transform panelDebugLog;
        public Text textDebugLog;
        [Header("Parameters")] public int maxLogCount;

        private static List<string> _logList = new List<string>();

        private bool m_IsEnabled = true;

        private void Awake()
        {
            if (Instance != null && Instance.gameObject != null)
            {
                Destroy(Instance.gameObject);
            }

            Instance = this;
            DontDestroyOnLoad(gameObject);
        }

        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.F11))
            {
                m_IsEnabled = !m_IsEnabled;
                panelDebugLog.localScale = new Vector3(m_IsEnabled ? 1f : 0f, 1f, 1f);
            }
        }

        public void Add(string newLog)
        {
            while (_logList.Count >= maxLogCount)
            {
                _logList.RemoveAt(0);
            }

            _logList.Add(GetTimestamp() + "  " + newLog);
            string allLog = string.Empty;
            for (int a = 0; a < _logList.Count; ++a)
            {
                allLog += _logList[a] + "\n";
            }

            if (textDebugLog != null)
            {
                textDebugLog.text = allLog;
            }
        }

        private string GetTimestamp()
        {
            DateTime dateTime = DateTime.Now;
            string timestamp = string.Empty;
            timestamp += "[";
            timestamp += dateTime.Hour < 10 ? ("0" + dateTime.Hour) : dateTime.Hour.ToString();
            timestamp += ":";
            timestamp += dateTime.Minute < 10 ? ("0" + dateTime.Minute) : dateTime.Minute.ToString();
            timestamp += ":";
            timestamp += dateTime.Second < 10 ? ("0" + dateTime.Second) : dateTime.Second.ToString();
            timestamp += ":";
            timestamp += dateTime.Millisecond < 10
                ? ("00" + dateTime.Millisecond)
                : (dateTime.Millisecond < 100 ? ("0" + dateTime.Millisecond) : dateTime.Millisecond.ToString());
            timestamp += "]";
            return timestamp;
        }
    }
}