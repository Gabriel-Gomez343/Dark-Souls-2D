using UnityEngine;

public class Sensor_HeroKnight : MonoBehaviour
{
    private int m_ColCount = 0;
    private float m_DisableTimer;

    private void OnEnable()
    {
        m_ColCount = 0;
    }

    public bool State()
    {
        if (m_DisableTimer > 0)
            return false;
        return m_ColCount > 0;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        // Check if the colliding GameObject is on the "Background" layer
        if (other.gameObject.layer == LayerMask.NameToLayer("Background"))
        {
            m_ColCount++;
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        // Check if the previously colliding GameObject was on the "Background" layer
        if (other.gameObject.layer == LayerMask.NameToLayer("Background"))
        {
            m_ColCount--;
        }
    }

    private void Update()
    {
        m_DisableTimer -= Time.deltaTime;
    }

    public void Disable(float duration)
    {
        m_DisableTimer = duration;
    }
}
