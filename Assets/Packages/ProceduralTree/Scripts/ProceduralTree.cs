﻿using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;

using UnityEngine;
using mattatz.ProceduralFlower;
using UnityEngine.Rendering;

namespace ProceduralModeling {

	public class ProceduralTree : ProceduralModelingBase {

		public TreeData Data { get { return data; } } 

		[SerializeField] TreeData data;
		[SerializeField, Range(2, 8)] protected int generations = 5;
		[SerializeField, Range(0.5f, 5f)] protected float length = 1f;
		[SerializeField, Range(0.1f, 2f)] protected float radius = 0.15f;

        [HideInInspector] public float height = 2f;
		[HideInInspector] public int leafCount = 6;
        public Vector2 leafScaleRange = new Vector2(0.2f, 0.825f);
        public Vector2 leafSegmentRange = new Vector2(0.2f, 0.92f);

		
		[SerializeField] StemData stemData;
		[SerializeField] ShapeData leafData;
        [SerializeField] int seed = 0;
        PFRandom rand;

		List<Point> leafPoints;
		List<Vector3> leafDirections;
		List<List<Point>> leafSegmentList;

		const float PI2 = Mathf.PI * 2f;

		void OnDrawGizmos() {
		//	ProceduralTree.DebugPoints.ForEach(x => Gizmos.DrawSphere(x.transform.position, 0.1f));
		}


		public static Mesh Build(TreeData data, int generations, float length, float radius, 
			 List<Point> leafPoints, List<Vector3> leafDirections, List<List<Point>> leafSegmentList) {

			data.Setup();

			var root = new TreeBranch(
				generations, 
				length, 
				radius, 
				data,
				leafPoints,
				leafDirections,
				leafSegmentList
			);

			var vertices = new List<Vector3>();
			var normals = new List<Vector3>();
			var tangents = new List<Vector4>();
			var uvs = new List<Vector2>();
			var triangles = new List<int>();

			float maxLength = TraverseMaxLength(root);

			Traverse(root, (branch) => {
				var offset = vertices.Count;

				var vOffset = branch.Offset / maxLength;
				var vLength = branch.Length / maxLength;

				for(int i = 0, n = branch.Segments.Count; i < n; i++) {
					var t = 1f * i / (n - 1);
					var v = vOffset + vLength * t;

					var segment = branch.Segments[i];
					var N = segment.Frame.Normal;
					var B = segment.Frame.Binormal;
					for(int j = 0; j <= data.radialSegments; j++) {
						// 0.0 ~ 2π
						var u = 1f * j / data.radialSegments;
						float rad = u * PI2;

						float cos = Mathf.Cos(rad), sin = Mathf.Sin(rad);
						var normal = (cos * N + sin * B).normalized;
						vertices.Add(segment.Position + segment.Radius * normal);
						normals.Add(normal);

						var tangent = segment.Frame.Tangent;
						tangents.Add(new Vector4(tangent.x, tangent.y, tangent.z, 0f));

						uvs.Add(new Vector2(u, v));
					}
				}

				for (int j = 1; j <= data.heightSegments; j++) {
					for (int i = 1; i <= data.radialSegments; i++) {
						int a = (data.radialSegments + 1) * (j - 1) + (i - 1);
						int b = (data.radialSegments + 1) * j + (i - 1);
						int c = (data.radialSegments + 1) * j + i;
						int d = (data.radialSegments + 1) * (j - 1) + i;

						a += offset;
						b += offset;
						c += offset;
						d += offset;

						triangles.Add(a); triangles.Add(d); triangles.Add(b);
						triangles.Add(b); triangles.Add(d); triangles.Add(c);
					}
				}
			});

			var mesh = new Mesh();
			mesh.vertices = vertices.ToArray();
			mesh.normals = normals.ToArray();
			mesh.tangents = tangents.ToArray();
			mesh.uv = uvs.ToArray();
			mesh.triangles = triangles.ToArray();
			return mesh;
		}

