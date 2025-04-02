using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using LabProject.Models;

namespace LabProject.Pages;

public class IndexModel : PageModel
{
    private readonly ILogger<IndexModel> _logger;
    public static List<ClassInformationModel> Classes = new List<ClassInformationModel>();
    public List<ClassInformationTable> FilteredClasses { get; set; } = new List<ClassInformationTable>();
    [BindProperty(SupportsGet = true)]
    public string? SearchTerm { get; set; } 
    [BindProperty(SupportsGet = true)]
    public int PageNumber { get; set; } = 1;
    private const int PageSize = 10;
    public IndexModel(ILogger<IndexModel> logger)
    {
        _logger = logger;
        if (!Classes.Any())
        {
            //GenerateSampleData();
        }
    }

    [BindProperty]
    public ClassInformationModel classInformationModel { get; set; }

    public void OnGet()
    {    
        if (classInformationModel == null)
        {
            classInformationModel = new ClassInformationModel();
        }
        var filteredData=Classes.AsQueryable();
        if (!string.IsNullOrEmpty(SearchTerm))
        {
            filteredData = filteredData.Where(c => c.ClassName.Contains(SearchTerm, StringComparison.OrdinalIgnoreCase));
        }
        var totalRecords=filteredData.Count();
        var paginatedData=filteredData.Skip((PageNumber-1)*PageSize).Take(PageSize).ToList();

        FilteredClasses=paginatedData.Select(x=>new ClassInformationTable
        {
            ClassName=x.ClassName,
            StudentCount=x.StudentCount,
            Description=x.Description,
            Id=x.Id
        }).ToList();
    }

    public IActionResult OnPost()
    {
        if (ModelState.IsValid)
        {
            int newId = Classes.Count > 0 ? Classes.Max(c => c.Id) + 1 : 1;

            classInformationModel.Id = newId;
            Classes.Add(classInformationModel);

            classInformationModel = new ClassInformationModel();

            return RedirectToPage();
        }

        return Page();
    }

    public IActionResult OnPostDelete(int id)
    {
        var classToDelete = Classes.FirstOrDefault(c => c.Id == id);
        if (classToDelete != null)
        {
            Classes.Remove(classToDelete);
        }
        return RedirectToPage();
    }

    public IActionResult OnPostEdit(int id)
    {
        var classToEdit = Classes.FirstOrDefault(c => c.Id == id);
        if (classToEdit != null)
        {
            classInformationModel = new ClassInformationModel
            {
                Id = classToEdit.Id,
                ClassName = classToEdit.ClassName,
                StudentCount = classToEdit.StudentCount,
                Description = classToEdit.Description
            };
        }
        return Page();
    }

    public IActionResult OnPostSaveEdit()
    {
        var classToEdit = Classes.FirstOrDefault(c => c.Id == classInformationModel.Id);
        if (classToEdit != null)
        {
            classToEdit.ClassName = classInformationModel.ClassName;
            classToEdit.StudentCount = classInformationModel.StudentCount;
            classToEdit.Description = classInformationModel.Description;
        }
        return RedirectToPage();
    }

    private void GenerateSampleData()
    {
        
        for (int i = 1; i <= 100; i++)
        {
           Classes.Add(new ClassInformationModel
           {
                Id=i,
                ClassName="Class "+i,
                StudentCount = new Random().Next(10, 50),
                Description = "Description for Class " + i
           });
        }
    }

}
