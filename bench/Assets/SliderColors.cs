using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SliderColors : MonoBehaviour
{

   // [SerializeField] private Color MaxHealthColor = Color.green;
    [SerializeField] private Color MaxHealthColor = Color.green;
    [SerializeField] private Color MidHealthColor = Color.yellow;
    [SerializeField] private Color MinHealthColor = Color.red;
    public Image fillImage;
    private Slider slider;
  

    // Start is called before the first frame update
    void Start() {
        slider=GetComponent<Slider>();
        //slider.gameObject.transform.Find("Fill Area").Find("Fill").GetComponent<Image>().color = color;
       // fillImage = GetComponent<Image>();
       //fillImage.color = Color.red;
       
    }

    // Update is called once per frame
    void Update()
    {
        if (slider.value < 0.3f)
            slider.targetGraphic.color = MinHealthColor;
        else if (slider.value < 0.6)
            slider.targetGraphic.color = MidHealthColor;
        else
            slider.targetGraphic.color = MaxHealthColor;
        
    }
}
