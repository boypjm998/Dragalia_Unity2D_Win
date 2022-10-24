using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(MeshFilter))]
[RequireComponent(typeof(MeshRenderer))]
public class Trail2D : MonoBehaviour
{
	[Header("Generation")]
	[Tooltip("Clears mesh and resets counters and timers")]
	public bool reset = false;
	[Tooltip("Toggle for spawning trail points")]
	public bool on = false;
	 
	[Tooltip("Amount of trail points spawned per second")]
	public float pointSpawnSpeed = 5;
	//[Tooltip("Optional parent that will offset the trail rotation")]
	//public GameObject parent;

	[Header("Attributes")]
	[Tooltip("Speed in Unity units per second")]
	public float speed = 5f;
	[Tooltip("Trail point life time in seconds")]
	public float lifeTime = 1f;
	[Tooltip("Trail point velocity dampening")]
	public float dampening = 0;
	[Tooltip("Adds to width based on time alive")]
	public float sizeChange = 0f;
	[Tooltip("Width of mesh trail in Unity units at time of trail point spawn")]
	public float width = 2;
	[Tooltip("Randomly moves starting width up or down between 0 and this value every time a mesh point spawns")]
	public float widthRnd = 0;
	[Tooltip("Minimum and maximum deviancy the randomness can set the width to (Width+-this value=Width limit)")]
	public float widthRndMinMax = 0;

	[Header("Manipulation")]
	[Tooltip("Sets the point direction to the point's normalized velocity vector")]
	public bool velocityDecidesDirection = false;
	[Tooltip("Sets the point direction to look at the previous point position")]
	public bool directionPreviousPointPos = false;
	[Tooltip("Each point looks at the previous point and rotates its direction toward it. Is ignored if 'Velocity Decides Direction' or 'Direction Previous Point Pos' is set")]
	[Range(0f,1f)]
	public float straightenDirectionFactor = 0f;
	private float prevStraightenDirectionFactor = 0f;
	[Tooltip("Each point looks at the previous point and moves toward it")]
	[Range(0f,1f)]
	public float straightenPositionFactor = 0f;

	[Header("Vertex color fading")]
	[Tooltip("An offset to the fading of vcolor.a in points, to avoid triangle popping at the end of the trail, especially on lower res meshes. Default: 2")]
	public int lifeTimeFadeOffset = 2;
	[Tooltip("Fades out vcolor.r from the start of all trail point chains")]
	public float startColorFade = 0;
	[Tooltip("Fades out vcolor.g from the end of all trail point chains")]
	public float endColorFade = 0;

	[Header("World space force")]
	[Tooltip("Applies a world space force to all trail points")]
	public bool worldForce = false;
	[Tooltip("Only applied if World Force is set")]
	public Vector2 worldForce1;
	[Tooltip("Applies a random world space force between World Force 1 and World Force 2 to all trail points")]
	public bool worldForceRandom = false;
	[Tooltip("Only applied if World Force Random is set")]
	public Vector2 worldForce2;
	
	private MeshFilter mf;
	private Mesh mesh;
	private int numPointsAlive = 0;
	private int latestSpawnedIndex = 0;
	private float timeUntilNextSpawn;
	private bool lastFrameWasOn = false;
	private bool activeChain = false;
	private int numChains = 0;
	
	private float currWidth = 2;
	private int numVerts, numTriPoints;
	private float spawnTimer = 0;
	private bool flip = false;

	private Quaternion newRotation = new Quaternion();
	private float newWidth;
	private float yUV;
	private byte lifeColor, startColor, endColor;


	public class TrailPoint{
		public Vector3 pos, dir, velocity;
		public float spawnTime, speed, width;
		public bool isFirst, flip;
	};

	private List<TrailPoint> points = new List<TrailPoint>();
	private TrailPoint tempPoint = new TrailPoint();

	private List<Vector3> verts = new List<Vector3>();
	private Vector3 tempVert = new Vector3();
	private Vector3 tempVertTransformOffset = new Vector3();
	private Vector3 tempVertLocalPosition = new Vector3();

