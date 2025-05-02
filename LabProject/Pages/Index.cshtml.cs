using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using LabProject.Models;
using LabProject.Data;
using Microsoft.EntityFrameworkCore;

namespace LabProject.Pages;

public class IndexModel : PageModel
{
    private readonly SchoolDbContext _context;
    private readonly ILogger<IndexModel> _logger;
    [BindProperty(SupportsGet = true)]
    public string? SearchTerm { get; set; }
    [BindProperty(SupportsGet = true)]
    public int PageNumber { get; set; } = 1;
    private const int PageSize = 10;
    public IndexModel(ILogger<IndexModel> logger, SchoolDbContext context)
    {
        _logger = logger;
        if (!ClassList.Any())
        {
            //GenerateSampleData();
        }
        _context = context;
    }
    [BindProperty]
    public Class EditableClass { get; set; }
    public List<Class> ClassList { get; set; } = new List<Class>();
    [BindProperty]
    public Class NewClass { get; set; } = new();
    public async Task OnGetAsync()
    {
        ClassList = await _context.Classes1.ToListAsync();

        if (NewClass == null)
        {
            NewClass = new Class();
        }
        var filteredData = ClassList.AsQueryable();
        if (!string.IsNullOrEmpty(SearchTerm))
        {
            filteredData = filteredData.Where(c => c.Name.Contains(SearchTerm, StringComparison.OrdinalIgnoreCase));
        }
        var totalRecords = filteredData.Count();
        var paginatedData = filteredData.Skip((PageNumber - 1) * PageSize).Take(PageSize).ToList();

        ClassList = paginatedData.Select(x => new Class
        {
            Name = x.Name,
            PersonCount = x.PersonCount,
            Description = x.Description,
            Id = x.Id,
            IsActive = x.IsActive
        }).ToList();

        var sessionUsername = HttpContext.Session.GetString("username");
        var sessionToken = HttpContext.Session.GetString("token");
        var sessionId = HttpContext.Session.GetString("session_id");

        var cookieUsername = Request.Cookies["username"];
        var cookieToken = Request.Cookies["token"];
        var cookieSessionId = Request.Cookies["session_id"];

        bool isValidLogin = !string.IsNullOrEmpty(sessionUsername) &&
                            !string.IsNullOrEmpty(sessionToken) &&
                            sessionUsername == cookieUsername &&
                            sessionToken == cookieToken &&
                            HttpContext.Session.Id == cookieSessionId;

        if (!isValidLogin)
        {
            TempData["Error"] = "Username or password is incorrect.";
            RedirectToPage("/Login");
        }
    }


    public async Task<IActionResult> OnPostAddAsync()
    {

        if (!ModelState.IsValid)
        {
            Console.WriteLine("ModelState geçersiz!");

            foreach (var entry in ModelState)
            {
                foreach (var error in entry.Value.Errors)
                {
                    Console.WriteLine($"Alan: {entry.Key}, Hata: {error.ErrorMessage}");
                }
            }
            ClassList = await _context.Classes1.ToListAsync();
            return Page();
        }
        try
        {
            _context.Classes1.Add(NewClass);
            await _context.SaveChangesAsync();
            System.Diagnostics.Debug.WriteLine("Kayıt başarılı!");
            return RedirectToPage();
        }
        catch (System.Exception ex)
        {
            System.Diagnostics.Debug.WriteLine("HATA: " + ex.Message);
            ModelState.AddModelError(string.Empty, "Hata oluştu: " + ex.Message);

            // Listeyi tekrar yükle, aksi takdirde sayfa boş kalabilir
            ClassList = await _context.Classes1.ToListAsync();
            return Page(); // Hatalı haliyle tekrar sayfayı döndür
        }


    }

    public async Task<IActionResult> OnPostDeleteAsync(int id)
    {
        var classToDelete = await _context.Classes1.FindAsync(id);
        if (classToDelete != null)
        {
            _context.Classes1.Remove(classToDelete);
            await _context.SaveChangesAsync();
        }
        return RedirectToPage();
    }

    public async Task<IActionResult> OnPostEditAsync(int id)
    {
        EditableClass = await _context.Classes1.FindAsync(id);
        ClassList = await _context.Classes1.ToListAsync(); // listeyi yeniden yükle
        return Page();
    }

    public async Task<IActionResult> OnPostSaveEditAsync()
    {
        var classToEdit = await _context.Classes1.FindAsync(EditableClass.Id);
        if (classToEdit != null)
        {
            classToEdit.Name = EditableClass.Name;
            classToEdit.PersonCount = EditableClass.PersonCount;
            classToEdit.Description = EditableClass.Description;
            classToEdit.IsActive = EditableClass.IsActive;

            await _context.SaveChangesAsync();
        }
        return RedirectToPage();
    }

public async Task<IActionResult> OnPostToggleActiveAsync(int id)
{
    var classToToggle = await _context.Classes1.FindAsync(id);
    if (classToToggle != null)
    {
        classToToggle.IsActive = !classToToggle.IsActive;
        await _context.SaveChangesAsync();
    }

    return RedirectToPage();
}

    private void GenerateSampleData()
    {

        // for (int i = 1; i <= 100; i++)
        // {
        //     Classes.Add(new ClassInformationModel
        //     {
        //         Id = i,
        //         ClassName = "Class " + i,
        //         StudentCount = new Random().Next(10, 50),
        //         Description = "Description for Class " + i
        //     });
        // }
    }

}
