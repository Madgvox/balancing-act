using UnityEngine;

public class CameraBounds : MonoBehaviour {
	public Camera camera;

	public GameObject wallPrefab;
	public GameObject floorPrefab;

	private void Awake () {
		var cameraPos = camera.transform.position;
		cameraPos.z = 0;
		var camHalfSize = new Vector3( camera.orthographicSize * camera.aspect, camera.orthographicSize );

		var leftWall = Instantiate( wallPrefab, transform );
		leftWall.transform.position = cameraPos + new Vector3( -camHalfSize.x - 0.5f, 0 );

		leftWall.transform.localScale = new Vector3( 1, camHalfSize.y * 2, 1 );

		var rightWall = Instantiate( wallPrefab, transform );
		rightWall.transform.position = cameraPos + new Vector3( camHalfSize.x + 0.5f, 0 );

		rightWall.transform.localScale = new Vector3( 1, camHalfSize.y * 2, 1 );

		var floor = Instantiate( floorPrefab, transform );
		floor.transform.position = cameraPos + new Vector3( 0, -camHalfSize.y - 0.5f );

		floor.transform.localScale = new Vector3( camHalfSize.x * 2, 1, 1 );
	}
}
