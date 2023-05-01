using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Principal;
using System.Text;
using System.Threading.Tasks;
using Example.EntityAutoMapper.Mapper.ObjectModel;

namespace Example.EntityAutoMapper.Entities
{
    [SamEntity]
    public interface IUser
    {
        [SamKey]
        string Id { get; set; }

        [SamProperty]
        string UserName { get; set; }

        //[SamNavigationProperty(typeof(IRole), nameof(IRole.UserId))]
        //List<IRole> Roles { get; set; }
    }
}