		protected override void BuildLeaves() {
			leafSegmentList.ForEach(segments => {
				
				var offset = leafSegmentRange.x * segments.Count;
				var len = (leafSegmentRange.y - leafSegmentRange.x) * segments.Count;
				var size = 1f;
				
				for (int o = 0; o < leafCount; o++) {
					var r = (float)(o + 1) / (leafCount + 1);
					int index = Mathf.Min(Mathf.FloorToInt(len * r + offset), segments.Count - 2);
					var from = segments[index];
					var to = segments[index + 1];
					var dir = (to.position - from.position).normalized;
					var leaf = CreateLeaf(segments[index], dir, (o % 4) * 90f + rand.SampleRange(-20f, 20f), true);
					leaf.transform.SetParent(this.transform);

					// lower leaf becomes bigger than upper one.
					size = rand.SampleRange(size, 1f - (r * 0.5f));
					leaf.transform.localScale *= Mathf.Lerp(leafScaleRange.x, leafScaleRange.y, size);
					leaf.transform.localPosition = segments[index].position;
				}
			});
		}

		protected override Mesh Build ()
		{
			foreach (Transform child in transform) {
				GameObject.Destroy(child.gameObject);
			}

            rand = new PFRandom(seed);
			leafData.Init();
			stemData.Init();
			leafPoints = new List<Point>();
			leafDirections = new List<Vector3>();
			leafSegmentList = new List<List<Point>>();
			return Build(data, generations, length, radius, leafPoints, leafDirections, leafSegmentList);
		}

		static float TraverseMaxLength(TreeBranch branch) {
			float max = 0f;
			branch.Children.ForEach(c => {
				max = Mathf.Max(max, TraverseMaxLength(c));
			});
			return branch.Length + max;
		}

		static void Traverse(TreeBranch from, Action<TreeBranch> action) {
			if(from.Children.Count > 0) {
				from.Children.ForEach(child => {
					Traverse(child, action);
				});
			}
			action(from);
		}

		List<Vector3> GetControls (int count, float height, float radius) {
			var controls = new List<Vector3>();
			count = Mathf.Max(4, count);
			for(int i = 0; i < count; i++) {
				var r = (float)i / (count - 1);
                var circle = rand.SampleUnitCircle() * radius;
				controls.Add(new Vector3(circle.x, r * height, circle.y));
			}
			return controls;
		}

		GameObject CreateBase (string name, PFPartType type, Mesh mesh, Material material, ShadowCastingMode shadowCastingMode, bool receiveShadows, bool visible) {
			var go = new GameObject(name);
			go.AddComponent<MeshFilter>().mesh = mesh;

			var rnd = go.AddComponent<MeshRenderer>();
			rnd.sharedMaterial = material;
			rnd.shadowCastingMode = shadowCastingMode;
			rnd.receiveShadows = receiveShadows;

			var part = go.AddComponent<PFPart>();
            part.SetType(type);
			part.Fade(visible ? 1f + part.EPSILON : 0f);

			return go;
		}
		
		GameObject CreateShape(string name, PFPartType type, ShapeData data, bool visible) {
			return CreateBase(name, type, data.mesh, data.material, data.shadowCastingMode, data.receiveShadows, visible);
		}

		GameObject CreateStem(string name, PFStem stem, ShadowCastingMode shadowCastingMode, bool receiveShadows, Func<float, float> f, float height, float bend, bool visible) {
			var controls = GetControls(4, height, bend);
			var mesh = stem.Build(controls, f);
			return CreateBase(name, PFPartType.Stover, mesh, stemData.material, stemData.shadowCastingMode, stemData.receiveShadows, visible);
		}
		
