﻿using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.IdentityModel.Tokens.Jwt;
using WebAPI.Domains;
using WebAPI.Interfaces;
using WebAPI.Repositories;
using WebAPI.Utils.BlobStorage;
using WebAPI.Utils.Mail;
using WebAPI.ViewModels;

namespace WebAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PacientesController : ControllerBase
    {
        private IPacienteRepository pacienteRepository { get; set; }

        private readonly EmailSedingService _emailSedingService;

        public PacientesController(EmailSedingService emailSedingService)
        {
            pacienteRepository = new PacienteRepository();
            _emailSedingService = emailSedingService;
        }

        [HttpGet("PerfilLogado")]
        public IActionResult GetLogged()
        {
            try
            {
                Guid idUsuario = Guid.Parse(HttpContext.User.Claims.First(c => c.Type == JwtRegisteredClaimNames.Jti).Value);

                return Ok(pacienteRepository.BuscarPorId(idUsuario));

            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        //[Authorize]
        [HttpGet("BuscarPorId")]
        public IActionResult BuscarPorId(Guid id)
        {
            return Ok(pacienteRepository.BuscarPorId(id));
        }

        //[HttpPost]
        //public async Task<IActionResult> Post(PacienteViewModel pacienteModel)
        //{
        //    Usuario user = new Usuario();

        //    user.Nome = pacienteModel.Nome;
        //    user.Email = pacienteModel.Email;
        //    user.TipoUsuarioId = pacienteModel.IdTipoUsuario;
        //    user.Foto = pacienteModel.Foto;
        //    user.Senha = pacienteModel.Senha;

        //    user.Paciente = new Paciente();

        //    user.Paciente.DataNascimento = pacienteModel.DataNascimento;
        //    user.Paciente.Rg = pacienteModel.Rg;
        //    user.Paciente.Cpf = pacienteModel.Cpf;

        //    user.Paciente.Endereco = new Endereco();

        //    user.Paciente.Endereco.Logradouro = pacienteModel.Logradouro;
        //    user.Paciente.Endereco.Numero = pacienteModel.Numero;
        //    user.Paciente.Endereco.Cep = pacienteModel.Cep;
        //    user.Paciente.Endereco.Cidade = pacienteModel.Cidade;

        //    pacienteRepository.Cadastrar(user);

        //    await _emailSedingService.SendWelcomeEmail(user.Email!, user.Nome!);

        //    return Ok();
        //}


        [HttpPost] //esse "[FromForm] muda no swagger para formulario, a forma de preenchimento dos dados para o cadastro do paciente"
        public async Task<IActionResult> Post([FromForm] PacienteViewModel pacienteModel)
        {
            try
            {
                // obj a ser cadastrado / preenchido
                Usuario user = new Usuario();

                // recebe os valores e preenche as propriedades que eu quero
                user.Nome = pacienteModel.Nome;
                user.Email = pacienteModel.Email;
                user.TipoUsuarioId = pacienteModel.IdTipoUsuario;

                // define o nome do container do meu blob
                var containerName = "images-vitalhub";

                // define a string de conecao
                var connectionString = "DefaultEndpointsProtocol=https;AccountName=vitahubg10;AccountKey=0f4F0m+JXwms9KPYeHtqmQ1m5ggzPOieFXDt7VfWqcUV1qDiviHjtesLPYi0jBJ7xDOiORGa4rHp+ASto9sJxw==;EndpointSuffix=core.windows.net";

                // chamar o metodo para upload de imagem
                user.Foto = await AzureBlobStorageHelper.UploadImageBlobAsync(pacienteModel.Arquivo!, connectionString, containerName);


                user.Senha = pacienteModel.Senha;


                user.Paciente = new Paciente();
                user.Paciente.DataNascimento = pacienteModel.DataNascimento;
                user.Paciente.Rg = pacienteModel.Rg;
                user.Paciente.Cpf = pacienteModel.Cpf;

                user.Paciente.Endereco = new Endereco();
                user.Paciente.Endereco.Logradouro = pacienteModel.Logradouro;
                user.Paciente.Endereco.Numero = pacienteModel.Numero;
                user.Paciente.Endereco.Cep = pacienteModel.Cep;
                user.Paciente.Endereco.Cidade = pacienteModel.Cidade;

                pacienteRepository.Cadastrar(user);

                await _emailSedingService.SendWelcomeEmail(user.Email!, user.Nome!);

                return Ok(user);
            }
            catch (Exception ex)
            {

                return BadRequest(ex.Message);
            }
           
        }

        [HttpGet("BuscarPorData")]
        public IActionResult GetByDate(DateTime data, Guid id)
        {
            try
            {
                return Ok(pacienteRepository.BuscarPorData(data, id));
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPut]
        public IActionResult UpdateProfile(Guid idUsuario, PacienteViewModel paciente)
        {
            try
            {
                return Ok(pacienteRepository.AtualizarPerfil(idUsuario, paciente));
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }
}
