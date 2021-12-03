using System;
using Appalachia.Audio.Behaviours;
using UnityEngine.Playables;

namespace Appalachia.Audio.Playables.Emitter
{
    [Serializable]
    public class AudioEmitterControlBehaviour : PlayableBehaviour
    {
        #region Fields and Autoproperties

        public AudioEmitterClipAction action;

        #endregion

        public override void OnPlayableDestroy(Playable playable)
        {
        }

        public override void ProcessFrame(Playable playable, FrameData info, object playerData)
        {
            var emitter = playerData as AudioEmitter;

            var playState = playable.GetPlayState();

            if (playState != PlayState.Playing)
            {
                return;
            }

            var previousTime = playable.GetPreviousTime();
            var currentTime = playable.GetTime();
            var duration = playable.GetDuration();

            var isFirstFrame = previousTime == 0.0;

            if ((emitter != null) && isFirstFrame)
            {
                switch (action)
                {
                    case AudioEmitterClipAction.CueIn:
                        emitter.CueIn();
                        break;
                    case AudioEmitterClipAction.CueOut:
                        emitter.CueOut();
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }
            }
        }
    }
}
