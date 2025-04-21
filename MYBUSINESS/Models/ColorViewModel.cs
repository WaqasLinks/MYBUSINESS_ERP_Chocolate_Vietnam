using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace MYBUSINESS.Models
{
    public class ColorViewModel
    {
        public int Id { get; set; }

        
        public string ColorName { get; set; }


        public string ColorCode { get; set; } // e.g. #FF0000
        public Color Color { get; set; }
    }
}