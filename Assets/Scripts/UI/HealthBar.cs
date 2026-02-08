namespace UI
{
    using UnityEngine;
    using UnityEngine.UI;

    public class HealthBar : MonoBehaviour
    { 
        private Slider slider;
        public void Awake()
        {
            slider = GetComponent<Slider>();
            if (slider == null)
            {
                Debug.LogWarning("HealthBar script requires Slider component");
            }
        }
        
        public void SetMaxHealth(int health)
        {
            slider.maxValue = health;
            slider.value = health;
        }
        public void SetHealth(int health)
        {
            slider.value = health; 
        }
    }

}
