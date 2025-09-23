﻿using Microsoft.SemanticKernel;
using ModelContextProtocol.Client;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace Intent.Modules.AI.Blazor.Tasks.Config
{

    internal static class McpHelper
    {
        static HttpClient BuildHttpClient(Dictionary<string, string>? headers)
        {
            var http = new HttpClient();
            if (headers != null)
                foreach (var (k, v) in headers)
                    http.DefaultRequestHeaders.TryAddWithoutValidation(k, v);
            return http;
        }

        public static async Task<List<IAsyncDisposable>> WireUpMcpAsync(
            Kernel kernel,
            List<McpServer> mcpServers)
        {
            var disposables = new List<IAsyncDisposable>();

            foreach (var server in mcpServers)
            {
                if (!server.Enabled) continue;

                IMcpClient client;

                switch (server.Transport)
                {
                    case McpTransport.Process:
                        {
                            if (string.IsNullOrWhiteSpace(server.Command))
                                throw new InvalidOperationException($"MCP '{server.Name}': command is required for process transport.");

                            var stdio = new StdioClientTransportOptions
                            {
                                Command = server.Command!,
                                Arguments = server.Args ?? new List<string>(),
                                WorkingDirectory = server.WorkingDirectory,
                                EnvironmentVariables = server.Env ?? new Dictionary<string, string>()
                            };
                            client = await McpClientFactory.CreateAsync(new StdioClientTransport(stdio));
                            break;
                        }

                    case McpTransport.Sse:
                        {
                            if (string.IsNullOrWhiteSpace(server.Url))
                                throw new InvalidOperationException($"MCP '{server.Name}': url is required for SSE transport.");

                            var sse = new SseClientTransportOptions
                            {
                                Endpoint = new Uri(server.Url!)
                            };
                            client = await McpClientFactory.CreateAsync(
                                new SseClientTransport(sse, BuildHttpClient(server.Headers)));
                            break;
                        }

                    default:
                        throw new NotSupportedException($"Unknown transport: {server.Transport}");
                }

                // Keep the client so we can dispose it later.
                disposables.Add(client);

                // v0.4: CreateAsync already connects; no InitializeAsync().
                var tools = await client.ListToolsAsync(); // IList<McpClientTool>

                // Optional filtering
                var filtered = (server.ToolFilter is { Count: > 0 })
                    ? tools.Where(t => server.ToolFilter.Any(f => string.Equals(f, t.Name, StringComparison.OrdinalIgnoreCase)))
                    : tools;

                var pluginName = string.IsNullOrWhiteSpace(server.Group) ? server.Name : $"{server.Group}:{server.Name}";
                if (!string.IsNullOrWhiteSpace(server.Description))
                    Console.WriteLine($"[MCP] {pluginName} – {server.Description}");

                // ---- Manual adapter: MCP tool -> SK KernelFunction (no bridge pkg) ----
                var functions = filtered.Select(t => ToKernelFunction(t, client)).ToList();
                kernel.Plugins.AddFromFunctions(pluginName, functions);
            }

            return disposables;
        }

        /// <summary>
        /// Creates an SK function that forwards calls to the given MCP tool on the given client.
        /// No dependency on ModelContextProtocol-SemanticKernel.
        /// </summary>
        private static KernelFunction ToKernelFunction(McpClientTool tool, IMcpClient client)
        {
            // Implement the function body as a delegate SK will call
            async Task<string> Impl(Kernel _, KernelArguments skArgs)
            {
                // Forward all SK arguments to the MCP tool as a simple dictionary
                var input = skArgs.ToDictionary(kv => kv.Key, kv => kv.Value);

                // v0.4: Call the MCP tool; input can be null or an object (dictionary works well)
                var result = await client.CallToolAsync(tool.Name, input);

                // Return text/JSON back to SK
                return result?.ToString() ?? string.Empty;
            }

            // Create the SK function from the delegate + metadata
            return KernelFunctionFactory.CreateFromMethod(
                (Func<Kernel, KernelArguments, Task<string>>)Impl,
                functionName: tool.Name,
                description: tool.Description
            // You can also supply 'parameters:' and 'returnParameter:' here if you want rich metadata
            );
        }
    }

}