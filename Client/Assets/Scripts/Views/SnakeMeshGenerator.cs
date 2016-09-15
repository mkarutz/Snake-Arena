using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public class SnakeMeshGenerator : MonoBehaviour {

    public Texture snakeSkin;
    public float snakeRadius;
    public float snakeVertexDensity;
    public float snakeLength;
    public float snakeTailStartLength;

    private SnakeState snake;
    private MeshFilter meshFilter;
    private MeshRenderer meshRenderer;

	// Use this for initialization
	void Awake () {
        this.snake = this.GetComponent<SnakeState>();
        this.meshFilter = this.gameObject.AddComponent<MeshFilter>();
        this.meshFilter.mesh = new Mesh();
        this.meshRenderer = this.gameObject.AddComponent<MeshRenderer>();
        this.meshRenderer.enabled = false;
        
        this.meshRenderer.material.shader = Shader.Find("Unlit/TexShader");
    }

    void OnEnable()
    {
        this.meshRenderer.enabled = true;
    }

    void OnDisable()
    {
        this.meshRenderer.enabled = false;
    }

    void Update ()
    {
        this.snakeTailStartLength = this.snakeLength - 5.0f;

        this.snakeLength = this.snake.GetSnakeLength();
        this.snakeRadius = this.snake.GetSnakeThickness() / 2.0f;
        this.snakeVertexDensity = 3.0f / this.snakeRadius;

        this.snakeSkin = Resources.Load<Texture>("SnakeSkin" + this.snake.snakeSkinID);
        this.meshRenderer.material.mainTexture = snakeSkin;
    }
	
	// Update is called once per frame
	void LateUpdate () {
        int backboneLength = (int)(this.snakeLength * this.snakeVertexDensity);
        if (backboneLength <= 0)
            return;
        Vector3[] backbone = new Vector3[backboneLength];
        
        float distance = 0.0f;
        for (int i = 0; i < backboneLength; i++)
        {
            distance = ((float)i / backboneLength) * this.snakeLength;
            backbone[i] = this.snake.CalcBackboneParametizedPosition(distance);
        }

        Vector3[] vertices = new Vector3[backboneLength * 2];
        Vector2[] uvs = new Vector2[backboneLength * 2];
        int[] triangles = new int[(backboneLength - 1) * 6];

        // Generate vertices and uv coords
        distance = 0.0f;
        for (int i = 0; i < backboneLength; i++)
        {
            uvs[i * 2] = new Vector2(1.0f, distance / (snakeRadius * 2.0f));
            uvs[i * 2 + 1] = new Vector2(0.0f, distance / (snakeRadius * 2.0f));

            float distanceDelta = 0.0f;

            Vector3 norm = Vector3.zero;
            if (i == 0 || i == backboneLength - 1)
            {
                int d = 0;
                if (i != 0) d = -1;
                Vector3 line = backbone[i + d + 1] - backbone[i + d];
                norm = Vector3.Cross(line.normalized, Vector3.back);
                distanceDelta += line.magnitude;
            }
            else
            {
                for (int l = 0; l < 2; l++)
                {
                    Vector3 line = backbone[i + l] - backbone[i + l - 1];
                    norm += Vector3.Cross(line.normalized, Vector3.back);
                    if (l == 1) distanceDelta += line.magnitude;
                }
            }
            norm.Normalize();

            float fct = Mathf.Clamp((backboneLength - i - 1) / (backboneLength * 0.1f), 0.0f, 1.0f);
            if (distance < snakeRadius * 2.0f)
            {
                // Head taper
                fct = Mathf.Clamp((distance / (snakeRadius * 2.0f)) + 0.3f, 0.0f, 1.0f);
            }

            vertices[i * 2] = backbone[i] + norm * snakeRadius * fct + Vector3.forward * distance;
            vertices[i * 2 + 1] = backbone[i] - norm * snakeRadius * fct + Vector3.forward * distance;

            distance += distanceDelta;
        }

        // Generate triangle sequence
        for (int i = 0; i < backboneLength - 1; i++)
        {
            triangles[(i * 6) + 0] = (i * 2);
            triangles[(i * 6) + 1] = (i + 1) * 2;
            triangles[(i * 6) + 2] = (i * 2) + 1;
            triangles[(i * 6) + 3] = (i * 2) + 1;
            triangles[(i * 6) + 4] = (i + 1) * 2;
            triangles[(i * 6) + 5] = ((i + 1) * 2) + 1;
        }

        //Debug.Log(vertices.Length);
        
        this.meshFilter.mesh.vertices = vertices;
        this.meshFilter.mesh.uv = uvs;
        this.meshFilter.mesh.triangles = triangles;
        this.meshFilter.mesh.UploadMeshData(false);
        this.meshFilter.mesh.RecalculateBounds();
    }
}
