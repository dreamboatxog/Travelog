using Microsoft.AspNetCore.Http;
using System;
using System.Buffers.Text;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Travelog.Contracts
{
    public record PhotoResponseDTO
    {
        public Guid Id { get; set; }
        public string FilePath{ get; set; }
        public string Description { get; set; }
    }
}
