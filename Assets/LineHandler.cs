using UnityEngine;

public class LineHandler : MonoBehaviour {
    [SerializeField]
    public LineRenderer line;

    [SerializeField]
    Rigidbody2D rigidbody;

    Vector3[] linePos;

	private void Awake () {
        linePos = new Vector3[ 2 ];
    }

	public void SetPoints ( Vector3 p1, Vector3 p2 ) {
        transform.position = p1 + ( p2 - p1 ) * 0.5f;
        linePos[ 0 ] = transform.InverseTransformPoint( p1 );
        linePos[ 1 ] = transform.InverseTransformPoint( p2 );
        line.SetPositions( linePos );
    }

    public void ResetPlay () {
        rigidbody.bodyType = RigidbodyType2D.Kinematic;
        rigidbody.velocity = Vector2.zero;
        rigidbody.angularVelocity = 0;
	}

    public void OnLose () {
        rigidbody.bodyType = RigidbodyType2D.Dynamic;
        var randomTorque = Random.Range( -300, 300 );

        rigidbody.AddTorque( randomTorque );

        var randomUp = Random.Range( 100, 300 );
        var randomHorizonal = Random.Range( 100, 200 );
        randomHorizonal = Random.value < 0.5 ? -randomHorizonal : randomHorizonal;
        rigidbody.AddForce( Vector3.up * randomUp + Vector3.right * randomHorizonal );
	}

	private void OnDrawGizmos () {
        Gizmos.color = Color.red;
        Gizmos.DrawLine( transform.position - Vector3.right, transform.position + Vector3.right );
        Gizmos.DrawLine( transform.position - Vector3.up, transform.position + Vector3.up );
	}

	internal void Reset () {
		throw new System.NotImplementedException();
	}
}
