using AutoMapper;
using IntroSEProject.API.Models;
using IntroSEProject.API.Services;
using IntroSEProject.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace IntroSEProject.API.Controllers
{
    [ApiController]
    [Route("/api/[controller]")]
    public class CategoriesController : Controller
    {
        private AppDbContext dbContext;
        private IMapper mapper;

        public CategoriesController(AppDbContext dbContext, IMapper mapper)
        {
            this.dbContext = dbContext;
            this.mapper = mapper;
        }
        [HttpGet]
        public async Task<IActionResult> GetPaging(int page = 1, int per_page = 0, string keyword = "")
        {
            
            IEnumerable<Category> list;
            if (string.IsNullOrEmpty(keyword))
            {
                list = await dbContext.Categories.ToListAsync();
            }
            else
            {
                list = await dbContext.Categories.Where(x => x.Name.Contains(keyword)).ToListAsync();
            }
            if (per_page == 0)
            {
                per_page = list.Count();
            }
            var model = mapper.Map<IEnumerable<CategoryModel>>(list);
            var pager = new Pager<CategoryModel>(model, page, per_page);
            return Ok(pager);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var item = await dbContext.Categories.FindAsync(id);
            if (item == null)
            {
                return NotFound();
            }
            var model = mapper.Map<CategoryModel>(item);
            return Ok(model);
        }

        [Authorize(Roles = "Admin")]
        [HttpPost]
        public async Task<IActionResult> Create(CategoryModel model)
        {
            var category = mapper.Map<Category>(model);
            try
            {
                await dbContext.Categories.AddAsync(category);
                await dbContext.SaveChangesAsync();
            }
            catch (Exception)
            {
                return BadRequest();
            }
            model.Id = category.Id;
            return Ok(model);
        }

        [Authorize(Roles = "Admin")]
        [HttpPut("{id}")]
        public async Task<IActionResult> Edit([FromRoute] int id, [FromBody] CategoryModel model)
        {
            var category = await dbContext.Categories.FindAsync(id);
            if (category == null)
            {
                return NotFound();
            }
            model.Id = id;
            mapper.Map(model, category);
            try
            {
                await dbContext.SaveChangesAsync();
                return Ok(model);
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!dbContext.Categories.Any(x => x.Id == id))
                if (!dbContext.Categories.Any(e => e.Id == model.Id))
                {
                    return NotFound();
                }    
                throw;
            }
            
        }

        [Authorize(Roles = "Admin")]
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var category = await dbContext.Categories.FindAsync(id);
            if (category == null)
            {
                return NotFound();
            }
            var model = mapper.Map<CategoryModel>(category);
            dbContext.Categories.Remove(category);
            await dbContext.SaveChangesAsync();
            return Ok(model);
        }


        [HttpGet("{cid}/items")]
        public async Task<IActionResult> GetAllItemsBelongToCategory(int cid)
        {
            var category = await dbContext.Categories.FindAsync(cid);
            if (category == null) return NotFound();
            var items = await dbContext.Items.Where(x => x.CategoryId == category.Id).ToListAsync();
            return Ok(mapper.Map<IEnumerable<ItemModel>>(items));
        }

        
    }
}
