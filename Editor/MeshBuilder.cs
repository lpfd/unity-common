
using System.Collections.Generic;
using System;
using UnityEngine;

namespace Leap.Forward
{
    class MeshBuilder
    {
        private readonly Dictionary<Vertex, int> _vertexMap = new Dictionary<Vertex, int>();
        private readonly List<Vertex> _vertices = new List<Vertex>();

        public void Build(Mesh mesh)
        {
            BuildValues(_ => _.vertex, _ => mesh.vertices = _);
            BuildValuesIfSet(_ => _.normal, _ => mesh.normals = _);
            BuildValuesIfSet(_ => _.boneWeight, _ => mesh.boneWeights = _);
            BuildValuesIfSet(_ => _.color, _ => mesh.colors = _);
            BuildValuesIfSet(_ => _.tangent, _ => mesh.tangents = _);
            BuildValuesIfSet(_ => _.uv, _ => mesh.uv = _);
            BuildValuesIfSet(_ => _.uv2, _ => mesh.uv2 = _);
            BuildValuesIfSet(_ => _.uv3, _ => mesh.uv3 = _);
            BuildValuesIfSet(_ => _.uv4, _ => mesh.uv4 = _);
            BuildValuesIfSet(_ => _.uv5, _ => mesh.uv5 = _);
            BuildValuesIfSet(_ => _.uv6, _ => mesh.uv6 = _);
            BuildValuesIfSet(_ => _.uv7, _ => mesh.uv7 = _);
            BuildValuesIfSet(_ => _.uv8, _ => mesh.uv8 = _);
        }

        public int Add(Vertex vertex)
        {
            if (_vertexMap.TryGetValue(vertex, out var index))
                return index;
            index = _vertices.Count;
            _vertexMap.Add(vertex, index);
            _vertices.Add(vertex);
            return index;
        }

        public Vertex this[int index] => _vertices[index];
        private void BuildValues<T>(Func<Vertex, T> getter, Action<T[]> setter)
        {
            var res = new T[_vertices.Count];
            for (var index = 0; index < _vertices.Count; index++)
            {
                res[index] = getter(_vertices[index]);
            }
            setter(res);
        }
        private void BuildValuesIfSet<T>(Func<Vertex, T> getter, Action<T[]> setter)
        {
            var eq = EqualityComparer<T>.Default;
            foreach (var vertex in _vertices)
            {
                if (!eq.Equals(getter(vertex), default(T)))
                {
                    BuildValues(getter, setter);
                    return;
                }
            }
        }
    }

}