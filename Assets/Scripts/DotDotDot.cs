using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public class DotDotDot : MonoBehaviour
{
    public float timeInterval = 0.3f;
    string text;
    Text textBox;
    int dots = 0;

    private void Start()
    {
        textBox = GetComponent<Text>();
        text = textBox.text;
        StartCoroutine(Loading());
    }

    IEnumerator Loading()
    {
        while (true)
        {
            if (dots == 3)
            {
                dots = 0;
                textBox.text = text;
            }
            else
            {
                dots++;
                textBox.text += ".";
            }

            yield return new WaitForSeconds(timeInterval);
        }
    }
}
