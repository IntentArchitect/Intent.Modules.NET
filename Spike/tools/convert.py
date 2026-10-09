"""Converts the Intent-generated MediatR apps (Spike/MediatorSpike) into the Mediator variant (Spike/mediator).

Every rule below is a transformation a Mediator module template must perform. Each one cites the
decision in DECISIONS.md that drives it. Run from anywhere:  python tools/convert.py
"""
import os
import re
import shutil
import sys

SPIKE = os.path.dirname(os.path.dirname(os.path.abspath(__file__)))
SRC = os.path.join(SPIKE, 'MediatorSpike')
DST = os.path.join(SPIKE, 'mediator')
APPS = ['CqrsSpike.Publisher', 'CqrsSpike.Subscriber']
SKIP_DIRS = {'bin', 'obj', '.agents', 'intent', '.vs'}
SKIP_FILES = {'.gitignore'}

MEDIATOR_PACKAGES = '''<PackageReference Include="Mediator.Abstractions" Version="3.0.2" />
    <PackageReference Include="Mediator.SourceGenerator" Version="3.0.2">
      <PrivateAssets>all</PrivateAssets>
      <IncludeAssets>runtime; build; native; contentfiles; analyzers</IncludeAssets>
    </PackageReference>'''

report = []


def note(rel, rule):
    report.append(f'{rule:<28} {rel}')


def read(p):
    with open(p, encoding='utf-8-sig') as f:
        return f.read()


def write(p, s):
    os.makedirs(os.path.dirname(p), exist_ok=True)
    with open(p, 'w', encoding='utf-8', newline='\n') as f:
        f.write(s)


def replace_handle_body_end(s, add_line):
    """Insert `add_line` before the closing brace of the Handle method (8-space indented member)."""
    m = re.search(r'\n        public async [^\n]*Handle\([^\n]*\n(        [^\n]*\n)*?        \{\n', s)
    if not m:
        raise ValueError('Handle method not found')
    start = m.end()
    end = s.index('\n        }\n', start)
    body = s[start:end].rstrip()
    return s[:start] + body + '\n' + add_line + s[end:]


def using_set(s, ns, present):
    line = f'using {ns};'
    if present and line not in s:
        s = re.sub(r'(using Intent\.RoslynWeaver\.Attributes;)', line + r'\n\1', s, count=1)
    if not present:
        s = s.replace(line + '\n', '')
    return s


def sort_usings(s):
    lines = s.split('\n')
    idx = [i for i, l in enumerate(lines) if l.startswith('using ') and l.endswith(';') and ' = ' not in l]
    if not idx:
        return s
    a, b = idx[0], idx[-1]
    if any(not lines[i].startswith('using ') for i in range(a, b + 1)):
        return s
    block = sorted(set(lines[a:b + 1]), key=lambda l: (0 if l.startswith('using System') else 1, l))
    return '\n'.join(lines[:a] + block + lines[b + 1:])


