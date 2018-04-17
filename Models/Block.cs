using System;
using System.ComponentModel.DataAnnotations;
using System.Runtime.InteropServices;

namespace Models
{
    public class Block
    {   
        public int Index { get; set; }
        
        [Required]
        public string Name { get; set; }
        
        public string PreviousHash { get; set; }
        
        public DateTime Date { get; set; }
        
        [Required]
        public string Data { get; set; }
        
        public string Hash { get; set; }

        /// <summary>
        /// Used for creating the new `Hash`
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return Index + PreviousHash + Data;
        }
    }
}