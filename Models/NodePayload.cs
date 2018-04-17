using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Models
{
    public class NodePayload
    {   
        [Required]
        public Block NewBlock { get; set; }
        
        [Required]
        public List<Block> Blockchain { get; set; }
    }
}