using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapGenerator : MonoBehaviour {

	public Map[] maps;
	public int mapIndex;
	public int eyeCandyOffset = 5;

	public Transform tilePrefab;
	public Transform extraTilePrefab;
	public Transform obstaclePrefab;
	public Transform mapFloor;
	public Transform navMeshFloor;
	public Transform navMeshPrefab;
	public Transform OrbitPivot;
	public Vector2 maxMapSize;

	public Map currentMap;

	[Range(0,1)]
	public float outlinePercent;
	[Range(0,1)]
	public float extraTileSizeScale;

	public float tileSize;
	List<Coord> allExtraTileCoords;
	List<Coord> allTileCoords;
	Queue<Coord> shuffledTileCoords;
	Queue<Coord> shuffledExtraTileCoords;
	Queue<Coord> shuffledOpenTileCoords;
	Transform[,] tileMap;

	float radius;

	void Awake(){
		FindObjectOfType<Spawner>().OnNewWave += OnNewWave;
	}

	void OnNewWave(int waveNumber){
		mapIndex = waveNumber - 1;
		GenerateMap ();
	}

	public void GenerateMap ()
	{
		if (OrbitPivot.Find ("Extra Tiles")!=null) {
			DestroyImmediate (OrbitPivot.Find ("Extra Tiles").gameObject);
		}

		currentMap = maps [mapIndex];
		tileMap = new Transform[currentMap.mapSize.x, currentMap.mapSize.y];
		System.Random prng = new System.Random (currentMap.seed);

		//generate coordinates
		allTileCoords = new List<Coord> ();
		for (int x = 0; x < currentMap.mapSize.x; x++) {
			for (int y = 0; y < currentMap.mapSize.y; y++) {
				allTileCoords.Add (new Coord (x, y));
			}
		}
		shuffledTileCoords = new Queue<Coord> (Utility.ShuffleArray (allTileCoords.ToArray (), currentMap.seed));

		allExtraTileCoords = new List<Coord> ();		
		for (int x = -currentMap.mapSize.x; x < maxMapSize.x - currentMap.mapSize.x; x++) {
			for (int y = -currentMap.mapSize.y; y < maxMapSize.y - currentMap.mapSize.y; y++) {
				allExtraTileCoords.Add (new Coord (x, y));
			}
		}
		shuffledExtraTileCoords = new Queue<Coord> (Utility.ShuffleArray (allTileCoords.ToArray (), currentMap.seed));

		//creating map holder object / extra tiles for eye candy object
		string holderName = "Generated Map";
		if (transform.FindChild (holderName)) {
			DestroyImmediate (transform.FindChild (holderName).gameObject);
		}
		string eyeCandyName = "Extra Tiles";
		if (transform.FindChild (eyeCandyName)) {
			DestroyImmediate (transform.FindChild (eyeCandyName).gameObject);
		}
			
		Transform mapHolder = new GameObject (holderName).transform;
		Transform extraMapHolder = new GameObject (eyeCandyName).transform;
		mapHolder.parent = transform;
		extraMapHolder.parent = transform;


		//spawning tiles
		for (int x = 0; x < currentMap.mapSize.x; x++) {
			for (int y = 0; y < currentMap.mapSize.y; y++) {
				Vector3 tilePosition = CoordToPosition (x, y);
				Transform newTile = Instantiate (tilePrefab, tilePosition, Quaternion.Euler (Vector3.right * 90)) as Transform;
				newTile.localScale = Vector3.one * (1 - outlinePercent) * tileSize;
				newTile.parent = mapHolder;
				tileMap [x, y] = newTile;
			}
		}

		bool[,] extraTileMap = new bool[(int)currentMap.mapSize.x, (int)currentMap.mapSize.y];

		//spawning eye candy tiles
		//spawning BOTTOM tiles
		for (int x = -currentMap.mapSize.x; x < (maxMapSize.x - currentMap.mapSize.x); x++) {
			for (int y = -currentMap.mapSize.y; y < 0; y++) {

				float extraTileHeight = Mathf.Lerp (currentMap.minExtraTileHeight, currentMap.maxExtraTileHeight, (float)prng.NextDouble ());
				float extraTileSize = Mathf.Lerp (currentMap.minExtraTileSize, currentMap.maxExtraTileSize, (float)prng.NextDouble ());
				Vector3 extraTilePosition = CoordToPosition (x, y);
				Quaternion randomRotation = Quaternion.Euler (Random.Range (0, 360), Random.Range (0, 360), Random.Range (0, 360));

				Transform newExtraTile = Instantiate (extraTilePrefab, extraTilePosition + Vector3.up * extraTileHeight, randomRotation) as Transform;
				newExtraTile.localScale = Vector3.one * (1 - extraTileSizeScale) * extraTileSize;
				newExtraTile.parent = extraMapHolder;

			}
		}

		//spawning eye candy tiles
		//spawning TOP tiles
		for (int x = -currentMap.mapSize.x; x < (maxMapSize.x - currentMap.mapSize.x); x++) {
			for (int y = currentMap.mapSize.y; y < maxMapSize.y - currentMap.mapSize.y; y++) {
				float extraTileHeight = Mathf.Lerp (currentMap.minExtraTileHeight, currentMap.maxExtraTileHeight, (float)prng.NextDouble ());
				float extraTileSize = Mathf.Lerp (currentMap.minExtraTileSize, currentMap.maxExtraTileSize, (float)prng.NextDouble ());
				Vector3 extraTilePosition = CoordToPosition (x, y);
				Quaternion randomRotation = Quaternion.Euler (Random.Range (0, 360), Random.Range (0, 360), Random.Range (0, 360));

				Transform newExtraTile = Instantiate (extraTilePrefab, extraTilePosition + Vector3.up * extraTileHeight, randomRotation) as Transform;
				newExtraTile.localScale = Vector3.one * (1 - extraTileSizeScale) * extraTileSize;
				newExtraTile.parent = extraMapHolder;

			}
		}

		//spawning eye candy tiles
		//spawning LEFT tiles
		for (int x = -currentMap.mapSize.x; x < 0; x++) {
			for (int y = 0; y < currentMap.mapSize.y; y++) {
				float extraTileHeight = Mathf.Lerp (currentMap.minExtraTileHeight, currentMap.maxExtraTileHeight, (float)prng.NextDouble ());
				float extraTileSize = Mathf.Lerp (currentMap.minExtraTileSize, currentMap.maxExtraTileSize, (float)prng.NextDouble ());
				Vector3 extraTilePosition = CoordToPosition (x, y);
				Quaternion randomRotation = Quaternion.Euler (Random.Range (0, 360), Random.Range (0, 360), Random.Range (0, 360));

				Transform newExtraTile = Instantiate (extraTilePrefab, extraTilePosition + Vector3.up * extraTileHeight, randomRotation) as Transform;
				newExtraTile.localScale = Vector3.one * (1 - extraTileSizeScale) * extraTileSize;
				newExtraTile.parent = extraMapHolder;

			}
		}

		//spawning eye candy tiles
		//spawning RIGHT tiles
		for (int x = currentMap.mapSize.x; x < (maxMapSize.x - currentMap.mapSize.x); x++) {
			for (int y = 0; y < currentMap.mapSize.y; y++) {
				float extraTileHeight = Mathf.Lerp (currentMap.minExtraTileHeight, currentMap.maxExtraTileHeight, (float)prng.NextDouble ());
				float extraTileSize = Mathf.Lerp (currentMap.minExtraTileSize, currentMap.maxExtraTileSize, (float)prng.NextDouble ());
				Vector3 extraTilePosition = CoordToPosition (x, y);
				Quaternion randomRotation = Quaternion.Euler (Random.Range (0, 360), Random.Range (0, 360), Random.Range (0, 360));

				Transform newExtraTile = Instantiate (extraTilePrefab, extraTilePosition + Vector3.up * extraTileHeight, randomRotation) as Transform;
				newExtraTile.localScale = Vector3.one * (1 - extraTileSizeScale) * extraTileSize;
				newExtraTile.parent = extraMapHolder;

			}
		}

		//creating obstacles
		bool[,] obstacleMap = new bool[(int)currentMap.mapSize.x, (int)currentMap.mapSize.y];
		int currentObstacleCount = 0;
		List<Coord> allOpenCoords = new List<Coord> (allTileCoords);

		int obstacleCount = (int)(currentMap.mapSize.x * currentMap.mapSize.y * currentMap.obstaclePercent);
		for (int i = 0; i < obstacleCount; i++) {
			Coord randomCoord = GetRandomCoord ();
			obstacleMap [randomCoord.x, randomCoord.y] = true;
			currentObstacleCount++;

			if (randomCoord != currentMap.mapCenter && MapIsFullyAccessible (obstacleMap, currentObstacleCount)) {
				float obstacleHeight = Mathf.Lerp (currentMap.minObstacleHeight, currentMap.maxObstacleHeight, (float)prng.NextDouble ());
				Vector3 obstaclePosition = CoordToPosition (randomCoord.x, randomCoord.y);

				Transform newObstacle = Instantiate (obstaclePrefab, obstaclePosition + Vector3.up * obstacleHeight/2, Quaternion.identity) as Transform;
				newObstacle.parent = mapHolder;
				newObstacle.localScale = new Vector3(((1 - outlinePercent) * tileSize),obstacleHeight,((1 - outlinePercent) * tileSize));

				Renderer obstacleRenderer = newObstacle.GetComponent<Renderer> ();
				Material obstacleMaterial = new Material (obstacleRenderer.sharedMaterial);
				float colorPercent = randomCoord.y / (float)currentMap.mapSize.y;
				obstacleMaterial.color = Color.Lerp (currentMap.foregroundColor, currentMap.backgroundColor, colorPercent);
				obstacleRenderer.sharedMaterial = obstacleMaterial;

				allOpenCoords.Remove (randomCoord);
			} else {
				obstacleMap [randomCoord.x, randomCoord.y] = false;
				currentObstacleCount--;
			}
		}

		shuffledOpenTileCoords = new Queue<Coord> (Utility.ShuffleArray (allOpenCoords.ToArray (), currentMap.seed));


		//creating navmesh masks
		Transform maskLeft = Instantiate (navMeshPrefab, Vector3.left * (currentMap.mapSize.x + maxMapSize.x) / 4f * tileSize, Quaternion.identity) as Transform;
		maskLeft.parent = mapHolder;
		maskLeft.localScale = new Vector3 ((maxMapSize.x - currentMap.mapSize.x) / 2f, 1, currentMap.mapSize.y) * tileSize;

		Transform maskRight = Instantiate (navMeshPrefab, Vector3.right * (currentMap.mapSize.x + maxMapSize.x) / 4f * tileSize, Quaternion.identity) as Transform;
		maskRight.parent = mapHolder;
		maskRight.localScale = new Vector3 ((maxMapSize.x - currentMap.mapSize.x) / 2f, 1, currentMap.mapSize.y) * tileSize;

		Transform maskTop = Instantiate (navMeshPrefab, Vector3.forward * (currentMap.mapSize.y + maxMapSize.y) / 4f * tileSize, Quaternion.identity) as Transform;
		maskTop.parent = mapHolder;
		maskTop.localScale = new Vector3 (maxMapSize.x, 1, (maxMapSize.y - currentMap.mapSize.y) / 2f) * tileSize;

		Transform maskBottom = Instantiate (navMeshPrefab, Vector3.back * (currentMap.mapSize.y + maxMapSize.y) / 4f * tileSize, Quaternion.identity) as Transform;
		maskBottom.parent = mapHolder;
		maskBottom.localScale = new Vector3 (maxMapSize.x, 1, (maxMapSize.y - currentMap.mapSize.y) / 2f) * tileSize;

		navMeshFloor.localScale = new Vector3 (maxMapSize.x, maxMapSize.y) * tileSize;

		extraMapHolder.parent = OrbitPivot;

		//creating map floor
		mapFloor.localScale = new Vector3 (currentMap.mapSize.x * tileSize, currentMap.mapSize.y * tileSize);

	}
	

	bool MapIsFullyAccessible(bool[,] obstacleMap, int currentObstacleCount){
		
		bool[,] mapFlags = new bool[obstacleMap.GetLength (0), obstacleMap.GetLength (1)];
		Queue<Coord> queue = new Queue<Coord> ();
		queue.Enqueue (currentMap.mapCenter);
		mapFlags [currentMap.mapCenter.x, currentMap.mapCenter.y] = true;

		int accessibleTileCount = 1;

		while (queue.Count > 0) {
			Coord tile = queue.Dequeue ();

			for (int x = -1; x <= 1; x++) {

				for (int y = -1; y <= 1; y++) {
					int neighborX = tile.x + x;
					int neighborY = tile.y + y;
					if (x == 0 || y == 0) {
						if (neighborX >= 0 && neighborX < obstacleMap.GetLength (0) && neighborY >= 0 && neighborY < obstacleMap.GetLength (1)) {
							if (!mapFlags [neighborX, neighborY] && !obstacleMap [neighborX, neighborY]) {
								mapFlags [neighborX, neighborY] = true;
								queue.Enqueue (new Coord (neighborX, neighborY));
								accessibleTileCount++;
							}
						}
					}
				}
			}
		}

		int targetAccessibleTileCount = (int)(currentMap.mapSize.x * currentMap.mapSize.y - currentObstacleCount);
		return targetAccessibleTileCount == accessibleTileCount;
	}

	Vector3 CoordToPosition(int x, int y){
		return new Vector3 (-currentMap.mapSize.x / 2f + 0.5f + x, 0f, -currentMap.mapSize.y / 2f + 0.5f + y) * tileSize;
	}

	public Transform GetTileFromPosition(Vector3 position){
		int x = Mathf.RoundToInt (position.x / tileSize + (currentMap.mapSize.x - 1) / 2f);
		int y = Mathf.RoundToInt (position.z / tileSize + (currentMap.mapSize.y - 1) / 2f);
		x = Mathf.Clamp (x, 0, tileMap.GetLength (0) -1);
		y = Mathf.Clamp (y, 0, tileMap.GetLength (1) -1);

		return tileMap [x, y];
	}

	public Coord GetRandomCoord(){
		Coord randomCoord = shuffledTileCoords.Dequeue ();
		shuffledTileCoords.Enqueue (randomCoord);
		return randomCoord;
	}

	public Transform GetRandomOpenTile(){
		Coord randomCoord = shuffledOpenTileCoords.Dequeue ();
		shuffledOpenTileCoords.Enqueue (randomCoord);
		return tileMap [randomCoord.x, randomCoord.y];
	}

	[System.Serializable]
	public struct Coord{
		public int x;
		public int y;

		public Coord(int _x,int _y){
			x= _x;
			y= _y;
		}

		public static bool operator ==(Coord c1, Coord c2){
			return c1.x == c2.x && c1.y == c2.y;
		}

		public static bool operator !=(Coord c1, Coord c2){
			return !(c1 == c2);
		}
	}

	[System.Serializable]
	public class Map{
		public Coord mapSize;
		[Range(0,1)]
		public float obstaclePercent;
		public int seed;
		public float minObstacleHeight;
		public float maxObstacleHeight;

		public float minExtraTileHeight = -30;
		public float maxExtraTileHeight = 30;
		public float minExtraTileSize = 0f;
		public float maxExtraTileSize = 2.6f;

		public Color foregroundColor;
		public Color backgroundColor;

		public Coord mapCenter {
			get{
				return new Coord (mapSize.x / 2, mapSize.y / 2);
			}
		}
	}
}
