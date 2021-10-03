using System;
using UnityEngine;

public class CoinGroup : MonoBehaviour {
	float coinsInGroup = 0;

	float coinsCollected = 0;
	float totalBonus = 0;

	private void Awake () {
		var coins = GetComponentsInChildren<Coin>();
		coinsInGroup = coins.Length;

		foreach( var coin in coins ) {
			coin.onCollect += OnCoinCollect;
			coin.partOfGroup = true;
		}
	}

	private void OnCoinCollect ( Coin coin ) {
		coinsCollected += 1;

		totalBonus += coin.points;

		// TODO SOUNDS

		if( coinsCollected == coinsInGroup ) {
			ScoreManager.ScorePoints( coin.transform.position, totalBonus, true );
			ScoreManager.SpawnText( coin.transform.position + Vector3.up * 0.3f, $"Group Bonus!\nx2", Color.magenta );
		}
	}
}
