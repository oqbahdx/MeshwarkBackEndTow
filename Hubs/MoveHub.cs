using Microsoft.AspNetCore.SignalR;

namespace Meshwark.Hubs
{
    public class MoveHub:Hub
    {
        public async Task MoveViewFromServer(float newX,float newY) {

            await Clients.Others.SendAsync("ReceiveNewPosition",newX,newY);
            Console.WriteLine($"receive postion form server app : {newX}/{newY}");
        
        }
    }
}
