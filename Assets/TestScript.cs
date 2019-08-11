using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestScript : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        Debug.LogFormat("Tier={0}", Graphics.activeTier);
    }

    void Update()
    {
        if (Input.GetButtonDown("Fire1"))
        {
            Debug.Log("Increasing");
            QualitySettings.IncreaseLevel();
        }
        if (Input.GetButtonDown("Fire2"))
        {
            Debug.Log("Decreasing");
            QualitySettings.DecreaseLevel();
        }
    }
}
