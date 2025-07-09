using UnityEngine;

public class HurtBoxCaller : MonoBehaviour
{
    [SerializeField] HurtBox hurtBoxScript;
    
    void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.CompareTag("HitBox")) return;
        hurtBoxScript.PauseAndShake();
    }
}
