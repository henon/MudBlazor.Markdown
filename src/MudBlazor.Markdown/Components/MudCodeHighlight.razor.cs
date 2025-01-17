﻿using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Rendering;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.JSInterop;

// ReSharper disable once CheckNamespace
namespace MudBlazor;

public class MudCodeHighlight : MudComponentBase, IDisposable
{
	private ElementReference _ref;
	private CodeBlockTheme _theme;
	private IMudMarkdownThemeService? _themeService;
	private bool _isFirstThemeSet;

	/// <summary>
	/// Code text to render
	/// </summary>
	[Parameter]
	public string Text { get; set; } = string.Empty;

	/// <summary>
	/// Language of the <see cref="Text"/>
	/// </summary>
	[Parameter]
	public string Language { get; set; } = string.Empty;

	/// <summary>
	/// Theme of the code block.<br/>
	/// Browse available themes here: https://highlightjs.org/static/demo/ <br/>
	/// Default is <see cref="CodeBlockTheme.Default"/>
	/// </summary>
	[Parameter]
	public CodeBlockTheme Theme
	{
		get => _theme;
		set
		{
			if (_theme == value)
				return;

			_theme = value;
			Task.Run(SetThemeAsync);
		}
	}

	[Inject]
	private IJSRuntime? Js { get; init; }

	[Inject]
	private IServiceProvider? ServiceProvider { get; init; }

	public void Dispose()
	{
		if (_themeService != null)
			_themeService.CodeBlockThemeChanged -= OnCodeBlockThemeChanged;

		GC.SuppressFinalize(this);
	}

	protected override bool ShouldRender() =>
		!string.IsNullOrEmpty(Text);

	protected override void BuildRenderTree(RenderTreeBuilder builder)
	{
		var i = 0;

		builder.OpenElement(i++, "pre");
		builder.OpenElement(i++, "code");

		builder.AddElementReferenceCapture(i++, x => _ref = x);
		builder.AddContent(i++, Text);

		builder.CloseElement();
		builder.CloseElement();
	}

	protected override void OnInitialized()
	{
		base.OnInitialized();

		_themeService = ServiceProvider?.GetService<IMudMarkdownThemeService>();

		if (_themeService != null)
			_themeService.CodeBlockThemeChanged += OnCodeBlockThemeChanged;
	}

	protected override async Task OnAfterRenderAsync(bool firstRender)
	{
		if (!firstRender || Js == null)
			return;

		await Js.InvokeVoidAsync("highlightCodeElement", _ref, Language)
			.ConfigureAwait(false);

		if (!_isFirstThemeSet)
		{
			await SetThemeAsync()
				.ConfigureAwait(false);
		}
	}

	private void OnCodeBlockThemeChanged(object? sender, CodeBlockTheme e) =>
		Theme = e;

	private async Task SetThemeAsync()
	{
		if (Js == null)
			return;

		var stylesheetPath = Theme.GetStylesheetPath();

		await Js.InvokeVoidAsync("setHighlightStylesheet", stylesheetPath)
			.ConfigureAwait(false);

		_isFirstThemeSet = true;
	}
}