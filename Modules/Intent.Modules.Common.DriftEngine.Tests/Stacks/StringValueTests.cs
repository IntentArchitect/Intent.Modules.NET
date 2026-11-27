using Intent.Modules.Common.DriftEngine.Code;
using Intent.Modules.Common.DriftEngine.Stacks.CSharp;
using Intent.Modules.Common.DriftEngine.Stacks.TypeScript;

namespace Intent.Modules.Common.DriftEngine.Tests.Stacks;

/// <summary>
/// The string a value is (<see cref="CodeGraph.StringOf"/>), as a proxy's or a page's URI is built: the same evaluation for
/// every language, from the facts each gives its values. Each case is a method whose return is the value; a hole is shown
/// as <c>{its text}</c>.
/// </summary>
public class StringValueTests
{
    private static readonly CodeGraph CSharp = ReadCSharp("""
        using System;

        namespace Shop.Web.Pages
        {
            public static class LogIn
            {
                public static string Url(string returnUrl) => $"user/login?returnUrl={Uri.EscapeDataString(returnUrl)}";
            }
        }

        namespace Shop.Web
        {
            public static class Routes
            {
                public const string Catalog = "api/catalog/";
                public static readonly string Items = Catalog + "items/";
            }

            public class CatalogService
            {
                private readonly string remoteServiceBaseUrl = "api/catalog/";
                private readonly string fromConstructor;
                private string changed = "api/old/";

                public CatalogService()
                {
                    fromConstructor = "api/ctor/";
                }

                public void Change() => changed += "x/";

                public string Item(int id)
                {
                    var uri = $"{remoteServiceBaseUrl}items/{id}";
                    return uri;
                }

                public string Page(int page, int[] types) => PageUri(remoteServiceBaseUrl, page, types);

                private static string PageUri(string baseUri, int page, int[] types)
                {
                    var filter = string.Empty;
                    if (types.Length > 0)
                    {
                        filter += string.Join("&", types) + "&";
                    }

                    return $"{baseUri}items?{filter}page={page}";
                }

                public string Items() => Routes.Items + "all";

                public string Login() => Pages.LogIn.Url("orders");

                public string Brands() => (Routes.Catalog + "brands/").TrimEnd('/');

                public string Facets(string query) => $"{remoteServiceBaseUrl}facets?{query}".TrimEnd('&', '?');

                public string Same(bool flag) => flag ? "api/same" : "api/same";

                public string Either(bool flag) => flag ? "api/a" : "api/b";

                public string Converted() => string.Concat(Routes.Catalog, new Uri("x", UriKind.Relative).ToString(), string.Empty);

                public string Constructed() => (string)fromConstructor + changed!;

                public string Formatted() => $"{remoteServiceBaseUrl,20}";
            }
        }
        """);

    private static readonly CodeGraph TypeScript = ReadTypeScript(("src/api/client.ts", """
        const BASE = '/api';
        const VERSION = 'v1';

        function request(method: string, path: string): Promise<unknown> {
          return fetch(`${BASE}${path}`, { method });
        }

        function itemsUri(id: string): string {
          return BASE + '/items/' + id;
        }

        export const api = {
          getItem(id: string) {
            return request('GET', `/items/${encodeURIComponent(id)}`);
          },
          itemUri(id: string) {
            return itemsUri(id);
          },
          versioned() {
            let path = `/${VERSION}`;
            path += '/x';
            return path;
          },
          trimmed() {
            return ` ${BASE}/t `.trim();
          },
          either(flag: boolean) {
            return flag ? BASE : `${BASE}`;
          },
        };

        export class Client {
          private readonly root = BASE + '/client';

          orders(id: number) {
            return `${this.root}/orders/${id}`;
          }
        }
        """));

    [Fact]
    public void A_field_a_local_and_interpolation_give_the_text_and_a_parameter_is_a_hole()
    {
        var item = CSharp.StringOf(Returned(CSharp, "CatalogService", "Item"));
        Show(item).ShouldBe("api/catalog/items/{id}");
        item.Kind.ShouldBe("interpolated");

        // The text is placed where it is first written: the field's initializer.
        item.Parts[0].At.Line.ShouldBe(21);
        item.Parts[1].Text.ShouldBe("id");
    }

