﻿using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Shop.Data;
using Shop.Model;

namespace Shop.Controllers
{
    [Route("v1/categories")]
    public class CategoryController:ControllerBase
    {
        [HttpGet]
        [Route("")]
        [AllowAnonymous]
        [ResponseCache(VaryByHeader = "User-Agent", Location = ResponseCacheLocation.Any, Duration = 30)]
        public async Task<ActionResult<List<Category>>> Get([FromServices] DataContext context)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);
            
            var categories = await context.Categories.AsNoTracking().ToListAsync();
            return Ok(categories);
            
        }
        [HttpGet]
        [Route("{id:int}")]
        [AllowAnonymous]    
        public async Task<ActionResult<Category>> GetId(int id,[FromServices] DataContext context)
        {
            var categpry = await context.Categories.AsNoTracking().FirstOrDefaultAsync(x=>x.Id==id);
            return Ok(categpry);
        }
        [HttpPost]
        [Route("")]
        public async Task<ActionResult<Category>> Post([FromServices] DataContext context, [FromBody] Category model)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);
            try
            {
                context.Categories.Add(model);
                await context.SaveChangesAsync();
                
                return Ok(model);
            }
            catch (Exception)
            {

                return StatusCode(500, "Erro ao salvar uma categoria");
            }

        }

        [HttpPut]
        [Route("{id:int}")]
        [Authorize(Roles ="employee")]
        public async Task<ActionResult<Category>> Put(int id, [FromBody] Category model,[FromServices] DataContext context)
        {
            if (id != model.Id)
                return NotFound(new { Message = "Ops Categoria não encontrada" });

            if (!ModelState.IsValid)
                return BadRequest(ModelState);
            try
            {
                context.Entry<Category>(model).State = EntityState.Modified;
                await context.SaveChangesAsync();
                return Ok(model);
            }
            catch (DbUpdateConcurrencyException)
            {

                return BadRequest(new {Message = "Não foi possivel atualizar a categoria, por favor tente novamente"});
            }
            catch (Exception)
            {

                return BadRequest(new { Message = "Não foi possivel atualizar a categoria, por favor tente novamente" });
            }

        }
        
        [HttpDelete]
        [Route("{id:int}")]
        [Authorize(Roles = "employee")]
        public async Task<ActionResult<List<Category>>> Delete(int id,[FromServices] DataContext context)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var receive = await context.Categories.FirstOrDefaultAsync(x => x.Id == id);
            if (receive==null)
            {
                return StatusCode(500, "Categoria não encontrada");
            }
            try
            {
                context.Remove(receive);
                await context.SaveChangesAsync();
                return StatusCode(200, "Categoria deletada com sucesso");
            }
            catch (Exception)
            {

                return StatusCode(500, "Não foi posivel excluir uma categoria");
            }

                
    
        }
    }
}
