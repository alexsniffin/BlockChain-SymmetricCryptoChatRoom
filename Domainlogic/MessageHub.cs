using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Domainlogic.Interfaces;
using Microsoft.AspNetCore.SignalR;
using Models;

namespace Domainlogic
{
    public class MessageHub : Hub
    {
        // connected IDs
        private static readonly HashSet<string> ConnectedIds = new HashSet<string>();

        private readonly IBlockchainLogic _blockchainLogic;

        public MessageHub(IBlockchainLogic blockchainLogic)
        {
            _blockchainLogic = blockchainLogic;
        }

        public override async Task OnConnectedAsync()
        {
            ConnectedIds.Add(Context.ConnectionId);
            
            await Clients.All.SendAsync("SendAction", "joined", ConnectedIds.Count);
        }

        public override async Task OnDisconnectedAsync(Exception ex)
        {
            ConnectedIds.Remove(Context.ConnectionId);
            
            await Clients.All.SendAsync("SendAction", "left", ConnectedIds.Count);
        }

        public async Task Send(NodePayload nodePayload)
        {
            var updatedBlockchain = _blockchainLogic.ResolveBlockchain(nodePayload);
            
            // Broadcast the updated blockchain to all users and who created it and at what time
            await Clients.All.SendAsync("SendMessage", updatedBlockchain, nodePayload.NewBlock.Name, nodePayload.NewBlock.Date);
        }
    }
}