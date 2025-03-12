using System.Collections.Generic;
using System;
using UnityEngine;
using System.Collections;

namespace Leap.Forward
{
    internal static partial class ExtensionMethods
    {
        public static IEnumerable<Transform> GetChildrenRecursive(this Transform parent)
        {
            Queue<Transform> queue = new Queue<Transform>();
            queue.Enqueue(parent);
            while (queue.Count > 0)
            {
                var c = queue.Dequeue();
                foreach (Transform t in c)
                {
                    yield return t;
                    queue.Enqueue(t);
                }
            }
        }

        public static Transform FindRecursive(this Transform parent, string name)
        {
            if (parent.name == name)
                return parent;
            foreach (var c in GetChildrenRecursive(parent))
            {
                if (c.name == name)
                    return c;
            }
            return null;
        }

        public static IEnumerable<IReadOnlyList<T>> ToTriplets<T>(this IEnumerable<T> source)
        {
            using var en = source.GetEnumerator();
            for (; ; )
            {
                if (!en.MoveNext()) yield break;
                var a = en.Current;
                if (!en.MoveNext()) yield break;
                var b = en.Current;
                if (!en.MoveNext()) yield break;
                var c = en.Current;
                yield return new Triplet<T>(a, b, c);
            }
        }

        public static Vertex[] GetVertices(this Mesh mesh)
        {
            if (mesh == null)
            {
                return Array.Empty<Vertex>();
            }

            var meshVertices = new VertexComponentSource<Vector3>(mesh.vertices);
            var normals = new VertexComponentSource<Vector3>(mesh.normals);
            var boneWeights = new VertexComponentSource<BoneWeight>(mesh.boneWeights);
            var colors = new VertexComponentSource<Color>(mesh.colors);
            var color32s = new VertexComponentSource<Color32>(mesh.colors32);
            var tangents = new VertexComponentSource<Vector4>(mesh.tangents);
            var uv = new VertexComponentSource<Vector2>(mesh.uv);
            var uv2 = new VertexComponentSource<Vector2>(mesh.uv2);
            var uv3 = new VertexComponentSource<Vector2>(mesh.uv3);
            var uv4 = new VertexComponentSource<Vector2>(mesh.uv4);
            var uv5 = new VertexComponentSource<Vector2>(mesh.uv5);
            var uv6 = new VertexComponentSource<Vector2>(mesh.uv6);
            var uv7 = new VertexComponentSource<Vector2>(mesh.uv7);
            var uv8 = new VertexComponentSource<Vector2>(mesh.uv8);

            var res = new Vertex[meshVertices.Length];
            for (var index = 0; index < meshVertices.Length; index++)
            {
                res[index].vertex = meshVertices[index];
                res[index].normal = normals[index];
                res[index].boneWeight = boneWeights[index];
                if (colors.HasValues)
                    res[index].color = colors[index];
                else
                    res[index].color = color32s[index];
                res[index].tangent = tangents[index];
                res[index].uv = uv[index];
                res[index].uv2 = uv2[index];
                res[index].uv3 = uv3[index];
                res[index].uv4 = uv4[index];
                res[index].uv5 = uv5[index];
                res[index].uv6 = uv6[index];
                res[index].uv7 = uv7[index];
                res[index].uv8 = uv8[index];

            }

            return res;
        }
        struct VertexComponentSource<T>
        {
            private readonly T[] _data;
            private readonly bool _hasData;

            public VertexComponentSource(T[] data)
            {
                _data = data;
                _hasData = data != null && data.Length > 0;
            }

            public T this[int index]
            {
                get
                {
                    return _hasData ? _data[index] : default(T);
                }
            }

            public int Length
            {
                get { return _hasData ? _data.Length : 0; }
            }

            public bool HasValues => _hasData;
        }

        private class Triplet<T> : IReadOnlyList<T>
        {
            private T _a, _b, _c;
            public Triplet(T a, T b, T c)
            {
                _a = a;
                _b = b;
                _c = c;
            }

            public T this[int index]
            {
                get
                {
                    switch (index)
                    {
                        case 0: return _a;
                        case 1: return _b;
                        case 2: return _c;
                        default: throw new IndexOutOfRangeException();
                    }
                }
            }

            public IEnumerator<T> GetEnumerator()
            {
                return new Enumerator(this);
            }

            IEnumerator IEnumerable.GetEnumerator()
            {
                return GetEnumerator();
            }

            private class Enumerator : IEnumerator<T>
            {
                private readonly Triplet<T> _triplet;
                private int _index;

                public Enumerator(Triplet<T> triplet)
                {
                    _triplet = triplet;
                    _index = -1;
                }
                public bool MoveNext()
                {
                    if (_index >= 2)
                    {
                        return false;
                    }
                    ++_index;
                    return true;
                }

                public void Reset()
                {
                    _index = -1;
                }

                public T Current
                {
                    get { return _triplet[_index]; }
                }

                object IEnumerator.Current => Current;

                public void Dispose()
                {
                }
            }

            public int Count => 3;
        }


    }
}