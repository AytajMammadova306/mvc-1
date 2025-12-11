using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.VisualStudio.Web.CodeGenerators.Mvc.Templates.Blazor;
using Pronia_self.DAL;
using Pronia_self.Models;
using Pronia_self.ViewModels;
using System.Threading.Tasks;
namespace Pronia_self.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize]

    public class TagController : Controller
    {
        private readonly AppDbContext _context;

        public TagController(AppDbContext context)
        {
            _context=context;
        }
        public async Task<IActionResult> Index()
        {
            List<GetTagVM> getTagVMs = await _context.Tags.Select(t => new GetTagVM
            {
                Id = t.Id,
                TagName = t.Name,
                ProductCount=t.ProductTags.Count,
            }).ToListAsync();
            return View(getTagVMs);
        }
        public IActionResult Create()
        {
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> Create(CreateTagVM createTagVM)
        {
            if (!ModelState.IsValid)
            {
                return View(createTagVM);
            }
            if (_context.Tags.Any(t => t.Name.ToLower() == createTagVM.TagName.ToLower()))
            {
                ModelState.AddModelError(nameof(CreateTagVM.TagName), "This tag already exists");
                return View(createTagVM);
            }
            _context.Tags.Add(new Tag
            {
                CreatedAt = DateTime.Now,
                Name = createTagVM.TagName,
                ProductTags = new()
            });
            _context.SaveChanges();
            return RedirectToAction(nameof(Index));
        }
        public async Task<IActionResult> Update(int? id)
        {
            if (id is null || id < 1)
            {
                return BadRequest();
            }
            Tag existed = await _context.Tags.FirstOrDefaultAsync(t=>t.Id==id);
            if (existed is null)
            {
                return NotFound();
            }
            UpdateTagVM tagVM = new UpdateTagVM
            {
                Name = existed.Name,
            };
            return View(tagVM);
        }
        [HttpPost]
        public async Task<IActionResult> Update(int? id, UpdateTagVM tagVM)
        {
            if (!ModelState.IsValid)
            {
                return View(tagVM);
            }
            bool result = await _context.Categories.AnyAsync(c => c.Name == tagVM.Name && c.Id != id);
            if (result)
            {
                ModelState.AddModelError(nameof(UpdateTagVM.Name), $"{tagVM.Name} Tag already exists");
                return View(tagVM);
            }
            Tag? existed = await _context.Tags.FirstOrDefaultAsync(c => c.Id == id);
            if (existed is null)
            {
                return NotFound();
            }
            existed.Name = tagVM.Name;
            _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }
    }
}
