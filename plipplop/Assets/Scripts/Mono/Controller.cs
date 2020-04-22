using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[System.Serializable]
public abstract class Controller : MonoBehaviour
{
    [HideInInspector] public bool autoPossess = false;
    [HideInInspector] public bool beginCrouched = false;
    [HideInInspector] public bool canRetractLegs = true;
    [HideInInspector] public bool freezeUntilPossessed = false;
    [HideInInspector] public bool useGravity = true;
    [HideInInspector] public bool animateHead = true;
    [HideInInspector] public float gravityMultiplier = 100f;
    [HideInInspector] public Rigidbody customExternalRigidbody;
    [HideInInspector] public AperturePreset customCamera = null;
    [HideInInspector] public Locomotion locomotion;
	[HideInInspector] public Transform visuals;
	[HideInInspector] public GameObject face;

	public Vector3 visualsOffset;
	public float unpossessSpawnDistance = 1f;

    float lastTimeGrounded = 0f;
    new internal Rigidbody rigidbody;
    Controller lastFocusedController;
    Vector3 previousVisualLocalPosition;
    internal ControllerSensor controllerSensor;
    internal bool isImmerged { get { return immersion > 0; } }
    RigidbodyConstraints previousConstraints;
    internal bool movingStick;
	bool isBeingThrown = false;
	int freeze = 0;
    int immersion = 0;

	bool isParalysed = false;
	public void Paralyse() { isParalysed = true; }
	public void UnParalyse() { isParalysed = false; }

	public virtual void Throw(Vector3 direction, float force)
	{
		isBeingThrown = true;
		rigidbody.AddForce(direction * force * Time.deltaTime);
	}

	public virtual void OnEject()
    {
        if (controllerSensor) Destroy(controllerSensor.gameObject);
        controllerSensor = null;
        RetractLegs();

		ToggleFace(false);
		Activity activity = gameObject.GetComponent<Activity>();
        if(activity != null) activity.Repair();

        visuals.transform.localPosition = previousVisualLocalPosition;
    }
    
    public GameObject GetEjectionClone()
    {
        return locomotion.GetEjectionClone();
    }

    public virtual void OnPossess()
    {
		previousVisualLocalPosition = visuals.transform.localPosition;

		if (freezeUntilPossessed && rigidbody.constraints == RigidbodyConstraints.FreezeAll) {
            rigidbody.constraints = previousConstraints;
        }
		controllerSensor = Instantiate(Game.i.library.controllerSensor, gameObject.transform).GetComponent<ControllerSensor>();
        controllerSensor.transform.localPosition = new Vector3(0f, 0f, controllerSensor.sensorForwardPosition);

		if (beginCrouched) RetractLegs();
		else ExtendLegs();
        
		ToggleFace(true);
		foreach (Transform t in visuals.GetComponentsInChildren<Transform>()) t.gameObject.layer = 0;

	}

	internal virtual void SpecificJump() {}
    internal virtual void OnJump()
    {
		//if (!canRetractLegs) return;
		if (AreLegsRetracted())
        {
            SpecificJump();
        }
        else if (WasGrounded() || isImmerged)
        {
             locomotion.Jump();
        }
	}

    internal void RetractLegs()
    {
        if (!canRetractLegs) return;
        locomotion.RetractLegs();
        OnLegsRetracted();

		// Reset visual local position when legs are retracted
		visuals.transform.localPosition = previousVisualLocalPosition;

		Activity activity = gameObject.GetComponent<Activity>();
		if (activity != null) activity.Repair();
	}

	internal void ExtendLegs()
    {
		Activity activity = gameObject.GetComponent<Activity>();
		if (activity != null) activity.Break();
		locomotion.ExtendLegs();
        OnLegsExtended();
    }

	public void ToggleLegs()
	{
		if (AreLegsRetracted()) ExtendLegs();
		else RetractLegs();
	}

    internal bool AreLegsRetracted() { return locomotion.AreLegsRetracted(); }
    internal virtual bool IsGrounded(float rangeMultiplier = 1f) { return locomotion.IsGrounded(); }
    internal virtual bool WasGrounded() { return Time.time - locomotion.preset.groundedBufferToleranceSeconds < lastTimeGrounded; }
    internal virtual void OnHoldJump() {}
	internal virtual void OnLegsRetracted() {}
	internal virtual void OnLegsExtended() {}
    internal virtual void SpecificMove(Vector3 direction) {}
    internal virtual void MoveCamera(Vector2 d) {}

