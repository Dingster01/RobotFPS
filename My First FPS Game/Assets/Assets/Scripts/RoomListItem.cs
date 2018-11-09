using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking.Match;

public class RoomListItem : MonoBehaviour
{
    public delegate void JoinRoomDelegate(MatchInfoSnapshot match);
    private JoinRoomDelegate joinRoomCallBack;

    [SerializeField]
    private Text roomNameText;

    private MatchInfoSnapshot match;

    public void Setup(MatchInfoSnapshot _match, JoinRoomDelegate _joinRoomCallBack)
    {
        match = _match;
        joinRoomCallBack = _joinRoomCallBack;

        roomNameText.text = match.name + " (" + match.currentSize + "/" + match.maxSize + ")";
    }

    public void JoinRoom()
    {
        joinRoomCallBack.Invoke(match);
    }
	
}
