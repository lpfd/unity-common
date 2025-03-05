using System.Collections;
using UnityEngine;

namespace Leap.Forward
{
    public interface ICoroutineRunner
    {
        Coroutine StartCoroutine(IEnumerator routine);
        void StopCoroutine(IEnumerator routine);
        void StopAllCoroutines();
    }
}