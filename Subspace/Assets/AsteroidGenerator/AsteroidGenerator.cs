using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;


[RequireComponent(typeof(MeshFilter))]
public class AsteroidGenerator : MonoBehaviour
{

    /* TODO:
        -- Generate Normals on the go
        -- Generate Texture
        -- Mesh Smoothing
        -- Generate LODs;
        -- Optimization
    */

    class Face
    {
        int vert1;
        int vert2;
        int vert3;

        public Face()
        {
            vert1 = 0;
            vert2 = 0;
            vert3 = 0;
        }

        public Face(int vert1, int vert2, int vert3)
        {
            this.vert1 = vert1;
            this.vert2 = vert2;
            this.vert3 = vert3;
        }

        public int GetVert1()
        {
            return vert1;
        }

        public int GetVert2()
        {
            return vert2;
        }

        public int GetVert3()
        {
            return vert3;
        }
    }

    [System.Serializable]
    public struct Peek
    {
        public float min;
        public float max;

        public Peek(float min, float max)
        {
            this.min = min;
            this.max = max;
        }
    }

    List<Vector3> vertices = new List<Vector3>();

    List<Face> faces = new List<Face>();
    List<int> triangles = new List<int>();
    Dictionary<UInt64, int> middlePointIndexCache = new Dictionary<UInt64, int>();

    List<Vector2> uv = new List<Vector2>();

    int index;
    
    int seed = 1;

    MeshFilter filter;
    Mesh mesh;

    [Tooltip("Size of Basic Sphere")]
    public float radiusMulti = 1;
    [Tooltip("How often should the sphere be subdivided")]
    [Range(0, 5)]
    public int subdivisions = 2;

    [Space]

    [Header("Deform of Mesh")]
    public bool deformMesh = true;
    [Tooltip("Gives Min and Max for Deformation")]
    public Peek peekDeform = new Peek(0.8f, 1f);

    public int largeDetail = 50;
    public float largeDetailMulti = 1;

    public int smallDetail = 5;
    public float smallDetailMulti = 0.5f;
    public int deformSize = 100;

    [Space]

    [Header("Noise of Mesh")]
    [Tooltip("Enable small Noise on Mesh")]
    public bool noiseMesh = true;
    [Tooltip("Strenght of small Noise")]
    public float noiseLevel = 0.1f;
    [Tooltip("Not recommended to use")]
    public bool changeTransformation = false;

    [Space]

    [Header("UVs of Mesh")]
    public bool generateUvs = true;
    public float uvScaleFactor = 0.5f;

    [Space]

    [Header("Smoothing")]
    public bool smoothing = false;
    public float smoothingFactor = .3f;

    // Use this for initialization
    private void Start()
    {
        seed = UnityEngine.Random.Range(1, 999999);

        UnityEngine.Random.InitState(seed);
        filter = GetComponent<MeshFilter>();
        mesh = new Mesh();
        mesh.Clear();
        mesh.name = "asteroid";
        GenerateMesh();
    }

    public void GenerateMeshInEditor()
    {
        seed = UnityEngine.Random.Range(1, 999999);

        UnityEngine.Random.InitState(seed);
        filter = GetComponent<MeshFilter>();
        mesh = new Mesh();
        mesh.Clear();
        mesh.name = "asteroid";
        GenerateMesh();
    }

    void Update()
    {
        if (subdivisions < 0)
            subdivisions = 0;
        
        if (subdivisions > 6)
            subdivisions = 6;

        if (Input.GetKeyDown(KeyCode.P))
        {
            subdivisions++;
        }

        if (Input.GetKeyDown(KeyCode.O))
        {
            subdivisions--;
        }

        if (Input.GetKeyDown(KeyCode.R))
        {
            GenerateMesh();
        }
       
    }

    #region Mesh Generation

    void GenerateMesh()
    {
        vertices.Clear();
        triangles.Clear();
        faces.Clear();
        middlePointIndexCache.Clear();
        mesh.Clear();
        uv.Clear();
        index = 0;
  

        GenerateIcoSphere();

        foreach (Face el in faces)
        {
            triangles.Add(el.GetVert1());
            triangles.Add(el.GetVert2());
            triangles.Add(el.GetVert3());
        }

        mesh.vertices = vertices.ToArray();
        mesh.triangles = triangles.ToArray();

        

        mesh.RecalculateNormals();

        if (deformMesh)
        {
            for (int i = 0; i < largeDetail; i++)
            {
                DeformMesh(1, largeDetailMulti);
            }

            for (int i = 0; i < smallDetail; i++)
            {
                DeformMesh(-1, smallDetailMulti);
            }

            mesh.vertices = vertices.ToArray();
            mesh.RecalculateNormals();
        }

        

        if (noiseMesh)
        {
            NoiseMesh();
            mesh.vertices = vertices.ToArray();
            mesh.RecalculateNormals();
        }

        mesh.RecalculateBounds();
        mesh.RecalculateTangents();

        if(generateUvs)
            GenerateUVs();

        if (smoothing)
        {
            SmoothMesh();
            mesh.RecalculateNormals();
        }

        filter.sharedMesh = mesh;
    }

