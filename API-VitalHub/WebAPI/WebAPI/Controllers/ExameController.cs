using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using WebAPI.Domains;
using WebAPI.Interfaces;
using WebAPI.Utils.OCR;
using WebAPI.ViewModels;

namespace WebAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ExameController : ControllerBase
    {
        private readonly IExameRepository _exameRepository;
        private readonly OcrService _ocrService;
        public ExameController(IExameRepository exameRepository, OcrService ocrService)
        {
            _exameRepository = exameRepository;
            _ocrService = ocrService;
        }

        [HttpPost]
        public async Task<IActionResult> Post([FromForm] ExameViewModel exameViewModel)
        {
            try
            {
                if (exameViewModel.Image == null || exameViewModel == null)
                {
                    return BadRequest("Nenhuma imagem encontrada!");
                }

                using (var stream = exameViewModel.Image.OpenReadStream())
                {
                    var result = await _ocrService.RecognizeTextAsync(stream);

                    exameViewModel.Descricao = result;

                    Exame exame = new Exame();
                    exame.Descricao = exameViewModel.Descricao;
                    exame.ConsultaId = exameViewModel.ConsultaId;

                    _exameRepository.Cadastrar(exame);

                    return Ok(exame);
                }

            }
            catch (Exception e)
            {

                return BadRequest(e.Message);
            }
        }

        [HttpGet("BuscarPorIdConsulta")]
        public IActionResult GetByIdConsult(Guid idConsult) {
            try
            {
                List<Exame> Lista = _exameRepository.BuscarPorIdConsulta(idConsult);

                return Ok(Lista);
            }
            catch (Exception e)
            {

                return BadRequest(e.Message);
            }
        }
    }
}