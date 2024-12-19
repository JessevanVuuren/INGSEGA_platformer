using UnityEngine;
using UnityEngine.UI;

public class HealthBar : MonoBehaviour
{
    public Slider slider;
    public Color low;
    public Color high;
    public Vector3 offset;
    public bool isPlayer = false;


    public void SetHealth(float health, float maxHealth)
    {
        if (!isPlayer) slider.gameObject.SetActive(health < maxHealth);
        slider.value = health;
        slider.maxValue = maxHealth;
        
        slider.fillRect.GetComponentInChildren<Image>().color = Color.Lerp(low, high, slider.normalizedValue);
    }

    void Update()
    {
        if (!isPlayer) slider.transform.position = Camera.main.WorldToScreenPoint(transform.parent.position + offset);
    }
}
