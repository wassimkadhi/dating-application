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
        await _tracker.UserConnected(Context.User.GetUsername(), Context.ConnectionId);
        await Clients.Others.SendAsync("UserIsOnline", Context.User.GetUsername());

        var currentUsers = await _tracker.GetOnlineUsers();

        await Clients.All.SendAsync("GetOnlinesUsers", currentUsers);

    }

    public override async Task OnDisconnectedAsync(Exception exception)
    {

        await _tracker.UserDisconnected(Context.User.GetUsername(), Context.ConnectionId);
        await Clients.Others.SendAsync("UserIsOffline", Context.User.GetUsername());

        var currentUsers = await _tracker.GetOnlineUsers();
        await Clients.All.SendAsync("GetOnlinesUsers", currentUsers);
        await base.OnDisconnectedAsync(exception);
    }


}
