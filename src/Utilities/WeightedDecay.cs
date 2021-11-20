using UnityEngine;

namespace Appalachia.Audio.Utilities
{
    public struct WeightedDecay
    {
        #region Constants and Static Readonly

        private const float redrawPenalty = 1f;
        private const float restoreSpeed = 0.05f;

        #endregion

        #region Fields and Autoproperties

        public float weightSum;

        public float[] weights;

        #endregion

        public int count
        {
            get => weights != null ? weights.Length : 0;
            set
            {
                if ((weights == null) || (weights.Length != value))
                {
                    weights = new float[value];
                    for (var i = 0; i < value; ++i)
                    {
                        weights[i] = 1f;
                    }

                    weightSum = value;
                }
            }
        }

        public int Draw(float q, int n)
        {
            n = Mathf.Max(0, n);
            count = n;

            var v = Mathf.Clamp01(q) * weightSum;
            int i, j;
            for (i = 0; (i < (n - 1)) && (v >= weights[i]); ++i)
            {
                v -= weights[i];
            }

            weights[i] = Mathf.Max(weights[i] - redrawPenalty, 0f);
            weightSum = 0f;
            for (j = 0; j < n; ++j)
            {
                weights[j] = Mathf.Min(weights[j] + restoreSpeed, 1f);
                weightSum += weights[j];
            }

            return i;
        }
    }
}
