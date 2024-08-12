using Core.Entities;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.DTOS
{
    public class ManagerUpdateDto
    {
        public Guid ? Id { get; set; }

        public string? FirstName { get; set; }
       
        public string? LastName { get; set; }

        public string? storeName { get; set; }

        public string? BusinessLocation { get; set; }

        public string? OldPassword { get; set; }

        public string? NewPassword { get; set; }

        public string? OldEmail { get; set; }

        public string? NewEmail { get; set; }

        public IFormFile? UserPhoto { get; set; }

    }
}
