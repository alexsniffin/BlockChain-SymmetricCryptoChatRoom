using System.Collections.Generic;
using Models;

namespace Domainlogic.Interfaces
{
    public interface IBlockchainLogic
    {
        List<Block> ResolveBlockchain(NodePayload nodePayload);
    }
}