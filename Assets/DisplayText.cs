using TMPro;
using UnityEngine;

public class DisplayText : MonoBehaviour {
	public TextMeshPro tmp;

	public float riseAmount;
	public float duration = 1f;
	public AnimationCurve curve;

	float timer;

	Vector3 v1;
	Vector3 v2;

	private void Awake () {
		v1 = tmp.transform.localPosition;
		v2 = tmp.transform.localPosition + Vector3.up * riseAmount;
	}

	public void SetProps ( string text, Color color ) {
		tmp.color = color;
		tmp.text = text;
	}

	private void Update () {
		timer += Time.deltaTime;

		var p = timer / duration;
		tmp.transform.localPosition = Vector3.Lerp( v1, v2, curve.Evaluate( p ) );

		var color = tmp.color;
		color.a = Mathf.Lerp( 1, 0, curve.Evaluate( p ) );
		tmp.color = color;

		if( p > 1 ) {
			Destroy( gameObject );
		}
	}
}
