using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class TalkableCharacter : MonoBehaviour
{
    public string questUniqueId = "";
    public float talkRadius = 2f;
    public Transform staticCameraObjective;
    public SkinnedMeshRenderer faceMesh;

    internal SphereCollider sphereTrigger;
    internal Aperture.StaticObjective objective;
    int? blendShapeIndex = null;
    List<string> vowels = new List<string>(){ "O", "U", "A", "E", "I" };
    bool isTalkingToMe { get { return DialogHooks.currentInterlocutor == this; } }
    float lastVowelTime = 0f;

    private void Awake()
    {
        sphereTrigger = gameObject.AddComponent<SphereCollider>();
        sphereTrigger.radius = talkRadius;
        sphereTrigger.isTrigger = true;
        objective = new Aperture.StaticObjective(new Geometry.PositionAndRotation() { position = staticCameraObjective.position, rotation = staticCameraObjective.rotation });
        if (faceMesh != null) blendShapeIndex = faceMesh.sharedMesh.GetBlendShapeIndex("Oh");
    }

    private void OnTriggerEnter(Collider other)
    {
        var ctrl = Game.i.player.GetCurrentController();
        if (other.gameObject != ctrl.gameObject) return;
        if (Vector3.Distance(other.transform.position, ctrl.transform.position) > talkRadius) return;

        if (Game.i.player.currentChatOpportunity != null)
        {
            if (Vector3.Distance(Game.i.player.currentChatOpportunity.transform.position, other.transform.position) > Vector3.Distance(transform.position, other.transform.position))
            {
                Game.i.player.currentChatOpportunity = this;
            }
        }
        else
        {
            Game.i.player.currentChatOpportunity = this;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        var ctrl = Game.i.player.GetCurrentController();
        if (other.gameObject != ctrl.gameObject) return;
        if (Vector3.Distance(other.transform.position, ctrl.transform.position) > talkRadius) return;

        if (Game.i.player.currentChatOpportunity == this)
        {
            Game.i.player.currentChatOpportunity = null;
        }
    }

    internal void Update()
    {
        if (isTalkingToMe) {
            var controller = Game.i.player.GetCurrentController();
            if (controller != null) controller.transform.LookAt(new Vector3(this.transform.position.x, controller.transform.position.y,this.transform.position.z));
            if (Time.time - lastVowelTime > 0.4f) {
                var letterIndex = vowels.FindIndex(o => o == DialogHooks.currentPronouncedLetter.ToUpper());
                var oAmount = (letterIndex >= 0 ? (vowels.Count - (float)letterIndex) / vowels.Count : 0f) * 100f;
                if (blendShapeIndex.HasValue) faceMesh.SetBlendShapeWeight(blendShapeIndex.Value, oAmount);
                if (oAmount > 0) lastVowelTime = Time.time;
            }
        }
        else {
            if (blendShapeIndex.HasValue) faceMesh.SetBlendShapeWeight(blendShapeIndex.Value, 0f);
            if (Game.i.aperture.GetStaticPositionsCount() > 0) Game.i.aperture.RemoveStaticObjective(objective);
        }
    }

    public abstract Dialog OnDialogTrigger();



    public void StartDialogue()
    {
        var dial = OnDialogTrigger();
        Game.i.PlayDialogue(dial, this);
        Game.i.aperture.AddStaticObjective(objective);
    }

    public abstract void Load(byte[] data);
    public abstract byte[] Save();

#if UNITY_EDITOR
    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.magenta;
        if (UnityEditor.EditorApplication.isPlaying)
        {
            if (Game.i.player.currentChatOpportunity == this)
                Gizmos.color = Color.green;
        }
        Gizmos.DrawWireSphere(transform.position, talkRadius);
    }

    private void OnDrawGizmos()
    {
        var style = new GUIStyle();
        style.imagePosition = ImagePosition.ImageAbove;
        style.alignment = TextAnchor.MiddleCenter;
        var tex = Resources.Load<Texture2D>("Editor/Sprites/SPR_D_Quest");
        var content = new GUIContent();
        content.image = tex;
        UnityEditor.Handles.Label(transform.position + Vector3.up, content, style);
    }
#endif

}
