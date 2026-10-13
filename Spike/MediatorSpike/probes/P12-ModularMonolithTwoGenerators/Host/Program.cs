using ModA;
using ModB;
using Mediator;
using Microsoft.Extensions.DependencyInjection;
var sp = new ServiceCollection().AddModA().AddModB().BuildServiceProvider();
using var scope = sp.CreateScope();
var sender = scope.ServiceProvider.GetRequiredService<ISender>();
Console.WriteLine("ISender impl: " + sender.GetType().AssemblyQualifiedName);
try { Console.WriteLine(await sender.Send(new ModA.PingModA())); } catch (Exception e) { Console.WriteLine("ModA failed: " + e.GetType().Name + ": " + e.Message); }
try { Console.WriteLine(await sender.Send(new ModB.PingModB())); } catch (Exception e) { Console.WriteLine("ModB failed: " + e.GetType().Name + ": " + e.Message); }
