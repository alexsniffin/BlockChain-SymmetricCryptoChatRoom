using System.Collections.Generic;
using Models;

namespace Domainlogic.Interfaces
{
    public interface IBlockLogic
    {
        void ResolveBlock(NodePayload nodePayload);
    }
}