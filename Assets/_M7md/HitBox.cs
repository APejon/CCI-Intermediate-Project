using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Collider2D))]
public class HitBox : MonoBehaviour
{
    public FighterController owner;          // filled by the attacker
    public ParticleSystem    impactFx;       // optional flavour

    private readonly HashSet<FighterController> alreadyHit = new();

    void OnEnable() => alreadyHit.Clear();   // fresh list each swing

    void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.CompareTag("HurtBox")) return;

        var defender = other.transform.root.GetComponent<FighterController>();
        if (!defender || defender == owner || alreadyHit.Contains(defender))
            return;

        alreadyHit.Add(defender);
        GameManager.Instance.RegisterPoint(owner, defender, transform.position);

        string sfxKey = defender.gameObject.tag + ("_hit");
        AudioManager.Instance.Play(sfxKey);
        
        if (impactFx)
        {
            ParticleSystem fx = Instantiate(impactFx, transform.position, Quaternion.identity);
            fx.Play();
        }
    }


    
}
