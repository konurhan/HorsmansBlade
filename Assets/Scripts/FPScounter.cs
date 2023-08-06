using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class FPScounter : MonoBehaviour
{
    public Transform FPSCount;
    [SerializeField] private List<float> last200Frames;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        CalculateFPS();
    }

    public void CalculateFPS()
    {
        if (last200Frames.Count < 200)
        {
            last200Frames.Add(Time.deltaTime);
            return;
        }

        last200Frames.Add(Time.deltaTime);
        last200Frames.RemoveAt(0);

        float totalTime = 0f;
        for (int i = 0; i < 200; i++)
        {
            totalTime += last200Frames[i];
        }
        FPSCount.GetComponent<TextMeshProUGUI>().text = "FPS: " + ((int)(200 / totalTime)).ToString();
    }
}
