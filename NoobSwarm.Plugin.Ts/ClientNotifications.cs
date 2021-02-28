namespace NoobSwarm.Plugin.Ts
{
    public enum ClientNotifications
    {
        [Value("notifytalkstatuschange")] NotifyTalkStatusChange,
        [Value("notifymessage")] NotifyMessage,
        [Value("notifymessagelist")] NotifyMessageList,
        [Value("notifycomplainlist")] NotifyComplainList,
        [Value("notifybanlist")] NotifyBanList,
        [Value("notifyclientmoved")] NotifyClientMoved,
        [Value("notifyclientleftview")] NotifyClientLeftView,
        [Value("notifycliententerview")] NotifyClientEnterView,
        [Value("notifyclientpoke")] NotifyClientPoke,
        [Value("notifyclientchatclosed")] NotifyClientChatClosed,
        [Value("notifyclientchatcomposing")] NotifyClientChatComposing,
        [Value("notifyclientupdated")] NotifyClientUpdated,
        [Value("notifyclientids")] NotifyClientIds,
        [Value("notifyclientdbidfromuid")] NotifyClientDbIdFromUid,
        [Value("notifyclientnamefromuid")] NotifyClientNameFromUid,
        [Value("notifyclientnamefromdbid")] NotifyClientNameFromDbId,
        /// <summary>
        /// Do not unsubscribe
        /// </summary>
        [Value("notifyclientuidfromclid")] NotifyClientUidFromClid,
        [Value("notifyconnectioninfo")] NotifyConnectionInfo,
        [Value("notifychannelcreated")] NotifyChannelCreated,
        [Value("notifychanneledited")] NotifyChannelEdited,
        [Value("notifychanneldeleted")] NotifyChannelDeleted,
        [Value("notifychannelmoved")] NotifyChannelMoved,
        [Value("notifyserveredited")] NotifyServerEdited,
        [Value("notifyserverupdated")] NotifyServerUpdated,
        [Value("channellist")] ChannelList,
        [Value("channellistfinished")] ChannelListFinished,
        [Value("notifytextmessage")] NotifyTextMessage,
        [Value("notifycurrentserverconnectionchanged")] NotifyCurrentServerConnectionChanged,
        [Value("notifyconnectstatuschange")] NotifyConnectStatusChange,
        [Value("any")] Any,
    }
}
