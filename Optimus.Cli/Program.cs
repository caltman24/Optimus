using System.CommandLine;
using Optimus.Cli.Commands;

var rootCommand = RootCliCommand.BuildCommand();

await rootCommand.InvokeAsync(args);