def convert_file(app, rel, s):
    A = f'{app}.Application'
    orig = s
    name = os.path.basename(rel)

    # --- template ids (provenance of every converted file)
    s = s.replace('"Intent.Application.MediatR.', '"Intent.Application.Mediator.')
    s = s.replace('"Intent.MediatR.DomainEvents.', '"Intent.Mediator.DomainEvents.')

    # --- D12: Application csproj carries the generator (PrivateAssets=all)
    if rel.endswith(f'{A}.csproj'):
        s = re.sub(r'<PackageReference Include="MediatR" Version="[^"]+" />', MEDIATOR_PACKAGES, s)
        note(rel, 'D12 packages')
        return s

    if not rel.endswith('.cs'):
        return s

    uses_mediatr = 'using MediatR;' in s or 'using MediatR.Pipeline;' in s or 'MediatR.' in s

    # --- D5: commands / queries become Mediator-native CQRS messages
    if re.search(r'class \w+(Command|Query) : IRequest', s):
        s = re.sub(r'class (\w+Command) : IRequest<([^\n]+)>, ICommand\b', r'class \1 : ICommand<\2>', s)
        s = re.sub(r'class (\w+Command) : IRequest, ICommand\b', r'class \1 : ICommand', s)
        s = re.sub(r'class (\w+Query) : IRequest<([^\n]+)>, IQuery\b', r'class \1 : IQuery<\2>', s)
        s = using_set(s, f'{A}.Common.Interfaces', False)
        note(rel, 'D5 message')

    # --- D5/D6: handlers -> ICommandHandler / IQueryHandler returning ValueTask
    m = re.search(r'class (\w+)Handler : IRequestHandler<(\w+)(, ([^\n]+))?>', s)
    if m:
        req, ret = m.group(2), m.group(4)
        kind = 'Query' if req.endswith('Query') else 'Command'
        iface = f'I{kind}Handler<{req}, {ret}>' if ret else f'ICommandHandler<{req}>'
        s = s.replace(m.group(0), f'class {m.group(1)}Handler : {iface}')
        if ret:
            s = re.sub(r'public async Task<' + re.escape(ret) + r'> Handle\(', f'public async ValueTask<{ret}> Handle(', s)
        else:
            s = re.sub(r'public async Task Handle\(' + req, f'public async ValueTask<Unit> Handle({req}', s)
            s = replace_handle_body_end(s, '            return Unit.Value;')
        note(rel, 'D6 handler' + ('' if ret else ' (+Unit)'))

    # --- D10/D13: pipeline behaviours on Mediator's IPipelineBehavior
    if '/Common/Behaviours/' in rel.replace('\\', '/'):
        if 'IRequestPreProcessor' in s:  # LoggingBehaviour: pre-processor is a pipeline behaviour in Mediator
            s = s.replace('LoggingBehaviour<TRequest> : IRequestPreProcessor<TRequest>', 'LoggingBehaviour<TRequest, TResponse> : MessagePreProcessor<TRequest, TResponse>')
            s = s.replace('ILogger<LoggingBehaviour<TRequest>>', 'ILogger<LoggingBehaviour<TRequest, TResponse>>')
            s = s.replace('public async Task Process(TRequest request, CancellationToken cancellationToken)', 'protected override async ValueTask Handle(TRequest request, CancellationToken cancellationToken)')
            s = s.replace('using MediatR.Pipeline;', 'using Mediator;')
        s = s.replace('MediatR.IPipelineBehavior', 'IPipelineBehavior').replace('MediatR.RequestHandlerDelegate', 'RequestHandlerDelegate')
        s = s.replace('RequestHandlerDelegate<TResponse> next', 'MessageHandlerDelegate<TRequest, TResponse> next')
        s = s.replace('public async Task<TResponse> Handle(', 'public async ValueTask<TResponse> Handle(')
        s = s.replace('await next(cancellationToken)', 'await next(request, cancellationToken)').replace('await next()', 'await next(request, cancellationToken)')
        s = s.replace('where TRequest : notnull, ICommand', 'where TRequest : IBaseCommand')
        s = s.replace('<see cref="ICommand"/>', '<see cref="IBaseCommand"/>')
        s = s.replace('where TRequest : notnull', 'where TRequest : IMessage')
        if 'UnitOfWorkBehaviour' in name and 'ICurrentUserService' not in s:
            s = using_set(s, f'{A}.Common.Interfaces', False)
        s = using_set(s, 'Mediator', True)
        note(rel, 'D10 behaviour')

    # --- D10/D8: AddMediatR -> AddMediator with a LITERAL ordered PipelineBehaviors array
    if name == 'DependencyInjection.cs' and 'services.AddMediatR(' in s:
        block = re.search(r'            services\.AddMediatR\(cfg =>\n            \{\n(.*?)\n            \}\);\n', s, re.S)
        behaviours = re.findall(r'cfg\.AddOpenBehavior\(typeof\((\w+)<,>\)\);', block.group(1))
        has_logging = os.path.exists(os.path.join(SRC, app, A, 'Common', 'Behaviours', 'LoggingBehaviour.cs'))
        ordered = (['LoggingBehaviour'] if has_logging else []) + behaviours
        items = ',\n'.join(f'                    typeof({b}<,>)' for b in ordered)
        new = ('            services.AddMediator((MediatorOptions options) =>\n            {\n'
               '                options.ServiceLifetime = ServiceLifetime.Scoped;\n'
               '                options.PipelineBehaviors =\n                [\n'
               f'{items}\n                ];\n            }});\n')
        s = s.replace(block.group(0), new)
        s = using_set(s, 'Mediator', True)
        note(rel, 'D8/D10 AddMediator')

    # --- D7: domain events -> abstract generic base + sealed per-event notification
    if rel.replace('\\', '/').endswith('Common/Models/DomainEventNotification.cs'):
        s = s.replace('public class DomainEventNotification<TDomainEvent> : INotification', 'public abstract class DomainEventNotification<TDomainEvent> : INotification')
        s = s.replace('public DomainEventNotification(TDomainEvent domainEvent)', 'protected DomainEventNotification(TDomainEvent domainEvent)')
        note(rel, 'D7 abstract base')
    m = re.search(r'INotificationHandler<DomainEventNotification<(\w+)>>', s)
    if m:
        evt = m.group(1)
        s = s.replace(f'INotificationHandler<DomainEventNotification<{evt}>>', f'INotificationHandler<{evt}Notification>')
        s = s.replace(f'DomainEventNotification<{evt}> notification', f'{evt}Notification notification')
        s = s.replace('public async Task Handle(', 'public async ValueTask Handle(')
        if f'{evt}' not in re.sub(rf'{evt}Notification', '', s):
            s = using_set(s, f'{app}.Domain.Events', False)
        note(rel, 'D7 handler')
    if name == 'DomainEventService.cs':
        s = re.sub(r'await _mediator\.Publish\(GetNotificationCorrespondingToDomainEvent\(domainEvent\), cancellationToken\);',
                   'var notification = DomainEventNotificationFactory.Create(domainEvent);\n'
                   '            if (notification is null)\n            {\n'
                   '                _logger.LogDebug("No notification is mapped for domain event {event}", domainEvent.GetType().Name);\n'
                   '                return;\n            }\n\n'
                   '            await _mediator.Publish(notification, cancellationToken);', s)
        s = re.sub(r'\n\n        private static INotification GetNotificationCorrespondingToDomainEvent.*?\n        }\n', '\n', s, flags=re.S)
        note(rel, 'D7 service')

    # --- every remaining dispatch call site: ISender / IPublisher / INotification keep their names
    if uses_mediatr:
        s = s.replace('using MediatR;', 'using Mediator;')
        if s.count('using Mediator;') > 1:
            s = s.replace('using Mediator;\n', '', s.count('using Mediator;') - 1)
    if s == orig:
        return orig  # untouched by any rule: keep byte-identical so the delta stays honest
    s = sort_usings(s)
    if not any(r.endswith(rel) for r in report):
        note(rel, 'using/template id only')
    return s