    void GenerateIcoSphere()
    {
        // create 12 vertices of a icosahedron
        float t = (float)((1f + Mathf.Sqrt(5.0f)) / 2.0f);

        AddVertex(new Vector3(-1, t, 0));
        AddVertex(new Vector3(1, t, 0));
        AddVertex(new Vector3(-1, -t, 0));
        AddVertex(new Vector3(1, -t, 0));
   
        AddVertex(new Vector3(0, -1, t));
        AddVertex(new Vector3(0, 1, t));
        AddVertex(new Vector3(0, -1, -t));
        AddVertex(new Vector3(0, 1, -t));
    
        AddVertex(new Vector3(t, 0, -1));
        AddVertex(new Vector3(t, 0, 1));
        AddVertex(new Vector3(-t, 0, -1));
        AddVertex(new Vector3(-t, 0, 1));

        // create 20 triangles of the icosahedron
        faces.Add(new Face(0, 11, 5));
        faces.Add(new Face(0, 5, 1));
        faces.Add(new Face(0, 1, 7));
        faces.Add(new Face(0, 7, 10));
        faces.Add(new Face(0, 10, 11));

        // 5 adjacent faces
        faces.Add(new Face(1, 5, 9));
        faces.Add(new Face(5, 11, 4));
        faces.Add(new Face(11, 10, 2));
        faces.Add(new Face(10, 7, 6));
        faces.Add(new Face(7, 1, 8));

        // 5 faces around point 3
        faces.Add(new Face(3, 9, 4));
        faces.Add(new Face(3, 4, 2));
        faces.Add(new Face(3, 2, 6));
        faces.Add(new Face(3, 6, 8));
        faces.Add(new Face(3, 8, 9));

        // 5 adjacent faces
        faces.Add(new Face(4, 9, 5));
        faces.Add(new Face(2, 4, 11));
        faces.Add(new Face(6, 2, 10));
        faces.Add(new Face(8, 6, 7));
        faces.Add(new Face(9, 8, 1));


        
        // refine triangles
        for (int i = 0; i < subdivisions; i++)
        {
            List<Face> faces2 = new List<Face>();
            foreach (var tri in faces)
            {
                // replace triangle by 4 triangles
                int a = GetMiddlePoint(tri.GetVert1(), tri.GetVert2());
                int b = GetMiddlePoint(tri.GetVert2(), tri.GetVert3());
                int c = GetMiddlePoint(tri.GetVert3(), tri.GetVert1());

                faces2.Add(new Face(tri.GetVert1(), a, c));
                faces2.Add(new Face(tri.GetVert2(), b, a));
                faces2.Add(new Face(tri.GetVert3(), c, b));
                faces2.Add(new Face(a, b, c));
            }
            faces = faces2;
        }
    }

    // return index of point in the middle of p1 and p2
    private int GetMiddlePoint(int p1, int p2)
    {
        // first check if we have it already
        bool firstIsSmaller = p1 < p2;
        UInt64 smallerIndex = (UInt64)(firstIsSmaller ? p1 : p2);
        UInt64 greaterIndex = (UInt64)(firstIsSmaller ? p2 : p1);
        UInt64 key = (smallerIndex << 32) + greaterIndex;

        int ret;
        if (this.middlePointIndexCache.TryGetValue(key, out ret))
        {
            return ret;
        }

        // not in cache, calculate it
        Vector3 point1 = vertices[p1];
        Vector3 point2 = vertices[p2];
        Vector3 middle = new Vector3(
            (point1.x + point2.x) / 2.0f,
            (point1.y + point2.y) / 2.0f,
            (point1.z + point2.z) / 2.0f);

        // add vertex makes sure point is on unit sphere
        int i = AddVertex(middle);

        // store it, return index
        this.middlePointIndexCache.Add(key, i);
        return i;
    }

    private int AddVertex(Vector3 p)
    {
        float length = Mathf.Sqrt(p.x * p.x + p.y * p.y + p.z * p.z);
        vertices.Add(new Vector3(p.x / length, p.y / length, p.z / length) * radiusMulti);

        return index++;
    }

    #endregion

    #region Mesh Deformation

