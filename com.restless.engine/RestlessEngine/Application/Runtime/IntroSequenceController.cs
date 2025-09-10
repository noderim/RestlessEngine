using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Video;
using System.Threading.Tasks;
using System.Threading;
using RestlessLib.TimeR;
using RestlessLib.Attributes;
using RestlessEngine.Diagnostics;

namespace RestlessEngine.Application.Runtime
{
    public class IntroSequenceController : TimedSequence
    {
        [HorizontalLine]
        [Header("Resources")]
        public List<VideoPlayer> videoPlayers;

        public override async Task PlaySequence(CancellationToken token)
        {
            LogManager.Log("Intro Sequance Started", LogTag.LifeCycle);
            await PrepareVideoAsync();
            await base.PlaySequence(token);
        }

        public override void Update()
        {
            base.Update();
            if (Input.anyKeyDown)
            {
                SkipSequence();
            }
        }

        private async Task PrepareVideoAsync()
        {
            foreach (var videoPlayer in videoPlayers)
            {
                if (videoPlayer == null) continue;

                videoPlayer.Prepare();
                while (!videoPlayer.isPrepared)  // Wait for it to be ready
                {
                    await Awaitable.NextFrameAsync();
                }
            }
        }
        public override void EndSequence()
        {
            base.EndSequence();
            LogManager.Log("Intro Sequance Ended", LogTag.LifeCycle);
        }
    }
}
