using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public class SnakeMeshGenerator : MonoBehaviour {

    public Texture snakeSkin;
    public float snakeRadius;
    public float snakeVertexDensity = 3.0f;
    public float snakeLength;
    public float snakeTailStartLength;

    private SnakeState snake;
    private MeshFilter meshFilter;
    private MeshRenderer meshRenderer;

    private Texture2D backboneEncTex;

    private int backboneVertsLength;
    //private int backboneLength;
    //private int backboneStartIdx;

	// Use this for initialization
	void Awake () {
        this.snake = this.GetComponent<SnakeState>();
        this.backboneVertsLength = (int)(this.snake.MaxSnakeLength() * this.snakeVertexDensity) + 1;
        //this.backboneLength = 0;
        //this.backboneStartIdx = 0;
        CalcParameters();

        this.meshFilter = this.gameObject.AddComponent<MeshFilter>();
        this.meshFilter.mesh = this.generateBackboneMesh();
        this.meshRenderer = this.gameObject.AddComponent<MeshRenderer>();
        this.meshRenderer.enabled = false;
        
        this.meshRenderer.material.shader = Shader.Find("Unlit/SnakeGenShader");

        this.backboneEncTex = new Texture2D(SnakeState.MAX_BACKBONE_POINTS, 1, TextureFormat.RGBAFloat, false);
        this.backboneEncTex.filterMode = FilterMode.Point;
        this.backboneEncTex.anisoLevel = 1;
        for (int i = 0; i < SnakeState.MAX_BACKBONE_POINTS; i++)
            this.backboneEncTex.SetPixel(i, 0, new Color(0.0f, 0.0f, 0.0f, 0.0f));
        this.backboneEncTex.Apply();
    }

    private Mesh generateBackboneMesh()
    {
        Mesh bbMesh = new Mesh();
        
        Vector3[] vertices = new Vector3[backboneVertsLength * 2];
        int[] triangles = new int[(backboneVertsLength - 1) * 6];

       // Debug.Log(backboneVertsLength);

        // Generate vertices
        // Head section should have higher density
        float distance = 0.0f;
        float distanceDelta = 1.0f / this.snakeVertexDensity;
        for (int i = 0; i < backboneVertsLength; i++)
        {
            vertices[i * 2].x = distance;
            vertices[i * 2].y = -1;
            vertices[i * 2].z = 1.0f;
            vertices[i * 2 + 1].x = distance;
            vertices[i * 2 + 1].y = 1;
            vertices[i * 2 + 1].z = 0.0f;

            distance += (i > 10 ? distanceDelta : distanceDelta / 4.0f);
        }

        // Generate triangle sequence
        for (int i = 0; i < backboneVertsLength - 1; i++)
        {
            triangles[(i * 6) + 0] = (i * 2);
            triangles[(i * 6) + 1] = (i + 1) * 2;
            triangles[(i * 6) + 2] = (i * 2) + 1;
            triangles[(i * 6) + 3] = (i * 2) + 1;
            triangles[(i * 6) + 4] = (i + 1) * 2;
            triangles[(i * 6) + 5] = ((i + 1) * 2) + 1;
        }
        
        bbMesh.vertices = vertices;
        bbMesh.triangles = triangles;
        bbMesh.UploadMeshData(false);

        return bbMesh;
    }

    void OnEnable()
    {
        this.meshRenderer.enabled = true;
    }

    void OnDisable()
    {
        this.meshRenderer.enabled = false;
    }

    private void CalcParameters()
    {
        this.snakeTailStartLength = this.snakeLength - 5.0f;

        this.snakeLength = this.snake.GetSnakeLength();
        this.snakeRadius = this.snake.GetSnakeThickness() / 2.0f;
    }

    void Update ()
    {
        CalcParameters();

        Bounds b = new Bounds();
        for (int i = 0; i < this.snake.GetBackboneLength(); i++)
        {
            if (i >= 1000)
            {
                Debug.LogError("Max shader backbone length exceeded.");
                break;
            }
            //this.meshRenderer.material.SetVector("_Backbone" + i.ToString(), this.snake.GetBackbonePoint(i));
            Vector2 pt = this.snake.GetBackbonePoint(i);
            this.backboneEncTex.SetPixel(i, 0, new Color(pt.x, pt.y, 0.0f, 0.0f));
            b.Encapsulate(this.snake.GetBackbonePoint(i));
        }
        this.backboneEncTex.Apply();

        this.meshRenderer.material.SetTexture("_BackboneTex", this.backboneEncTex);
        this.meshRenderer.material.SetInt("_BackboneLength", this.snake.GetBackboneLength());
        this.meshRenderer.material.SetFloat("_SnakeLength", this.snakeLength);
        this.meshRenderer.material.SetFloat("_SnakeRadius", this.snakeRadius);

        this.snakeSkin = Resources.Load<Texture>("SnakeSkin" + this.snake.snakeSkinID);
        this.meshRenderer.material.mainTexture = snakeSkin;
        this.meshFilter.mesh.bounds = b;
    }
}
