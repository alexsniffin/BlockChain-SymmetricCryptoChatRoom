using System.Collections.Generic;
using Domainlogic.Interfaces;
using Models;

namespace Domainlogic
{
    public class BlockchainLogic : IBlockchainLogic
    {
        private readonly BlockLogic _blockLogic;

        public BlockchainLogic(BlockLogic blockLogic)
        {
            _blockLogic = blockLogic;
        }
        
        public List<Block> ResolveBlockchain(NodePayload nodePayload)
        {
            // Resolve the new block
            _blockLogic.ResolveBlock(nodePayload);
            
            // Add the new block to the chain
            nodePayload.Blockchain.Add(nodePayload.NewBlock);

            // Return the updated blockchain
            return nodePayload.Blockchain;
        }
    }
}