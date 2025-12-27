using Bogus;
using Microsoft.EntityFrameworkCore;
using Shop.Models;

namespace Shop
{
    public class AppDbContext : DbContext
    {
        public DbSet<Product> Products { get; set; }
        public DbSet<ProductWeight> ProductWeights { get; set; }
        public DbSet<ProductComposition> ProductCompositions { get; set; }
        public DbSet<ProductStorageCondition> ProductStorageConditions { get; set; }
        public DbSet<SaleHistory> SalesHistory { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlite("Data Source=App.db");
        }

        public void SeedDataIfEmpty()
        {
            if (!Products.Any())
            {
                var faker = new Faker("ru");

                var products = new List<Product>();
                var simpleProductNames = new[]
                {
                    "огурцы", "помидоры", "картофель", "морковь", "капуста", "перец", "лук", "чеснок",
                    "кабачки", "баклажаны", "свекла", "тыква", "яблоки", "груши", "бананы", "апельсины", "лимоны"
                };
                for (int i = 0; i < 10; i++)
                {
                    var product = new Product
                    {
                        Name = faker.Random.ArrayElement(simpleProductNames),
                        Quantity = faker.Random.Int(10, 100),
                        Price = Math.Round(faker.Random.Decimal(1, 50), 2)
                    };
                    products.Add(product);
                }
                Products.AddRange(products);
                SaveChanges();

                foreach (var product in products)
                {
                    ProductWeights.Add(new ProductWeight { ProductId = product.Id, Weight = Math.Round(faker.Random.Decimal(1, 5), 2) });
                    ProductCompositions.Add(new ProductComposition { ProductId = product.Id, Composition = faker.Lorem.Sentence() });
                    ProductStorageConditions.Add(new ProductStorageCondition { ProductId = product.Id, Conditions = faker.Lorem.Sentence() });
                }
                SaveChanges();
            }
        }
    }
}
