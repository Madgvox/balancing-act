using UnityEngine;

public class GameplaySection : MonoBehaviour {

	public int minimumLevel = 1;
	public float difficultyWeight = 1;

	public float baseScrollSpeed = 1;

	public float bufferHeight = 5;

	public AnimationCurve scrollCurve = AnimationCurve.Linear( 0, 0, 1, 1 );

	float _sectionHeight = 0;

	public float sectionHeight {
		get {
			return _sectionHeight;
		}
	} 

	private void Awake () {
		_sectionHeight = CalculateSectionHeight();
	}

	private float CalculateSectionHeight () {
		var height = 0f;

		foreach( var comp in GetComponentsInChildren<Platform>() ) {
			if( comp == transform ) continue;

			var localPos = comp.transform.position - transform.position;

			if( localPos.y > height ) height = localPos.y;
		}

		return height + bufferHeight;
	}

	private void OnDrawGizmos () {
		var min = 0f;
		var max = CalculateSectionHeight();

		var bounds = new Bounds();

		var cam = Camera.main;

		var width = cam.orthographicSize * cam.aspect * 2;

		bounds.min = new Vector3( 0, min );
		bounds.max = new Vector3( width, max );

		Gizmos.matrix = transform.localToWorldMatrix;
		Gizmo.Bounds( bounds );

		Gizmos.color = Color.red;
		Gizmo.Line( Vector3.zero, new Vector3( width, 0 ) );

		Gizmos.color = new Color32( 255, 0, 0, 64 );

		var tickCount = max * baseScrollSpeed;

		for( var i = 0; i < tickCount; i++ ) {
			var p = i / tickCount;

			var sample = scrollCurve.Evaluate( p );

			var yVal = sample * max;
			Gizmo.Line( new Vector3( 0, yVal ), new Vector3( width, yVal ) );
		}
	}
}
