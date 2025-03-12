using System.Collections.Generic;
using System;
using UnityEditor;
using System.Collections;

namespace Leap.Forward
{
    public class CoroutineProgress : IEnumerator, IDisposable
    {
        private readonly string _title;
        private readonly IEnumerator<ProgressReport> _coroutine;

        public struct ProgressReport
        {
            public ProgressReport(string info, float progress)
            {
                Info = info;
                Progress = progress;
            }

            public ProgressReport(float progress)
            {
                Info = String.Empty;
                Progress = progress;
            }

            public string Info;
            public float Progress;
        }

        public CoroutineProgress(string title, IEnumerator<ProgressReport> coroutine)
        {
            _title = title;
            _coroutine = coroutine;
            EditorUtility.DisplayProgressBar(_title, "", 0);
        }


        public bool MoveNext()
        {
            try
            {
                if (_coroutine.MoveNext())
                {
                    ProgressReport rep = _coroutine.Current;
                    EditorUtility.DisplayProgressBar(_title, rep.Info, rep.Progress);
                    return true;
                }
                HideProgressBar();
                return false;
            }
            catch
            {
                HideProgressBar();
                throw;
            }
        }

        private void HideProgressBar()
        {
            EditorUtility.ClearProgressBar();
        }

        public void Reset()
        {
        }

        public object Current => null;
        public void Dispose()
        {
            EditorUtility.ClearProgressBar();
        }
    }

}