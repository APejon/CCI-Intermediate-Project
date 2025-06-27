using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Collider2D))]
public class HitBox : MonoBehaviour
{
    public FighterController owner;          // filled by the attacker
    public ParticleSystem impactFx;       // optional flavour

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

        string hitSFX = defender.gameObject.tag + ("_hit");
        string gruntSFX = defender.gameObject.tag + ("_grunt");
        AudioManager.Instance.Play(hitSFX);
        AudioManager.Instance.Play(gruntSFX);


        if (impactFx != null)
        {
            Vector2 contactPoint = other.ClosestPoint(transform.position);
            ParticleSystem fx = Instantiate(impactFx, contactPoint, Quaternion.identity);
            //fx.Play();
            Destroy(fx.gameObject, fx.main.duration + fx.main.startLifetime.constantMax);
        }
    }

    
}
