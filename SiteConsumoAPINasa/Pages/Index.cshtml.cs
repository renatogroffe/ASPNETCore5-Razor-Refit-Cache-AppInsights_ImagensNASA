using System;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using SiteConsumoAPINasa.HttpClients;
using SiteConsumoAPINasa.Models;

namespace SiteConsumoAPINasa.Pages
{
    public class IndexModel : PageModel
    {
        private readonly ILogger<IndexModel> _logger;
        private readonly IConfiguration _configuration;
        private readonly IMemoryCache _memoryCache;
        private readonly IImagemDiariaAPI _apiImagemDiaria;
        public string Saudacao { get; set; }
        public DateTime DataConsulta { get; set; }
        public InfoImagemNASA ImagemDiariaNASA { get; set; }

        public IndexModel(ILogger<IndexModel> logger,
            IConfiguration configuration,
            IMemoryCache memoryCache,
            IImagemDiariaAPI apiImagemDiaria)
        {
            _logger = logger;
            _configuration = configuration;
            _memoryCache = memoryCache;
            _apiImagemDiaria = apiImagemDiaria;
        }

        public void OnGet()
        {
            Saudacao = _configuration["Saudacao"];

            bool utilizouCache = true;

            DataConsulta = DateTime.Now.Date.AddDays(
                new Random().Next(0, 7) * -1);
            string dataHttpRequest = $"{DataConsulta:yyyy-MM-dd}";

            ImagemDiariaNASA = _memoryCache.GetOrCreate<InfoImagemNASA>(
                $"ResultadoConsultaGitHub-{dataHttpRequest}", context =>
            {
                utilizouCache = false;
                context.SetAbsoluteExpiration(TimeSpan.FromDays(1));
                context.SetPriority(CacheItemPriority.High);
                var infoImagem = _apiImagemDiaria.GetInfo(
                    _configuration["APIKeyNASA"], dataHttpRequest).Result;
                
                _logger.LogInformation(
                    $"Carregadas informacoes para imagem do dia {DataConsulta:dd/MM/yyyy}: {infoImagem.Title}");
                
                return infoImagem;
            });

            if (utilizouCache)
                _logger.LogInformation(
                    $"Utilizado cache da consulta a imagem do dia {DataConsulta:dd/MM/yyyy}: {ImagemDiariaNASA.Title}");
        }
    }
}