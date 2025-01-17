﻿using System;

// ReSharper disable once CheckNamespace
namespace MudBlazor;

public interface IMudMarkdownThemeService
{
	event EventHandler<CodeBlockTheme> CodeBlockThemeChanged;

	void SetCodeBlockTheme(CodeBlockTheme theme);
}