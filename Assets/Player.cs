using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour {
    [SerializeField]
    public LineHandler line;

    [SerializeField]
    public TrackMouse mouseTracker;

    [SerializeField]
    public PlayerBall playerBall;

    [SerializeField]
    public GameUI ui;

    float freezeTimer;
    bool frozen;
    bool playing = false;
    bool falling = false;

    public float loseAngle = 10;
    float loseDot;

    bool skimming = false;
    float skimTimer = 0f;

    public float pointsPerSecondBalancing = 10;

	private void Awake () {
        var loseNormal = Quaternion.AngleAxis( loseAngle, Vector3.back ) * Vector3.right;

        loseDot = Vector3.Dot( Vector3.up, loseNormal );

        Debug.Log( loseDot );
	}

	// Start is called before the first frame update
	void Start () {
        playerBall.onDestroy += OnPlayerDestroy;
        playerBall.onCollision += OnPlayerCollision;
        playerBall.onCoinCollide += CollectCoin;
        playerBall.onSkimStart += OnSkimStart;
        playerBall.onSkimStop += OnSkimStop;
        ui.onPlayAgainClick += BeginPlay;
    }

	private void OnSkimStart () {
        skimming = true;
    }

    private void OnSkimStop () {
        skimming = false;
        ScoreManager.ScorePoints( Vector3.zero, Mathf.Floor( skimTimer ) * 250 );

        skimTimer = 0;
    }

    private void CollectCoin ( Coin coin ) {
        coin.Collect();
	}

	private void OnPlayerDestroy () {
		if( ( playing & !frozen ) || falling ) {
            playerBall.OnDestroy();
            if( !falling ) {
              Lose();
            }
        }
    }

	private void OnPlayerCollision ( Vector2 position, Vector2 vel ) {
        ScoreManager.ResetMultiplier();

        ScoreManager.SpawnText( position, "Combo Lost", Color.red );

        if( Mathf.Abs( vel.x ) > 3f && playing && !frozen ) {
            playerBall.rigidbody.velocity = -playerBall.rigidbody.velocity;
            Lose();
		}
    }

    private void BeginPlay () {
        playing = true;
        falling = false;
        FreezeForTime( 2 );

        mouseTracker.transform.position = Vector3.zero;

        line.ResetPlay();
        playerBall.OnPlay();
        mouseTracker.OnPlay();

        ui.DisableGameEnd();

        ScrollManager.instance.Cleanup();
        ScrollManager.instance.StartRunning();

        ScoreManager.OnReset();
    }

    private void Lose () {
        playing = false;
        falling = true;

        line.OnLose();
        mouseTracker.OnLose();

        ui.EnableGameEnd();

        ScrollManager.instance.StopRunning();
    }

    void Freeze () {
        frozen = true;
        mouseTracker.hinge.useLimits = true;
        playerBall.transform.position = ( mouseTracker.transform.position - (Vector3)mouseTracker.hinge.connectedAnchor );

        var white = new Color( 1, 1, 1, 0.5f );
        playerBall.GetComponentInChildren<SpriteRenderer>().color = white;
        line.line.startColor = white;
        line.line.endColor = white;
        playerBall.GetComponentInChildren<TrailRenderer>().enabled = false;
    }

    void Unfreeze () {
		frozen = false;
		mouseTracker.hinge.useLimits = false;

        var white = new Color( 1, 1, 1, 1f );
        playerBall.GetComponentInChildren<SpriteRenderer>().color = white;
        line.line.startColor = white;
        line.line.endColor = white;
        playerBall.GetComponentInChildren<TrailRenderer>().enabled = true;
    }

    void FreezeForTime ( float time ) {
        freezeTimer = time;
        Freeze();
    }

    // Update is called once per frame
    void Update () {
        if( freezeTimer > 0 ) {
            freezeTimer -= Time.deltaTime;

            if( freezeTimer < 0 ) {
                Unfreeze();
            }
        }

        if( playing ) {
            var ray = Camera.main.ScreenPointToRay( Input.mousePosition );

            var worldPlane = new Plane( Vector3.forward, Vector3.zero );

            worldPlane.Raycast( ray, out float d );

            var worldPoint = (Vector2)ray.GetPoint( d );

            mouseTracker.rigidbody.MovePosition( worldPoint );

            if( frozen ) {
                playerBall.transform.position = ( worldPoint - mouseTracker.hinge.connectedAnchor );
			}

            if( skimming && !frozen ) {
                skimTimer += Time.deltaTime;

                if( skimTimer > 0.25f ) {
                    ScoreManager.ScorePoints( Vector3.zero, Mathf.Floor( skimTimer + 0.75f ) * 100 * Time.deltaTime );
				}
			} else {
                skimTimer = 0;
			}

            if( Input.GetKey( KeyCode.UpArrow ) ) {
                var anchor = mouseTracker.hinge.connectedAnchor;

                anchor.y += 1f * Time.deltaTime;

                mouseTracker.hinge.connectedAnchor = anchor;
            }

            if( Input.GetKey( KeyCode.DownArrow ) ) {
                var anchor = mouseTracker.hinge.connectedAnchor;

                anchor.y -= 1f * Time.deltaTime;

                mouseTracker.hinge.connectedAnchor = anchor;
            }

            var playerDelta = ( mouseTracker.transform.position - playerBall.transform.position );
            var playerNormal = playerDelta.normalized;

            if( !frozen ) {
                var verticalDot = Vector3.Dot( Vector3.up, playerNormal );

                var scoreDot = Mathf.Abs( verticalDot );

                ScoreManager.ScorePoints( Vector2.zero, scoreDot * pointsPerSecondBalancing * ScrollManager.instance.scrollSpeed * Time.deltaTime );

                var stretchRange = mouseTracker.hinge.connectedAnchor.sqrMagnitude * 0.2f;

                var minStretch = mouseTracker.hinge.connectedAnchor.sqrMagnitude - stretchRange;
                var maxStretch = mouseTracker.hinge.connectedAnchor.sqrMagnitude + stretchRange;

                if( verticalDot > loseDot ) {
                    Lose();
                } else if( playerDelta.sqrMagnitude < minStretch || playerDelta.sqrMagnitude > maxStretch ) {
					Lose();
				}
            }

            line.SetPoints( mouseTracker.transform.position, playerBall.transform.position );
        }
    }
}
