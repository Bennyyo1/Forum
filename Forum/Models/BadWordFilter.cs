using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

public class BadWordFilter : IAsyncPageFilter
{
	private readonly string[] _badWords = { "badword1", "badword2" }; 
	private readonly string _replacement = "********"; // ersätt fula ord med

	public async Task OnPageHandlerExecutionAsync(PageHandlerExecutingContext context, PageHandlerExecutionDelegate next)
	{
		
		var resultContext = await next();

		if (resultContext.Result is PageResult pageResult && pageResult.Model is PageModel pageModel)
		{
			var content = pageModel.ViewData["Content"]?.ToString();
			if (content != null)
			{
				pageModel.ViewData["Content"] = Filter(content);
			}
		}
	}

	public Task OnPageHandlerSelectionAsync(PageHandlerSelectedContext context)
	{
		
		return Task.CompletedTask;
	}

	public string Filter(string input)
	{
		foreach (var badWord in _badWords)
		{
			var regex = new Regex(badWord, RegexOptions.IgnoreCase);
			input = regex.Replace(input, _replacement);
		}
		return input;
	}
}
