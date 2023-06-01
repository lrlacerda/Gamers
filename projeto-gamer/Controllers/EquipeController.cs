using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Elfie.Serialization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using projeto_gamer.Infra;
using projeto_gamer.Models;

namespace projeto_gamer.Controllers
{
    [Route("[controller]")]
    public class EquipeController : Controller
    {
        private readonly ILogger<EquipeController> _logger;

        public EquipeController(ILogger<EquipeController> logger)
        {
            _logger = logger;
        }

        Context c = new Context(); // instancia de contexto para chamar o banco de dados

        [Route("Listar")] //http://localhost/Equipe/Listar
        public IActionResult Index()
        {
            //variável que armazena as equipes listadas do banco de dados
            ViewBag.Equipe = c.Equipe.ToList();

            //retorna a view de equipe (Tela)
            return View();
        }

        [Route("Cadastrar")]
        public IActionResult Cadastrar(IFormCollection form)
        {
            //instancia do objeto Equipe
            Equipe novaEquipe = new Equipe();

            //atribuição de valores recebidos do formulário
            novaEquipe.Nome = form["Nome"].ToString();

            // novaEquipe.Imagem = form["Imagem"].ToString();

            //início da logica do upload da imagem
            if (form.Files.Count > 0)
            {
                var file = form.Files[0];

                var folder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/img/Equipes");

                if (!Directory.Exists(folder))
                {
                    Directory.CreateDirectory(folder);
                }

                var path = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/img/", folder, file.FileName);

                using (var stream = new FileStream(path, FileMode.Create))
                {
                    file.CopyTo(stream);
                }

                novaEquipe.Imagem = file.FileName;
            }
            else
            {
                novaEquipe.Imagem = "padão.png";
            }

            //adiciona objeto na tela do BD
            c.Equipe.Add(novaEquipe);

            //salva as alterações feitas na BD
            c.SaveChanges();

            //retorna para o local chamando a rota de listar(método index)
            return LocalRedirect("~/Equipe/Listar");
        }

        [Route("Excluir/{id}")]
        public IActionResult Excluir(int id)
        {
            Equipe equipeBuscada = c.Equipe.First(e => e.IdEquipe == id);

            c.Remove(equipeBuscada);

            c.SaveChanges();

            return LocalRedirect("~/Equipe/Listar");
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View("Error!");
        }
    }
}