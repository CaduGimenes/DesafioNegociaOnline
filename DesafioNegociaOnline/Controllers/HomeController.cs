using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using DesafioNegociaOnline.Models;
using RestSharp;
using Newtonsoft.Json;

namespace DesafioNegociaOnline.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;

        private db_selecao_imdbContext Database = new db_selecao_imdbContext();
        public static Address Address { get; set; }

        private static string _errorMessage;
        public static string ErrorMessage
        {
            get => _errorMessage;
            set
            {
                if (value != null)
                {
                    Origin = null;
                }

                _errorMessage = value;
            }
        }

        public static string Origin { get; set; }

        public HomeController(ILogger<HomeController> logger)
        {
            _logger = logger;
        }

        public IActionResult Index()
        {
            ViewData["Origin"] = Origin;
            ViewData["ErrorMessage"] = ErrorMessage;
            return View(Address);
        }

        [HttpPost]
        public IActionResult QueryWs(string cep)
        {
            try
            {
                if (!ValidateCEP(cep))
                {
                    ErrorMessage = "CEP Invalido.";
                    return RedirectToAction("Index");
                }

                Address = GetAddress(cep);

                if (Address == null)
                {
                    ErrorMessage = "CEP inválido!";
                    return RedirectToAction("Index");
                }

                Origin = "WS";
                ErrorMessage = null;

            }
            catch
            {
                ErrorMessage = "Ocorreu um erro ao consultar o CEP! Verifique o campo CEP.";

            }

            return RedirectToAction("Index");
        }

        [HttpPost]
        public IActionResult InsertDb(string cep)
        {
            try
            {
                if (!ValidateCEP(cep))
                {
                    ErrorMessage = "CEP Invalido.";

                    return RedirectToAction("Index");
                }

                Address = GetAddress(cep);

                Address.Cep = Address.Cep.Replace("-", "");

                if (Database.Address.Where(x => x.Cep == cep).Count() != 0)
                {
                    ErrorMessage = "O CEP já existe!";

                    return RedirectToAction("Index");
                }

                Database.Address.Add(Address);

                Database.SaveChanges();

                Address = null;
                ErrorMessage = null;
                Origin = null;
            }
            catch
            {
                ErrorMessage = "Ocorreu um erro ao salvar os dados! Verifique o campo CEP.";

            }

            return RedirectToAction("Index");

        }

        [HttpPost]
        public IActionResult QueryDb(string cep)
        {
            try
            {
                if (!ValidateCEP(cep))
                {
                    ErrorMessage = "CEP Invalido.";

                    return RedirectToAction("Index");
                }

                Address = Database.Address.Where(x => x.Cep == cep).FirstOrDefault();

                if (Address == null)
                {
                    ErrorMessage = "CEP não encontado na base de dados! Tente consultar com WS.";

                    return RedirectToAction("Index");
                }

                Origin = "DB";
                ErrorMessage = null;

            }
            catch
            {
                ErrorMessage = "Ocorreu um erro ao consultar os dados! Verifique o campo CEP.";

            }

            return RedirectToAction("Index");
        }

        private Address GetAddress(string cep)
        {
            try
            {

                RestClient client = new RestClient($"https://viacep.com.br/ws/{cep}/json/");

                var request = new RestRequest(Method.GET);
                IRestResponse response = client.Execute(request);

                return JsonConvert.DeserializeObject<Address>(response.Content);
            }
            catch
            {
                throw new Exception();
            }
        }

        private bool ValidateCEP(string cep)
        {
            return System.Text.RegularExpressions.Regex.IsMatch(cep, ("[0-9]{5}[0-9]{3}"));
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