		GameObject CreateLeaf (Point segment, Vector3 dir, float angle, bool visible) {
			var stem = new PFStem(10, 2, 0.01f);
			var root = CreateStem("Stem", stem, leafData.shadowCastingMode, leafData.receiveShadows, (r) => Mathf.Max(1f - r, 0.2f), 0.05f, 0.0f, visible);
			root.transform.localPosition = segment.position;
			root.transform.localRotation *= Quaternion.FromToRotation(Vector3.forward, dir) * Quaternion.AngleAxis(angle, Vector3.forward);

			var leaf = CreateShape("Leaf", PFPartType.Stover, leafData, visible);
			leaf.transform.SetParent(root.transform, false);
			leaf.transform.localPosition = stem.Tip.position;
			leaf.transform.localRotation *= Quaternion.AngleAxis(rand.SampleRange(0f, 30f), Vector3.up);

			var part = root.GetComponent<PFPart>();
			part.SetSpeed(5f);
			part.Animate();
			part.Add(leaf.GetComponent<PFPart>(), 1f);

			return root;
		}


/*
		GameObject CreateLeaf (Point segment, Vector3 dir, float angle, bool visible, ShapeData leafData) {

//			var stem = new PFStem(10, 2, 0.01f);

//			var root = CreateStem("Stem", stem, leafData.shadowCastingMode, leafData.receiveShadows, (r) => Mathf.Max(1f - r, 0.2f), 0.05f, 0.0f, visible);
//			root.transform.localPosition = segment.position;
//			root.transform.localRotation *= Quaternion.FromToRotation(Vector3.forward, dir) * Quaternion.AngleAxis(angle, Vector3.forward);

			var leaf = CreateShape("Leaf", PFPartType.Stover, leafData, visible);
			leaf.transform.SetParent(this.transform);
			leaf.transform.localPosition = segment.position;
		//	leaf.transform.localRotation *= Quaternion.AngleAxis(rand.SampleRange(0f, 30f), Vector3.up);
			leaf.transform.localRotation *= Quaternion.FromToRotation(Vector3.forward, dir) * Quaternion.AngleAxis(angle, Vector3.forward);

			leaf.transform.localScale = new Vector3(0.5f, 0.5f, 0.5f);

			var part = leaf.GetComponent<PFPart>();
			part.SetSpeed(5f);
			part.Animate();
		//	part.Add(leaf.GetComponent<PFPart>(), 1f);

			return leaf;
		}

		
*/
	}

	[System.Serializable]
	public class TreeData {
		public int randomSeed = 0;
		[Range(0.25f, 0.95f)] public float lengthAttenuation = 0.8f, radiusAttenuation = 0.5f;
		[Range(1, 3)] public int branchesMin = 1, branchesMax = 3;
        [Range(-45f, 0f)] public float growthAngleMin = -15f;
        [Range(0f, 45f)] public float growthAngleMax = 15f;
        [Range(1f, 10f)] public float growthAngleScale = 4f;
        [Range(0f, 45f)] public float branchingAngle = 15f;
		[Range(4, 20)] public int heightSegments = 10, radialSegments = 8;
		[Range(0.0f, 0.35f)] public float bendDegree = 0.1f;

		Rand rnd;

		public void Setup() {
			rnd = new Rand(randomSeed);
		}

		public int Range(int a, int b) {
			return rnd.Range(a, b);
		}

		public float Range(float a, float b) {
			return rnd.Range(a, b);
		}

		public int GetRandomBranches() {
			return rnd.Range(branchesMin, branchesMax + 1);
		}

		public float GetRandomGrowthAngle() {
			return rnd.Range(growthAngleMin, growthAngleMax);
		}

		public float GetRandomBendDegree() {
			return rnd.Range(-bendDegree, bendDegree);
		}
	}

	public class TreeBranch {
		public int Generation { get { return generation; } }
		public List<TreeSegment> Segments { get { return segments; } }
		public List<TreeBranch> Children { get { return children; } }

		public Vector3 From { get { return from; } }
		public Vector3 To { get { return to; } }
		public float Length { get { return length; } } 
		public float Offset { get { return offset; } }

		int generation;

		List<TreeSegment> segments;
		List<TreeBranch> children;

		Vector3 from, to;
		float fromRadius, toRadius;
		float length;
		float offset;

		// for Root branch constructor
		public TreeBranch(int generation, float length, float radius, TreeData data,  List<Point> leafPoints, 
				List<Vector3> leafDirections, List<List<Point>> leafSegmentList) : 
		this(new List<TreeBranch>(), generation, generation, Vector3.zero, Vector3.up, Vector3.right, 
				Vector3.back, length, radius, 0f, data, leafPoints, leafDirections, leafSegmentList) {
		}

