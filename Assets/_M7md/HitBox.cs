using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Collider2D))]
public class HitBox : MonoBehaviour
{
    public FighterController owner;          // filled by the attacker
    public AudioClip         impactSfx;      // optional flavour
    public ParticleSystem    impactFx;       // optional flavour

    private readonly HashSet<FighterController> alreadyHit = new();

    void OnEnable() => alreadyHit.Clear();   // fresh list each swing

    void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.CompareTag("HurtBox")) return;

        var defender = other.GetComponentInParent<FighterController>();
        if (!defender || defender == owner || alreadyHit.Contains(defender))
            return;

        alreadyHit.Add(defender);

        GameManager.Instance.RegisterPoint(owner, defender, transform.position);
    }

}
