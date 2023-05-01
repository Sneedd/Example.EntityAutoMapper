using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Example.EntityAutoMapper.Common;
using Example.EntityAutoMapper.Entities;
using Example.EntityAutoMapper.Mapper;
using Microsoft.EntityFrameworkCore;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Example.EntityAutoMapper
{
    public class Benchmark
    {
        private CommonDbContext _commonContext;
        private SamDbContext _samContext;
        private SamEntityFactory _factory;

        public void Setup() 
        {
            // EfCore Setup
            _commonContext = new CommonDbContext();

            // Sam Setup
            var generator = new SamEntityGenerator();
            var entities = generator.Generate();
            _factory = new SamEntityFactory(entities);
            _samContext = new SamDbContext(_factory);
        } 

        public void IterationCleanup() 
        {
            _commonContext.Database.EnsureDeleted();
            _samContext.Database.EnsureDeleted();
        }

        public void TestEfCore()
        {
            var context = _commonContext;

            // Create the mapper sets
            var userSet = context.Set<User>();
            var roleSet = context.Set<Role>();

            // Create, add and save some users and roles
            userSet.Add(new User 
            {
                Id = Guid.NewGuid().ToString(),
                UserName = "Admin",
            });
            userSet.Add(new User
            {
                Id = Guid.NewGuid().ToString(),
                UserName = "Guest",
            });
            userSet.Add(new User
            {
                Id = Guid.NewGuid().ToString(),
                UserName = "Moderator",
            });
            roleSet.Add(new Role
            {
                Id = Guid.NewGuid().ToString(),
                Name = "GlobalAdministrator",
            });
            roleSet.Add(new Role
            {
                Id = Guid.NewGuid().ToString(),
                Name = "SecurityAdministrator",
            });
            roleSet.Add(new Role
            {
                Id = Guid.NewGuid().ToString(),
                Name = "BackupAdministrator",
            });
            context.SaveChanges();

            // Get all users
            var users = userSet.ToList();
            Assert.AreEqual(3, users.Count());

            // Filter for one user
            var singleUser = userSet.Where(a => a.UserName == "Admin").ToList();
            Assert.AreEqual(1, singleUser.Count());

            // Get all roles
            var roles = roleSet.ToList();
            Assert.AreEqual(3, roles.Count());
        }

        public void TestSam()
        {
            var context = _samContext;
            var factory = _factory;

            // Create the mapper set
            var userSet = context.Set<IUser>();
            var roleSet = context.Set<IRole>();

            // Create, add and save some users
            userSet.Add(factory.Create<IUser>(u =>
            {
                u.Id = Guid.NewGuid().ToString();
                u.UserName = "Admin";
            }));
            userSet.Add(factory.Create<IUser>(u =>
            {
                u.Id = Guid.NewGuid().ToString();
                u.UserName = "Guest";
            }));
            userSet.Add(factory.Create<IUser>(u =>
            {
                u.Id = Guid.NewGuid().ToString();
                u.UserName = "Moderator";
            }));
            roleSet.Add(factory.Create<IRole>(r => 
            {
                r.Id = Guid.NewGuid().ToString();
                r.Name = "GlobalAdministrator";
            }));
            roleSet.Add(factory.Create<IRole>(r =>
            {
                r.Id = Guid.NewGuid().ToString();
                r.Name = "SecurityAdministrator";
            }));
            roleSet.Add(factory.Create<IRole>(r =>
            {
                r.Id = Guid.NewGuid().ToString();
                r.Name = "BackupAdministrator";
            }));
            context.SaveChanges();

            // Get all users
            var users = userSet.ToList();
            Assert.AreEqual(3, users.Count());

            // Filter for one user
            var singleUser = userSet.Where(a => a.UserName == "Admin").ToList();
            Assert.AreEqual(1, singleUser.Count());

            // Get all roles
            var roles = roleSet.ToList();
            Assert.AreEqual(3, roles.Count());
        }
    }
}
