using UnityEngine;

namespace com.PROS.SalvationLand
{
    public class PointerUpdate : MonoBehaviour
    {
        private void Update()
        {
            transform.localPosition = new Vector3(0f, Mathf.Sin(Time.time * 5f) * 0.2f + 2.1f, 0f);
            transform.localRotation = Quaternion.Euler(0f, Time.time * 300f, 0f);
        }
    }
}