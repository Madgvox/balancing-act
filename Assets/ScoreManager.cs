using System;
using UnityEngine;

public class ScoreManager : MonoBehaviour {
    public static ScoreManager instance;

    public DisplayText textPrefab;

    float _score = 0;
    public float score {
        get {
            return _score;
		}
        set {
            var oldScore = _score;
            _score = value;
            onScoreChange?.Invoke( _score, oldScore );
		}
	}

    int _multiplier = 1;
    public int multiplier {
        get {
            return _multiplier;
		}
        set {
            _multiplier = value;
            onMultiplierChange?.Invoke( _multiplier );
		}
	}

    public float scoreSinceLastMultiplier = 0;

    public Action<float,float> onScoreChange;
    public Action<int> onMultiplierChange;

	private void Awake () {
        instance = this;
	}

    public static void ScorePoints ( Vector3 position, float points, bool countAsCoin = false ) {
        instance.AddPoints( points, countAsCoin );
	}

    public static void ScorePoints ( Coin coin ) {
        instance.AddPoints( coin.points, true );
    }

    void AddPoints ( float points, bool isCoin = false ) {
        score += points * multiplier;

        if( isCoin ) {
            scoreSinceLastMultiplier += points * multiplier;
        }

        if( scoreSinceLastMultiplier > multiplier * 100 ) {
            scoreSinceLastMultiplier = 0;
            multiplier += 1;
        }
    }

    public static void ResetMultiplier () {
        instance.multiplier = 1;
        instance.scoreSinceLastMultiplier = 0;
	}

    public static void OnReset () {
        instance.score = 0;
        ResetMultiplier();
	}

	public static void SpawnText ( Vector2 position, string v, Color color ) {

        Camera camera = Camera.main;

        var camPos = camera.transform.position;
        camPos.z = 0;
        var camHalfSize = new Vector3( camera.orthographicSize * camera.aspect, camera.orthographicSize );


        var text = Instantiate( instance.textPrefab, instance.transform );
        text.transform.position = position;
        text.SetProps( v, color );
        text.tmp.ForceMeshUpdate();

        var bounds = text.tmp.textBounds;
        var left = bounds.min + text.transform.position;
        var right = bounds.max + text.transform.position;

        if( left.x < camPos.x - camHalfSize.x ) {
            text.transform.position = new Vector3( camPos.x - camHalfSize.x + bounds.extents.x, text.transform.position.y, 0 );
		} else if( right.x > camPos.x + camHalfSize.x ) {
            text.transform.position = new Vector3( camPos.x + camHalfSize.x - bounds.extents.x, text.transform.position.y, 0 );
        }
    }
}
