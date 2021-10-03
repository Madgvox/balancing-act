using System;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class ScrollManager : MonoBehaviour {
    public static ScrollManager instance;

	public List<GameplaySection> sectionPrefabs;

	public List<GameplaySection> activeGamplaySections;

	GameplaySection lastSection;

	public float scrollSpeedTarget = 1f;
	public float scrollSpeedVel;

	public float scrollSpeed = 1f;

	public int currentLevel = 1;

	public int levelUpCounter = 0;

	public float timer = 0;

	bool running = false;

	class SectionComparer : IComparer<GameplaySection> {
		public int Compare ( GameplaySection x, GameplaySection y ) {
			var levelWeight = x.minimumLevel - y.minimumLevel;

			if( levelWeight == 0 ) {
				var difficultyWeight = x.difficultyWeight - y.difficultyWeight;

				if( difficultyWeight < 0 ) return -1;
				if( difficultyWeight > 0 ) return 1;
				return 0;
			}

			return levelWeight;
		}
	}

	private void Awake () {
        instance = this;

		sectionPrefabs.Sort( new SectionComparer() );
	}

	public void Update () {
		if( running ) {
			timer += Time.deltaTime;

			scrollSpeed = Mathf.SmoothDamp( scrollSpeed, scrollSpeedTarget, ref scrollSpeedVel, 5f );

			Camera camera = Camera.main;

			var camPos = camera.transform.position;
			camPos.z = 0;
			var camHalfSize = new Vector3( camera.orthographicSize * camera.aspect, camera.orthographicSize );

			var spawnLocation = camPos + new Vector3( -camHalfSize.x, camHalfSize.y );
			var despawnHeight = camPos.y - camHalfSize.y;

			if( lastSection == null ) {
				SpawnSection( spawnLocation );
			} else {
				var lastSectionTop = lastSection.transform.position + new Vector3( 0, lastSection.sectionHeight * scrollSpeed );

				if( lastSectionTop.y < spawnLocation.y ) {
					SpawnSection( lastSectionTop );
				}
			}

			for( int i = activeGamplaySections.Count - 1; i >= 0; i-- ) {
				GameplaySection section = activeGamplaySections[ i ];
				section.transform.position -= new Vector3( 0, scrollSpeed * Time.deltaTime );

				var sectionTop = section.transform.position.y + section.sectionHeight + 5;

				if( sectionTop < despawnHeight ) {
					activeGamplaySections.RemoveAt( i );
					Destroy( section.gameObject );
				}
			}
		}
	}

	private void SpawnSection ( Vector3 position ) {
		var prefab = GetRandomWeightedSection();

		var inst = Instantiate( prefab, position, Quaternion.identity, transform );

		activeGamplaySections.Add( inst );

		lastSection = inst;

		levelUpCounter += 1;
		if( levelUpCounter > 2 ) {
			levelUpCounter = 0;
			currentLevel += 1;
			scrollSpeedTarget += 0.25f;
		}
	}

	GameplaySection GetRandomWeightedSection () {
		var totalWeight = 0f;
		var selection = sectionPrefabs.FindAll( s => {
			if( s.minimumLevel <= currentLevel ) {
				totalWeight += s.difficultyWeight;
				return true;
			}

			return false;
		});

		var sum = 0f;
		var randomWeight = Random.Range( 0, totalWeight );

		for( int i = 0; i < selection.Count; i++ ) {
			var section = selection[ i ];
			sum += section.difficultyWeight;

			if( sum > randomWeight ) {
				return section;
			}
		}

		return null;
	}

	public void Cleanup () {
		foreach( var section in activeGamplaySections ) {
			Destroy( section.gameObject );
		}

		activeGamplaySections.Clear();

		scrollSpeed = 1;
		scrollSpeedTarget = 1;
		levelUpCounter = 0;
		currentLevel = 1;
		timer = 0;
	}

	public void StartRunning () {
		running = true;
	}

	public void StopRunning () {
		running = false;
	}
}
