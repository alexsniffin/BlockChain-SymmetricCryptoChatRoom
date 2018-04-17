using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using Domainlogic.Interfaces;
using Models;

namespace Domainlogic
{
    public class BlockLogic : IBlockLogic
    {
        public void ResolveBlock(NodePayload nodePayload)
        {
            if (!ValidateBlock(nodePayload.NewBlock, nodePayload.Blockchain.Last()))
                throw new InvalidDataException("Validation of block returned invalid!");
        }

        private static bool ValidateBlock(Block newBlock, Block prevBlock)
        {
            var hash = CalculateHash(newBlock).ToLower();
            
            if (prevBlock.Index + 1 != newBlock.Index || 
                prevBlock.Hash != newBlock.PreviousHash || 
                hash != newBlock.Hash) 
                return false;

            return true; 
        }
        
        /// <summary>
        /// Calculate a SHA256 hash from a given `Block`
        /// </summary>
        /// <param name="block"></param>
        /// <returns></returns>
        private static string CalculateHash(Block block)
        {
            var sha256 = SHA256.Create();
            
            var bytes = Encoding.UTF8.GetBytes(block.ToString());
            
            var hash = sha256.ComputeHash(bytes);
            
            return ByteArrayToString(hash);
        }
        
        /// <summary>
        /// Convert byte array to hex string
        /// </summary>
        /// <param name="ba"></param>
        /// <returns></returns>
        private static string ByteArrayToString(byte[] ba)
        {
            return BitConverter.ToString(ba).Replace("-","");
        }
    }
}