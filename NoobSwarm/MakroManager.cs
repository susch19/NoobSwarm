using NoobSwarm.Lights;
using NoobSwarm.Lights.LightEffects;
using NoobSwarm.Makros;

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

using Vulcan.NET;

namespace NoobSwarm
{
    public class MakroManager
    {

        public event EventHandler RecordingStarted;
        public event EventHandler<IReadOnlyCollection<RecordKey>> RecordingFinished;

        public record RecordKey(Makros.Key Key, int TimeBeforePress, bool Pressed);

        public Makros.Key StopRecordKey { get; set; } = Makros.Key.PAUSE;

        // Synchron recording variables
        private bool isSynchronRecording;
        private AutoResetEvent synchronRecordingResetEvent = new(false);

        private List<RecordKey> records = new();

        private bool isAsyncRecording;
        private TaskCompletionSource<LedKey>? asyncTaskCompletionSource;
        private CancellationTokenSource? asyncToken;
        private readonly Stopwatch stopWatch = new();


        public void FinishRecording()
        {
            if (isSynchronRecording)
                synchronRecordingResetEvent.Set();
            else if (isAsyncRecording)
                asyncToken?.Cancel();

            RecordingFinished?.Invoke(this, records);
        }


        /// <summary>
        /// Records all <see cref="LedKey"/> pressed till <see cref="VulcanKeyboard.VolumeKnobTurnedReceived"/> is received
        /// </summary>
        public ReadOnlyCollection<RecordKey> StartRecording()
        {
            RecordingStarted?.Invoke(this, new EventArgs());
            stopWatch.Reset();
            records.Clear();
            isSynchronRecording = true;

            synchronRecordingResetEvent.WaitOne();
            isSynchronRecording = false;

            return new ReadOnlyCollection<RecordKey>(records);
        }

        /// <summary>
        /// Async ecording of all pressed keys until <paramref name="token"/> is cancelled or
        /// <see cref="VulcanKeyboard.VolumeKnobTurnedReceived"/> is received
        /// </summary>
        /// <param name="token">Token to cancel the recording</param>
        /// <returns>async enumerable which contains the recording keys</returns>
        public async IAsyncEnumerable<LedKey> Record([EnumeratorCancellation] CancellationToken token)
        {
            RecordingStarted?.Invoke(this, new EventArgs());
            asyncToken = CancellationTokenSource.CreateLinkedTokenSource(token);
            asyncToken.Token.Register(() => asyncTaskCompletionSource?.TrySetCanceled());

            isAsyncRecording = true;
            try
            {
                while (!asyncToken.Token.IsCancellationRequested)
                {
                    asyncTaskCompletionSource = new TaskCompletionSource<LedKey>(TaskCreationOptions.AttachedToParent);
                    yield return await asyncTaskCompletionSource.Task;
                }
            }
            finally
            {
                isAsyncRecording = false;
            }
        }

        public void KeyReceived(Makros.Key key, bool down)
        {
            if(StopRecordKey == key)
            {
                FinishRecording();
            }
            else if (isSynchronRecording || isAsyncRecording)
            {
                records.Add(new(key, (int)stopWatch.Elapsed.TotalMilliseconds, down));
                stopWatch.Restart();
            }
        }

    }
}