    virtual internal void Shout()
    {
        Pyromancer.PlayGameEffect(Game.i.library.gfxs["gfx_speak"], transform, Vector3.up*0.5f);
    }

    virtual internal void BaseMove(Vector3 direction)
    {
        if (direction.magnitude > 0.1f) movingStick = true;
        else movingStick = false;

        if (IsFrozen() || isParalysed) return;
		if (AreLegsRetracted()) SpecificMove(direction);
        else locomotion.Move(direction);
    }

    virtual internal void StopHorizontalVelocity()
    {
        rigidbody.velocity = Vector3.Scale(rigidbody.velocity, Vector3.up);
    }

    public void Move(float fb, float rl)
    {
		if (IsFrozen()) return;
		BaseMove(Vector3.ClampMagnitude(new Vector3(rl, 0f, fb), 1f));
    }

    public void MoveCamera(float h, float v)
    {
        MoveCamera(new Vector2(h, v));
    }
    internal bool IsPossessed()
    {
        return Game.i != null && Game.i.player != null && Game.i.player.IsPossessing(this);
    }

	public virtual bool IsVisibleByNPC()
	{
		return IsPossessed() && !AreLegsRetracted();
	}

    public void SetUnderwater()
    {
        immersion++;
        locomotion.isImmerged = isImmerged;
        Kick();
    }

	public void Kick()
	{
		if (IsPossessed() && !Game.i.player.IsPossessingBaseController())
		{
			Game.i.player.TeleportBaseControllerAndPossess();
		}
	}

    public void SetOverwater()
    {
        immersion--;
        locomotion.isImmerged = isImmerged;
    }

    virtual internal void Awake()
    {
        rigidbody = customExternalRigidbody==null ? GetComponent<Rigidbody>() : customExternalRigidbody;
        if (!rigidbody) rigidbody = gameObject.AddComponent<Rigidbody>();
        if (freezeUntilPossessed) {
            previousConstraints = rigidbody.constraints;
            rigidbody.constraints = RigidbodyConstraints.FreezeAll;
        }

        if (visuals == null)
        {
            visuals = transform.GetChild(0);
            var msg = "No visual linked for controller " + gameObject.name + ", took the first TChild (" + visuals.gameObject.name + ") instead";
            if (visuals.gameObject.name != "Visuals")
                Debug.LogWarning(msg);
            //else
                //Debug.Log(msg);
        }

        // Loco and anims
        locomotion = GetComponent<Locomotion>();
        if (!locomotion) locomotion = gameObject.AddComponent<Locomotion>();
        else locomotion.Initialize();

        previousVisualLocalPosition = visuals.transform.localPosition;

        //DEBUG
        foreach (var renderer in GetComponentsInChildren<Renderer>()) {
            renderer.material = Instantiate(renderer.material);
        }
    }

    virtual internal void Start()
    {
        if (autoPossess) Game.i.player.Possess(this);

        if (!IsPossessed()) RetractLegs();

        rigidbody.useGravity = false;
    }

    void ToggleFace(bool isActive)
    {
        if(face) face.SetActive(isActive);
	}

    virtual internal void Update()
    {
        rigidbody.useGravity = false;
        UpdateLastTimeGrounded();

		if(controllerSensor && !isImmerged)
		{
			if(controllerSensor.IsThereAnyController())
			{
				Controller focusedController = controllerSensor.GetFocusedController();
				if (lastFocusedController != null && lastFocusedController != focusedController)
				{
					foreach (Transform t in lastFocusedController.visuals.GetComponentsInChildren<Transform>()) t.gameObject.layer = 0;
				}
				foreach (Transform t in focusedController.visuals.GetComponentsInChildren<Transform>()) t.gameObject.layer = 9;
				lastFocusedController = focusedController;
			}
			else if (lastFocusedController != null)
			{
				foreach (Transform t in lastFocusedController.visuals.GetComponentsInChildren<Transform>()) t.gameObject.layer = 0;
				lastFocusedController = null;
			}
		}

        if (IsPossessed() && !AreLegsRetracted())
        {
            if (animateHead) 
            {
                AlignPropOnHeadDummy();
            }
            if(!IsFrozen())
            {
                Vector3? norm;
                Vector3 up = Vector3.up;
                try
                {
                    norm = locomotion.GetGroundNormal();
                }
                catch
                {
                    norm = null;
                }
                var y = transform.eulerAngles.y;
                if (norm.HasValue)
                {
                    up = norm.Value;
                }
                transform.up = up;// Vector3.Lerp(transform.up, up, Time.deltaTime * 10f);
                transform.Rotate(Vector3.up * y);
            }
        }

        if (isBeingThrown && (IsGrounded()|| isImmerged))
		{
			UnParalyse();
			isBeingThrown = false;
		}
	}

