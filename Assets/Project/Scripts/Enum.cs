using System;

[Serializable]
public enum SocketServerEventType
{
    //Requests
    reqJoinBoard,
    reqBet,
    reqCardSeen,
    reqPack,
    reqShowHand,
    reqLeave,
    
    //Responses
    resUserJoined,
    resKickOut,
    resPlayersState, //--> aama tne aHand ma user na card mlse 
    resBoardState,
    resResult,
    resPlaceBet,
    resCardSeen,
    resPack,
    resShowHand,
    resTurnMissed,
    resPlayerTurn,
    resPlayerLeft,
    resGameInitializeTimer,
    resHand,
    resetTable,
    resGameState

}
