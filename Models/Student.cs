using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace SampleSecureWeb.Models
{
    public class Student
    {
        [Key]
        public string NIM { get; set; } = null!;
        public string FullName { get; set;} = null!;
    }
}