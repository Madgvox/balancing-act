using System;
using UnityEngine;

[SelectionBase]
public class Coin : MonoBehaviour {
	public SpriteRenderer renderer;
	
	public Action<Coin> onCollect;

	public ParticleSystem destroyParticles;

	public float points;
	public bool partOfGroup;

	public void Destroy () {
		Destroy( gameObject );
		Instantiate( destroyParticles, transform.position, Quaternion.identity, transform.parent );
	}

	public void Collect () {
		onCollect?.Invoke( this );

		ScoreManager.ScorePoints( this );
		ScoreManager.SpawnText( transform.position, $"+{points}", renderer.color );
		Destroy();
	}
}
