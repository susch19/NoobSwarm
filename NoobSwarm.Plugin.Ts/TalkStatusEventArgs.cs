using System;

namespace NoobSwarm.Plugin.Ts
{
    public class TalkStatusEventArgs : EventArgs
    {
        /// <summary>
        /// The client id (clid)
        /// </summary>
        public uint ClientId { get; set; }

        /// <summary>
        /// This this me?
        /// </summary>
        public bool IsMe { get; set; }

        /// <summary>
        /// Talking or not
        /// </summary>
        public bool IsTalking { get; set; }

        public TalkStatusEventArgs(uint clientId, bool isMe, bool isTalking)
        {
            ClientId = clientId;
            IsMe = isMe;
            IsTalking = isTalking;
        }
    }
}