    [Fact]
    public void A_method_of_the_code_is_followed_with_its_parameters_bound_to_the_calls_arguments()
    {
        // filter is changed with +=, so it is a hole; baseUri and page are the caller's.
        Show(CSharp.StringOf(Returned(CSharp, "CatalogService", "Page"))).ShouldBe("api/catalog/items?{filter}page={page}");
        Show(CSharp.StringOf(Returned(CSharp, "CatalogService", "Login"))).ShouldBe("user/login?returnUrl={Uri.EscapeDataString(returnUrl)}");
    }

    [Fact]
    public void Another_types_constants_are_read_through_its_name()
    {
        var items = CSharp.StringOf(Returned(CSharp, "CatalogService", "Items"));
        items.Kind.ShouldBe("string");
        items.Value.ShouldBe("api/catalog/items/all");
    }

    [Fact]
    public void A_trim_takes_written_characters_from_known_text_only()
    {
        Show(CSharp.StringOf(Returned(CSharp, "CatalogService", "Brands"))).ShouldBe("api/catalog/brands");
        Show(CSharp.StringOf(Returned(CSharp, "CatalogService", "Facets"))).ShouldBe("api/catalog/facets?{query}");
    }

    [Fact]
    public void A_value_given_several_ways_is_known_only_when_they_agree()
    {
        Show(CSharp.StringOf(Returned(CSharp, "CatalogService", "Same"))).ShouldBe("api/same");
        Show(CSharp.StringOf(Returned(CSharp, "CatalogService", "Either"))).ShouldBe("{flag ? \"api/a\" : \"api/b\"}");
    }

    [Fact]
    public void Conversions_that_keep_the_text_and_string_Concat_are_followed()
    {
        Show(CSharp.StringOf(Returned(CSharp, "CatalogService", "Converted"))).ShouldBe("api/catalog/x");
    }

    [Fact]
    public void A_field_has_the_one_value_its_type_gives_it_unless_its_code_changes_it_otherwise()
    {
        Show(CSharp.StringOf(Returned(CSharp, "CatalogService", "Constructed"))).ShouldBe("api/ctor/{changed!}");
    }

    [Fact]
    public void A_hole_with_a_format_or_alignment_stays_a_hole()
    {
        Show(CSharp.StringOf(Returned(CSharp, "CatalogService", "Formatted"))).ShouldBe("{remoteServiceBaseUrl}");
    }

    [Fact]
    public void TypeScript_follows_module_constants_template_literals_and_its_functions()
    {
        var module = TypeScript.Types.Single(x => x.Name == "client" && x.Kind == "module");
        var fetch = module.Members.Single(x => x.Name == "request").Calls.Single(x => x.Name == "fetch");
        Show(TypeScript.StringOf(fetch.Arguments[0])).ShouldBe("/api{path}");

        var getItem = Member(TypeScript, "api", "getItem").Calls.Single(x => x.Name == "request");
        Show(TypeScript.StringOf(getItem.Arguments[1])).ShouldBe("/items/{encodeURIComponent(id)}");

        Show(TypeScript.StringOf(Returned(TypeScript, "api", "itemUri"))).ShouldBe("/api/items/{id}");
        Show(TypeScript.StringOf(Returned(TypeScript, "api", "versioned"))).ShouldBe("{path}");
        Show(TypeScript.StringOf(Returned(TypeScript, "api", "trimmed"))).ShouldBe("/api/t");
        Show(TypeScript.StringOf(Returned(TypeScript, "api", "either"))).ShouldBe("/api");
        Show(TypeScript.StringOf(Returned(TypeScript, "Client", "orders"))).ShouldBe("/api/client/orders/{id}");
    }

