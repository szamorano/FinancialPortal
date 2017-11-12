namespace FinancialPortal.Migrations
{
    using FinancialPortal.Models;
    using Microsoft.AspNet.Identity;
    using Microsoft.AspNet.Identity.EntityFramework;
    using System;
    using System.Data.Entity;
    using System.Data.Entity.Migrations;
    using System.Linq;

    internal sealed class Configuration : DbMigrationsConfiguration<FinancialPortal.Models.ApplicationDbContext>
    {
        public Configuration()
        {
            AutomaticMigrationsEnabled = true;
        }

        protected override void Seed(FinancialPortal.Models.ApplicationDbContext context)
        {
            var roleManager = new RoleManager<IdentityRole>(
                new RoleStore<IdentityRole>(context));
            if (!context.Roles.Any(r => r.Name == "Admin"))
            {
                roleManager.Create(new IdentityRole { Name = "Admin" });
            }
            if (!context.Roles.Any(r => r.Name == "Customer"))
            {
                roleManager.Create(new IdentityRole { Name = "Customer" });
            }

            var userManager = new UserManager<ApplicationUser>(new UserStore<ApplicationUser>(context));


            if (!context.Users.Any(u => u.Email == "stevenzamorano.code@gmail.com"))
            {
                userManager.Create(new ApplicationUser
                {
                    UserName = "stevenzamorano.code@gmail.com",
                    Email = "stevenzamorano.code@gmail.com",
                    FirstName = "Steven",
                    LastName = "Zamorano",
                }, "Password1!");
            }
            var userId = userManager.FindByEmail("stevenzamorano.code@gmail.com").Id;
            userManager.AddToRole(userId, "Admin");


            if (!context.Users.Any(u => u.Email == "mjaang@coderfoundry.com"))
            {
                userManager.Create(new ApplicationUser
                {
                    UserName = "mjaang@coderfoundry.com",
                    Email = "mjaang@coderfoundry.com",
                    FirstName = "Mark",
                    LastName = "Jaang",
                }, "Password1!");
            }
            var userId_mark = userManager.FindByEmail("mjaang@coderfoundry.com").Id;
            userManager.AddToRole(userId_mark, "Customer");


            if (!context.Users.Any(u => u.Email == "rchapman@coderfoundry.com"))
            {
                userManager.Create(new ApplicationUser
                {
                    UserName = "rchapman@coderfoundry.com",
                    Email = "rchapman@coderfoundry.com",
                    FirstName = "Ryan",
                    LastName = "Chapman",
                }, "Password1!");
            }
            var userId_ryan = userManager.FindByEmail("rchapman@coderfoundry.com").Id;
            userManager.AddToRole(userId_ryan, "Customer");


            if (!context.Categories.Any(c => c.Name == "Entertainment"))
            {
                var category = new Category();
                category.Name = "Entertainment";
                context.Categories.Add(category);
            }
            if (!context.Categories.Any(c => c.Name == "Clothing"))
            {
                var category = new Category();
                category.Name = "Clothing";
                context.Categories.Add(category);
            }
            if (!context.Categories.Any(c => c.Name == "Mortgage"))
            {
                var category = new Category();
                category.Name = "Mortgage";
                context.Categories.Add(category);
            }
            if (!context.Categories.Any(c => c.Name == "Rent"))
            {
                var category = new Category();
                category.Name = "Rent";
                context.Categories.Add(category);
            }
            if (!context.Categories.Any(c => c.Name == "Utilities"))
            {
                var category = new Category();
                category.Name = "Utilities";
                context.Categories.Add(category);
            }
        }
    }
}









            //if (!context.Category.Any(c => c.Name == "Entertainment"))
            //{
            //    var category = new Category();
            //    category.Name = "Entertainment";
            //    context.Categories.Add(category);
            //}




            //  This method will be called after migrating to the latest version.

            //  You can use the DbSet<T>.AddOrUpdate() helper extension method 
            //  to avoid creating duplicate seed data. E.g.
            //
            //    context.People.AddOrUpdate(
            //      p => p.FullName,
            //      new Person { FullName = "Andrew Peters" },
            //      new Person { FullName = "Brice Lambson" },
            //      new Person { FullName = "Rowan Miller" }
            //    );
            //
        