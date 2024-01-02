using System.Runtime.CompilerServices;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;

namespace API;


[Authorize]
public class PresenceHub : Hub
{

    private readonly presenceTracker _tracker;

    public PresenceHub(presenceTracker tracker)
    {
        _tracker = tracker;
    }



    public override async Task OnConnectedAsync()
    {
       var isOnline= await _tracker.UserConnected(Context.User.GetUsername(), Context.ConnectionId);
       if(isOnline)
            await Clients.Others.SendAsync("UserIsOnline", Context.User.GetUsername());

        var currentUsers = await _tracker.GetOnlineUsers();

        await Clients.Caller.SendAsync("GetOnlinesUsers", currentUsers);

    }

    public override async Task OnDisconnectedAsync(Exception exception)
    {

        var isOffline = await _tracker.UserDisconnected(Context.User.GetUsername(), Context.ConnectionId);
        if(isOffline)
            await Clients.Others.SendAsync("UserIsOffline", Context.User.GetUsername());
        await base.OnDisconnectedAsync(exception);
    }


}
