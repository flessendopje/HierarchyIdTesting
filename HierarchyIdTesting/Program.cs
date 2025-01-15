using Microsoft.EntityFrameworkCore;


using (var context = new AppDbContext())
{
    
    context.Categories.AddRange([
        new Category { Name = "Parent 1", ParentId = null, Hierarchy = HierarchyId.Parse("/1/") },
        new Category { Name = "Parent 2", ParentId = null, Hierarchy = HierarchyId.Parse("/2/") },
        new Category { Name = "P1 C1", ParentId = null, Hierarchy = HierarchyId.Parse("/1/1/") },
        new Category { Name = "P1 C2", ParentId = null, Hierarchy = HierarchyId.Parse("/1/2/") },
        new Category { Name = "P2 C1", ParentId = null, Hierarchy = HierarchyId.Parse("/2/1/") },
        new Category { Name = "P2 C1 GC1", ParentId = null, Hierarchy = HierarchyId.Parse("/2/1/1/") },
        new Category { Name = "P2 C1 GC2", ParentId = null, Hierarchy = HierarchyId.Parse("/1/1/2/") },
    ]);

    context.SaveChanges();
    Console.WriteLine("New person added!");

    var categoriesTree = await context.Categories
        .Where(c => new HierarchyId("/1/1/2/").IsDescendantOf(c.Hierarchy))
        .OrderBy(c => c.Hierarchy)
        .ToListAsync();

    foreach(var cat in categoriesTree)
    {
        Console.WriteLine(cat.Hierarchy);
    }

    var categories = context.Categories;
    Console.WriteLine("List of people in the database:");
    foreach (var category in categories)
    {
        Console.WriteLine($"Id: {category.Id}, parentId: {category.ParentId} Name: {category.Name}, HierarchyNode: {category.Hierarchy}");
    }
}

public class AppDbContext : DbContext
{
    string connectionString = "Data Source=DESKTOP-COCRRIA;Initial Catalog=Testing;Integrated Security=True;Trust Server Certificate=True";
    public DbSet<Category> Categories { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Category>(entity =>
        {
            entity.HasKey(p => p.Id);
        });
        base.OnModelCreating(modelBuilder);
    }
    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        optionsBuilder.UseSqlServer(connectionString, x => x.UseHierarchyId());
    }
}

public class Category
{
    public int Id { get; set; }
    public int? ParentId { get; set; }
    public string Name { get; set; }
    public HierarchyId? Hierarchy { get; set; }
}

//CREATE TABLE categories (
//id INT IDENTITY(1,1) PRIMARY KEY,
//parentId INT NULL,
//name NVARCHAR(255) NOT NULL,
//Hierarchy HierarchyId null,
//);