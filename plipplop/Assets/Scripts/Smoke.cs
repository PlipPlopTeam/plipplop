using UnityEngine;

public class Smoke : Hider
{
    [Header("Smoke")]
    public ParticleSystem ps;

    public override void SetRange(float value)
    {
        var sh = ps.shape;
        sh.radius = value;
    }

    public override void Activate()
    {
        base.Activate();
        ps.Play();
    }

    public override void Desactivate()
    {
        base.Desactivate();
        ps.Stop();
    }
}
