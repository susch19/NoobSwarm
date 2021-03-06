using MessagePack;

using NoobSwarm.Lights;
using NoobSwarm.Lights.LightEffects;
using NoobSwarm.Makros;

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

using Vulcan.NET;

namespace NoobSwarm
{
    public class MakroManager : IDisposable
    {
        public event EventHandler? RecordingStarted;
        public event EventHandler<RecordKey>? RecordAdded;
        public event EventHandler<IReadOnlyCollection<RecordKey>>? RecordingFinished;

        [MessagePackObject()]
        public record RecordKey(
            [property: Key(0)] Makros.Key Key,
            [property: Key(1)] int TimeBeforePress,
            [property: Key(2)] bool Pressed);

        public Makros.Key StopRecordKey { get; set; } = Makros.Key.PAUSE;

        private bool isSynchronRecording;
        private readonly List<RecordKey> records = new();
        private bool isRecording;
        private bool isAsyncRecording;
        private TaskCompletionSource<Makros.Key>? asyncTaskCompletionSource;
        private CancellationTokenSource? asyncToken;
        private readonly Stopwatch stopWatch = new();
        private readonly SemaphoreSlim slim = new SemaphoreSlim(0);
        private readonly Queue<Makros.Key> keyQueue = new();
        private readonly AutoResetEvent resetEvent = new(false);

        public void BeginRecording(List<RecordKey> recordedKeys)
        {
            stopWatch.Reset();
            records.AddRange(recordedKeys);
            isRecording = true;
            RecordingStarted?.Invoke(this, EventArgs.Empty);
        }

        public void FinishRecording()
        {
            if (isSynchronRecording)
                slim.Release();
            else if (isAsyncRecording)
                asyncToken?.Cancel();

            isRecording = false;
            RecordingFinished?.Invoke(this, records);
        }

        public void Clear(ICollection<RecordKey>? keys = null)
        {
            stopWatch.Reset();

            if (keys is null || keys.Count == 0)
                records.Clear();
            else
                records.AddRange(keys);
        }

        /// <summary>
        /// Records all <see cref="LedKey"/> pressed till <see cref="VulcanKeyboard.VolumeKnobTurnedReceived"/> is received
        /// </summary>
        public async Task<List<RecordKey>> StartRecording(CancellationToken token)
        {
            stopWatch.Reset();
            records.Clear();
            RecordingStarted?.Invoke(this, new EventArgs());
            isSynchronRecording = true;
            try
            {

                await slim.WaitAsync(token);
            }
            catch (OperationCanceledException x)
            {

            }
            isSynchronRecording = false;

            return records.ToList();
        }

        /// <summary>
        /// Async ecording of all pressed keys until <paramref name="token"/> is cancelled or
        /// <see cref="VulcanKeyboard.VolumeKnobTurnedReceived"/> is received
        /// </summary>
        /// <param name="token">Token to cancel the recording</param>
        /// <returns>async enumerable which contains the recording keys</returns>
        public async IAsyncEnumerable<Makros.Key> Record([EnumeratorCancellation] CancellationToken token)
        {
            RecordingStarted?.Invoke(this, new EventArgs());
            asyncToken = CancellationTokenSource.CreateLinkedTokenSource(token);
            asyncToken.Token.Register(() => asyncTaskCompletionSource?.TrySetCanceled());

            isAsyncRecording = true;
            try
            {
                while (!asyncToken.Token.IsCancellationRequested)
                {
                    asyncTaskCompletionSource = new TaskCompletionSource<Makros.Key>(TaskCreationOptions.AttachedToParent);
                    Makros.Key res;
                    while (!keyQueue.TryDequeue(out res))
                        resetEvent.WaitOne();
                    asyncTaskCompletionSource.SetResult(res);

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
            if (StopRecordKey == key)
            {
                FinishRecording();
            }
            else if (isSynchronRecording || isAsyncRecording || isRecording)
            {
                var rec = new RecordKey(key, (int)stopWatch.Elapsed.TotalMilliseconds, down);
                records.Add(rec);
                stopWatch.Restart();
                RecordAdded?.Invoke(this, rec);
                if (isAsyncRecording)
                {

                    keyQueue.Enqueue(key);
                    resetEvent.Set();
                }
            }
        }

        public void Dispose()
        {
            asyncTaskCompletionSource?.TrySetCanceled();
            asyncToken?.Cancel();
            stopWatch.Stop();

            slim.Dispose();
            resetEvent?.Close();
            resetEvent?.Dispose();
        }
    }
}

