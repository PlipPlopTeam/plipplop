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
    private Vector3 origin;

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
        origin = transform.position;
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
        container.Constraint();
        transform.up = Vector3.up;
    }

    public override void StopUsing(NonPlayableCharacter user)
    {
        base.StopUsing(user);
        user.Drop();
        container.UnConstraint();
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
                users[0].agentMovement.GoThere(target, true);
                users[0].agentMovement.onDestinationReached += () =>
                {
                    users[0].animator.SetTrigger("Pickup Ground");
                    Game.i.WaitAndDo(0.5f, () =>
                    {

                        shellItem = new GameObject().AddComponent<Item>();
                        shellItem.Visual(shellPrefab);
                        shellItem.gameObject.name = "Shell";
                        users[0].Carry(shellItem);
                        Pyromancer.PlayGameEffect("gfx_sand_poof", shellItem.transform.position);
                        Game.i.WaitAndDo(0.5f, () =>
                        {
                            users[0].agentMovement.GoThere(transform.position, true);
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
                    users[0].agentMovement.Stop();
                    users[0].agentMovement.OrientToward(transform.position);
                    users[0].animator.SetTrigger("Pickup Ground");
                    Game.i.WaitAndDo(0.5f, () =>
                    {
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
