﻿using System;
using System.IO;
using System.Reflection;
using System.Threading.Tasks;
using MudBlazor.Markdown.Core.Services.Interfaces;

namespace MudBlazor.Markdown.Core.Services
{
	internal sealed class MarkdownService : IMarkdownService
	{
		public Task<string> GetSampleAsync() =>
			GetMarkdownAsync("sample");

		public Task<string> GetEnderalSampleAsync() =>
			GetMarkdownAsync("sample-enderal");

		private static async Task<string> GetMarkdownAsync(string name)
		{
			var assembly = Assembly.GetExecutingAssembly();
			var resourceName = $"{assembly.GetName().Name}.{name}.md";

			await using var stream = assembly.GetManifestResourceStream(resourceName) ?? throw new InvalidOperationException("Markdown resource not found");
			using var reader = new StreamReader(stream);

			return await reader.ReadToEndAsync()
				.ConfigureAwait(false);
		}
	}
}
