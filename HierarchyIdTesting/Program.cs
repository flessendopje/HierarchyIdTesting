using Microsoft.EntityFrameworkCore;


using (var context = new AppDbContext())
{
    //execute this once then comment or remove.
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

    var parentId = new HierarchyId("/1/");
    Console.WriteLine($"Parent hierarchy: {parentId}");
    Console.WriteLine();
    //get tree to root
    Console.WriteLine("Hierarchy to top from parent (get tree to the top from current category):");
    var categoriesTree = await context.Categories
        .Where(c => parentId.IsDescendantOf(c.Hierarchy))
        .OrderBy(c => c.Hierarchy)
    .ToListAsync();
    foreach (var cat in categoriesTree)
    {
        Console.WriteLine(cat.Hierarchy);
    }

    Console.WriteLine();
    //get tree down from here
    Console.WriteLine("Hierarchy to bottom from parent (get all categories below the current one):");
    var childCategories = await context.Categories
        .Where(c => c.Hierarchy.IsDescendantOf(parentId) && c.Hierarchy != parentId)
        .OrderBy(c => c.Hierarchy)
        .ToListAsync();

    foreach(var cat in childCategories)
    {
        Console.WriteLine(cat.Hierarchy);
    }



    var categories = context.Categories;
    Console.WriteLine();
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