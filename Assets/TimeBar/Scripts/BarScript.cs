using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class BarScript : MonoBehaviour
{
    private float fillAmount;

    [SerializeField] private float lerpSpeed;

    [SerializeField] private Image content;
    [SerializeField] private TMPro.TextMeshProUGUI valueText;

    [SerializeField] private bool lerpColors;
    [SerializeField] private Color fullColor;
    [SerializeField] private Color lowColor;

    public float MaxValue { get; set; }

    public float Value
    {
        set
        {
            string[] tmp = valueText.text.Split(' ');
            valueText.text = Mathf.Round(value) + " " + tmp[1];
            fillAmount = Map(value, 0, MaxValue, 0, 1);
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        if (lerpColors)
        {
            content.color = fullColor;
        }
    }

    // Update is called once per frame
    void Update()
    {
        HandleBar();    
    }

    private void HandleBar()
    {
        if (fillAmount != content.fillAmount)
        {
            content.fillAmount = Mathf.Lerp(content.fillAmount, fillAmount, Time.deltaTime * lerpSpeed);
        }

        if (lerpColors)
        {
            content.color = Color.Lerp(lowColor, fullColor, fillAmount);
        }
        
    }

    private float Map(float value, float inMin, float inMax, float outMin, float outMax)
    {
        //Scale the value with an known range(inMin to inMax) to an sepecified range (outMin to outMax)
        return (((value - inMin) * (outMax - outMin)) / (inMax - inMin)) + outMin;
        // EG: (((30 - 0) * (120 - 20)) / (100 - 0))) + 20
        // ((30 * 100) / (100)) + 20
        // (3000/100) + 20
        // 50
    }
}
