using UnityEngine;

public class BucketGathering : Activity
{
    [Header("BucketGathering")]
    [Header("References")]
    public Container container;
    public GameObject shellPrefab;
    [Header("Settings")]
    public Vector2 radius;
    public int count = 1;

    private bool collecting = false;
    private bool going = false;
    private bool storing = false;
    private int currentCount = 0;
    private Item shellItem;
    private Vector3 target;

    public void Start()
    {
        Initialize();
    }

    public void Initialize()
    {
        collecting = false;
        going = false;
        storing = false;
        currentCount = 0;
    }

    public override void Break()
    {
        base.Break();
        if (users.Count > 0) users[0].Drop();
    }

    public override void StartUsing(NonPlayableCharacter user)
    {
        base.StartUsing(user);
        Initialize();
        if (user.look != null) user.look.FocusOn(transform);
        collecting = true;
        Constraint();
        transform.up = Vector3.up;
    }

    public override void StopUsing(NonPlayableCharacter user)
    {
        collecting = false;
        base.StopUsing(user);
        user.Drop();
        user.movement.onDestinationReached = null;
        UnConstraint();
    }

    public Vector3 GetLootPosition()
    {
        return origin + Geometry.GetRandomPointOnCircle(Random.Range(radius.x, radius.y));
    }

    public override void Update()
    {
        base.Update();

        if(collecting)
        {
            if(!going)
            {
                target = GetLootPosition();
                going = true;
                users[0].movement.GoThere(target, true);
                users[0].movement.onDestinationReached += () =>
                {
                    users[0].animator.SetTrigger("Pickup Ground");
                    Game.i.WaitAndDo(0.5f, () =>
                    {
                        if (users.Count < 1) return; // No one is using me anymore

                        shellItem = new GameObject().AddComponent<Item>();
                        shellItem.Visual(shellPrefab);
                        shellItem.gameObject.name = "Shell";
                        users[0].Carry(shellItem);
                        Pyromancer.PlayGameEffect("gfx_sand_poof", shellItem.transform.position);
                        Game.i.WaitAndDo(0.5f, () => {
                            if (users.Count < 1) return; // No one is using me anymore
                            users[0].movement.GoThere(transform.position, true);
                            storing = true;
                        });
                    });
                };
            }
            else if(storing)
            {
                if(users[0].range.IsInRange(gameObject))
                {
                    storing = false;
                    users[0].movement.Stop();
                    users[0].movement.OrientToward(transform.position);
                    users[0].animator.SetTrigger("Pickup Ground");
                    Game.i.WaitAndDo(0.5f, () => {
                        if (users.Count < 1) return; // No one is using me anymore
                        users[0].Drop();
                        container.Store(shellItem);
                        currentCount++;
                        if(currentCount >= count) Exit(users[0]);
                        else Game.i.WaitAndDo(1f, () => { going = false; });
                    });
                }
            }
        }
    }

#if UNITY_EDITOR
    public override void OnDrawGizmosSelected()
    {
        base.OnDrawGizmosSelected();
        Gizmos.color = new Color32(255, 215, 0, 255);
        UnityEditor.Handles.DrawWireDisc(transform.position, Vector3.up, radius.x);
        UnityEditor.Handles.DrawWireDisc(transform.position, Vector3.up, radius.y);
    }

    void OnValidate()
    {
        if (radius.x < 0) radius.x = 0;
        if (radius.y < radius.x) radius.y = radius.x;
    }
#endif
}