    void DeformMesh(float dir, float scaleMulti)
    {
       
        int startVert = UnityEngine.Random.Range(0, mesh.vertices.Length - 3);

        List<int> deformFaces = new List<int>();

        for (int i = 0; i < deformSize; i++)
        {
            foreach (Face face in faces)
            {
                if (face.GetVert1() == startVert || face.GetVert2() == startVert || face.GetVert3() == startVert)
                {
                    if (!deformFaces.Contains(face.GetVert1()))
                    {
                        deformFaces.Add(face.GetVert1());
                    }
                    if (!deformFaces.Contains(face.GetVert2()))
                    {
                        deformFaces.Add(face.GetVert2());
                    }
                    if (!deformFaces.Contains(face.GetVert3()))
                    {
                        deformFaces.Add(face.GetVert3());
                    }
                }
            }

            startVert = deformFaces[deformFaces.Count-1];
        }
        

        for (int i = 0; i < deformFaces.Count; i++)
        {
            vertices[deformFaces[i]] += mesh.normals[deformFaces[i]] * UnityEngine.Random.Range(peekDeform.min, peekDeform.max) * dir * scaleMulti;
        }
      
        
    }

    void NoiseMesh()
    {
        for (int i = 0; i < vertices.Count; i++)
        {
            vertices[i] += mesh.normals[i] * UnityEngine.Random.Range(-noiseLevel, +noiseLevel);
        }

        if (changeTransformation)
        {
            transform.localScale = new Vector3(UnityEngine.Random.Range(0.8f, 1), UnityEngine.Random.Range(0.8f, 1), UnityEngine.Random.Range(0.8f, 1));
        }
        else
        {
            transform.localScale = new Vector3(1, 1, 1);
        }
            
    }

    #endregion

    #region Mesh Smoothing

    void SmoothMesh()
    {
        Vector3[] verts = mesh.vertices;
        int[] tris = mesh.triangles;
        Vector3[] normalsVerts = mesh.normals;
        CalcFaceNormals(verts, tris);
        print("Nuber of tris + " +  tris.Length);
        for (int i = 0; i < tris.Length; i += 3)
        {
            float angle = Vector3.Angle(normalsVerts[tris[i]], normalsFace[i]);

            if (angle > 50)
            {
                print("Move Vert " + i);
                verts[tris[i]] -= normalsVerts[tris[i]] * smoothingFactor;
            }
            angle = Vector3.Angle(normalsVerts[tris[i + 1]], normalsFace[i]);

            if (angle > 50)
            {
                print("Move Vert " + i+ 1);
                verts[tris[i + 1]] -= normalsVerts[tris[i + 1]] * smoothingFactor;
            }

            angle = Vector3.Angle(normalsVerts[tris[i + 2]], normalsFace[i]);

            if (angle > 50)
            {
                print("Move Vert " + i+2);
                verts[tris[i + 2]] -= normalsVerts[tris[i + 2]] * smoothingFactor;
            }
        }

        mesh.vertices = verts;
    }

    private Dictionary<int, Vector3> normalsFace = new Dictionary<int, Vector3>();

    void CalcFaceNormals(Vector3[] verts, int[] tris)
    {
        normalsFace.Clear();
        for (int i = 0; i < tris.Length; i += 3)
        {
            Vector3 v0 = transform.TransformPoint(verts[tris[i]]);
            Vector3 v1 = transform.TransformPoint(verts[tris[i + 1]]);
            Vector3 v2 = transform.TransformPoint(verts[tris[i + 2]]);
            Vector3 center = (v0 + v1 + v2) / 3;

            Vector3 dir = Vector3.Cross(v1 - v0, v2 - v0);
            dir /= dir.magnitude;

            normalsFace.Add(i, dir);
        }
    }

    #endregion

    #region Mesh UVs

    void GenerateUVs()
    {

        int[] tris = mesh.triangles;

        Vector3[] verts = mesh.vertices;
        Vector2[] uvs = new Vector2[verts.Length];

        // Iterate over each face (here assuming triangles)
        for (int i = 0; i < tris.Length; i += 3)
        {
            // Get the three vertices bounding this triangle.
            Vector3 v1 = verts[tris[i]];
            Vector3 v2 = verts[tris[i + 1]];
            Vector3 v3 = verts[tris[i + 2]];

            // Compute a vector perpendicular to the face.
            Vector3 normal = Vector3.Cross(v3 - v1, v2 - v1);

            //Debug.Log("surface " + i / 3 + " : " + (normal * 0.5f).magnitude);

            // Form a rotation that points the z+ axis in this perpendicular direction.
            // Multiplying by the inverse will flatten the triangle into an xy plane.
            Quaternion rotation = Quaternion.Inverse(Quaternion.LookRotation(normal));

            // Assign the uvs, applying a scale factor to control the texture tiling.
            uvs[tris[i]] = (Vector2)(rotation * v1) * uvScaleFactor;
            uvs[tris[i + 1]] = (Vector2)(rotation * v2) * uvScaleFactor;
            uvs[tris[i + 2]] = (Vector2)(rotation * v3) * uvScaleFactor;
        }

        mesh.uv = uvs;
    }

    #endregion
}
