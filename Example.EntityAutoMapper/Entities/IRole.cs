using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Example.EntityAutoMapper.Mapper.ObjectModel;

namespace Example.EntityAutoMapper.Entities
{
    [SamEntity]
    public interface IRole
    {
        [SamKey]
        string Id { get; set; }

        [SamProperty]
        string Name { get; set; }

        [SamProperty]
        string UserId { get; set; }
    }
}
