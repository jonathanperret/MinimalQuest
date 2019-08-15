using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class QualityControl : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        Debug.LogFormat("Tier={0}", Graphics.activeTier);
    }

    void Update()
    {
#if UNITY_ANDROID
        if (OVRInput.GetDown(OVRInput.Button.One))
        {
            Debug.Log("Increasing");
            QualitySettings.IncreaseLevel();
        }
        if (OVRInput.GetDown(OVRInput.Button.Two))
        {
            Debug.Log("Decreasing");
            QualitySettings.DecreaseLevel();
        }
#endif
    }
}
