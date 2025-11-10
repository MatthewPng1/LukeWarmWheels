using UnityEngine;

public class collectibles : MonoBehaviour
{
    [Header("Optional pickup effects")]
    public AudioClip pickupSfx;
    public GameObject pickupVfx;

    void Start()
    {
        // Ensure we have a collider
        if (GetComponent<Collider2D>() == null)
        {
            gameObject.AddComponent<CircleCollider2D>().isTrigger = true;
        }
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            Debug.Log("Item collected!");

            // Play local effects
            if (pickupSfx != null)
            {
                AudioSource.PlayClipAtPoint(pickupSfx, transform.position);
            }
            if (pickupVfx != null)
            {
                Instantiate(pickupVfx, transform.position, Quaternion.identity);
            }

            // Ensure manager exists — create one if missing so UI updates
            if (CollectibleManager.Instance == null)
            {
                var go = new GameObject("CollectibleManager");
                var mgr = go.AddComponent<CollectibleManager>();
                mgr.RecountCollectibles();
            }

            // Notify manager (if present) so UI can update
            if (CollectibleManager.Instance != null)
            {
                CollectibleManager.Instance.RegisterCollect();
            }

            Destroy(gameObject);
        }
    }
}
