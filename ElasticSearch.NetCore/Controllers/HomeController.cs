using Elastic.Clients.Elasticsearch;
using Elastic.Clients.Elasticsearch.Aggregations;
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

    public async Task<IActionResult> Index(string query)
    {
        SearchResponse<Book> searchResponse;

        if (!string.IsNullOrEmpty(query))
        {
            // Contains
            searchResponse = await _elasticsearchClient.SearchAsync<Book>(s => s
                .Index("books")
                .Query(q => q
                    .Match(b => b
                        .Field(f => f.Title)
                        .Query(query)
                    )
                )
            );
            // Equals
            //searchResponse = await _elasticsearchClient.SearchAsync<Book>(s => s
            //    .Index("books")
            //    .Query(q => q
            //        .Term(b => b
            //            .Field(f => f.Isbn)
            //            .Value(query)
            //        )
            //    )
            //);
        }
        else
        {
            searchResponse = await _elasticsearchClient.SearchAsync<Book>(s => s
                .Index("books")
                .Query(q => q
                    .MatchAll(_ => { }))
                .Aggregations(a => a
                    .Add("pageCounts", aggregation => aggregation
                        .Range(range => range
                            .Field(x => x.PageCount)
                            .Ranges(new[]
                            {
                                new AggregationRange { From = 0 },
                                new AggregationRange { From = 200, To = 400 },
                                new AggregationRange { From = 400, To = 600 },
                                new AggregationRange { From = 600 }
                            })
                        )
                    )
                )
            );
        }

        if (searchResponse.IsValidResponse)
        {
            Console.WriteLine("Found.");
        }
        else
        {
            Console.WriteLine("Not Found.");
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
