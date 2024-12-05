using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Travelog.Contracts.UpdatePlace
{
    public record PhotoUpdateDTO
    {
        
        public Guid? Id { get; set; } // Идентификатор фотографии (если она существует)
        public IFormFile? File { get; set; } // Новый файл (если он есть)
        public string? FilePath { get; set; } // URL уже загруженной фотографии (если файл не изменен)
        public string? Description { get; set; } // Описание фотографии
    }

}