	private List<Color32> vcols = new List<Color32>();
	private Color32 tempVcol = new Color32();

	private List<Vector2> uvs = new List<Vector2>();
	private Vector2 tempUv = new Vector2();
	private Quaternion quaternionOffset = Quaternion.Euler(0,0,90);
	void Start()
	{
		Init();
	}

	void OnValidate() {
		pointSpawnSpeed = Mathf.Max(pointSpawnSpeed, 0.001f);
		lifeTime = Mathf.Max(lifeTime, 0.001f);
		width = Mathf.Max(width, 0.0001f);
		widthRnd = Mathf.Max(widthRnd, 0);
		widthRndMinMax = Mathf.Max(widthRndMinMax, 0);
		straightenDirectionFactor = Mathf.Clamp01(straightenDirectionFactor);
		straightenPositionFactor = Mathf.Clamp01(straightenPositionFactor);
		lifeTimeFadeOffset = Mathf.Max(lifeTimeFadeOffset, 0);
		startColorFade = Mathf.Max(startColorFade, 0);
		endColorFade = Mathf.Max(endColorFade, 0);
		currWidth = width;
		if(velocityDecidesDirection){
			directionPreviousPointPos = false;
			if(prevStraightenDirectionFactor == 0){
				prevStraightenDirectionFactor = straightenDirectionFactor;
			}
			straightenDirectionFactor = 0;
		} else if(directionPreviousPointPos){
			if(prevStraightenDirectionFactor == 0){
				prevStraightenDirectionFactor = straightenDirectionFactor;
			}
			straightenDirectionFactor = 0;
		} else if(prevStraightenDirectionFactor != 0){
			straightenDirectionFactor = prevStraightenDirectionFactor;
			prevStraightenDirectionFactor = 0;
		}
	}

	void Init(){
		numPointsAlive = 0;
		numChains = 0;
		mf = GetComponent<MeshFilter>();
		if(mesh != null)
		{
			mesh.Clear();
		} else {
			mesh = new Mesh();
		}
		spawnTimer = 0;
		currWidth = width;
		points.Clear();
		
		numVerts = 0;
		numTriPoints = 0;

		activeChain = false;
		lastFrameWasOn = false;

		reset = false;
	}
 
