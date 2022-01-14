using System;
using Appalachia.Audio.Behaviours;
using Appalachia.Core.Objects.Initialization;
using Appalachia.Core.Objects.Root;
using Appalachia.Utility.Async;
using Unity.Profiling;
using UnityEngine.Playables;

namespace Appalachia.Audio.Playables.Emitter
{
    [Serializable]
    public class AudioEmitterControlBehaviour : AppalachiaPlayable<AudioEmitterControlBehaviour>
    {
        #region Fields and Autoproperties

        public AudioEmitterClipAction action;

        #endregion

        protected override async AppaTask Initialize(Initializer initializer)
        {
            await AppaTask.CompletedTask;
        }

        protected override void OnPause(Playable playable, FrameData info)
        {
        }

        protected override void OnPlay(Playable playable, FrameData info)
        {
        }

        protected override void Update(Playable playable, FrameData info, object playerData)
        {
            using (_PRF_Update.Auto())
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

        protected override void WhenDestroyed(Playable playable)
        {
        }

        protected override void WhenStarted(Playable playable)
        {
        }

        protected override void WhenStopped(Playable playable)
        {
        }
    }
}
