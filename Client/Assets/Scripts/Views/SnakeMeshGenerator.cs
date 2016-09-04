using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public class SnakeMeshGenerator : MonoBehaviour {

    public Texture snakeSkin;
    public float snakeThickness;
    public float snakeVertexDensity;

    private SnakeState snake;
    private MeshFilter meshFilter;
    private MeshRenderer meshRenderer;

	// Use this for initialization
	void Start () {
        this.snake = this.GetComponent<SnakeState>();
        this.meshFilter = this.gameObject.AddComponent<MeshFilter>();
        this.meshFilter.mesh = new Mesh();
        this.meshRenderer = this.gameObject.AddComponent<MeshRenderer>();

        this.meshRenderer.material.mainTexture = snakeSkin;
        this.meshRenderer.material.shader = Shader.Find("Unlit/TexShader");
    }
	
	// Update is called once per frame
	void LateUpdate () {
        int backboneLength = (int)(this.snake.snakeLength * this.snakeVertexDensity);
        Vector3[] backbone = new Vector3[backboneLength];
        
        float distance = 0.0f;
        for (int i = 0; i < backboneLength; i++)
        {
            distance = ((float)i / backboneLength) * this.snake.snakeLength;
            backbone[i] = this.snake.CalcBackboneParametizedPosition(distance);
        }

        Vector3[] vertices = new Vector3[backboneLength * 2];
        Vector2[] uvs = new Vector2[backboneLength * 2];
        int[] triangles = new int[(backboneLength - 1) * 6];

        // Generate vertices and uv coords
        distance = 0.0f;
        for (int i = 0; i < backboneLength; i++)
        {
            uvs[i * 2] = new Vector2(1.0f, distance / (snakeThickness * 2.0f));
            uvs[i * 2 + 1] = new Vector2(0.0f, distance / (snakeThickness * 2.0f));

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
            if (distance < snakeThickness * 2.0f)
            {
                // Head taper
                fct = Mathf.Clamp((distance / (snakeThickness * 2.0f)) + 0.3f, 0.0f, 1.0f);
            }

            vertices[i * 2] = backbone[i] + norm * snakeThickness * fct + Vector3.forward * distance;
            vertices[i * 2 + 1] = backbone[i] - norm * snakeThickness * fct + Vector3.forward * distance;

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
        
        this.meshFilter.mesh.vertices = vertices;
        this.meshFilter.mesh.uv = uvs;
        this.meshFilter.mesh.triangles = triangles;
        this.meshFilter.mesh.UploadMeshData(false);
    }
}
