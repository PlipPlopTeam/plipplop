using UnityEngine;

public class Face : MonoBehaviour
{
    public Transform holder;
    public Vector3 offset;
    GameObject obj;

    public void Start()
    {
        obj = Instantiate(Game.i.library.facePrefab, holder.transform);
        obj.transform.localPosition = offset;
        obj.transform.forward = holder.transform.forward;
    }
}
