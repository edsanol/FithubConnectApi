using Microsoft.AspNetCore.SignalR;

namespace Api.Hubs
{
    public class NotificationHub : Hub
    {
        /// <summary>
        /// Se llama automáticamente cuando un cliente se conecta al Hub.
        /// </summary>
        public override async Task OnConnectedAsync()
        {
            // Puedes obtener info del usuario si usas JWT Bearer.
            // Ejemplo: var userId = Context.User?.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            // Ejemplo: var role   = Context.User?.FindFirst(ClaimTypes.Role)?.Value;

            await base.OnConnectedAsync();
        }

        /// <summary>
        /// Se llama automáticamente cuando un cliente se desconecta del Hub.
        /// </summary>
        public override async Task OnDisconnectedAsync(Exception? exception)
        {
            // Ejemplo: notificar a todos que el usuario se ha desconectado, guardar logs, etc.

            await base.OnDisconnectedAsync(exception);
        }

        /// <summary>
        /// Permite que un cliente se "suscriba" a un canal específico.
        /// Internamente, SignalR usa grupos para agrupar las conexiones.
        /// </summary>
        /// <param name="channelId">Id del canal que se desea unir.</param>
        public async Task JoinChannel(long channelId)
        {
            // Se suscribe al "grupo" que representa este canal
            await Groups.AddToGroupAsync(Context.ConnectionId, channelId.ToString());

            // (Opcional) Envías un mensaje a todos en el canal para avisar que alguien se unió
            await Clients.Group(channelId.ToString())
                .SendAsync("ChannelJoined", $"Connection {Context.ConnectionId} joined channel {channelId}");
        }

        /// <summary>
        /// Permite salir de un canal específico.
        /// </summary>
        public async Task LeaveChannel(long channelId)
        {
            // Se desuscribe del "grupo" 
            await Groups.RemoveFromGroupAsync(Context.ConnectionId, channelId.ToString());

            // (Opcional) Notificas a todos que un usuario salió
            await Clients.Group(channelId.ToString())
                .SendAsync("ChannelLeft", $"Connection {Context.ConnectionId} left channel {channelId}");
        }

        /// <summary>
        /// Envía un mensaje a todos los usuarios suscritos a un canal.
        /// </summary>
        public async Task SendMessageToChannel(long channelId, string message)
        {
            await Clients.Group(channelId.ToString())
                .SendAsync("ReceiveMessage", channelId, message);
        }
    }
}
