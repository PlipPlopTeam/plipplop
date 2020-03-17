using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

using System.IO;
#if UNITY_EDITOR
using UnityEditor;
#endif

public class AfficheSpot : MonoBehaviour
{
    public Material afficheMat;

    public List<Sprite> affiches;

    public float rotationRange = 10;

    public float afficheSive;
    public float afficheSizeVariation;
    

    public List<GameObject> affichesActives = new List<GameObject>();
    
    public enum Shape { CIRCLE, RECTANGLE, POINT }
    [Header("Settings")]
    public Shape shape;
    public float range;
    public Vector2 size;


    
[ContextMenu("Add Affiches")]
    public void AddAffiches()
{
    if (affichesActives.Count > 0 && shape == Shape.POINT)
    {
        return;
    }
    
    GameObject _a = new GameObject();

    SpriteRenderer _sr = _a.AddComponent<SpriteRenderer>();
    _sr.sprite = affiches[Random.Range(0, affiches.Count)];
    _sr.material = afficheMat;

    _a.name = _sr.sprite.name;

    _a.transform.position = GetPosition();

    _a.transform.rotation = transform.rotation;
    
    _a.transform.SetParent(transform);
    
    _a.transform.Rotate(Vector3.forward * Random.Range(-rotationRange/2, rotationRange/2),Space.Self);

    _a.transform.position += _a.transform.forward * .001f * -affichesActives.Count;

    _a.transform.localScale = Vector3.one *
                              Random.Range((afficheSive - afficheSizeVariation / 2),
                                  afficheSive + afficheSizeVariation / 2);
    affichesActives.Add(_a);

    _a.isStatic = true;

}
    
[ContextMenu("Remove Affiches")]
    public void RemoveAffiches()
    {
        foreach (var _a in affichesActives)
        {
            DestroyImmediate(_a);
        }
        
        affichesActives.Clear();
    }

    Vector3 GetPosition()
    {
        switch (shape)
        {
                case Shape.CIRCLE:
                    Vector3 _pos = Random.insideUnitCircle * range;
                    return transform.position + new Vector3(_pos.x, _pos.y, 0);
                    break;
                case Shape.RECTANGLE:
                    return transform.position + (transform.right * Random.Range(-size.x / 2, size.x / 2)) +
                           Random.Range(-size.y / 2, size.y / 2) * transform.up;
                    break;
                case Shape.POINT:
                    return transform.position;
                    break;
        }

        return Vector3.zero;
    }
    

#if UNITY_EDITOR
private void OnDrawGizmos()
{
var style = new GUIStyle();
style.imagePosition = ImagePosition.ImageAbove;
style.alignment = TextAnchor.MiddleCenter;
var tex = Resources.Load<Texture2D>("Editor/Sprites/SPR_flap");
var content = new GUIContent();
content.image = tex;
Handles.Label(transform.position, content, style);
}

private void OnDrawGizmosSelected()
{
if (shape == AfficheSpot.Shape.CIRCLE)
{
Handles.DrawWireDisc(transform.position, transform.forward, range);
}
else if (shape == AfficheSpot.Shape.RECTANGLE)
{
//Handles.DrawWireCube(transform.position, new Vector3(size.x, size.y, 0));
    Gizmos.matrix = transform.localToWorldMatrix;
    Gizmos.DrawWireCube(transform.position,new Vector3(size.x,size.y,0));

}
    else if (shape==AfficheSpot.Shape.POINT)
    {
        Handles.DrawWireDisc(transform.position, Vector3.up , .1f);
    }
}
#endif
}

#if UNITY_EDITOR
[CustomEditor(typeof(AfficheSpot)), CanEditMultipleObjects]
[ExecuteInEditMode]
public class AfficheSpotEditor : Editor
{
    private AfficheSpot afficheSpot;
    
    void OnEnable()
    {
        afficheSpot = (AfficheSpot) target;
        
    }
    
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        if (GUILayout.Button("Add Affiche"))
        {
            afficheSpot.AddAffiches();
        }
        if (GUILayout.Button("Remove Affiche"))
        {
            afficheSpot.RemoveAffiches();
        }
    }
}
#endif