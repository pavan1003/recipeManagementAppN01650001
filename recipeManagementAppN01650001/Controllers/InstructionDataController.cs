using recipeManagementAppN01650001.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity.Infrastructure;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Web.Http.Description;

namespace recipeManagementAppN01650001.Controllers
{
    public class InstructionDataController : ApiController
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        /// <summary>
        /// Return a list of instructions
        /// </summary>
        /// <returns>An array of Instructions</returns>
        /// <example>
        /// GET: api/InstructionData/ListInstructions
        /// </example>
        [HttpGet]
        [Route("api/InstructionData/ListInstructions")]
        public List<InstructionDto> ListInstructions()
        {
            List<Instruction> Instructions = db.Instructions.ToList();
            List<InstructionDto> InstructionDtos = new List<InstructionDto>();

            Instructions.ForEach(a => InstructionDtos.Add(new InstructionDto()
            {
                InstructionId = a.InstructionId,
                StepNumber = a.StepNumber,
                Description = a.Description,
                RecipeId = a.RecipeId,
                RecipeTitle = a.Recipe.Title
            }));

            return InstructionDtos;
        }


        /// <summary>
        /// Finds a specific instruction by its ID
        /// </summary>
        /// <param name="id">The ID of the instruction</param>
        /// <returns>The instruction with the specified ID</returns>
        /// <example>
        /// GET: api/InstructionData/FindInstruction/5
        /// </example>
        [ResponseType(typeof(Instruction))]
        [HttpGet]
        [Route("api/InstructionData/FindInstruction/{id}")]
        public IHttpActionResult FindInstruction(int id)
        {
            Instruction instruction = db.Instructions.Find(id);
            if (instruction == null)
            {
                return NotFound();
            }

            InstructionDto instructionDto = new InstructionDto
            {
                InstructionId = instruction.InstructionId,
                StepNumber = instruction.StepNumber,
                Description = instruction.Description,
                RecipeId = instruction.RecipeId
            };

            return Ok(instructionDto);
        }

        /// <summary>
        /// Adds a new instruction
        /// </summary>
        /// <param name="instruction">The instruction to add</param>
        /// <returns>Confirmation of instruction creation</returns>
        /// <example>
        /// POST: api/InstructionData/AddInstruction
        /// BODY: { "StepNumber": 1, "Description": "Mix ingredients" }
        /// </example>
        [ResponseType(typeof(Instruction))]
        [HttpPost]
        [Route("api/InstructionData/AddInstruction")]
        [Authorize]
        public IHttpActionResult AddInstruction(Instruction instruction)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            db.Instructions.Add(instruction);
            db.SaveChanges();

            return Ok();
        }

        /// <summary>
        /// Deletes a specific instruction by its ID
        /// </summary>
        /// <param name="id">The ID of the instruction to delete</param>
        /// <returns>Confirmation of instruction deletion</returns>
        /// <example>
        /// POST: api/InstructionData/DeleteInstruction/5
        /// </example>
        [ResponseType(typeof(Instruction))]
        [HttpPost]
        [Route("api/InstructionData/DeleteInstruction/{id}")]
        [Authorize]
        public IHttpActionResult DeleteInstruction(int id)
        {
            Instruction instruction = db.Instructions.Find(id);
            if (instruction == null)
            {
                return NotFound();
            }

            db.Instructions.Remove(instruction);
            db.SaveChanges();

            return Ok();
        }

        /// <summary>
        /// Updates a specific instruction by its ID
        /// </summary>
        /// <param name="id">The ID of the instruction to update</param>
        /// <param name="instruction">The updated instruction data</param>
        /// <returns>No content if update is successful</returns>
        /// <example>
        /// POST: api/InstructionData/UpdateInstruction/5
        /// BODY: { "InstructionId": 5, "StepNumber": 1, "Description": "Mix ingredients thoroughly" }
        /// </example>
        [ResponseType(typeof(void))]
        [HttpPost]
        [Route("api/InstructionData/UpdateInstruction/{id}")]
        [Authorize]
        public IHttpActionResult UpdateInstruction(int id, Instruction instruction)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (id != instruction.InstructionId)
            {
                return BadRequest();
            }

            db.Entry(instruction).State = EntityState.Modified;

            try
            {
                db.SaveChanges();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!InstructionExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return StatusCode(HttpStatusCode.NoContent);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

        /// <summary>
        /// Checks if an instruction exists by its ID
        /// </summary>
        /// <param name="id">The ID of the instruction</param>
        /// <returns>true if the instruction exists; otherwise, false</returns>
        /// <example>
        /// bool exists = InstructionExists(5);
        /// </example>
        private bool InstructionExists(int id)
        {
            return db.Instructions.Count(e => e.InstructionId == id) > 0;
        }
    }
}
