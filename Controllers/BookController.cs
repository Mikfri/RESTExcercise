using BookLib;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860
/*Denne kontroller blev skabt ved at højreklikke Controllers-mappen: Add> Controllers...> API(i venstre side..)> API Controller with Read/Write..  
De hyppigste brugte STATUS koder for REST https://www.restapitutorial.com/lessons/httpmethods.html*/

namespace RestExcercise1.Controllers
{

    //---: REST consumer (HTTP client) (REpresentational State Transfer) REST handler om Headers
    [Route("api/[controller]")]
    [ApiController]
    public class BookController : ControllerBase
    {

        BookRepository _bookRepository = new BookRepository();
        public BookController(BookRepository bookRepository)
        {
            _bookRepository = bookRepository;
        }

        // GET: api/<BookController>
        [HttpGet]
        //[EnableCors("MyAllowedOrigin")]
        public IEnumerable<Book> Get()
        {
            return _bookRepository.Get();
        }

        // GET: api/<BookController>
        /// <summary>
        /// Filtrere og sorter på vores forskellige properties.
        /// .. kan alternativt ændres til [HttpGet] ved at udkommentere vores anden GET metode.
        /// Bemærk vi da ikke længere vil have `filter` i URL'en ala: ../api/Book/filter?titleIncludes=for&orderBy
        /// </summary>
        /// <param name="maxPrice"></param>
        /// <param name="minPrice"></param>
        /// <param name="titleIncludes"></param>
        /// <param name="orderBy"></param>
        /// <returns></returns>
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [HttpGet("filter")]
        public ActionResult<IEnumerable<Book>> Get(
            [FromQuery] int? maxPrice,
            [FromQuery] int? minPrice,
            [FromQuery] string? titleIncludes,
            [FromQuery] string? orderBy)
        {
            IEnumerable<Book> filteredBooks = _bookRepository.Get(maxPrice, minPrice, titleIncludes, orderBy);

            if (filteredBooks == null || !filteredBooks.Any())
            {
                return NotFound("ERROR: CODE 404\nNo books match the criteria");
            }

            return Ok(filteredBooks);
        }

        // GET(int id): api/<BookController>
        [ProducesResponseType(StatusCodes.Status200OK)]         //Disse statuskoder har noget at gøre med beskeder fra SWAGGER
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        //[EnableCors("MyAllowedOrigin")]
        [HttpGet("{id}")]
        public ActionResult<Book> Get(int id)
        {
            Book book = _bookRepository.GetByID(id);
            if (book == null) return NotFound("ERROR: CODE 404\nID: " + id + " NOT FOUND!");
            return Ok(book);

            //return _bookRepository.GetByID(id);   //GammelSetup uden STATUS (RestExcersise1)
        }


        // POST: api/<BookController>
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status409Conflict)]
        //[EnableCors("MyAllowedOrigin")]
        [HttpPost]
        public ActionResult<Book> Post(Book value)
        {
            try
            {
                ActionResult<Book> request = _bookRepository.Add(value);

                if (request.Result is ConflictObjectResult conflictResult)
                {
                    return Conflict($"ERROR: {conflictResult.StatusCode}. {conflictResult.Value}\n");
                }
                if (request.Result is OkObjectResult okResult && okResult.Value is Book book)
                {
                    return CreatedAtAction(nameof(Get), new { id = book.Id }, book);   //Angiver handlingens navn som senere genere korrekt URL til ressourcen. 
                    //return Created($"/api/Book/{book.Id}", book);                     // Her skal vi sætte den direkte URL

                }
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }

            return BadRequest("Ukendt fejl ved oprettelse af bog.");
        }


        //[ProducesResponseType(StatusCodes.Status201Created)]
        //[ProducesResponseType(StatusCodes.Status400BadRequest)]
        //[HttpPost]
        //public ActionResult<Book> Post(Book value)
        //{
        //    try
        //    {
        //        Book newBook = _bookRepository.Add(value);
        //        return CreatedAtAction(nameof(Get), new { id = newBook.Id }, newBook);
        //    }
        //    catch (Exception ex)
        //    {
        //        return BadRequest($"ERROR CODE: 400\n{ex.Message}");
        //    }
        //}

        /* CODE 204: No Content
            Ressourcen er blevet opdateret korrekt. Imidlertid er der ikke behov for at sende en fuld repræsentation
            af den opdaterede ressource tilbage til klienten, da klienten allerede har den nødvendige information.
            "Bogen er nu opdateret! Men returneres bogen så du kan se dens ændringer? Nej = 204*/

        // PUT api/<BookController>/
        [ProducesResponseType(StatusCodes.Status200OK)] // eller 204?
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        //[EnableCors("MyAllowedOrigin")]
        [HttpPut("{id}")]
        public ActionResult<Book> Put(int id, Book value)
        {
            try
            {
                Book updatedBook = _bookRepository.Update(id, value);

                //if (updatedBook == null)
                //{
                //    return NotFound($"ERROR CODE: 404\nID: {id} NOT FOUND!");
                //}

                return Ok(updatedBook);
            }
            catch (Exception ex)
            {
                return BadRequest($"ERROR CODE: 400\n{ex.Message}");
            }
        }


        // DELETE api/<BookController>/
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        //[EnableCors("MyAllowedOrigin")]
        [HttpDelete("{id}")]
        public ActionResult<Book> Delete(int id)
        {
            try
            {
                Book deletedBook = _bookRepository.Remove(id);

                if (deletedBook == null)
                {
                    return NotFound($"ERROR CODE: 404\nID: {id} NOT FOUND!");
                }

                return Ok(deletedBook);
            }
            catch (Exception ex)
            {
                return BadRequest($"ERROR CODE: 400\n{ex.Message}");
            }
        }
    }
}
