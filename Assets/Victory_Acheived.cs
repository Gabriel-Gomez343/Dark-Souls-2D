using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using UnityEngine.SceneManagement;

public class Victory_Acheived : MonoBehaviour
{
    public Image bannerImage;
    private float delay = 2.0f;
    private float resetDelay = 2.0f;

    private Animator animator;

    private void Start()
    {
        animator = GetComponent<Animator>();
        bannerImage.enabled = false;
    }

    private void Update()
    {
        // Check if the enemy is dead
        if (animator.GetBool("isDead"))
        {
            StartCoroutine(ShowBannerAfterDelay()); // Adjust the delay as needed
        }
    }

    private IEnumerator ShowBannerAfterDelay()
    {
        yield return new WaitForSeconds(delay);

        // Enable the banner image
        bannerImage.enabled = true;

         yield return new WaitForSeconds(resetDelay);

        // Reset the scene
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
    
}
