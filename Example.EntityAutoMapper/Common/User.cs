using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Example.EntityAutoMapper.Entities;

namespace Example.EntityAutoMapper.Common
{
    public class User : IUser
    {
        [Key]
        public string Id { get; set; }

        [Column]
        public string UserName { get; set; }
    }
}
