#region

using System;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;

#endregion

namespace Appalachia.Core.Audio.Components
{
    [Serializable]
    public class AudioSignalSmoothAnalyzer
    {
        [HideInInspector] public int referenceID;

        [FoldoutGroup("Trend")]
        [HorizontalGroup("Trend/A", .5f), ReadOnly, NonSerialized, ShowInInspector, LabelWidth(70)]
        public float min = GAC.THRESHOLD_DB_HIGH;

        [HorizontalGroup("Trend/A", .5f), ReadOnly, NonSerialized, ShowInInspector, LabelWidth(70)]
        public float max = GAC.THRESHOLD_DB_LOW;

        [HorizontalGroup("Trend/B", .5f), ReadOnly, NonSerialized, ShowInInspector, LabelWidth(70)]
        public float avg;

        [HorizontalGroup("Trend/B", .5f), ReadOnly, NonSerialized, ShowInInspector, LabelWidth(70)]
        public float realAvg;

        [HorizontalGroup("Trend/C", .5f), ReadOnly, ShowInInspector, LabelWidth(70)]
        public float last;

        [NonSerialized] public float sum;

        [HorizontalGroup("Trend/C1", .5f), PropertyRange(0.01f, .2f), LabelWidth(70)]
        public float maxStepUp = 0.01f;

        [HorizontalGroup("Trend/C1", .5f), PropertyRange(0.01f, .2f), LabelWidth(70)]
        public float maxStepDown = 0.01f;

        [HideInInspector] public float maxStepTarget = 0.01f;

        [HorizontalGroup("Trend/D", .5f), PropertyRange(1, 128), LabelWidth(70), LabelText("Buffer Up")]
        public int bufferSizeUp = 24;

        [HorizontalGroup("Trend/D", .5f), PropertyRange(1, 128), LabelWidth(70), LabelText("Buffer Down")]
        public int bufferSizeDown = 48;

        [HorizontalGroup("Trend/E", .5f), ReadOnly, ShowInInspector, LabelWidth(70), LabelText("Target")]
        public int bufferTarget = 24;

        private Queue<float> buffer = new Queue<float>(128);

        [HorizontalGroup("Trend/E", .5f), ReadOnly, ShowInInspector, LabelWidth(70), LabelText("Count")]
        public int bufferCount => buffer?.Count ?? 0;

        public void Add(float value)
        {
            bufferSizeUp = Mathf.Max(1,   bufferSizeUp);
            bufferSizeDown = Mathf.Max(1, bufferSizeDown);

            if (value < last)
            {
                bufferTarget = bufferSizeDown;
                maxStepTarget = maxStepDown * Time.deltaTime;
            }
            else if (value == 0.0f)
            {
                bufferTarget = bufferSizeUp;
                maxStepTarget = maxStepUp * Time.deltaTime;
            }
            else if (value == last)
            {
            }
            else
            {
                bufferTarget = bufferSizeUp;
                maxStepTarget = maxStepUp * Time.deltaTime;
            }

            if (buffer == null)
            {
                buffer = new Queue<float>();
            }

            last = value;
            var maxDrops = 2;
            var dropCount = 0;

            while ((bufferCount >= bufferTarget) && (dropCount < maxDrops))
            {
                var deq = buffer.Dequeue();

                if ((sum - deq) >= 0)
                {
                    sum -= deq;
                }

                dropCount += 1;
            }

            if ((bufferCount > 2) && (sum < 0.0001f) && (sum > -0.0001f))
            {
                sum = 0.0f;
            }

            sum += value;
            buffer.Enqueue(value);

            realAvg = sum / bufferCount;

            var diff = realAvg - avg;

            if (diff > maxStepTarget)
            {
                avg += maxStepTarget;
            }
            else if (diff < -maxStepTarget)
            {
                avg -= maxStepTarget;
            }
            else
            {
                avg = realAvg;
            }

            if (value > max)
            {
                max = value;
            }

            if (value < min)
            {
                min = value;
            }
        }

        [Button]
        public void Reset()
        {
            referenceID = -1;
            buffer.Clear();
            sum = 0.0f;
            min = GAC.THRESHOLD_DB_HIGH;
            max = GAC.THRESHOLD_DB_LOW;
            avg = 0.0f;
            last = 0.0f;
        }
    }
}
