using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DentalNova.Core.Dtos
{
    public class TokenDto
    {
        public string Token { get; set; }
        public DateTime Expiracion { get; set; }
    }
}
