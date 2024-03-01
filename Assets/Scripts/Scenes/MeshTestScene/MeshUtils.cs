using System.Collections.Generic;
using UnityEngine;

namespace UnityTests
{

    public static class MeshUtils
    {
        public static void Cut(
            Mesh mesh,
            Vector3 point,
            Vector3 planeNormal,
            out Mesh mesh1,
            out Mesh mesh2)
        {
            var originTriangles = mesh.triangles;
            var originVertices = mesh.vertices;
            var originNormals = mesh.normals;
            var originUVs = mesh.uv;

            List<Vector3> upVertices = new();
            List<Vector3> upNormals = new();
            List<Vector2> upUvs = new();

            List<Vector3> downVertices = new();
            List<Vector3> downNormals = new();
            List<Vector2> downUvs = new();

            List<int> upTriangles = new();
            List<int> downTriangles = new();

            var vertexStorage = new (bool Positive, int TargetIndex)[originVertices.Length];
            for (int i = 0; i < originVertices.Length; i++)
            {
                var vertex = originVertices[i];
                var vertexOnPlane = vertex - point;
                var positive = Vector3.Dot(planeNormal, vertexOnPlane) > 0;

                if (positive)
                {
                    var index = upVertices.Count;
                    upVertices.Add(vertex);
                    upNormals.Add(originNormals[i]);
                    upUvs.Add(originUVs[i]);

                    vertexStorage[i] = (positive, index);
                }
                else
                {
                    var index = downVertices.Count;
                    downVertices.Add(vertex);
                    downNormals.Add(originNormals[i]);
                    downUvs.Add(originUVs[i]);

                    vertexStorage[i] = (positive, index);
                }
            }

            int upNewVertexCount = 0;
            int downNewVertexCount = 0;
            List<int> triangleSplitUpBuffer = new(3);
            List<int> triangleSplitDownBuffer = new(3);

            for (int i = 0; i < originTriangles.Length; i += 3)
            {
                triangleSplitUpBuffer.Clear();
                triangleSplitDownBuffer.Clear();

                for (int j = 0; j < 3; j++)
                {
                    int vertexIndex = originTriangles[i + j];
                    if (vertexStorage[vertexIndex].Positive)
                        triangleSplitUpBuffer.Add(vertexIndex);
                    else
                        triangleSplitDownBuffer.Add(vertexIndex);
                }

                if (triangleSplitUpBuffer.Count == 3)
                {
                    foreach (var vertexIndex in triangleSplitUpBuffer)
                        upTriangles.Add(vertexStorage[vertexIndex].TargetIndex);
                }
                else if (triangleSplitDownBuffer.Count == 3)
                {
                    foreach (var vertexIndex in triangleSplitDownBuffer)
                        downTriangles.Add(vertexStorage[vertexIndex].TargetIndex);
                }
                else
                {
                    upNewVertexCount += 2;
                    downNewVertexCount += 2;

                    if (triangleSplitUpBuffer.Count == 2)
                    {
                        var newVertex1 = (originVertices[triangleSplitUpBuffer[0]] + originVertices[triangleSplitDownBuffer[0]]) / 2;
                        var newVertex2 = (originVertices[triangleSplitUpBuffer[1]] + originVertices[triangleSplitDownBuffer[0]]) / 2;
                        var newVertex1Normal = (originNormals[triangleSplitUpBuffer[0]] + originNormals[triangleSplitDownBuffer[0]]) / 2;
                        var newVertex2Normal = (originNormals[triangleSplitUpBuffer[1]] + originNormals[triangleSplitDownBuffer[0]]) / 2;
                        var newVertex1Uv = (originUVs[triangleSplitUpBuffer[0]] + originUVs[triangleSplitDownBuffer[0]]) / 2;
                        var newVertex2Uv = (originUVs[triangleSplitUpBuffer[1]] + originUVs[triangleSplitDownBuffer[0]]) / 2;

                        var upNewVertex1Index = upVertices.Count;
                        var upNewVertex2Index = upVertices.Count + 1;
                        var downNewVertex1Index = downVertices.Count;
                        var downNewVertex2Index = downVertices.Count + 1;

                        upVertices.Add(newVertex1);
                        upNormals.Add(newVertex1Normal);
                        upUvs.Add(newVertex1Uv);

                        upVertices.Add(newVertex2);
                        upNormals.Add(newVertex2Normal);
                        upUvs.Add(newVertex2Uv);

                        downVertices.Add(newVertex1);
                        downNormals.Add(newVertex1Normal);
                        downUvs.Add(newVertex1Uv);

                        downVertices.Add(newVertex2);
                        downNormals.Add(newVertex2Normal);
                        downUvs.Add(newVertex2Uv);

                        var existVertex1Index = vertexStorage[triangleSplitUpBuffer[0]].TargetIndex;
                        var existVertex2Index = vertexStorage[triangleSplitUpBuffer[1]].TargetIndex;
                        var existVertex3Index = vertexStorage[triangleSplitDownBuffer[0]].TargetIndex;

                        upTriangles.Add(existVertex1Index);
                        upTriangles.Add(existVertex2Index);
                        upTriangles.Add(upNewVertex2Index);

                        upTriangles.Add(upNewVertex2Index);
                        upTriangles.Add(upNewVertex1Index);
                        upTriangles.Add(existVertex1Index);

                        downTriangles.Add(downNewVertex1Index);
                        downTriangles.Add(downNewVertex2Index);
                        downTriangles.Add(existVertex3Index);
                    }
                    else
                    {
                        var newVertex1 = (originVertices[triangleSplitUpBuffer[0]] + originVertices[triangleSplitDownBuffer[0]]) / 2;
                        var newVertex2 = (originVertices[triangleSplitUpBuffer[0]] + originVertices[triangleSplitDownBuffer[1]]) / 2;
                        var newVertex1Normal = (originNormals[triangleSplitUpBuffer[0]] + originNormals[triangleSplitDownBuffer[0]]) / 2;
                        var newVertex2Normal = (originNormals[triangleSplitUpBuffer[0]] + originNormals[triangleSplitDownBuffer[1]]) / 2;
                        var newVertex1Uv = (originUVs[triangleSplitUpBuffer[0]] + originUVs[triangleSplitDownBuffer[0]]) / 2;
                        var newVertex2Uv = (originUVs[triangleSplitUpBuffer[0]] + originUVs[triangleSplitDownBuffer[1]]) / 2;

                        var upNewVertex1Index = upVertices.Count;
                        var upNewVertex2Index = upVertices.Count + 1;
                        var downNewVertex1Index = downVertices.Count;
                        var downNewVertex2Index = downVertices.Count + 1;

                        upVertices.Add(newVertex1);
                        upNormals.Add(newVertex1Normal);
                        upUvs.Add(newVertex1Uv);

                        upVertices.Add(newVertex2);
                        upNormals.Add(newVertex2Normal);
                        upUvs.Add(newVertex2Uv);

                        downVertices.Add(newVertex1);
                        downNormals.Add(newVertex1Normal);
                        downUvs.Add(newVertex1Uv);

                        downVertices.Add(newVertex2);
                        downNormals.Add(newVertex2Normal);
                        downUvs.Add(newVertex2Uv);

                        var existVertex1Index = vertexStorage[triangleSplitUpBuffer[0]].TargetIndex;
                        var existVertex2Index = vertexStorage[triangleSplitDownBuffer[0]].TargetIndex;
                        var existVertex3Index = vertexStorage[triangleSplitDownBuffer[1]].TargetIndex;

                        //upTriangles.Add(existVertex1Index);
                        //upTriangles.Add(upNewVertex1Index);
                        //upTriangles.Add(upNewVertex2Index);

                        //downTriangles.Add(downNewVertex1Index);
                        //downTriangles.Add(existVertex2Index);
                        //downTriangles.Add(existVertex3Index);

                        //downTriangles.Add(downNewVertex2Index);
                        //downTriangles.Add(downNewVertex1Index);
                        //downTriangles.Add(existVertex3Index);
                    }
                }
            }

            mesh1 = new()
            {
                vertices = upVertices.ToArray(),
                triangles = upTriangles.ToArray(),
                normals = upNormals.ToArray(),
                uv = upUvs.ToArray(),
            };


            mesh2 = new()
            {
                vertices = downVertices.ToArray(),
                triangles = downTriangles.ToArray(),
                normals = downNormals.ToArray(),
                uv = downUvs.ToArray(),
            };
        }
    }
}
