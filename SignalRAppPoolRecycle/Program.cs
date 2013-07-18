using System;
using System.Timers;
using Microsoft.AspNet.SignalR.Client;
using Microsoft.AspNet.SignalR.Client.Hubs;

namespace SignalRAppPoolRecycle
{
    class Program
    {
        private static HubConnection _hubConnection;
        private static IHubProxy _conversationProxy;
        private static readonly Timer Timer = new Timer(10000);
        public static void Main()
        {
            Timer.Elapsed += Elapsed;
            Timer.Start();
            _hubConnection = new HubConnection("http://localhost:50025");
            _conversationProxy = _hubConnection.CreateHubProxy("RecycleHub");
            _conversationProxy.On("Broadcast", message => Console.WriteLine(message));
            _conversationProxy.On("Online", message => Console.WriteLine(message));

            _hubConnection.Closed += HubConnectionOnClosed;
            _hubConnection.StateChanged += HubConnectionStateChanged;
            ConnectToHub();
            Console.ReadLine();
        }

        private static void Elapsed(object sender, ElapsedEventArgs e)
        {
            if (_hubConnection.State == ConnectionState.Connected)
            {
                _conversationProxy.Invoke("Relay");
            }
        }

        private static void HubConnectionStateChanged(StateChange stateChange)
        {
            Console.WriteLine(@"{0} - Client state is {1}", DateTime.Now, _hubConnection.State);
            Console.WriteLine(@"Connection ID is {0}", _hubConnection.ConnectionId);

            if (_hubConnection.State == ConnectionState.Connected)
            {
                Console.WriteLine(@"{0} - UserOnline sent to hub", DateTime.Now);
                _conversationProxy.Invoke("UserOnline").Wait();
            }
        }

        private static void HubConnectionOnClosed()
        {
            Console.WriteLine(@"{0} - Hub connection closed", DateTime.Now);
            ConnectToHub();
        }

        private static void ConnectToHub()
        {
            _hubConnection.Start().Wait();
        }


    }
}
