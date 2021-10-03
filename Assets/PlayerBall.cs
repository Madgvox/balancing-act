using System;
using System.Collections.Generic;
using UnityEngine;

public class PlayerBall : MonoBehaviour {
	[SerializeField]
	ParticleSystem destroyParticles;

	[SerializeField]
	public Rigidbody2D rigidbody;

	public Action onSkimStop;
	public Action onSkimStart;
	public Action onDestroy;
	public Action<Coin> onCoinCollide;
	public Action<Vector2,Vector2> onCollision;

	List<Collider2D> skimmingColliders = new List<Collider2D>();

	public void OnDestroy () {
#if UNITY_EDITOR
		if( !UnityEditor.EditorApplication.isPlayingOrWillChangePlaymode ) return;
#endif
		gameObject.SetActive( false );
		
		var clone = Instantiate( destroyParticles );
		clone.transform.position = transform.position;
		clone.Play();
	}

	internal void OnPlay () {
		gameObject.SetActive( true );
		rigidbody.velocity = Vector2.zero;
		rigidbody.angularVelocity = 0;
	}

	private void OnTriggerEnter2D ( Collider2D collider ) {
		if( collider.CompareTag( "danger" ) ) {
			onDestroy?.Invoke();
		} else if( collider.CompareTag( "coin" ) ) {
			onCoinCollide?.Invoke( collider.GetComponent<Coin>() );
		}

		if( collider.CompareTag( "skim" ) ) {
			if( skimmingColliders.Count == 0 ) {
				onSkimStart.Invoke();
			}
			skimmingColliders.Add( collider );
		}
	}

	private void OnTriggerExit2D ( Collider2D collider ) {
		if( collider.CompareTag( "skim" ) ) {
			skimmingColliders.Remove( collider );
			if( skimmingColliders.Count == 0 ) {
				onSkimStop.Invoke();
			}
		}
	}

	private void OnCollisionEnter2D ( Collision2D collision ) {
		onCollision?.Invoke( collision.contacts[ 0 ].point, collision.relativeVelocity );
	}
}
