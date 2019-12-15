using UnityEngine;
using System.Collections;
using TMPro;

public class GoalDetection : MonoBehaviour {
    public GameObject textMeshPro;

    void OnCollisionEnter(Collision col) {
        if (col.gameObject.name == "BeachBall") {
            textMeshPro.GetComponent<TextMeshPro>().SetText("success!");
        }
    }
}
