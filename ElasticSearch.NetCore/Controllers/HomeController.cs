using Elastic.Clients.Elasticsearch;
using ElasticSearch.NetCore.Models;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;

namespace ElasticSearch.NetCore.Controllers;

public class HomeController : Controller
{
    private readonly ILogger<HomeController> _logger;
    private readonly ElasticsearchClient _elasticsearchClient;

    public HomeController(ILogger<HomeController> logger, ElasticsearchClient elasticsearchClient)
    {
        _logger = logger;
        _elasticsearchClient = elasticsearchClient;
    }

    public async Task<IActionResult> Index()
    {
        var searchResponse = await _elasticsearchClient.SearchAsync<Book>(s => s
            .Index("books")
            .Query(q => q.MatchAll(new Elastic.Clients.Elasticsearch.QueryDsl.MatchAllQuery()))
        );

        if (searchResponse.IsValidResponse)
        {
            System.Console.WriteLine("Found.");
        }
        else
        {
            System.Console.WriteLine("Not Found.");
        }
        return View(searchResponse);
    }

    public IActionResult Privacy()
    {
        return View();
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}
