using UnityEngine;
using UnityEngine.UI;

public class HealthBar : MonoBehaviour
{
    [SerializeField] private Slider healthSlider;
    [SerializeField] private Image fillImage;
    [SerializeField] private Gradient healthGradient;

    private Camera mainCamera;

    void Start()
    {
        mainCamera = Camera.main;

        if (healthSlider == null)
            healthSlider = GetComponent<Slider>();

        if (fillImage == null && healthSlider != null)
            fillImage = healthSlider.fillRect.GetComponent<Image>();
    }

    void Update()
    {
        // Make health bar face camera
        if (mainCamera != null)
        {
            transform.LookAt(transform.position + mainCamera.transform.rotation * Vector3.forward,
                           mainCamera.transform.rotation * Vector3.up);
        }
    }

    public void UpdateHealth(float currentHealth, float maxHealth)
    {
        if (healthSlider == null) return;

        float healthPercentage = currentHealth / maxHealth;
        healthSlider.value = healthPercentage;

        if (fillImage != null && healthGradient != null)
        {
            fillImage.color = healthGradient.Evaluate(healthPercentage);
        }
    }
}
