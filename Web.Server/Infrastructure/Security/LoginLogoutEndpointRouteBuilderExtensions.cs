﻿using Havit.NewProjectTemplate.Web.Server.Infrastructure.ConfigurationExtensions;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.AspNetCore.Mvc;

namespace Havit.NewProjectTemplate.Web.Server.Infrastructure.Security;

// zdroj: https://github.com/dotnet/blazor-samples/tree/main/8.0/BlazorWebAppOidc

internal static class LoginLogoutEndpointRouteBuilderExtensions
{
	internal static IEndpointConventionBuilder MapLoginAndLogout(this IEndpointRouteBuilder endpoints)
	{
		var group = endpoints.MapGroup("");

		group.MapGet("/login", (string returnUrl) => TypedResults.Challenge(GetAuthProperties(returnUrl)))
			.AllowAnonymous();

		// Sign out of the Cookie and OIDC handlers. If you do not sign out with the OIDC handler,
		// the user will automatically be signed back in the next time they visit a page that requires authentication
		// without being able to choose another account.
		group.MapPost("/logout", ([FromForm] string returnUrl) => TypedResults.SignOut(GetAuthProperties(returnUrl),
			[CookieAuthenticationDefaults.AuthenticationScheme, AuthenticationConfigurationExtension.MsOidcScheme]));

		return group;
	}

	private static AuthenticationProperties GetAuthProperties(string returnUrl)
	{
		// Převzatý komentář: T_O_D_O: Use HttpContext.Request.PathBase instead.
		const string pathBase = "/";

		// Prevent open redirects.
		if (string.IsNullOrEmpty(returnUrl))
		{
			returnUrl = pathBase;
		}
		else if (!Uri.IsWellFormedUriString(returnUrl, UriKind.Relative))
		{
			returnUrl = new Uri(returnUrl, UriKind.Absolute).PathAndQuery;
		}
		else if (returnUrl[0] != '/')
		{
			returnUrl = $"{pathBase}{returnUrl}";
		}

		return new AuthenticationProperties { RedirectUri = returnUrl };
	}
}