def domain_events(app):
    folder = os.path.join(SRC, app, f'{app}.Domain', 'Events')
    if not os.path.isdir(folder):
        return []
    return sorted(f[:-3] for f in os.listdir(folder) if f.endswith('.cs'))


def emit_domain_event_types(app):
    events = domain_events(app)
    if not events:
        return
    base = os.path.join(DST, app, f'{app}.Application', 'Common', 'Models')
    for evt in events:
        write(os.path.join(base, f'{evt}Notification.cs'), f'''using {app}.Domain.Events;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Mediator.DomainEvents.DomainEventNotificationType", Version = "1.0")]

namespace {app}.Application.Common.Models
{{
    public sealed class {evt}Notification : DomainEventNotification<{evt}>
    {{
        public {evt}Notification({evt} domainEvent) : base(domainEvent)
        {{
        }}
    }}
}}
''')
        note(f'{app}/.../Common/Models/{evt}Notification.cs', 'D7 per-event (new)')
    arms = '\n'.join(f'                {e} e => new {e}Notification(e),' for e in events)
    write(os.path.join(base, 'DomainEventNotificationFactory.cs'), f'''using {app}.Domain.Common;
using {app}.Domain.Events;
using Intent.RoslynWeaver.Attributes;
using Mediator;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Mediator.DomainEvents.DomainEventNotificationFactory", Version = "1.0")]

namespace {app}.Application.Common.Models
{{
    /// <summary>
    /// Maps a domain event to its Mediator notification. Generated per modelled domain event so that
    /// dispatch stays free of reflection: Mediator's source generator cannot see open generic
    /// notification types such as DomainEventNotification&lt;T&gt;.
    /// </summary>
    public static class DomainEventNotificationFactory
    {{
        public static INotification? Create(DomainEvent domainEvent)
        {{
            return domainEvent switch
            {{
{arms}
                _ => null
            }};
        }}
    }}
}}
''')
    note(f'{app}/.../Common/Models/DomainEventNotificationFactory.cs', 'D7 factory (new)')


def main():
    if os.path.exists(DST):
        shutil.rmtree(DST)
    for app in APPS:
        root = os.path.join(SRC, app)
        for d, dirs, files in os.walk(root):
            dirs[:] = [x for x in dirs if x not in SKIP_DIRS]
            for f in files:
                if f in SKIP_FILES:
                    continue
                src = os.path.join(d, f)
                rel = os.path.relpath(src, SRC).replace('\\', '/')
                if rel.endswith('/Common/Interfaces/ICommand.cs') or rel.endswith('/Common/Interfaces/IQuery.cs'):
                    note(rel, 'D5 removed')
                    continue
                dst = os.path.join(DST, rel)
                if f.endswith(('.cs', '.csproj')):
                    write(dst, convert_file(app, rel, read(src)))
                else:
                    os.makedirs(os.path.dirname(dst), exist_ok=True)
                    shutil.copyfile(src, dst)
        emit_domain_event_types(app)
    leftovers = []
    for d, _, files in os.walk(DST):
        for f in files:
            if f.endswith(('.cs', '.csproj')) and re.search(r'\bMediatR\b', read(os.path.join(d, f))):
                leftovers.append(os.path.relpath(os.path.join(d, f), DST))
    if '-v' in sys.argv:
        print('\n'.join(sorted(report)))
    print(f'converted {len(report)} files -> {DST}')
    print('MediatR leftovers:', leftovers or 'none')
    return 1 if leftovers else 0


if __name__ == '__main__':
    sys.exit(main())