		protected TreeBranch(List<TreeBranch> branches, int generation, int generations, Vector3 from, 
			Vector3 tangent, Vector3 normal, Vector3 binormal, float length, float radius, float offset, 
			TreeData data, List<Point> leafPoints, List<Vector3> leafDirections,  List<List<Point>> leafSegmentList) {
			
			this.generation = generation;

			this.fromRadius = radius;
			this.toRadius = (generation == 0) ? 0f : radius * data.radiusAttenuation;

			this.from = from;

            var scale = Mathf.Lerp(1f, data.growthAngleScale, 1f - 1f * generation / generations);
            var rotation = Quaternion.AngleAxis(scale * data.GetRandomGrowthAngle(), normal) * Quaternion.AngleAxis(scale * data.GetRandomGrowthAngle(), binormal);
            this.to = from + rotation * tangent * length;

			this.length = length;
			this.offset = offset;

			segments = BuildSegments(data, fromRadius, toRadius, normal, binormal);

            branches.Add(this);

			children = new List<TreeBranch>();
			if(generation > 0) {
				int count = data.GetRandomBranches();
				for(int i = 0; i < count; i++) {
                    float ratio;
                    if(count == 1)
                    {
                        // for zero division
                        ratio = 1f;
                    } else
                    {
                        ratio = Mathf.Lerp(0.5f, 1f, (1f * i) / (count - 1));
                    }

                    var index = Mathf.FloorToInt(ratio * (segments.Count - 1));
					var segment = segments[index];

                    Vector3 nt, nn, nb;
                    if(ratio >= 1f)
                    {
                        // sequence branch
                        nt = segment.Frame.Tangent;
                        nn = segment.Frame.Normal;
                        nb = segment.Frame.Binormal;
                    } else
                    {
                        var phi = Quaternion.AngleAxis(i * 90f, tangent);
                        // var psi = Quaternion.AngleAxis(data.branchingAngle, normal) * Quaternion.AngleAxis(data.branchingAngle, binormal);
                        var psi = Quaternion.AngleAxis(data.branchingAngle, normal);
                        var rot = phi * psi;
                        nt = rot * tangent;
                        nn = rot * normal;
                        nb = rot * binormal;
                    }

					var child = new TreeBranch(
                        branches,
						this.generation - 1, 
                        generations,
						segment.Position, 
						nt, 
						nn, 
						nb, 
						length * Mathf.Lerp(1f, data.lengthAttenuation, ratio), 
						radius * Mathf.Lerp(1f, data.radiusAttenuation, ratio),
						offset + length,
						data,

						leafPoints,
						leafDirections, 
						leafSegmentList
					);

					children.Add(child);
				}
			} else {
				// add location of the tip
				leafPoints.Add(new Point(this.To, Quaternion.identity));
			//	leafDirections.Add((this.To - this.From).normalized);

				List<Point> leafSegment = new List<Point>();
				segments.ForEach(segment => {
					leafSegment.Add(new Point(segment.Position, Quaternion.identity));
				});
				leafSegmentList.Add(leafSegment);
			}
		}

		List<TreeSegment> BuildSegments (TreeData data, float fromRadius, float toRadius, Vector3 normal, Vector3 binormal) {
			var segments = new List<TreeSegment>();

			var points = new List<Vector3>();

			var length = (to - from).magnitude;
			var bend = length * (normal * data.GetRandomBendDegree() + binormal * data.GetRandomBendDegree());
			points.Add(from);
			points.Add(Vector3.Lerp(from, to, 0.25f) + bend);
			points.Add(Vector3.Lerp(from, to, 0.75f) + bend);
			points.Add(to);

			var curve = new CatmullRomCurve(points);

			var frames = curve.ComputeFrenetFrames(data.heightSegments, normal, binormal, false);
			for(int i = 0, n = frames.Count; i < n; i++) {
				var u = 1f * i / (n - 1);
                var radius = Mathf.Lerp(fromRadius, toRadius, u);

				var position = curve.GetPointAt(u);
				var segment = new TreeSegment(frames[i], position, radius);
				segments.Add(segment);
			}
			return segments;
		}

	}

	public class TreeSegment {
		public FrenetFrame Frame { get { return frame; } }
		public Vector3 Position { get { return position; } }
        public float Radius { get { return radius; } }

		FrenetFrame frame;
		Vector3 position;
        float radius;

		public TreeSegment(FrenetFrame frame, Vector3 position, float radius) {
			this.frame = frame;
			this.position = position;
            this.radius = radius;
		}
	}

	public class Rand {
		System.Random rnd;

		public float value {
			get {
				return (float)rnd.NextDouble();
			}
		}

		public Rand(int seed) {
			rnd = new System.Random(seed);
		}

		public int Range(int a, int b) {
			var v = value;
			return Mathf.FloorToInt(Mathf.Lerp(a, b, v));
		}

		public float Range(float a, float b) {
			var v = value;
			return Mathf.Lerp(a, b, v);
		}
	}

}