    private static readonly CodeGraph Endpoints = ReadTypeScript(
        ("src/app/core/constants/paths.constants.ts", """
            export const AUTHENTICATION_PATHS = {
              base: 'auth',
              logIn: 'log-in',
            };

            export const POKEMON_PATHS = {
              base: 'pokemon',
            };
            """),
        ("src/app/core/constants/urls.constants.ts", """
            import { AUTHENTICATION_PATHS, POKEMON_PATHS } from './paths.constants';

            export const AUTH_URLS = {
              logIn: `/${AUTHENTICATION_PATHS.base}/${AUTHENTICATION_PATHS.logIn}`,
            };

            export const POKEMON_URLS = {
              detail: (id: string) => `/${POKEMON_PATHS.base}/${id}`,
            };
            """),
        ("src/app/core/constants/endpoints.constants.ts", """
            import { inject } from '@angular/core';
            import { ENVIRONMENT } from '../tokens/environment.token';

            const getUserEndpoints = (baseUrl: string) => ({
              v1: {
                user: `${baseUrl}/v1/user`,
              },
            });

            const getPokemonEndpoints = (baseUrl: string, host: string) => ({
              v1: {
                pokemon: (pokemonIdOrName: string | number) => `${host}/v2/pokemon/${pokemonIdOrName}`,
              },
            });

            export const getEndpoints = () => {
              const environment = inject(ENVIRONMENT);
              const POKEMON_API_HOST = 'https://pokeapi.co/api';
              return {
                user: getUserEndpoints(environment.apiBaseUrl),
                pokemon: getPokemonEndpoints(environment.apiBaseUrl, POKEMON_API_HOST),
              } as const;
            };
            """),
        ("src/app/features/user/user.service.ts", """
            import { HttpClient } from '@angular/common/http';
            import { inject, Injectable } from '@angular/core';
            import { getEndpoints } from '../../core/constants/endpoints.constants';
            import { AUTH_URLS, POKEMON_URLS } from '../../core/constants/urls.constants';

            @Injectable({ providedIn: 'root' })
            export class UserService {
              private readonly http = inject(HttpClient);
              private readonly endpoints = getEndpoints();
              readonly authUrls = AUTH_URLS;
              readonly pokemonId = 'x';
              readonly detailUrl = computed(() => POKEMON_URLS.detail(this.pokemonId));

              getMe() {
                return this.http.get(this.endpoints.user.v1.user);
              }

              getPokemon(pokemonIdOrName: string | number) {
                return this.http.get(this.endpoints.pokemon.v1.pokemon(pokemonIdOrName));
              }
            }
            """));

    [Fact]
    public void TypeScript_follows_a_member_read_off_what_a_field_a_call_or_a_function_holds()
    {
        Show(Endpoints.StringOf(Sent(Endpoints, "UserService", "getMe"))).ShouldBe("{environment.apiBaseUrl}/v1/user");
        Show(Endpoints.StringOf(Sent(Endpoints, "UserService", "getPokemon"))).ShouldBe("https://pokeapi.co/api/v2/pokemon/{pokemonIdOrName}");
    }

    [Fact]
    public void A_path_of_members_reads_through_constants_and_runs_what_a_called_step_holds()
    {
        var service = Endpoints.Types.Single(x => x.Name == "UserService");
        Show(Endpoints.StringOf(service, [("authUrls", false), ("logIn", false)])!).ShouldBe("/auth/log-in");
        Show(Endpoints.StringOf(service, [("detailUrl", true)])!).ShouldBe("/pokemon/x");
    }

    private static ValueNode Sent(CodeGraph graph, string type, string member) => Member(graph, type, member).Body!.Calls.Single(x => x.Name == "get").Arguments[0];

    private static MemberNode Member(CodeGraph graph, string type, string member) => graph.Types.Single(x => x.Name == type).Members.Single(x => x.Name == member);

    private static ValueNode Returned(CodeGraph graph, string type, string member) => Member(graph, type, member).Body!.Returns.Single();

    private static string Show(LiteralNode literal) =>
        literal.Kind == "string" ? literal.Value! : string.Concat(literal.Parts.Select(x => x.Literal is { Kind: "string" } text && text.Value == x.Text ? x.Text : $"{{{x.Text}}}"));

    private static CodeGraph ReadCSharp(string source) => Read(new CSharpExtension().Languages.Single(), ("Shop.cs", source));

    private static CodeGraph ReadTypeScript(params (string Path, string Source)[] files) => Read(new TypeScriptExtension().Languages.Single(), files);

    private static CodeGraph Read(Extensions.ILanguage language, params (string Path, string Source)[] files)
    {
        var root = Directory.CreateTempSubdirectory("driftengine-strings-").FullName;
        try
        {
            foreach (var (path, source) in files)
            {
                var full = Path.Combine(root, path);
                Directory.CreateDirectory(Path.GetDirectoryName(full)!);
                File.WriteAllText(full, source);
            }

            var graph = new CodeGraph();
            language.Read(root, files.Select(x => x.Path).ToList(), graph, TestContext.Current.CancellationToken);
            return graph;
        }
        finally
        {
            Directory.Delete(root, recursive: true);
        }
    }
}