	public void MeshTrail(){

		float dt = Time.deltaTime;
		
		float zToDeg = transform.rotation.eulerAngles.z * Mathf.Deg2Rad;
		float sinTRotZDeg = Mathf.Sin(zToDeg);
		float cosTRotZDeg = Mathf.Cos(zToDeg);

		if(reset){
			Init();
		}
		if(on){
			spawnTimer += dt;
			if(!lastFrameWasOn){
				SpawnNewPoint(sinTRotZDeg, cosTRotZDeg);
				SpawnNewPoint(sinTRotZDeg, cosTRotZDeg);
				numChains++;
				timeUntilNextSpawn = 1 / pointSpawnSpeed;
			}
			else if(timeUntilNextSpawn <= 0f){ // time for spawn
				SpawnNewPoint(sinTRotZDeg, cosTRotZDeg);
				timeUntilNextSpawn += 1 / pointSpawnSpeed;
			}
			timeUntilNextSpawn -= dt;
		}
		if(lastFrameWasOn && !on){
			activeChain = false;
			spawnTimer = 0;
		}
		lastFrameWasOn = on;

		numPointsAlive = points.Count;

		// Points loop
		for (int i = 0; i < numPointsAlive; i++)
		{
			tempPoint = points[i];
			if(i == latestSpawnedIndex && activeChain){
				tempPoint.pos = transform.position;
				tempPoint.dir.x = -sinTRotZDeg;
				tempPoint.dir.y = cosTRotZDeg;
			}else {
				tempPoint.pos += tempPoint.velocity * dt;
				tempPoint.spawnTime += dt; // only age when moving
				if(dampening > 0){
					tempPoint.velocity += -tempPoint.velocity * dampening * dt;
				}
				if(worldForce){
					if(worldForceRandom){
						tempPoint.velocity += new Vector3(Random.Range(worldForce1.x, worldForce2.x),Random.Range(worldForce1.y, worldForce2.y)) * dt;
					} else {
						tempPoint.velocity += (Vector3)worldForce1 * dt;
					}
				}
				if(velocityDecidesDirection){
					tempPoint.dir = tempPoint.velocity.normalized;
				} else if(directionPreviousPointPos && i != 0){
					tempPoint.dir = Vector3.Normalize(tempPoint.pos - points[i-1].pos);
				}
			}
			
			if(!directionPreviousPointPos && !velocityDecidesDirection && i != 0 && straightenDirectionFactor > 0){
				tempPoint.dir = Vector3.Lerp(tempPoint.dir,  Vector3.Normalize(tempPoint.pos - points[i-1].pos), straightenDirectionFactor);
			}

			if(i != 0 && straightenPositionFactor > 0){
				tempPoint.pos = Vector3.Lerp(tempPoint.pos, points[i-1].pos, straightenPositionFactor);
			}
		}

		// Verts loop
		if(numPointsAlive > 1){
			numVerts = numPointsAlive+numChains*2;

			numTriPoints = numPointsAlive * 3;
			int[] triangles = new int[numTriPoints];
			
			int vertIndex = 0;
			int triIndexOffset = 0; // for when there are multiple chains of points
			
			float startColorOffset = 0;
			float endColorOffset = 0;
			bool tipFound = false;
			float percentToDone = 0;
			float timeBetweenSpawns = 1/pointSpawnSpeed;
			float parentDone = lifeTime-timeBetweenSpawns; // the last part starts
			int endTimeIndex = 0;
			
			for (int i = 0; i < numPointsAlive; i++)
			{
				tempPoint = points[i];
				
				yUV = tempPoint.spawnTime/lifeTime; // point in life
				newWidth = tempPoint.width * .5f + sizeChange * yUV;
				
				if(i+lifeTimeFadeOffset >= numPointsAlive || points[i+lifeTimeFadeOffset].spawnTime >= lifeTime){
					lifeColor = 0;
				} else {
					lifeColor = (byte)(255-(points[i+lifeTimeFadeOffset].spawnTime / lifeTime)*255);
				}
				
				if(i == 0){
					startColorOffset = tempPoint.spawnTime;
				} else if(points[i-1].isFirst){
					startColorOffset = tempPoint.spawnTime;
				}

				float val; 
				if(startColorFade > 0){
					val = (tempPoint.spawnTime - startColorOffset) / startColorFade;
					val = Mathf.Clamp01(val);
					startColor = (byte)(val*255);
				} else {
					startColor = 255;
				}
				
				if(!tipFound){
					for (int search = i; search < numPointsAlive; search++)
					{
						if(points[search].isFirst){ // found our tip
							tipFound = true;
							endTimeIndex = search;
							endColorOffset = points[search].spawnTime;
							break;
						}
					}
				}
				
				if(endTimeIndex == i){ // we're at the tip, reset search in case there are more chains
					tipFound = false;
				}
				
				if(endColorFade > 0){
					percentToDone = Mathf.Max(0, (endColorOffset-parentDone));
					val = (endColorOffset-tempPoint.spawnTime-percentToDone)/endColorFade;
					val = Mathf.Clamp01(val);
					
					endColor = (byte)(val*255);
				} else {
					endColor = 255;
				}

				tempVcol.r = startColor;
				tempVcol.g = endColor;
				tempVcol.b = 255;
				tempVcol.a = lifeColor;

				// verts

				tempVertTransformOffset = tempPoint.pos - transform.position;// + parent.transform.position;
				newRotation = Quaternion.Inverse(transform.rotation) * Quaternion.Euler(tempPoint.dir);// pointrot;
				tempVertLocalPosition = quaternionOffset * tempPoint.dir * newWidth;

				if(i == 0 || points[i-1].isFirst == true){
					if(tempPoint.flip){
						tempVert = newRotation * (-tempVertLocalPosition + tempVertTransformOffset);
						verts.Add(tempVert);
						tempUv.x = 1;
					} else {
						tempVert = newRotation * (tempVertLocalPosition + tempVertTransformOffset);
						verts.Add(tempVert);
						tempUv.x = 0;
					}
					tempUv.y = yUV;
					uvs.Add(tempUv);
					vcols.Add(tempVcol);
					vertIndex++;
				}

				if(tempPoint.flip){
					tempVert = newRotation * (tempVertLocalPosition + tempVertTransformOffset);
					verts.Add(tempVert);
					tempUv.x = 0;
				} else {
					tempVert = newRotation * (-tempVertLocalPosition + tempVertTransformOffset);
					verts.Add(tempVert);
					tempUv.x = 1;
				}
				tempUv.y = yUV;
				uvs.Add(tempUv);
				vcols.Add(tempVcol);
				vertIndex++;

				if(tempPoint.isFirst == true){
					if(tempPoint.flip){
						tempVert = newRotation * (-tempVertLocalPosition + tempVertTransformOffset);
						verts.Add(tempVert);
						tempUv.x = 1;
					} else {
						tempVert = newRotation * (tempVertLocalPosition + tempVertTransformOffset);
						verts.Add(tempVert);
						tempUv.x = 0;
					}
					tempUv.y = yUV;
					uvs.Add(tempUv);
					vcols.Add(tempVcol);
					vertIndex++;
				}
				
				// triangles
				if(tempPoint.isFirst == true){
					if(tempPoint.flip == true){
						triangles[i*3  ] = i+triIndexOffset;
						triangles[i*3+1] = i+1+triIndexOffset;
						triangles[i*3+2] = i+2+triIndexOffset;
					} else {
						triangles[i*3  ] = i+1+triIndexOffset;
						triangles[i*3+1] = i+triIndexOffset;
						triangles[i*3+2] = i+2+triIndexOffset;
					}
					triIndexOffset += 2;
				} else {
					if(tempPoint.flip == true){
						triangles[i*3  ] = i+triIndexOffset;
						triangles[i*3+1] = i+1+triIndexOffset;
						triangles[i*3+2] = i+2+triIndexOffset;
					} else {
						triangles[i*3  ] = i+triIndexOffset;
						triangles[i*3+1] = i+2+triIndexOffset;
						triangles[i*3+2] = i+1+triIndexOffset;
					}
				}
			}
			
			mesh.Clear();
			mesh.SetVertices(verts);
			mesh.SetColors(vcols);
			mesh.SetUVs(0,uvs);
			mesh.triangles = triangles;
			mesh.RecalculateBounds();
			mesh.RecalculateNormals();
			mf.sharedMesh = mesh;
			verts.Clear();
			vcols.Clear();
			uvs.Clear();
		}
		else {
			mesh.Clear();
		}

		int c = points.Count - 1;
		for (int i = c; i >= 0; i--) // oldest first
		{
			if(points[i].spawnTime > lifeTime){
				RemovePoint(i);
			}
		}
	}
	void Update()
	{
		MeshTrail();
	}

	void SpawnNewPoint(float sinTRotZDeg, float cosTRotZDeg){
		TrailPoint point = new TrailPoint();
		point.pos = transform.position;
		point.dir = new Vector3(-sinTRotZDeg,cosTRotZDeg,0);
		point.speed = speed;
		point.velocity = point.dir * point.speed;
		if(widthRnd > 0){
			currWidth += Random.Range(-widthRnd, widthRnd);
			if(currWidth < width-widthRndMinMax){
				currWidth = width-widthRndMinMax;
			} else if(currWidth > width+widthRndMinMax){
				currWidth = width+widthRndMinMax;
			}
		}
		point.width = currWidth;
		point.flip = flip;
		flip = !flip;
		
		if(on && activeChain == false){ // first one in chain
			activeChain = true;
			point.isFirst = true;
		} else {
			point.isFirst = false;
		}
		
		points.Insert(0, point);
		
		latestSpawnedIndex = 0;
	}

	void RemovePoint(int i){
		points.RemoveAt(i);
		numPointsAlive--;
	
		if(i == 0 || points[i-1].isFirst == true){
			// must be last one in a chain
			numChains--;
		} else {
			points[i-1].isFirst = true;
		}
	}
}