using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using Intent.Modules.Common.DriftEngine.Stacks.TypeScript;
using Intent.NodeJs;

namespace Intent.Modules.Common.DriftEngine.Reporting
{
    /// <summary>
    /// Runs the TypeScript language's worker on Intent Architect's own Node.js, so reading TypeScript needs no Node.js
    /// installed on the machine.
    /// </summary>
    internal sealed class IntentNodeRunner : INodeRunner
    {
        private static readonly TimeSpan Timeout = TimeSpan.FromMinutes(5);

        private readonly INodeJsProcessManager _nodeJs;

        public IntentNodeRunner(INodeJsProcessManager nodeJs)
        {
            _nodeJs = nodeJs;
        }

        public NodeRun Run(string script, IReadOnlyList<string> arguments)
        {
            var errors = new StringBuilder();
            var exitCode = 0;
            using var exited = new ManualResetEventSlim();
            INodeJsProcess process;
            try
            {
                process = _nodeJs.StartProcess(script, new NodeJsProcessOptions
                {
                    Arguments = arguments,
                    OnError = x =>
                    {
                        lock (errors)
                        {
                            errors.AppendLine(x);
                        }
                    },
                    OnExit = x =>
                    {
                        exitCode = x.ExitCode;
                        exited.Set();
                    }
                });
            }
            catch (Exception e)
            {
                return new NodeRun(null, $"Intent Architect's Node.js couldn't be started ({e.Message}).");
            }

            if (!exited.Wait(Timeout))
            {
                process.Stop();
                return new NodeRun(-1, $"It didn't finish within {Timeout.TotalMinutes} minutes.");
            }

            lock (errors)
            {
                return new NodeRun(exitCode, errors.ToString());
            }
        }
    }
}
