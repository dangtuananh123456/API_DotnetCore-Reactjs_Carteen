using Microsoft.EntityFrameworkCore;
using Bogus;

namespace IntroSEProject.Models
{

    public class AppDbContext : DbContext
    {

        public AppDbContext(DbContextOptions<AppDbContext> options)
            : base(options)
        {
             
        }
        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            builder.Entity<OrderItem>().HasOne<Item>(x => x.Item)
                .WithMany(x => x.OrderItems)
                .HasForeignKey(x => x.ItemId);
            builder.Entity<OrderItem>().HasOne<Order>(x => x.Order)
                .WithMany(x => x.OrderItems)
                .HasForeignKey(x => x.OrderId);

            //seed data here
            //seed data for user table
            Randomizer.Seed = new Random(8675390);
            var Gender = new[] { "Male", "Female" };
            var userId = 1;
            var userDataFaker = new Faker<User>()
                //.StrictMode(true)
                .RuleFor(u => u.Id, f => userId++)
                .RuleFor(u => u.Gender, f =>
                {
                    var index = f.Random.Number(0, 1);
                    return Gender[index];
                })
                .RuleFor(u => u.FullName, (f, u) => f.Name.FirstName() + " " + f.Name.LastName())
                .RuleFor(u => u.Email, (f, u) => f.Internet.Email(u.FullName))
                .RuleFor(u => u.Avatar, (f, u) => f.Internet.Avatar())
                .RuleFor(u => u.DateOfBirth, f => f.Date.Past())
                .RuleFor(u => u.PhoneNumber, f => f.Phone.PhoneNumber())
                .RuleFor(u => u.Password, f => f.Internet.Password())
                .FinishWith((f, u) => System.Console.WriteLine("User was created successfully with id = ", u.Id));


            builder.Entity<User>()
                .HasData(userDataFaker.GenerateBetween(10, 10));

            //seed data category
            var categoryId = 1;
            var dataCategoryFaker = new Faker<Category>()
                .RuleFor(c => c.Id, f => categoryId++)
                .RuleFor(c => c.Thumbnail, f => f.Image.PicsumUrl())
                .RuleFor(c => c.Name, f => f.Lorem.Sentence(3))
                .RuleFor(c => c.Description, f => f.Lorem.Paragraph(1))
                .FinishWith((f, c) => System.Console.WriteLine("Successfully"));
            builder.Entity<Category>()
                .HasData(dataCategoryFaker.GenerateBetween(10, 10));

            //seed data item table 
            var itemId = 1;
            var dataItemFaker = new Faker<Item>()
                .RuleFor(i => i.Id, f => itemId++)
                .RuleFor(i => i.Thumbnail, f => f.Image.PicsumUrl())
                .RuleFor(i => i.Name, f => f.Lorem.Sentence(3))
                .RuleFor(i => i.Description, f => f.Lorem.Sentence(20))
                .RuleFor(i => i.Price, f => f.Random.Decimal() + 10)
                .RuleFor(i => i.Discount, f => f.Random.Decimal() + 10)
                .RuleFor(i => i.Stock, f => f.Random.Int(1, 10))
                .RuleFor(i => i.CategoryId, f => f.Random.Int(1, 10))
                .FinishWith((f, i) => System.Console.WriteLine("Item was create success fully"));
            builder.Entity<Item>()
                .HasData(dataItemFaker.GenerateBetween(100, 100));
            

            //seed data for 
        }

        public DbSet<Item> Items { set; get; }
        public DbSet<CartItem> CartItems { set; get; }
        public DbSet<Category> Categories { set; get; }  
        public DbSet<Order> Orders { set; get; }
        public DbSet<OrderItem> OrderItems { set; get; }
        public DbSet<Review> Reviews { set; get; }
        public DbSet<User> Users { set; get; }
        public DbSet<Payment> Payments { set; get; }
        public DbSet<SeatReservation> SeatReservations { set; get; }
    }
}