    virtual internal void AlignPropOnHeadDummy()
    {
        var prs = locomotion.GetHeadDummy();
        visuals.transform.SetPositionAndRotation(prs.position + visualsOffset, prs.rotation);
        visuals.transform.localScale = prs.scale;
    }

	virtual internal void ResetVisuals()
	{
		visuals.transform.localPosition = Vector3.zero;
		visuals.transform.localScale = Vector3.one;
		visuals.transform.localRotation = Quaternion.identity;
	}

    virtual internal void FixedUpdate()
    {
        if (useGravity && !isImmerged) {
            ApplyGravity();
        }
    }

    void UpdateLastTimeGrounded()
    {
		if (IsGrounded()) lastTimeGrounded = Time.time;
    }

    internal void ApplyGravity(float factor=1f)
    {
        var gravity = Vector3.down * 9.81f * (gravityMultiplier/100f) * factor;
        rigidbody.AddForce(gravity, ForceMode.Acceleration);
    }

    // Trying to possess somESubject else
    virtual internal void OnTryPossess()
    {
        if (!isImmerged && controllerSensor && controllerSensor.IsThereAnyController())
        {
            var focused = controllerSensor.GetFocusedController();
            if (!focused.isImmerged) 
            {
                Game.i.player.Possess(focused);
            }
        }
        else if (!Game.i.player.IsPossessingBaseController()) 
        {
            Game.i.player.TeleportBaseControllerAndPossess(unpossessSpawnDistance);
        }
    }

	public void Freeze()
	{
		freeze++;
		RefreshFreeze();
	}

	public void UnFreeze()
	{
		freeze--;
		RefreshFreeze();
	}

	bool IsFrozen()
    {
		return freeze > 0;
    }

	public void RefreshFreeze()
	{
		if (IsFrozen()) ApplyFreeze();
		else RemoveFreeze();
	}

	public void ApplyFreeze()
	{
		rigidbody.isKinematic = true;
		rigidbody.useGravity = false;
	}

	public void RemoveFreeze()
	{
		rigidbody.isKinematic = false;
		rigidbody.useGravity = true;
	}

#if UNITY_EDITOR
    // Draw a gizmo if i'm being possessed
    void OnDrawGizmos()
    {
        if (EditorApplication.isPlaying) {
            // Possession
            if (IsPossessed()) {
                Gizmos.DrawIcon(transform.position + Vector3.up * 2f, "Favorite Icon");
            }
            else {
                Gizmos.DrawIcon(transform.position + Vector3.up * 2f, "d_CollabChangesConflict Icon");
            }
        }
        else {
            if (autoPossess) {
                var sf = Mathf.Clamp(HandleUtility.GetHandleSize(transform.position), 1f, 6f);
                var style = new GUIStyle(GUI.skin.textField);
                style.fontStyle = FontStyle.BoldAndItalic;
                style.fontSize = Mathf.FloorToInt(12f / sf);
                style.normal.textColor = Color.white;
                style.alignment = TextAnchor.MiddleCenter;
                style.imagePosition = style.fontSize > 3 ? ImagePosition.ImageAbove : ImagePosition.ImageOnly;

                var asPressedTex = Resources.Load<Texture2D>("Editor/Sprites/SPR_D_AutoPossess");
                var content = new GUIContent();
                content.image = asPressedTex;
                content.text = "[AUTO-POSSESS]";

                if (style.fontSize > 2) Handles.Label(transform.position + Vector3.up *2f, content, style);
                else Gizmos.DrawIcon(transform.position + Vector3.up*2f, "SPR_GIZ_AUTOPOSSESS", false);
            }
        }
    }

    void OnDrawGizmosSelected()
    {
        if (EditorApplication.isPlaying) {
            Handles.Label(transform.position + Vector3.up*2f, string.Join("\n", new string[] {
                    string.Format("Grounded? {0}", IsGrounded()),
                    string.Format("Retracted? {0}", AreLegsRetracted()),
                    string.Format("Immerged? {0}", isImmerged)
                })
            );
        }
    }
#endif
}
